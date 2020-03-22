using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class StoreViewData
    {
        public string StoreName { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public string StoreDescription { get; }
        public string PlaceId { get; }
        //public int ResultsNum { get; }
        
        public StoreViewData(string storeName, double latitude, double longitude, string storeDescription, string placeId)
        {
            StoreName = storeName;
            Latitude = latitude;
            Longitude = longitude;
            StoreDescription = storeDescription;
            PlaceId = placeId;
            //ResultsNum = resultsNum;
        }
    }
}
