using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ExogredientController : Controller
    {
        static UserDAO userDAO = new UserDAO(Constants.SQLConnection);
        static AnonymousUserDAO anonymousUserDAO = new AnonymousUserDAO(Constants.SQLConnection);
        static LogDAO logdao = new LogDAO(Constants.NOSQLConnection);
        static MapDAO mapdao = new MapDAO(Constants.MapSQLConnection);
        static MaskingService mask = new MaskingService(mapdao);
        static DataStoreLoggingService dsLogging = new DataStoreLoggingService(logdao, mask);
        static FlatFileLoggingService ffLogging = new FlatFileLoggingService(mask);
        static UserManagementService userManagementService = new UserManagementService(userDAO, anonymousUserDAO, dsLogging, ffLogging, mask);
        static LoggingManager loggingManager = new LoggingManager(ffLogging, dsLogging);
        static AuthorizationService authorizationService = new AuthorizationService();
        static SessionService sessionService = new SessionService(userDAO, authorizationService);
        static VerificationService verificationService = new VerificationService(userManagementService, sessionService);
        static UpdatePasswordManager updatePasswordManager = new UpdatePasswordManager(loggingManager, userManagementService,
                                                                                       verificationService, sessionService, authorizationService);

        [EnableCors]
        [HttpGet("sendResetLink")]
        [Produces("application/json")]
        public async Task<IActionResult> sendResetLinkAsync(string username, string ipAddress)
        {
            try
            {
                return Ok(await updatePasswordManager.SendResetPasswordLinkAsync(username, ipAddress));
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
