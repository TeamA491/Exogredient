using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns the number of total stores that are associated with
        /// searchTerm, latitude, longitude, radius, and searchBy. 
        /// </summary>
        /// <param name="searchTerm">Term to search for</param>
        /// <param name="latitude">Latitdue of the center of search</param>
        /// <param name="longitude">Longitude of the center of search</param>
        /// <param name="radius">The radius from the center of search</param>
        /// <param name="searchBy">Method of search. It must be either "ingredient" or "store"</param>
        /// <param name="failureCount">Number of failures</param>
        /// <param name="username">Username who invoked this action</param>
        /// <param name="ipAddress">IP Address of the user</param>
        /// <returns>The number of retrieved stores</returns>
        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm,
            double latitude, double longitude, double radius, string searchBy,
            int failureCount, string username, string ipAddress)
        {
            try
            {
                var normalizedTerm = searchBy==Constants.searchByIngredient ?
                    StringUtilityService.NormalizeTerm(searchTerm, this._enUSDicPath, this._enUSAffPath) : searchTerm;

                var num = await _searchService.GetTotalStoreResultsNumberAsync
                    (normalizedTerm, latitude, longitude, radius, searchBy).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetTotalStoreResultsNumberOperation, username, ipAddress).ConfigureAwait(false);

                return num;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetTotalStoreResultsNumberOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                failureCount += 1;

                if (failureCount >= Constants.MaxSearchRelatedAttempts)
                {
                    throw e;
                }
                else
                {
                    return await GetTotalStoreResultsNumberAsync(searchTerm, latitude, longitude,
                        radius, searchBy, failureCount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Returns the number of total retreived ingredients that are associated with
        /// storeID and ingredient name if not null
        /// </summary>
        /// <param name="storeId">The storeID of a store from which to get ingredients</param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <param name="failureCount">Number of failures</param>
        /// <param name="username">Username who invoked this action</param>
        /// <param name="ipAddress">IP Address of the user</param>
        /// <returns>The number of retreived ingredients</returns>
        public async Task<int> GetTotalIngredientResultsNumberAsync(int storeId,
            string ingredientName, int failureCount, string username, string ipAddress)
        {
            try
            {
                 int num = await _searchService.GetTotalIngredientResultsNumberAsync(storeId,
                           ingredientName).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetTotalIngredientResultsNumberOperation,
                    username, ipAddress).ConfigureAwait(false);

                return num;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetTotalIngredientResultsNumberOperation, username,
                    ipAddress,e.Message).ConfigureAwait(false);

                failureCount += 1;

                if(failureCount >= Constants.MaxSearchRelatedAttempts)
                {
                    throw e;
                }
                else
                {
                    return await GetTotalIngredientResultsNumberAsync(storeId, ingredientName,
                        failureCount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Returns the stores of a certain page of pagination that are associated with
        /// searchTerm, latitude, longitude, radius, and searchBy.
        /// </summary>
        /// <param name="searchTerm"> Term to search for</param>
        /// <param name="latitude">Latitdue of the center of search</param>
        /// <param name="longitude">Longitude of the center of search</param>
        /// <param name="radius">The radius from the center of search</param>
        /// <param name="searchBy">Method of search. Must be either "ingredient" or "store"</param>
        /// <param name="lastStoreData">
        /// The data (distance or num of ingredients) of the last store of last page that was displayed
        /// (only needed when page changes, otherwise shoud be -1).
        /// </param>
        /// <param name="lastStoreId">
        /// The storeID of the last store of last page that was displayed
        /// (only needed when page changes, otherwise should be 0).
        /// </param>
        /// <param name="lastPageResultsNum">
        /// The number of results of last page that was displayed
        /// (only needed when page moved backward, otherwise it's ignored).
        /// </param>
        /// <param name="skipPages">The number of pages moved in pagination</param>
        /// <param name="sortOption">The sort option. Must either "distance" or "ingredientNum"</param>
        /// <param name="fromSmallest">
        /// If true, results are sorted from smallest(ascending).
        /// Otherwise, from biggest(descending).
        /// </param>
        /// <param name="failureCount">Number of failures</param>
        /// <param name="username">Username who invoked this action</param>
        /// <param name="ipAddress">IP Address of the user</param>
        /// <returns>The list of StoreResult objects</returns>
        public async Task<List<StoreResult>> GetStoresAsync(string searchTerm,
            double latitude, double longitude, double radius, string searchBy,
            double lastStoreData, int lastStoreId, int lastPageResultsNum,
            int skipPages, string sortOption, bool fromSmallest, int failureCount,
            string username, string ipAddress)
        {
            try
            {
                var normalizedTerm = searchBy==Constants.searchByIngredient ?
                    StringUtilityService.NormalizeTerm(searchTerm, this._enUSDicPath, this._enUSAffPath):searchTerm;

                var stores = await _searchService.GetStoresAsync(normalizedTerm,
                    latitude, longitude, radius, searchBy, lastStoreData, lastStoreId,
                    lastPageResultsNum, skipPages, sortOption, fromSmallest).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    $"{Constants.SearchOperation}/{searchBy}/{searchTerm}", username, ipAddress).ConfigureAwait(false);

                return stores;

            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    $"{Constants.SearchOperation}/{searchBy}/{searchTerm}", username, ipAddress, e.Message).ConfigureAwait(false);

                failureCount += 1;

                if(failureCount >= Constants.MaxSearchRelatedAttempts)
                {
                    throw e;
                }
                else
                {
                    return await GetStoresAsync(searchTerm, latitude,
                        longitude, radius, searchBy, lastStoreData, lastStoreId,
                        lastPageResultsNum, skipPages, sortOption, fromSmallest,
                        failureCount, username, ipAddress).ConfigureAwait(false);
                }
                
            }
        }

        /// <summary>
        /// Reads the ingredients of a certain page of pagination that are associated with
        /// storeId, ingredientName if not null.
        /// </summary>
        /// <param name="username">Username who invoked this action</param>
        /// <param name="ipAddress">IP Address of the user</param>
        /// <param name="failureCount">Number of failures</param>
        /// <param name="storeId">StoreID of a store</param>
        /// <param name="skipPages">The number of pages moved in pagination</param>
        /// <param name="lastPageResultsNum">
        /// The number of results of last page that was displayed
        /// (only needed when page moved backward).
        /// </param>
        /// <param name="lastIngredientName">
        /// The last ingredient name of of last page that was displayed
        /// (only needed when page changes, otherwise should be null).
        /// </param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <returns>List of IngredientResult objects</returns>
        public async Task<List<IngredientResult>> GetIngredientsAsync(string username,
            string ipAddress, int failureCount, int storeId, int skipPages,
            int lastPageResultsNum, string lastIngredientName, string ingredientName)
        {
            try
            {
                var normalizedIngredient =
                    StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);

                var ingredients = await _searchService.GetIngredientsAsync(storeId, normalizedIngredient,
                    lastIngredientName, lastPageResultsNum, skipPages).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetIngredientsOperation, username, ipAddress).ConfigureAwait(false);

                return ingredients;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetIngredientsOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                failureCount += 1;

                if (failureCount >= Constants.MaxSearchRelatedAttempts)
                {
                    throw e;
                }
                else
                {
                    return await GetIngredientsAsync(username, ipAddress, failureCount, storeId, skipPages,
                        lastPageResultsNum, lastIngredientName, ingredientName).ConfigureAwait(false);
                }             
            }
        }

        /// <summary>
        /// Gets the data of a store
        /// </summary>
        /// <param name="storeId">The storeID of a store</param>
        /// <param name="failureCount">Number of failures</param>
        /// <param name="username">Username who invoked this action</param>
        /// <param name="ipAddress">IP Address of the user</param>
        /// <returns>The StoreViewData object of the store</returns>
        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId,
            int failureCount, string username, string ipAddress)
        {
            try
            {
                var data = await _searchService.GetStoreViewDataAsync(storeId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreViewDataOperation, username, ipAddress).ConfigureAwait(false);

                return data;

            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetStoreViewDataOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                failureCount += 1;

                if(failureCount >= Constants.MaxSearchRelatedAttempts)
                {
                    throw e;
                }
                else
                {
                    return await GetStoreViewDataAsync(storeId, failureCount,username,ipAddress).ConfigureAwait(false);
                }                
            }
        }
    }
}
