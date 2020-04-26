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
        static UserDAO userDAO = new UserDAO(Constants.SQLConnection);
        static IPAddressDAO ipAddressDAO = new IPAddressDAO(Constants.SQLConnection);
        static LogDAO logdao = new LogDAO(Constants.NOSQLConnection);
        static MapDAO mapdao = new MapDAO(Constants.MapSQLConnection);
        static MaskingService mask = new MaskingService(mapdao);
        static DataStoreLoggingService dsLogging = new DataStoreLoggingService(logdao, mask);
        static FlatFileLoggingService ffLogging = new FlatFileLoggingService(mask);
        static UserManagementService usermanagementService = new UserManagementService(userDAO, ipAddressDAO, dsLogging, ffLogging, mask);
        static LoggingManager loggingManager = new LoggingManager(ffLogging, dsLogging);

        [EnableCors]
        [HttpGet("register")]
        [Produces("application/json")]
        public async Task<IActionResult> RegisterAsync(string firstName, string lastName,
                                                      string email, string username, string phoneNumber,
                                                      string ipAddress, string hashedPassword, string salt,
                                                      string proxyPassword)
        {
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


        [EnableCors]
        [HttpGet("sendEmailCode")]
        [Produces("application/json")]
        public async Task<IActionResult> SendEmailCodeAsync(string username, string email, string ipAddress)
        {
            var verificationService = new VerificationService(usermanagementService);
            var sendEmailCodeManger = new SendEmailCodeManager(loggingManager,verificationService);

            var emailResult = await sendEmailCodeManger.SendEmailCodeAsync(username,email,ipAddress,Constants.InitialFailureCount);

            if(emailResult.ExceptionOccurred)
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
            var verificationService = new VerificationService(usermanagementService);
            var sendPhoneCodeManager = new SendPhoneCodeManager(loggingManager, verificationService);

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
            var verifyEmailCodeManager = new VerifyEmailCodeManager(loggingManager,usermanagementService);

            var result = await verifyEmailCodeManager.VerifyEmailCodeAsync(username,emailCode,ipAddress,Constants.InitialFailureCount);

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
            var verificationService = new VerificationService(usermanagementService);
            var verifyPhoneCodeManager = new VerifyPhoneCodeManager(usermanagementService, loggingManager, verificationService);

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
    }
}
