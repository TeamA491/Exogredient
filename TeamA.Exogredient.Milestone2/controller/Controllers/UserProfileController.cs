using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Exceptions;

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
        public async Task<IActionResult> GetProfileScores(string username)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetProfileScore(username,"localhost", 0 , null).ConfigureAwait(false));
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
        public async Task<IActionResult> GetRecentUploads(string username, int pagination)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetRecentUploads(username, pagination, "localhost", 0, null).ConfigureAwait(false));

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
        public async Task<IActionResult> GetInProgressUploads(string username, int pagination)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetInProgressUploads(username, pagination, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> DeleteUpload(string performingUser, string uploadId)
        {
            try
            {
                return Ok(await _userProfileManager.DeleteUploads(new List<string>() { uploadId}, performingUser, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> GetSaveList(string username, int pagination)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetSaveList(username, pagination, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> DeleteSaveList(string username, int storeId, string ingredient) 
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.DeleteSaveList(username, storeId, ingredient, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> GetSaveListPaginationSize(string username)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetSaveListPaginationSize(username, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> GetInProgressUploadPaginationSize(string username)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetInProgressUploadPaginationSize(username, "localhost", 0, null).ConfigureAwait(false));
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
        public async Task<IActionResult> GetRecentUploadPaginationSize(string username)
        {
            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await _userProfileManager.GetRecentUploadPaginationSize(username, "localhost", 0, null).ConfigureAwait(false));
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