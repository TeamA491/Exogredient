using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using Microsoft.AspNetCore.Http;

namespace IngredientViewController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientViewController : ControllerBase
    {
      [HttpGet("GetIngredients")]
      [Produces("application/json")]
      public async Task<IActionResult> SearchIngredientAtStoreAsync(string ingredientName, int storeID, int pagination, string username, string ipAddress)
        {

            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager( uploadService, loggingManager, userManagementService);

            return Ok(await ingredientManager.GetUploadsByIngredientNameandStoreId(ingredientName, storeID, pagination, Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false));
        }

        [HttpPost("Upvote")]
        public async Task<bool> UpvoteIngredient(int uploadId, string username, string ipAddress)
        {
            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager(uploadService, loggingManager, userManagementService);

            return await ingredientManager.EditUpvotesonUpload(Constants.PositiveVote, uploadId, Constants.InitialFailureCount, username, ipAddress);
        }

        [HttpPost("UndoUpvote")]
        public async Task<bool> UndoUpvoteIngredient(int uploadId, string username, string ipAddress)
        {
            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager(uploadService, loggingManager, userManagementService);

            return await ingredientManager.EditUpvotesonUpload(Constants.NegativeVote, uploadId, Constants.InitialFailureCount, username, ipAddress);
        }

        [HttpPost("Downvote")]
        public async Task<bool> DownvoteIngredient(int uploadId, string username, string ipAddress)
        {
            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager(uploadService, loggingManager, userManagementService);

            return await ingredientManager.EditDownvotesonUpload(Constants.PositiveVote, uploadId, Constants.InitialFailureCount, username, ipAddress);
        }

        [HttpPost("UndoDownvote")]
        public async Task<bool> UndoDownvoteIngredient(int uploadId, string username, string ipAddress)
        {
            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager(uploadService, loggingManager, userManagementService);

            return await ingredientManager.EditDownvotesonUpload(Constants.NegativeVote, uploadId, Constants.InitialFailureCount, username, ipAddress);
        }

        [HttpGet("GetIngredientViewPaginationSize")]
        [Produces("application/json")]
        public async Task<IActionResult> GetIngredientViewPaginationSize(string ingredientName, int storeId, string username, string ipAddress)
        {
            // New up DAL
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var mapDao = new MapDAO(Constants.MapSQLConnection);
            var logDao = new LogDAO(Constants.SQLConnection);
            var userDao = new UserDAO(Constants.SQLConnection);
            var anonymousUserDao = new AnonymousUserDAO(Constants.SQLConnection);

            // New up Service
            var uploadService = new UploadService(uploadDAO);
            var maskingService = new MaskingService(mapDao);
            var ffLoggingService = new FlatFileLoggingService(maskingService);
            var dsLoggingService = new DataStoreLoggingService(logDao, maskingService);
            var userManagementService = new UserManagementService(userDao, anonymousUserDao, dsLoggingService, ffLoggingService, maskingService);

            // New up Managers
            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var ingredientManager = new IngredientManager(uploadService, loggingManager, userManagementService);

            try
            {
                // Return status code of 200 as well as the content.
                return Ok(await ingredientManager.GetIngredientViewPaginationSize(ingredientName, storeId, Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false));
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
