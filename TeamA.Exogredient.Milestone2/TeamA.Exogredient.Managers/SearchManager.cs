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

        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy,
                                                          int failureCount, string username, string ipAddress)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var normalizedTerm = searchBy==Constants.searchByIngredient ? StringUtilityService.NormalizeTerm(searchTerm, this._enUSDicPath, this._enUSAffPath) : searchTerm;
                sw.Stop();
                Console.WriteLine($"INormalize: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                var num = await _searchService.GetTotalStoreResultsNumberAsync(normalizedTerm, latitude, longitude, radius, searchBy).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"IGet: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"ILog: {sw.Elapsed}");
                sw.Reset();
                return num;
            }
            catch(Exception e)
            {
                failureCount += 1;
                Console.WriteLine(e.Message);
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                if(failureCount >= 3)
                {
                    throw e;
                }
                else
                {
                    return await GetTotalStoreResultsNumberAsync(searchTerm, latitude, longitude, radius, searchBy, failureCount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }

        public async Task<int> GetTotalIngredientResultsNumberAsync(int storeId, string ingredientName, int failureCount)
        {
            try
            {
                return await _searchService.GetTotalIngredientResultsNumberAsync(storeId, ingredientName).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                failureCount += 1;
                Console.WriteLine(e.Message);
                if(failureCount >= 3)
                {
                    throw e;
                }
                else
                {
                    return await GetTotalIngredientResultsNumberAsync(storeId, ingredientName, failureCount).ConfigureAwait(false);
                }
            }
        }

        public async Task<List<StoreResult>> GetStoresAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy,
                                              double lastStoreData, int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption, bool fromSmallest, int failureCount, string username, string ipAddress)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var normalizedTerm = searchBy==Constants.searchByIngredient?StringUtilityService.NormalizeTerm(searchTerm, this._enUSDicPath, this._enUSAffPath):searchTerm;
                sw.Stop();
                Console.WriteLine($"Normalize: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                var stores = await _searchService.GetStoresAsync(normalizedTerm, latitude, longitude, radius, searchBy, lastStoreData, lastStoreId, lastPageResultsNum, skipPages, sortOption, fromSmallest).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"Get: {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine($"Log: {sw.Elapsed}");
                sw.Reset();
                return stores;

            }
            catch (Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                failureCount += 1;
                Console.WriteLine(e.Message);
                if(failureCount >= 3)
                {
                    throw e;
                }
                else
                {
                    return await GetStoresAsync(searchTerm, latitude, longitude, radius, searchBy, lastStoreData, lastStoreId, lastPageResultsNum, skipPages,
                        sortOption, fromSmallest, failureCount, username, ipAddress).ConfigureAwait(false);
                }
                
            }
        }

        public async Task<List<IngredientResult>> GetIngredientsAsync(string username, string ipAddress, int failureCount, int storeId, int skipPages, int lastPageResultsNum, string lastIngredientName, string ingredientName)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var ingredients = await _searchService.GetIngredientsAsync(storeId, normalizedIngredient, lastIngredientName, lastPageResultsNum, skipPages).ConfigureAwait(false);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress).ConfigureAwait(false);
                return ingredients;
            }
            catch(Exception e)
            {
                failureCount += 1;
                Console.WriteLine(e.Message);
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                if(failureCount >= 3)
                {
                    throw e;
                }
                else
                {
                    return await GetIngredientsAsync(username, ipAddress, failureCount, storeId, skipPages, lastPageResultsNum, lastIngredientName, ingredientName).ConfigureAwait(false);
                }             
            }
        }

        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId, int failureCount)
        {
            try
            {
                var data = await _searchService.GetStoreViewDataAsync(storeId).ConfigureAwait(false);
                return data;

            }
            catch(Exception e)
            {
                failureCount += 1;
                if(failureCount >= 3)
                {
                    throw e;
                }
                else
                {
                    return await GetStoreViewDataAsync(storeId, failureCount).ConfigureAwait(false);
                }                
            }
        }
    }
}
