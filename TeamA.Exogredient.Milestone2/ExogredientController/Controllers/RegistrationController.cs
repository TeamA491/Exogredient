using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExogredientController.Controllers
{
    [Route("api")]
    public class RegistrationController : Controller
    {
        // DAO
        private static readonly UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
        private static readonly AnonymousUserDAO _anonymousUserDAO = new AnonymousUserDAO(Constants.SQLConnection);
        private static readonly LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);

        // SERVICE
        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private static readonly DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
        private static readonly FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);
        private static readonly UserManagementService _userManagementService = new UserManagementService(_userDAO, _anonymousUserDAO, _dsLoggingService, _ffLoggingService, _maskingService);
        private static readonly AuthorizationService _authorizationService = new AuthorizationService();
        private static readonly SessionService _sessionService = new SessionService(_userDAO, _authorizationService);
        private static readonly VerificationService _verificationService = new VerificationService(_userManagementService, _sessionService);

        // MANAGER
        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly SendEmailCodeManager _sendEmailCodeManger = new SendEmailCodeManager(_loggingManager, _verificationService);
        private static readonly SendPhoneCodeManager _sendPhoneCodeManager = new SendPhoneCodeManager(_loggingManager, _verificationService);
        private static readonly VerifyEmailCodeManager _verifyEmailCodeManager = new VerifyEmailCodeManager(_loggingManager, _userManagementService);
        private static readonly VerifyPhoneCodeManager _verifyPhoneCodeManager = new VerifyPhoneCodeManager(_userManagementService, _loggingManager, _verificationService);
        private static readonly RegistrationManager _registrationManager = new RegistrationManager(_userManagementService, _loggingManager);

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
    }
}
