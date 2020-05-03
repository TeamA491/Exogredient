using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.Exceptions;
using TeamA.Exogredient.DataHelpers.Upload;
using Image = Google.Cloud.Vision.V1.Image;
using UploadController;

namespace ExogredientController.Controllers
{
    [Route("api")]
    public class ExogredientController : Controller
    {
        // DAO
        private static readonly StoreDAO _storeDAO = new StoreDAO(Constants.SQLConnection);
        private static readonly UploadDAO _uploadDAO = new UploadDAO(Constants.SQLConnection);
        private static readonly UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
        private static readonly AnonymousUserDAO _anonymousUserDAO = new AnonymousUserDAO(Constants.SQLConnection);
        private static readonly LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);
        private static readonly SaveListDAO _saveListDAO = new SaveListDAO(Constants.SQLConnection);

        // SERVICE
        private static readonly SearchService _searchService = new SearchService(_storeDAO, _uploadDAO);
        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private static readonly DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
        private static readonly FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);
        private static readonly UserManagementService _userManagementService = new UserManagementService(_userDAO, _anonymousUserDAO, _dsLoggingService, _ffLoggingService, _maskingService);
        private static readonly AuthorizationService _authorizationService = new AuthorizationService();
        private static readonly SessionService _sessionService = new SessionService(_userDAO, _authorizationService);
        private static readonly VerificationService _verificationService = new VerificationService(_userManagementService, _sessionService);
        private static readonly AuthenticationService _authenticationService = new AuthenticationService();
        private static readonly GoogleImageAnalysisService _googleImageAnalysisService = new GoogleImageAnalysisService();
        private static readonly StoreService _storeService = new StoreService(_storeDAO);
        private static readonly UploadService _uploadService = new UploadService(_uploadDAO);
        private static readonly SaveListService _saveListService = new SaveListService(_saveListDAO);

        // MANAGER
        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly UpdatePasswordManager _updatePasswordManager = new UpdatePasswordManager(_loggingManager, _userManagementService,_verificationService, _sessionService, _authorizationService);
        private static readonly SendEmailCodeManager _sendEmailCodeManger = new SendEmailCodeManager(_loggingManager, _verificationService);
        private static readonly SendPhoneCodeManager _sendPhoneCodeManager = new SendPhoneCodeManager(_loggingManager, _verificationService);
        private static readonly VerifyEmailCodeManager _verifyEmailCodeManager = new VerifyEmailCodeManager(_loggingManager, _userManagementService);
        private static readonly VerifyPhoneCodeManager _verifyPhoneCodeManager = new VerifyPhoneCodeManager(_userManagementService, _loggingManager, _verificationService);
        private static readonly RegistrationManager _registrationManager = new RegistrationManager(_userManagementService, _loggingManager);
        private static readonly SearchManager _searchManager = new SearchManager(_searchService, _loggingManager, Constants.DicFileRelativePath, Constants.AffFileRelativePath);
        private static readonly LogInManager _loginManager = new LogInManager(_userManagementService, _loggingManager, _authenticationService, _sessionService);
        private static readonly UploadManager _uploadManager = new UploadManager(_loggingManager, _googleImageAnalysisService, _storeService, _uploadService, _userManagementService);
        private static readonly UserProfileManager _userProfileManager = new UserProfileManager(_uploadService, _storeService, _saveListService, _loggingManager, _userManagementService);

        // SEARCH
        [EnableCors]
        [HttpGet("getTotalNum")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTotalStoreResultsNumberAsync(
            string searchTerm, double latitude, double longitude, double radius, string searchBy,
            string username, string ipAddress)
        {
            try
            {
                return Ok(await _searchManager.GetTotalStoreResultsNumberAsync(searchTerm,
                    latitude, longitude, radius, searchBy, Constants.InitialFailureCount,
                    username, ipAddress).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getStoreResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoresAsync(string searchTerm, double latitude,
            double longitude, double radius, string searchBy, double lastStoreData,
            int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption,
            bool fromSmallest, string username, string ipAddress)
        {
            try
            {
                return Ok(await _searchManager.GetStoresAsync(searchTerm, latitude,
                    longitude, radius, searchBy, lastStoreData, lastStoreId,
                    lastPageResultsNum, skipPages, sortOption, fromSmallest,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("storeViewData")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoreViewDataAsync(int storeId, string username,
            string ipAddress, string ingredientName = null)
        {
            try
            {
                var storeViewData = await _searchManager.GetStoreViewDataAsync(storeId,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                var totalResultsNum = await _searchManager.GetTotalIngredientResultsNumberAsync(storeId,
                    ingredientName, Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                return Ok(new Tuple<int, StoreViewData>(totalResultsNum, storeViewData));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getIngredientResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetIngredientsAsync(string username,
            string ipAddress, int storeId, int skipPages, int lastPageResultsNum,
            string lastIngredientName = null, string ingredientName = null)
        {
            try
            {
                return Ok(await _searchManager.GetIngredientsAsync(username, ipAddress, Constants.InitialFailureCount, storeId,
                    skipPages, lastPageResultsNum, lastIngredientName, ingredientName).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("storeImage")]
        public IActionResult GetStoreImage(int storeId, string username, string ipAddress)
        {
            try
            {

                var image = System.IO.File.OpenRead($"images/store{storeId}.jpg");

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress).ConfigureAwait(false);

                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            }
        }

        // REGISTRATION
        [EnableCors]
        [HttpGet("register")]
        [Produces("application/json")]
        public async Task<IActionResult> RegisterAsync(string firstName, string lastName,
                                                      string email, string username, string phoneNumber,
                                                      string ipAddress, string hashedPassword, string salt,
                                                      string proxyPassword)
        {
            var result = await _registrationManager.RegisterAsync(firstName, lastName, email, username, phoneNumber,
                ipAddress, hashedPassword, salt, proxyPassword, Constants.InitialFailureCount);

            if (result.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            else
            {
                return Ok(new { successful = result.Data, message = result.Message });
            }
        }


        [EnableCors]
        [HttpGet("sendEmailCode")]
        [Produces("application/json")]
        public async Task<IActionResult> SendEmailCodeAsync(string username, string email, string ipAddress)
        {
            var emailResult = await _sendEmailCodeManger.SendEmailCodeAsync(username, email, ipAddress, Constants.InitialFailureCount);

            if (emailResult.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, emailResult.Message);
            }
            else
            {
                return Ok();
            }

        }

        [EnableCors]
        [HttpGet("sendPhoneCode")]
        [Produces("application/json")]
        public async Task<IActionResult> SendPhoneCodeAsync(string username, string phoneNumber, string ipAddress)
        {
            var phoneReuslt = await _sendPhoneCodeManager.SendPhoneCodeAsync(username, phoneNumber, ipAddress, Constants.InitialFailureCount);

            if (phoneReuslt.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, phoneReuslt.Message);
            }
            else
            {
                return Ok();
            }

        }

        [EnableCors]
        [HttpGet("verifyEmailCode")]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyEmailCodesAsync(string username, string emailCode, string ipAddress)
        {
            var result = await _verifyEmailCodeManager.VerifyEmailCodeAsync(username, emailCode, ipAddress, Constants.InitialFailureCount);

            if (result.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            else
            {
                return Ok(new { successful = result.Data, message = result.Message });
            }


        }

        [EnableCors]
        [HttpGet("verifyPhoneCode")]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyPhoneCodesAsync(string username, string phoneCode, string phoneNumber,
                                                               string ipAddress, bool duringRegistration)
        {
            var result = await _verifyPhoneCodeManager.VerifyPhoneCodeAsync(username, phoneCode, ipAddress, phoneNumber,
                                                              duringRegistration, Constants.InitialFailureCount);

            if (result.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            else
            {
                return Ok(new { successful = result.Data, message = result.Message });
            }
        }

        // LOGIN
        [EnableCors]
        [HttpGet("authenticate")]
        [Produces("application/json")]
        public async Task<IActionResult> LogInAsync(string username, string hashedPassword, string ipAddress)
        {
            var result = await _loginManager.LogInAsync(username, ipAddress, hashedPassword, Constants.InitialFailureCount);
            var authenResult = result.Data;

            if (result.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }

            if (authenResult.IsSuccessful)
            {
                return Ok(new
                {
                    successful = authenResult.IsSuccessful,
                    token = authenResult.Token,
                    userType = authenResult.UserType
                });
            }
            else
            {
                return Ok(new { successful = authenResult.IsSuccessful, message = result.Message });
            }
        }

        [EnableCors]
        [HttpGet("getSalt")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSaltAsync(string username, string ipAddress)
        {
            try
            {
                var salt = await _loginManager.GetSaltAsync(username, ipAddress);
                if (salt == null)
                {
                    return Ok(new { successful = false, data = Constants.InvalidLogInUserMessage });
                }
                else
                {
                    return Ok(new { successful = true, data = salt });
                }
            }
            catch (Exception e)
            {
                var lol = e.Message;
                Console.WriteLine(e.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // RESET PASSWORD
        [EnableCors]
        [HttpGet("sendResetLink")]
        [Produces("application/json")]
        public async Task<IActionResult> sendResetLinkAsync(string username, string ipAddress)
        {
            try
            {
                return Ok(await _updatePasswordManager.SendResetPasswordLinkAsync(username, ipAddress));
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        [EnableCors]
        [HttpGet("updatePassword")]
        [Produces("application/json")]
        public async Task<IActionResult> updatePassword(string username, string ipAddress, string hashedPassword,
                                                        string proxyPassword, string salt, string token)
        {
            var tokenResult = _updatePasswordManager.ValidateToken(token, username);

            if (tokenResult.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!tokenResult.Data)
            {
                return Ok(new { successful = false, message = tokenResult.Message });
            }

            var updateResult =await _updatePasswordManager.UpdatePasswordAsync(username, ipAddress, hashedPassword,
                proxyPassword, salt, Constants.InitialFailureCount);

            if(!updateResult.ExceptionOccurred){
                return Ok(new { successful = updateResult.Data, message = updateResult.Message });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // UPLOAD
        [EnableCors]
        [HttpPost("NewUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUploadAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                formFile.OpenReadStream();

                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                var post = new UploadPost(image, data[Constants.CategoryKey], data[Constants.UsernameKey], data[Constants.IPAddressKey],
                                          DateTime.Now, data[Constants.NameKey], data[Constants.DescriptionKey],
                                          Int32.Parse(data[Constants.RatingKey]), Double.Parse(data[Constants.PriceKey]), data[Constants.PriceUnitKey], data[Constants.ExtensionKey], Int32.Parse(data[Constants.ImageSizeKey]));

                var result = await _uploadManager.CreateUploadAsync(post, Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new SuccessResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpPost("DraftUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> DraftUploadAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                var post = new UploadPost(image, data[Constants.CategoryKey],
                                          data[Constants.UsernameKey],
                                          data[Constants.IPAddressKey],
                                          Constants.NoValueDatetime,
                                          data[Constants.NameKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.NameKey].ToString(),
                                          data[Constants.DescriptionKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.DescriptionKey].ToString(),
                                          data[Constants.RatingKey].ToString().Equals(Constants.NaN) ? Constants.NoValueInt : Int32.Parse(data[Constants.NameKey]),
                                          data[Constants.PriceKey].ToString().Equals(Constants.NaN) ? Constants.NoValueDouble : Double.Parse(data[Constants.NameKey].ToString()),
                                          data[Constants.PriceUnitKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.PriceUnitKey].ToString(),
                                          data[Constants.ExtensionKey],
                                          Int32.Parse(data[Constants.ImageSizeKey]));

                var result = await _uploadManager.DraftUploadAsync(post, Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new SuccessResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpPost("Vision")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> AnalyzeImageAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                formFile.OpenReadStream();

                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                Byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);

                    bytes = memoryStream.ToArray();
                }

                var img = Image.FromBytes(bytes);

                var result = await _uploadManager.AnalyzeImageAsync(img, data[Constants.UsernameKey], data[Constants.IPAddressKey], Constants.NoValueInt).ConfigureAwait(false);

                var suggestionsArray = result.Data.Suggestions.ToArray();


                return Ok(new AnalyzeResponse()
                {
                    Message = result.Message,
                    ExceptionOccurred = result.ExceptionOccurred,
                    Suggestions = suggestionsArray,
                    Category = result.Data.Category,
                    Name = result.Data.Name
                });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpPost("ContinueUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> ContinueUploadProgressAsync(IFormCollection data)
        {
            try
            {
                var result = await _uploadManager.ContinueUploadProgressAsync(data[Constants.UsernameKey], Int32.Parse(data[Constants.UniqueIdKey]), data[Constants.IPAddressKey], Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new ContinueResponse()
                {
                    Message = result.Message,
                    ExceptionOccurred = result.ExceptionOccurred,
                    Description = result.Data.Description,
                    Rating = result.Data.Rating,
                    Image = new Bitmap(result.Data.Photo),
                    Price = result.Data.Price,
                    PriceUnit = result.Data.PriceUnit,
                    IngredientName = result.Data.IngredientName
                });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpPost("DeleteUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteUploadAsync(IFormCollection data)
        {
            try
            {
                var result = await _uploadManager.DeleteUploadAsync(data[Constants.UsernameKey], Int32.Parse(data[Constants.UniqueIdKey]), data[Constants.IPAddressKey], Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new SuccessResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // USER PROFILE
        [EnableCors]
        [HttpGet("ProfileScore/{username}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetProfileScoresAsync(string username, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetProfileScoreAsync(username, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists. 
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("RecentUploads/{username}/{pagination}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRecentUploadsAsync(string username, int pagination, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetRecentUploadsAsync(username, pagination, ipAddress, 0, null).ConfigureAwait(false));

            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("InProgressUploads/{username}/{pagination}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetInProgressUploadsAsync(string username, int pagination, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetInProgressUploadsAsync(username, pagination, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("SaveList/{username}/{pagination}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSaveListAsync(string username, int pagination, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetSaveListAsync(username, pagination, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpDelete("SaveList/{username}/{storeId}/{ingredient}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteSaveListAsync(string username, int storeId, string ingredient, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.DeleteSaveListAsync(username, storeId, ingredient, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("SaveListPagination/{username}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSaveListPaginationSizeAsync(string username, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetSaveListPaginationSizeAsync(username, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("InProgressUploadPagination/{username}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetInProgressUploadPaginationSizeAsync(string username, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetInProgressUploadPaginationSizeAsync(username, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableCors]
        [HttpGet("RecentUploadPagination/{username}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRecentUploadPaginationSizeAsync(string username, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetRecentUploadPaginationSizeAsync(username, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                // Return an 404 error when the resource does not exists.
                return NotFound(ae.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
