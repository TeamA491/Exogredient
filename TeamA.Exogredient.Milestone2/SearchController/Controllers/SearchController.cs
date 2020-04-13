using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SearchController.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        int failureCount = 0;

        [EnableCors]
        [HttpGet("getTotalNum")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTotalStoreResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy,
                                                                         string username, string ipAddress)
        {
            var connection = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";
            var map = "server=localhost;user=root;database=mapping_table;port=3306;password=1234567890";
            var log = "server=localhost;user=root;database=exogredient_log;port=3306;password=1234567890";
            StoreDAO storeDao = new StoreDAO(connection);
            UploadDAO uploadDAO = new UploadDAO(connection);
            var logdao = new LogDAO(log);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(map);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {
                return Ok(await searchManager.GetTotalStoreResultsNumberAsync(searchTerm, latitude, longitude, radius, searchBy,
                    failureCount, username, ipAddress).ConfigureAwait(false));
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getStoreResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoresByStoreNameAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy,
            double lastStoreData, int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption, bool fromSmallest, string username, string ipAddress)
        {
            Console.WriteLine(searchBy);
            Console.WriteLine(fromSmallest);

            var connection = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";
            var map = "server=localhost;user=root;database=mapping_table;port=3306;password=1234567890";
            var log = "server=localhost;user=root;database=exogredient_log;port=3306;password=1234567890";
            StoreDAO storeDao = new StoreDAO(connection);
            UploadDAO uploadDAO = new UploadDAO(connection);
            var logdao = new LogDAO(log);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(map);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {
                return Ok(await searchManager.GetStoresAsync(searchTerm, latitude, longitude, radius, searchBy, lastStoreData, lastStoreId,
                    lastPageResultsNum, skipPages, sortOption, fromSmallest, failureCount, username, ipAddress).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [EnableCors]
        [HttpGet("storeViewData")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStoreViewDataAsync(int storeId, string ingredientName = null)
        {
            var connection = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";
            var map = "server=localhost;user=root;database=mapping_table;port=3306;password=1234567890";
            var log = "server=localhost;user=root;database=exogredient_log;port=3306;password=1234567890";
            StoreDAO storeDao = new StoreDAO(connection);
            UploadDAO uploadDAO = new UploadDAO(connection);
            var logdao = new LogDAO(log);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(map);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);
       
            try
            {
                var storeViewData = await searchManager.GetStoreViewDataAsync(storeId, failureCount).ConfigureAwait(false);
                var totalResultsNum = await searchManager.GetTotalIngredientResultsNumberAsync(storeId, ingredientName, failureCount).ConfigureAwait(false);
                return Ok(new Tuple<int, StoreViewData>(totalResultsNum, storeViewData));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [EnableCors]
        [HttpGet("getIngredientResults")]
        [Produces("application/json")]
        public async Task<IActionResult> GetIngredientsAsync(string username, string ipAddress,
            int storeId, int skipPages, int lastPageResultsNum, string lastIngredientName = null, string ingredientName = null)
        {
            Console.WriteLine(username);
            Console.WriteLine(ipAddress);
            Console.WriteLine(failureCount);
            Console.WriteLine(storeId);
            Console.WriteLine(skipPages);
            Console.WriteLine(lastIngredientName);
            Console.WriteLine(ingredientName);

            var connection = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";
            var map = "server=localhost;user=root;database=mapping_table;port=3306;password=1234567890";
            var log = "server=localhost;user=root;database=exogredient_log;port=3306;password=1234567890";
            StoreDAO storeDao = new StoreDAO(connection);
            UploadDAO uploadDAO = new UploadDAO(connection);
            var logdao = new LogDAO(log);
            var searchService = new SearchService(storeDao, uploadDAO);
            var mapdao = new MapDAO(map);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "en_US/en_US.aff";
            string enUSDic = "en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            try
            {             
                return Ok(await searchManager.GetIngredientsAsync(username, ipAddress, failureCount, storeId,
                    skipPages, lastPageResultsNum, lastIngredientName, ingredientName).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [EnableCors]
        [HttpGet("image")]
        public IActionResult GetImage(int id)
        {
            Console.WriteLine("Image retrieved");
            try
            {
                var image = System.IO.File.OpenRead($"images/store{id}.jpg");
                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

    }
}
