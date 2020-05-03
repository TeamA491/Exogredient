using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

namespace ExogredientController.Controllers
{
    [Route("api")]
    public class ExogredientController : Controller
    {
        // DAO
        static StoreDAO storeDao = new StoreDAO(Constants.SQLConnection);
        static UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
        static UserDAO userDAO = new UserDAO(Constants.SQLConnection);
        static AnonymousUserDAO anonymousUserDAO = new AnonymousUserDAO(Constants.SQLConnection);
        static LogDAO logdao = new LogDAO(Constants.NOSQLConnection);
        static MapDAO mapdao = new MapDAO(Constants.MapSQLConnection);

        // SERVICE
        static SearchService searchService = new SearchService(storeDao, uploadDAO);
        static MaskingService mask = new MaskingService(mapdao);
        static DataStoreLoggingService dsLogging = new DataStoreLoggingService(logdao, mask);
        static FlatFileLoggingService ffLogging = new FlatFileLoggingService(mask);
        static UserManagementService userManagementService = new UserManagementService(userDAO, anonymousUserDAO, dsLogging, ffLogging, mask);
        static AuthorizationService authorizationService = new AuthorizationService();
        static SessionService sessionService = new SessionService(userDAO, authorizationService);
        static VerificationService verificationService = new VerificationService(userManagementService, sessionService);
        static AuthenticationService authenticationService = new AuthenticationService();

        // MANAGER
        static LoggingManager loggingManager = new LoggingManager(ffLogging, dsLogging);
        static UpdatePasswordManager updatePasswordManager = new UpdatePasswordManager(loggingManager, userManagementService,verificationService, sessionService, authorizationService);     
        static SendEmailCodeManager sendEmailCodeManger = new SendEmailCodeManager(loggingManager, verificationService);
        static SendPhoneCodeManager sendPhoneCodeManager = new SendPhoneCodeManager(loggingManager, verificationService);
        static VerifyEmailCodeManager verifyEmailCodeManager = new VerifyEmailCodeManager(loggingManager, userManagementService);
        static VerifyPhoneCodeManager verifyPhoneCodeManager = new VerifyPhoneCodeManager(userManagementService, loggingManager, verificationService);
        static RegistrationManager registrationManager = new RegistrationManager(userManagementService, loggingManager);
        static SearchManager searchManager = new SearchManager(searchService, loggingManager, Constants.DicFileRelativePath, Constants.AffFileRelativePath);
        static LogInManager loginManager = new LogInManager(userManagementService, loggingManager, authenticationService, sessionService);

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
                return Ok(await searchManager.GetTotalStoreResultsNumberAsync(searchTerm,
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
                return Ok(await searchManager.GetStoresAsync(searchTerm, latitude,
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
                var storeViewData = await searchManager.GetStoreViewDataAsync(storeId,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                var totalResultsNum = await searchManager.GetTotalIngredientResultsNumberAsync(storeId,
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
                return Ok(await searchManager.GetIngredientsAsync(username, ipAddress, Constants.InitialFailureCount, storeId,
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

                _ = loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress).ConfigureAwait(false);

                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                _ = loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
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
            var result = await registrationManager.RegisterAsync(firstName, lastName, email, username, phoneNumber,
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
            var emailResult = await sendEmailCodeManger.SendEmailCodeAsync(username, email, ipAddress, Constants.InitialFailureCount);

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
            var phoneReuslt = await sendPhoneCodeManager.SendPhoneCodeAsync(username, phoneNumber, ipAddress, Constants.InitialFailureCount);

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
            var result = await verifyEmailCodeManager.VerifyEmailCodeAsync(username, emailCode, ipAddress, Constants.InitialFailureCount);

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
            var result = await verifyPhoneCodeManager.VerifyPhoneCodeAsync(username, phoneCode, ipAddress, phoneNumber,
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
            var result = await loginManager.LogInAsync(username, ipAddress, hashedPassword, Constants.InitialFailureCount);
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
                var salt = await loginManager.GetSaltAsync(username, ipAddress);
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
                return Ok(await updatePasswordManager.SendResetPasswordLinkAsync(username, ipAddress));
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
            var tokenResult = updatePasswordManager.ValidateToken(token, username);

            if (tokenResult.ExceptionOccurred)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!tokenResult.Data)
            {
                return Ok(new { successful = false, message = tokenResult.Message });
            }

            var updateResult =await updatePasswordManager.UpdatePasswordAsync(username, ipAddress, hashedPassword,
                proxyPassword, salt, Constants.InitialFailureCount);

            if(!updateResult.ExceptionOccurred){
                return Ok(new { successful = updateResult.Data, message = updateResult.Message });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
