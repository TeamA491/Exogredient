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
        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy)
        {
            return await _storeDAO.GetTotalStoreResultsNumberAsync(searchTerm, latitude, longitude, radius, searchBy).ConfigureAwait(false);
        }

        public async Task<int> GetTotalIngredientResultsNumberAsync(int storeId, string ingredientName)
        {
            return await _uploadDAO.GetTotalIngredientResultsNumber(storeId, ingredientName).ConfigureAwait(false);
        }

        public async Task<List<StoreResult>> GetStoresAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy, double lastStoreData, int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption, bool fromSmallest)
        {
            return await _storeDAO.ReadBySearchTermAsync(searchTerm, latitude, longitude, radius, searchBy, lastStoreData, lastStoreId, lastPageResultsNum, skipPages, sortOption, fromSmallest).ConfigureAwait(false);
        }

        public async Task<List<IngredientResult>> GetIngredientsAsync(int storeId, string ingredientName, string lastIngredientName, int lastPageResultsNum, int skipPages)
        {
            return await _uploadDAO.ReadIngredientsByStoreIdAsync(storeId, ingredientName, lastIngredientName, lastPageResultsNum, skipPages).ConfigureAwait(false);
        }

        public async Task<StoreViewData> GetStoreViewDataAsync(int storeId)
        {
            return await _storeDAO.ReadStoreViewDataByIdAsync(storeId).ConfigureAwait(false);
        }

    }
}
