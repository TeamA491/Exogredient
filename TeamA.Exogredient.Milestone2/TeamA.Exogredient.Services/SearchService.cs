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

        public async Task<List<StoreResult>> GetStoresByIngredientNameAsync(string ingredientName, double latitude, double longitude, double radius, int pagination)
        {
            return await _storeDAO.ReadByIngredientNameAsync(ingredientName,latitude,longitude,radius,pagination).ConfigureAwait(false);

        }

        public async Task<List<StoreResult>> GetStoresByStoreNameAsync(string storeName, double latitude, double longitude, double radius, int pagination)
        {
            return await _storeDAO.ReadByStoreNameAsync(storeName, latitude, longitude, radius, pagination);
        }

        public async Task<List<IngredientResult>> GetIngredientsAsync(int storeId, string ingredientName, int pagination)
        {
            return await _uploadDAO.ReadIngredientsByStoreIdAsync(storeId, ingredientName, pagination).ConfigureAwait(false);
        }

        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId)
        {
            return await _storeDAO.ReadStoreViewDataByIdAsync(storeId);
        }

    }
}
