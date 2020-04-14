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


namespace SearchController.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        [EnableCors]
        [HttpGet("getTotalNum")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTotalStoreResultsNumberAsync(
            string searchTerm, double latitude, double longitude, double radius, string searchBy,
            string username, string ipAddress)
        {
            StoreDAO storeDao = new StoreDAO(Constants.SQLConnection);
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.LogSQLConnection);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {
                // HACK ONLY FOR TESTING PURPOSE. MUST BE REMOVED IN PRODUCTION.
                if (searchTerm == "error")
                {
                    throw new Exception("error");
                }

                return Ok(await searchManager.GetTotalStoreResultsNumberAsync(searchTerm,
                    latitude, longitude, radius, searchBy,Constants.InitialFailureCount,
                    username, ipAddress).ConfigureAwait(false));
            }
            catch(Exception e)
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
            StoreDAO storeDao = new StoreDAO(Constants.SQLConnection);
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.LogSQLConnection);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {
                return Ok(await searchManager.GetStoresAsync(searchTerm, latitude,
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
            StoreDAO storeDao = new StoreDAO(Constants.SQLConnection);
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.LogSQLConnection);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);
       
            try
            {
                var storeViewData = await searchManager.GetStoreViewDataAsync(storeId,
                    Constants.InitialFailureCount, username, ipAddress).ConfigureAwait(false);

                var totalResultsNum = await searchManager.GetTotalIngredientResultsNumberAsync(storeId,
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
            StoreDAO storeDao = new StoreDAO(Constants.SQLConnection);
            UploadDAO uploadDAO = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.LogSQLConnection);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {             
                return Ok(await searchManager.GetIngredientsAsync(username, ipAddress, Constants.InitialFailureCount, storeId,
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
            var logdao = new LogDAO(Constants.LogSQLConnection);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);

            try
            {
                
                var image = System.IO.File.OpenRead($"images/store{storeId}.jpg");

                _ = loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress).ConfigureAwait(false);

                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                _ = loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreImageOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            }
        }

    }
}
