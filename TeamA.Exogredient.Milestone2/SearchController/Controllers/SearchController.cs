using System;
using System.Collections.Generic;
using System.Linq;
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
        [EnableCors]
        [HttpGet("ingredient")]
        public async Task<IEnumerable<StoreResult>> GetStoreByIngredientAsync(string ingredient, double latitude, double longitude,
            double radius, int pagination, int failureCount, string username, string ipAddress)
        {
            //Console.WriteLine($"ingredient: {ingredient}");
            //Console.WriteLine($"latitude: {latitude}");
            //Console.WriteLine($"longitude: {longitude}");
            //Console.WriteLine($"radius: {radius}");
            //Console.WriteLine($"pagination: {pagination}");
            //Console.WriteLine($"failure: {failureCount}");
            //Console.WriteLine($"username: {username}");
            //Console.WriteLine($"ip: {ipAddress}");
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
            string enUSAff = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.aff";
            string enUSDic = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.dic";
            SearchManager searchManager = new SearchManager(searchService, loggingManager, enUSDic, enUSAff);

            var result = await searchManager.GetStoresByIngredientAsync(ingredient, latitude, longitude, radius, pagination, failureCount, username, ipAddress).ConfigureAwait(false);
            var stores = result.Data;
            return stores;
        }
    }
}
