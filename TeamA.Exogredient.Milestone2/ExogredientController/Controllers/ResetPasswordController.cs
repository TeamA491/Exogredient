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
    public class ResetPasswordController : Controller
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
        private static readonly UpdatePasswordManager _updatePasswordManager = new UpdatePasswordManager(_loggingManager, _userManagementService, _verificationService, _sessionService, _authorizationService);

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
            catch
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

            var updateResult = await _updatePasswordManager.UpdatePasswordAsync(username, ipAddress, hashedPassword,
                proxyPassword, salt, Constants.InitialFailureCount);

            if (!updateResult.ExceptionOccurred)
            {
                return Ok(new { successful = updateResult.Data, message = updateResult.Message });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
