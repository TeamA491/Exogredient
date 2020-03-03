using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SearchIngredientManager
    {
        private readonly SearchService _searchService;
        private readonly string _enUSDicPath;
        private readonly string _enUSAffPath;

        public SearchIngredientManager(SearchService searchService, string enUSDicPath, string enUSAffPath)
        {
            _searchService = searchService;
            _enUSDicPath = enUSDicPath;
            _enUSAffPath = enUSAffPath;
        }

        //TODO Replace message to constants
        public async Task<Result<List<StoreResult>>> Search(string ingredient, double latitude, double longitude, double radius, int currentNumExceptions)
        {
            try
            {
                //if()
                var normalizedIngredient = StringUtilityService.NormalizeTerm(ingredient, this._enUSDicPath, this._enUSAffPath);
                var stores = await _searchService.SearchByIngredientAsync(normalizedIngredient, latitude, longitude, radius).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.IngredientSearchSuccessMessage, stores, false, currentNumExceptions);

            }
            catch
            {
                return SystemUtilityService.CreateResult<List<StoreResult>>(Constants.IngredientSearchUnsuccessMessage, null, true, currentNumExceptions + 1);
            }
        }
    }
}
