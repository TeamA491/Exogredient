using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;
using Image = Google.Cloud.Vision.V1.Image;
using System.IO;
using Google.Cloud.Vision.V1;
using System.Drawing;
using System.Drawing.Imaging;
using TeamA.Exogredient.DataHelpers.Upload;

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

        [HttpPost("NewUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUploadAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                formFile.OpenReadStream();

                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                var post = new UploadPost(image, data[Constants.CategoryKey], data[Constants.UsernameKey], data[Constants.IPAddressKey],
                                          DateTime.Now, data[Constants.NameKey], data[Constants.DescriptionKey],
                                          Int32.Parse(data[Constants.RatingKey]), Double.Parse(data[Constants.PriceKey]), data[Constants.PriceUnitKey], data[Constants.ExtensionKey], Int32.Parse(data[Constants.ImageSizeKey]));

                var result = await _uploadManager.CreateUploadAsync(post, Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new CreateResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch
            {
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("DraftUpload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> DraftUploadAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                var test = data[Constants.PriceKey];

                var test2 = test.ToString();

                var post = new UploadPost(image, data[Constants.CategoryKey],
                                          data[Constants.UsernameKey],
                                          data[Constants.IPAddressKey],
                                          Constants.NoValueDatetime,
                                          data[Constants.NameKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.NameKey].ToString(),
                                          data[Constants.DescriptionKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.DescriptionKey].ToString(),
                                          data[Constants.RatingKey].ToString().Equals(Constants.NaN) ? Constants.NoValueInt : Int32.Parse(data[Constants.NameKey]),
                                          data[Constants.PriceKey].ToString().Equals(Constants.NaN) ? Constants.NoValueDouble : Double.Parse(data[Constants.NameKey].ToString()),
                                          data[Constants.PriceUnitKey].ToString().Equals(Constants.NaN) ? Constants.NoValueString : data[Constants.PriceUnitKey].ToString(),
                                          data[Constants.ExtensionKey],
                                          Int32.Parse(data[Constants.ImageSizeKey]));

                var result = await _uploadManager.DraftUploadAsync(post, Constants.NoValueInt).ConfigureAwait(false);

                return Ok(new CreateResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred, Success = result.Data });
            }
            catch (Exception e)
            {
                var lol = e.Message;
                // Return generic server error.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost("Vision")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> AnalyzeImageAsync(IFormCollection data, IFormFile formFile)
        {
            try
            {
                formFile.OpenReadStream();

                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var image = new Bitmap(memoryStream);

                Byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);

                    bytes = memoryStream.ToArray();
                }

                var img = Image.FromBytes(bytes);

                var result = await _uploadManager.AnalyzeImageAsync(img, data[Constants.UsernameKey], data[Constants.IPAddressKey], Constants.NoValueInt).ConfigureAwait(false);

                var suggestionsArray = result.Data.Suggestions.ToArray();


                return Ok(new AnalyzeResponse() { Message = result.Message, ExceptionOccurred = result.ExceptionOccurred,
                                                  Suggestions = suggestionsArray, Category = result.Data.Category, Name = result.Data.Name});
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