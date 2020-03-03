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

        public async Task<List<StoreResult>> SearchByIngredientAsync(string ingredient, double latitude, double longitude, double radius)
        {
            return await _storeDAO.ReadByIngredientAsync(ingredient,latitude,longitude,radius).ConfigureAwait(false);

        }
    }
}
