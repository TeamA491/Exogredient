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

namespace LoginController.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        static UserDAO userDAO = new UserDAO(Constants.SQLConnection);
        static IPAddressDAO ipAddressDAO = new IPAddressDAO(Constants.SQLConnection);
        static LogDAO logdao = new LogDAO(Constants.NOSQLConnection);
        static MapDAO mapdao = new MapDAO(Constants.MapSQLConnection);
        static MaskingService mask = new MaskingService(mapdao);
        static DataStoreLoggingService dsLogging = new DataStoreLoggingService(logdao, mask);
        static FlatFileLoggingService ffLogging = new FlatFileLoggingService(mask);
        static UserManagementService usermanagementService = new UserManagementService(userDAO, ipAddressDAO, dsLogging, ffLogging, mask);
        static LoggingManager loggingManager = new LoggingManager(ffLogging, dsLogging);
        static AuthenticationService authenService = new AuthenticationService();
        static AuthorizationService authzService = new AuthorizationService();
        static SessionService sessionService = new SessionService(userDAO, authzService);
        static LogInManager loginManager = new LogInManager(usermanagementService, loggingManager, authenService, sessionService);

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
                return Ok(new { successful = authenResult.IsSuccessful,
                    token = authenResult.Token, userType = authenResult.UserType });
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
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
