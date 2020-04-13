using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public class SearchService
    {
        private readonly StoreDAO _storeDAO;
        private readonly UploadDAO _uploadDAO;
        public SearchService(StoreDAO storeDAO, UploadDAO uploadDAO)
        {
            _storeDAO = storeDAO;
            _uploadDAO = uploadDAO;
        }

        /// <summary>
        /// Returns the number of total stores that are associated with
        /// searchTerm, latitude, longitude, radius, and searchBy 
        /// </summary>
        /// <param name="searchTerm">Term to search for</param>
        /// <param name="latitude">Latitdue of the center of search</param>
        /// <param name="longitude">Longitude of the center of search</param>
        /// <param name="radius">The radius from the center of search</param>
        /// <param name="searchBy">Method of search. It must be either "ingredient" or "store"</param>
        /// <returns>The number of retrieved stores</returns>
        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm,
            double latitude, double longitude, double radius, string searchBy)
        {
            return await _storeDAO.GetTotalStoreResultsNumberAsync(searchTerm, latitude,
                longitude, radius, searchBy).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the number of total retreived ingredients that are associated with
        /// storeID and ingredient name if not null
        /// </summary>
        /// <param name="storeId">The storeID of a store from which to get ingredients</param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <returns>The number of total retreived ingredients of the store</returns>
        public async Task<int> GetTotalIngredientResultsNumberAsync(int storeId, string ingredientName)
        {
            return await _uploadDAO.GetTotalIngredientResultsNumberAsync(storeId, ingredientName).ConfigureAwait(false);
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
        /// <returns>The list of StoreResult objects</returns>
        public async Task<List<StoreResult>> GetStoresAsync(string searchTerm,
            double latitude, double longitude, double radius, string searchBy,
            double lastStoreData, int lastStoreId, int lastPageResultsNum,
            int skipPages, string sortOption, bool fromSmallest)
        {
            return await _storeDAO.ReadBySearchTermAsync(searchTerm, latitude,
                longitude, radius, searchBy, lastStoreData, lastStoreId,
                lastPageResultsNum, skipPages, sortOption, fromSmallest).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the ingredients of a certain page that are associated with
        /// storeId, ingredientName if not null.
        /// </summary>
        /// <param name="storeId">StoreID of a store</param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <param name="lastIngredientName">
        /// The last ingredient name of of last page that was displayed
        /// (only needed when page changes, otherwise should be null).
        /// </param>
        /// <param name="lastPageResultsNum">
        /// The number of results of last page that was displayed
        /// (only needed when page moved backward).
        /// </param>
        /// <param name="skipPages">The number of pages moved in pagination</param>
        /// <returns>List of IngredientResult objects</returns>
        public async Task<List<IngredientResult>> GetIngredientsAsync(int storeId, string ingredientName,
            string lastIngredientName, int lastPageResultsNum, int skipPages)
        {
            return await _uploadDAO.ReadIngredientsByStoreIdAsync(storeId, ingredientName, lastIngredientName,
                lastPageResultsNum, skipPages).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the data of a store
        /// </summary>
        /// <param name="storeId">The storeID of a store</param>
        /// <returns>The StoreViewData object of the store</returns>
        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId)
        {
            return await _storeDAO.ReadStoreViewDataByIdAsync(storeId).ConfigureAwait(false);
        }

    }
}
