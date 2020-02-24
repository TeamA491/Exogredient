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
        public async Task<Result<List<SearchResultStoreObject>>> Search(string ingredient, double latitude, double longitude, double radius, int currentNumExceptions)
        {
            try
            {
                var normalizedIngredient = StringUtilityService.AutoCorrect(StringUtilityService.Stem(ingredient), this._enUSDicPath, this._enUSAffPath);
                var storeDict = await _searchService.SearchByIngredientAsync(normalizedIngredient).ConfigureAwait(false);
                var centerLocation = new Geocode(latitude, longitude);
                var storeList = new List<SearchResultStoreObject>();
                foreach(KeyValuePair<SearchResultStoreObject,Geocode>pair in storeDict)
                {
                    if(pair.Value.ComputeDistance(centerLocation) <= radius)
                    {
                        storeList.Add(pair.Key);
                    }
                }

                return SystemUtilityService.CreateResult(Constants.IngredientSearchSuccessMessage, storeList, false, currentNumExceptions);

            }
            catch
            {
                return SystemUtilityService.CreateResult<List<SearchResultStoreObject>>(Constants.IngredientSearchUnsuccessMessage, null, true, currentNumExceptions + 1);
            }
        }
    }
}
