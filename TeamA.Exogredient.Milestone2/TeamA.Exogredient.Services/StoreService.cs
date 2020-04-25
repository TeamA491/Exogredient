using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class StoreService
    {
        private readonly StoreDAO _storeDAO;
        public StoreService(StoreDAO storeDAO)
        {
            _storeDAO = storeDAO;
        }
    }
}
