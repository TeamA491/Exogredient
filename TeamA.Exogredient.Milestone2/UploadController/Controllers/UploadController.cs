using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;
using System.Drawing;
using Image = Google.Cloud.Vision.V1.Image;
using System.IO;

namespace UploadController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly UploadManager _uploadManager;

        public UploadController(UploadManager uploadManager)
        {
            _uploadManager = uploadManager;
        }

        //[HttpPost("NewUpload")]
        //[Produces("application/json")]
        //public async Task<IActionResult> CreateUploadAsync()
        //{

        //}

        //[HttpPost("NewUpload")]
        //[Produces("application/json")]
        //public async Task<IActionResult> DraftUploadAsync()
        //{

        //}


        [HttpGet("Vision/{username}/{collection}/{ipAddress}")]
        [Produces("application/json")]
        public async Task<IActionResult> AnalyzeImageAsync(string username, IFormCollection collection, string ipAddress)
        {
            try
            {
                IFormFile file = collection.Files.Last();

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var image = Image.FromStream(memoryStream);
                    return Ok(await _uploadManager.AnalyzeImageAsync(username, image, ipAddress, Constants.NoValueInt).ConfigureAwait(false));
                }
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("DraftUpload/{username}/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> ContinueUploadProgressAsync(string username, int id, string ipAddress = Constants.LocalHost)
        {
            try
            {
                return Ok(await _uploadManager.ContinueUploadProgressAsync(username, id, ipAddress, Constants.NoValueInt).ConfigureAwait(false));
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("DeleteUpload/{username}/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteUploadAsync(string username, int id, string ipAddress = Constants.LocalHost)
        {
            try
            {
                return Ok(await _uploadManager.DeleteUploadAsync(username, id, ipAddress, Constants.NoValueInt).ConfigureAwait(false));
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}