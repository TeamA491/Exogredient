using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers.Upload;
using Image = Google.Cloud.Vision.V1.Image;
using UploadController;
using Google.Apis.Logging;
using TeamA.Exogredient.DataHelpers;
using System.Collections.Generic;

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
        private static readonly SnapshotDAO _snapshotDAO = new SnapshotDAO(Constants.SQLConnection);

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
        private static readonly SnapshotService _snapshotService = new SnapshotService(_logDAO, _userDAO, _uploadDAO, _snapshotDAO);

        // MANAGER
        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly UpdatePasswordManager _updatePasswordManager = new UpdatePasswordManager(_loggingManager, _userManagementService, _verificationService, _sessionService, _authorizationService);
        private static readonly SendEmailCodeManager _sendEmailCodeManger = new SendEmailCodeManager(_loggingManager, _verificationService);
        private static readonly SendPhoneCodeManager _sendPhoneCodeManager = new SendPhoneCodeManager(_loggingManager, _verificationService);
        private static readonly VerifyEmailCodeManager _verifyEmailCodeManager = new VerifyEmailCodeManager(_loggingManager, _userManagementService);
        private static readonly VerifyPhoneCodeManager _verifyPhoneCodeManager = new VerifyPhoneCodeManager(_userManagementService, _loggingManager, _verificationService);
        private static readonly RegistrationManager _registrationManager = new RegistrationManager(_userManagementService, _loggingManager);
        private static readonly SearchManager _searchManager = new SearchManager(_searchService, _loggingManager, Constants.DicFileRelativePath, Constants.AffFileRelativePath);
        private static readonly LogInManager _loginManager = new LogInManager(_userManagementService, _loggingManager, _authenticationService, _sessionService);
        private static readonly UploadManager _uploadManager = new UploadManager(_loggingManager, _googleImageAnalysisService, _storeService, _uploadService, _userManagementService);
        private static readonly UserProfileManager _userProfileManager = new UserProfileManager(_uploadService, _storeService, _saveListService, _loggingManager, _userManagementService);
        private static readonly ReadSnapshotManager _readSnapshotManager = new ReadSnapshotManager(_loggingManager, _snapshotService);


        // UPLOAD
        [EnableCors]
        [HttpPost("NewUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUploadAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                Bitmap image;
                string ext;
                long imageSizeBytes;

                if (formFile is null)
                {
                    var id = Int32.Parse(data[Constants.UniqueIdKey]);
                    var res = await _uploadService.ContinueUploadProgressAsync(id).ConfigureAwait(false);
                    image = new Bitmap(res.Photo);
                    FileInfo imageInfo = new FileInfo(res.Photo);
                    imageSizeBytes = imageInfo.Length;
                    ext = Path.GetExtension(res.Photo);
                    await _uploadService.DeleteUploadsAsync(new List<int>() { id });
                }
                else
                {
                    formFile.OpenReadStream();

                    using var memoryStream = new MemoryStream();
                    await formFile.CopyToAsync(memoryStream);
                    image = new Bitmap(memoryStream);

                    ext = data[Constants.ExtensionKey];
                    imageSizeBytes = Int32.Parse(data[Constants.ImageSizeKey]);

                }

                var post = new UploadPost(image, data[Constants.CategoryKey], data[Constants.UsernameKey], data[Constants.IPAddressKey],
                                          DateTime.Now, data[Constants.NameKey], data[Constants.DescriptionKey],
                                          Int32.Parse(data[Constants.RatingKey]), Double.Parse(data[Constants.PriceKey]), data[Constants.PriceUnitKey], ext, (int)imageSizeBytes);

                var result = await _uploadManager.CreateUploadAsync(post, Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new SuccessResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch (Exception e)
            {
                var lol = e.Message;
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
                                          data[Constants.RatingKey].ToString().Equals(Constants.NaN) ? Constants.NoValueInt : Int32.Parse(data[Constants.RatingKey]),
                                          data[Constants.PriceKey].ToString().Equals(Constants.NaN) ? Constants.NoValueDouble : Double.Parse(data[Constants.PriceKey].ToString()),
                                          data[Constants.PriceUnitKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.PriceUnitKey].ToString(),
                                          data[Constants.ExtensionKey],
                                          Int32.Parse(data[Constants.ImageSizeKey]));

                var result = await _uploadManager.DraftUploadAsync(post, Int32.Parse(data[Constants.UniqueIdKey]), Constants.NoValueInt).ConfigureAwait(false);

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

                byte[] bytes = System.IO.File.ReadAllBytes(result.Data.Photo);

                var splitResult = result.Data.Photo.Split("\\");
                var imageName = "";

                foreach (var res in splitResult)
                {
                    imageName = res;
                }

                return Ok(new ContinueResponse()
                {
                    Message = result.Message,
                    ExceptionOccurred = result.ExceptionOccurred,
                    Description = result.Data.Description,
                    Rating = result.Data.Rating,
                    Image = bytes,
                    ImageName = imageName,
                    Price = result.Data.Price,
                    PriceUnit = result.Data.PriceUnit,
                    IngredientName = result.Data.IngredientName,
                    Category = result.Data.Category
                });
            }
            catch (Exception e)
            {
                var lol = e.Message;
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
        [HttpPost("SaveList/{username}/{storeId}/{ingredient}")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateSaveListAsync(int storeId, string ingredient, string username)
        {
            return Ok(await _userProfileManager.CreateSaveListAsync(storeId, ingredient, username));
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

        ///////////////////////////////////////////////////////////STARTING USER ANALYSIS////////////////////////////////////////////////////////

        /// <summary>
        /// Method to get snapshot specific to the year and month value and format it to send up.
        /// </summary>
        /// <param name="year">The year to get the snapshot.</param>
        /// <param name="month">The month to get the snapshot.</param>
        /// <returns>The snapshot object.</returns>
        [HttpGet("FetchSingle/{year}/{month}")]
        public async Task<IActionResult> GetSingleSnapshotAsync(int year, int month)
        {
            try
            {
                return Ok(await _readSnapshotManager.ReadOneSnapshotAsync(year, month).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to get multiple snapshot specific to the year.
        /// It will also format the data in the multiple snapshot and store it as one snapshot object.
        /// </summary>
        /// <param name="year">The year to get all the snapshots.</param>
        /// <returns>The snapshot object with the filtered data.</returns>
        [HttpGet("FetchMulti/{year}")]
        public async Task<IActionResult> GetMultiSnapshotAsync(int year)
        {
            try
            {
                return Ok(await _readSnapshotManager.ReadMultiSnapshotAsync(year).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to get year and month from snapshots in databse.
        /// </summary>
        /// <returns>A json formatted string.</returns>
        [HttpGet("FetchYearMonth")]
        public async Task<IActionResult> GetYearMonth()
        {

            try
            {
                return Ok(await _readSnapshotManager.GetYearMonthAsync().ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
