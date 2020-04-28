using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class StoreRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public StoreRecord(string name, double latitude, double longitude, string placeID, string storeDescription)
        {
            _data.Add(Constants.StoreDAOStoreNameColumn, name);
            _data.Add(Constants.StoreDAOLatitudeColumn, latitude);
            _data.Add(Constants.StoreDAOLongitudeColumn, longitude);
            _data.Add(Constants.StoreDAOPlaceIdColumn, placeID);
            _data.Add(Constants.StoreDAOStoreDescriptionColumn, storeDescription);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
