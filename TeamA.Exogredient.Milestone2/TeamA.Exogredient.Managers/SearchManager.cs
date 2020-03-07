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
        private readonly string _enUSDicPath;
        private readonly string _enUSAffPath;

        public SearchManager(SearchService searchService, string enUSDicPath, string enUSAffPath)
        {
            _searchService = searchService;
            _enUSDicPath = enUSDicPath;
            _enUSAffPath = enUSAffPath;
        }

        //TODO Replace message to constants
        public async Task<Result<List<StoreResult>>> GetStoresByIngredientSearchAsync(string ingredientName, double latitude, double longitude, double radius, int failureCounter)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var stores = await _searchService.GetStoresByIngredientNameAsync(normalizedIngredient, latitude, longitude, radius).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCounter);

            }
            catch
            {
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCounter + 1);
            }
        }


        public async Task<Result<List<StoreResult>>> GetStoresByStoreSearchAsync(string storeName, double latitude, double longitude, double radius, int failureCounter)
        {
            try
            {
                var lowercaseStore = storeName.ToLower();
                var stores = await _searchService.GetStoresByStoreNameAsync(lowercaseStore, latitude, longitude, radius);

                return SystemUtilityService.CreateResult(Constants.StoresFetchSuccessMessage, stores, false, failureCounter);
            }
            catch
            {
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.StoresFetchUnsuccessMessage, null, true, failureCounter + 1);
            }
        }

        public async Task<Result<List<IngredientResult>>> GetIngredientsAfterSearchAsync(int failureCounter, int storeId, string ingredientName = null)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredientName, this._enUSDicPath, this._enUSAffPath);
                var ingredients = await _searchService.GetIngredientsAsync(storeId, normalizedIngredient).ConfigureAwait(false);
                return SystemUtilityService.CreateResult(Constants.IngredientsFetchSuccessMessage, ingredients, false, failureCounter);
            }
            catch
            {
                return SystemUtilityService.CreateResult<List<IngredientResult>>(Constants.IngredientsFetchUnsuccessMessage, null, true, failureCounter+1);
            }
            
        }
    }
}
