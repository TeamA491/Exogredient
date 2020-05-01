using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Exceptions;
using System.Reflection.Metadata;
using TeamA.Exogredient.AppConstants;

namespace controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileManager _userProfileManager;
        public UserProfileController(UserProfileManager userProfileManager)
        {
            _userProfileManager = userProfileManager;
        }

        [HttpGet("ProfileScore/{username}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetProfileScoresAsync(string username, string ipAddress = Constants.LocalHost)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetProfileScoreAsync(username,ipAddress, 0 , null).ConfigureAwait(false));
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

        [HttpDelete("Upload/{performingUser}/{uploadId}")]
        public async Task<IActionResult> DeleteUploadAsync(string performingUser, int uploadId, string ipAddress = Constants.LocalHost)
        {
            try
            {
                return Ok(await _userProfileManager.DeleteUploadsAsync(new List<int>() { uploadId}, performingUser, ipAddress, 0, null).ConfigureAwait(false));
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
            catch (NotAuthorizedException na)
            {
                // Return forbidden status code when user is not allowed to delete.
                return StatusCode(StatusCodes.Status403Forbidden, na.Message);
            }
            catch
            {
                // Return generic server error for all other exceptions.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

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