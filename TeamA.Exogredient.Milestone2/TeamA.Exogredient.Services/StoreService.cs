using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;

using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.Photos.Request;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using Location = GoogleApi.Entities.Places.Search.NearBy.Request.Location;
using System.Linq;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public class StoreService
    {
        private readonly StoreDAO _storeDAO;
        public StoreService(StoreDAO storeDAO)
        {
            _storeDAO = storeDAO;
        }

        //https://www.jerriepelser.com/tutorials/airport-explorer/google-places/retrieving/
        public async Task<int> FindStoreAsync(double latitude, double longitude)
        {
            var result = await _storeDAO.FindStoreAsync(latitude, longitude).ConfigureAwait(false);

            if (result == Constants.NoStoreFoundCode)
            {
                var searchResponse = await GooglePlaces.NearBySearch.QueryAsync(new PlacesNearBySearchRequest
                {
                    Key = Constants.GoogleApiKey,
                    Location = new Location(latitude, longitude),
                    Radius = 60
                }).ConfigureAwait(false);

                // If we did not get a good response, or the list of results are empty, store not found
                if (!searchResponse.Status.HasValue || searchResponse.Status.Value != Status.Ok || !searchResponse.Results.Any())
                {
                    return result;
                }

                // Get the first result
                var resultsEnumerator = searchResponse.Results.GetEnumerator();
                resultsEnumerator.Reset();
                resultsEnumerator.MoveNext();
                resultsEnumerator.MoveNext();

                var nearbyResult = resultsEnumerator.Current;

                var placeId = nearbyResult.PlaceId;

                // Execute the details request
                var detailsResonse = await GooglePlaces.Details.QueryAsync(new PlacesDetailsRequest
                {
                    Key = Constants.GoogleApiKey,
                    PlaceId = placeId
                });

                // If we did not get a good response then get out of here
                if (!detailsResonse.Status.HasValue || detailsResonse.Status.Value != Status.Ok)
                {
                    throw new Exception(Constants.GooglePlacesApiFailureMessage);
                }

                // Retrieve the details
                var detailsResult = detailsResonse.Result;

                var formattedAddress = detailsResult.FormattedAddress;
                var storeName = detailsResult.Name;

                // Closest address is a city, not a store
                if (!formattedAddress.Any(char.IsDigit))
                {
                    return result;
                }

                var geometry = detailsResult.Geometry;
                var storeLat = geometry.Location.Latitude;
                var storeLong = geometry.Location.Longitude;

                var record = new StoreRecord(storeName, storeLat, storeLong, placeId, Constants.NoValueString);

                result = await _storeDAO.CreateAsync(record).ConfigureAwait(false);

                return result;
            }
            else
            {
                return result;
            }
        }
    }
}
