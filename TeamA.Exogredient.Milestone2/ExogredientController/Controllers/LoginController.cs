using System;
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
    public class LoginController : Controller
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
        private static readonly AuthenticationService _authenticationService = new AuthenticationService();

        // MANAGER
        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly LogInManager _loginManager = new LogInManager(_userManagementService, _loggingManager, _authenticationService, _sessionService);

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
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
