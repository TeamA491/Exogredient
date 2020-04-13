using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Managers;
using System.Text;
using System.Reflection;
using System.Linq;
using WeCantSpell.Hunspell;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {

        public class Store
        {
            public string name { get; set; }
            //public IEnumerable<Ingredient> ingredients { get; set; }
        }

        public async static Task Main()
        {

            
            var connection = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";
            var map = "server=localhost;user=root;database=mapping_table;port=3306;password=1234567890";
            var log = "server=localhost;user=root;database=exogredient_log;port=3306;password=1234567890";
            StoreDAO storeDao = new StoreDAO(connection);
            UploadDAO uploadDAO = new UploadDAO(connection);
            var logdao = new LogDAO(log);
            var searchService = new SearchService(storeDao,uploadDAO);
            var mapdao = new MapDAO(map);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao,mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            string enUSAff = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.aff";
            string enUSDic = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.dic";
            var search = new SearchManager(searchService,loggingManager, enUSDic, enUSAff);
            /*
            var result = await search.IngredientSearchAsync("beef", 12.00, 12.00, 23.37, 0).ConfigureAwait(false);
            var result1 = await search.StoreSearchAsync("store", 12.00, 12.00, 23.37, 0).ConfigureAwait(false);

            foreach(var store in result.Data)
            {
                Console.WriteLine(store.StoreId);
                Console.WriteLine(store.StoreName);
                Console.WriteLine(store.IngredientNum);
                Console.WriteLine(store.Distance);
            }

            foreach(var store in result1.Data)
            {
                Console.WriteLine(store.StoreId);
                Console.WriteLine(store.StoreName);
                Console.WriteLine(store.IngredientNum);
                Console.WriteLine(store.Distance);
            }
            */

            var result = await search.GetIngredientsAsync("testing","127.1.1.0",0, 1,1);
            var result2 = await search.GetIngredientsAsync("testing","127.1.1.0",0, 2,1, "beef");

            foreach (var ingredient in result.Data)
            {
                Console.WriteLine(ingredient.IngredientName + " " + ingredient.AveragePrice + " " + ingredient.UploadNum);
            }

            Console.WriteLine("");
            foreach (var ingredient in result2.Data)
            {
                Console.WriteLine(ingredient.IngredientName + " " + ingredient.AveragePrice + " " + ingredient.UploadNum);
            }

            /*
            string enUSAff = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.aff";
            string enUSDic = "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US/en_US.dic";
            var dictionary = WordList.CreateFromFiles(enUSDic, enUSAff);
            //Console.WriteLine(dictionary.Check("young"));
            //Console.WriteLine(StringUtilityService.Stem("cilantro"));
            //var list = dictionary.Suggest("cherimoya").ToList();
            Console.WriteLine(StringUtilityService.NormalizeTerm("YOUNG APPLES CHERIMOYA",enUSDic,enUSAff));
            */

            //Console.WriteLine(StringUtilityService.Stem("cherimoya"));
            //Console.WriteLine(StringUtilityService.AutoCorrect("cherimoya",enUSDic,enUSAff));
            //Console.WriteLine(dictionary.Check("cherimoya"));

        }
    }
}
