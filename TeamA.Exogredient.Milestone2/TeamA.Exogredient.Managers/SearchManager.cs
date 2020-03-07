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

        //TODO Replace message to constants
        public async Task<Result<List<StoreResult>>> GetStoresByIngredientAsync(string ingredientName, double latitude, double longitude,
                                                                                double radius, int pagination, int failureCounter, string username, string ipAddress)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var stores = await _searchService.GetStoresByIngredientNameAsync(normalizedIngredient, latitude, longitude, radius, pagination).ConfigureAwait(false);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCounter);

            }
            catch(Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByIngredientOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCounter + 1);
            }
        }


        public async Task<Result<List<StoreResult>>> GetStoresByStoreAsync(string storeName, double latitude, double longitude,
                                                                           double radius, int pagination, int failureCounter, string username, string ipAddress)

        {
            try
            {
                var lowercaseStore = storeName.ToLower();
                var stores = await _searchService.GetStoresByStoreNameAsync(lowercaseStore, latitude, longitude, radius, pagination);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByStoreOperation, username, ipAddress).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCounter);
            }
            catch(Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetStoresByStoreOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCounter + 1);
            }
        }

        public async Task<Result<List<IngredientResult>>> GetIngredientsAsync(string username, string ipAddress, int failureCounter, int storeId, int pagination, string ingredientName = null)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var ingredients = await _searchService.GetIngredientsAsync(storeId, normalizedIngredient, pagination).ConfigureAwait(false);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.IngredientsFetchSuccessMessage, ingredients, false, failureCounter);
            }
            catch(Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.GetIngredientsOperation, username, ipAddress, e.Message).ConfigureAwait(false);
                return SystemUtilityService.CreateResult<List<IngredientResult>>(Constants.IngredientsFetchUnsuccessMessage, null, true, failureCounter+1);
            }
            
        }
    }
}
