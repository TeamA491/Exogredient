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

namespace RegisterationController.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationController : Controller
    {
        [EnableCors]
        [HttpGet("register")]
        [Produces("application/json")]
        public async Task<IActionResult> RegisterAsync(string firstName, string lastName,
                                                      string email, string username, string phoneNumber,
                                                      string ipAddress, string hashedPassword, string salt,
                                                      string proxyPassword)
        {
            var userDAO = new UserDAO(Constants.SQLConnection);
            var ipAddressDAO = new IPAddressDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.NOSQLConnection);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var ffLogging = new FlatFileLoggingService(mask);
            var usermanagementService = new UserManagementService(userDAO, ipAddressDAO,dsLogging,ffLogging,mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            var registrationManager = new RegistrationManager(usermanagementService,loggingManager);

            var result = await registrationManager.RegisterAsync(firstName,lastName,email,username,phoneNumber,
                ipAddress,hashedPassword,salt,proxyPassword,Constants.InitialFailureCount);

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
