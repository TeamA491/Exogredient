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

        //public async Task<List<StoreResult>> GetStoresByIngredientNameAsync(string ingredientName, double latitude, double longitude, double radius, int pagination)
        //{
        //    return await _storeDAO.ReadByIngredientNameAsync(ingredientName,latitude,longitude,radius,pagination).ConfigureAwait(false);

        //}

        //public async Task<List<StoreResult>> GetStoresByStoreNameAsync(string storeName, double latitude, double longitude, double radius, int pagination)
        //{
        //    return await _storeDAO.ReadByStoreNameAsync(storeName, latitude, longitude, radius, pagination);
        //}
        public async Task<int> GetTotalResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy)
        {
            return await _storeDAO.GetTotalResultsNumberAsync(searchTerm, latitude, longitude, radius, searchBy).ConfigureAwait(false);
        }

        public async Task<List<StoreResult>> GetStoresAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy, double lastStoreData, int lastStoreId, string sortOption, bool fromSmallest)
        {
            return await _storeDAO.ReadBySearchTermAsync(searchTerm, latitude, longitude, radius, searchBy, lastStoreData, lastStoreId, sortOption, fromSmallest).ConfigureAwait(false);
        }

        public async Task<List<IngredientResult>> GetIngredientsAsync(int storeId, string ingredientName, string lastIngredientName)
        {
            return await _uploadDAO.ReadIngredientsByStoreIdAsync(storeId, ingredientName, lastIngredientName).ConfigureAwait(false);
        }

        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId)
        {
            return await _storeDAO.ReadStoreViewDataByIdAsync(storeId).ConfigureAwait(false);
        }

    }
}
