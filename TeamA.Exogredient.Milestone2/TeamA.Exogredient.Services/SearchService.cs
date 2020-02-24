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
        public SearchService(StoreDAO storeDAO)
        {
            _storeDAO = storeDAO;
        }

        public async Task<Dictionary<SearchResultStoreObject,Geocode>> SearchByIngredientAsync(string ingredient)
        {
            return await _storeDAO.ReadByIngredientAsync(ingredient).ConfigureAwait(false);

        }
    }
}
