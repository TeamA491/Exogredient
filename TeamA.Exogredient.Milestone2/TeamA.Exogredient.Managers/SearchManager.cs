using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SearchManager
    {
        private readonly SearchService _searchService;
        private readonly LoggingManager _loggingManager;
        private readonly string _enUSDicPath;
        private readonly string _enUSAffPath;

        public SearchManager(SearchService searchService, LoggingManager loggingManager, string enUSDicPath, string enUSAffPath)
        {
            _searchService = searchService;
            _enUSDicPath = enUSDicPath;
            _enUSAffPath = enUSAffPath;
            _loggingManager = loggingManager;
        }

        //TODO Replace message to constants
        public async Task<Result<List<StoreResult>>> GetStoresByIngredientNameAsync(string ingredientName, double latitude, double longitude,
                                                                                double radius, int pagination, int failureCount, string username, string ipAddress)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                sw.Stop();
                Console.WriteLine($"Normalize: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                var stores = await _searchService.GetStoresByIngredientNameAsync(normalizedIngredient, latitude, longitude, radius, pagination).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"Get: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"Log: {sw.Elapsed}");
                sw.Reset();
                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCount);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCount + 1);
            }
        }


        public async Task<Result<List<StoreResult>>> GetStoresByStoreNameAsync(string storeName, double latitude, double longitude,
                                                                           double radius, int pagination, int failureCount, string username, string ipAddress)

        {
            try
            {
                var lowercaseStore = storeName.ToLower();
                var stores = await _searchService.GetStoresByStoreNameAsync(lowercaseStore, latitude, longitude, radius, pagination);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByStoreOperation, username, ipAddress).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCount);
            }
            catch(Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByStoreOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCount + 1);
            }
        }

        public async Task<Result<List<IngredientResult>>> GetIngredientsAsync(string username, string ipAddress, int failureCount, int storeId, int pagination, string ingredientName)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var ingredients = await _searchService.GetIngredientsAsync(storeId, normalizedIngredient, pagination).ConfigureAwait(false);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.IngredientsFetchSuccessMessage, ingredients, false, failureCount);
            }
            catch(Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<IngredientResult>>(Constants.IngredientsFetchUnsuccessMessage, null, true, failureCount+1);
            }
        }

        public async Task<Result<StoreViewData>> GetStoreViewDataAsync(int storeId, int failureCount)
        {
            try
            {
                var data = await _searchService.GetStoreViewDataAsync(storeId).ConfigureAwait(false);
                return SystemUtilityService.CreateResult("Successfully fetched Store View data", data, false, failureCount);

            }
            catch(Exception e)
            {
                return SystemUtilityService.CreateResult<StoreViewData>("Error fetching Store View data", null, false, failureCount+1);
            }
        }


    }
}
