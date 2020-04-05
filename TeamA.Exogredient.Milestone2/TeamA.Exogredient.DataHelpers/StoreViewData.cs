using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class StoreViewData
    {
        public int StoreId { get; }
        public string StoreName { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public string StoreDescription { get; }
        public string PlaceId { get; }
        
        public StoreViewData(int storeId, string storeName, double latitude, double longitude, string storeDescription, string placeId)
        {
            StoreId = storeId;
            StoreName = storeName;
            Latitude = latitude;
            Longitude = longitude;
            StoreDescription = storeDescription;
            PlaceId = placeId;
        }
    }
}
