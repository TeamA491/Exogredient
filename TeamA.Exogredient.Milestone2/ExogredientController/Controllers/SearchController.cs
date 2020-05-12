using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using ticket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExogredientController.Controllers
{
    [Route("api")]
    public class SearchController : Controller
    {
        // DAO
        private static readonly StoreDAO _storeDAO = new StoreDAO(Constants.SQLConnection);
        private static readonly UploadDAO _uploadDAO = new UploadDAO(Constants.SQLConnection);
        private static readonly LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);

        // SERVICE
        private static readonly SearchService _searchService = new SearchService(_storeDAO, _uploadDAO);
        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private static readonly DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
        private static readonly FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);

        // MANAGER
        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly SearchManager _searchManager = new SearchManager(_searchService, _loggingManager, Constants.DicFileRelativePath, Constants.AffFileRelativePath);

        // SEARCH
        [EnableCors]
        [HttpGet("getTotalNum")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTotalStoreResultsNumberAsync(
            string searchTerm, double latitude, double longitude, double radius, string searchBy,
            string username, string ipAddress)
        {
            try
            {
                return Ok(await _searchManager.GetTotalStoreResultsNumberAsync(searchTerm,
                    latitude, longitude, radius, searchBy, Constants.InitialFailureCount,
                    username, ipAddress).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getStoreResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoresAsync(string searchTerm, double latitude,
            double longitude, double radius, string searchBy, double lastStoreData,
            int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption,
            bool fromSmallest, string username, string ipAddress)
        {
            try
            {
                return Ok(await _searchManager.GetStoresAsync(searchTerm, latitude,
                    longitude, radius, searchBy, lastStoreData, lastStoreId,
                    lastPageResultsNum, skipPages, sortOption, fromSmallest,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("storeViewData")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoreViewDataAsync(int storeId, string username,
            string ipAddress, string ingredientName = null)
        {
            try
            {
                var storeViewData = await _searchManager.GetStoreViewDataAsync(storeId,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                var totalResultsNum = await _searchManager.GetTotalIngredientResultsNumberAsync(storeId,
                    ingredientName, Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                return Ok(new Tuple<int, StoreViewData>(totalResultsNum, storeViewData));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getIngredientResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetIngredientsAsync(string username,
            string ipAddress, int storeId, int skipPages, int lastPageResultsNum,
            string lastIngredientName = null, string ingredientName = null)
        {
            try
            {
                return Ok(await _searchManager.GetIngredientsAsync(username, ipAddress, Constants.InitialFailureCount, storeId,
                    skipPages, lastPageResultsNum, lastIngredientName, ingredientName).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [EnableCors]
        [HttpGet("storeImage")]
        public IActionResult GetStoreImage(int storeId, string username, string ipAddress)
        {
            try
            {

                var image = System.IO.File.OpenRead($"images/store{storeId}.jpg");

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress).ConfigureAwait(false);

                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            }
        }

    }
}
