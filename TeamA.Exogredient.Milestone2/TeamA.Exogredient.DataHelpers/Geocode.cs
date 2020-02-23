using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class Geocode
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Geocode(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        //https://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula
        public double ComputeDistance(Geocode geocode)
        {
            var kiloToMile = 0.62137119224;
            var earthRadius = 6371;
            var tempCalculation = 0.5 - Math.Cos(DegreeToRadian(geocode.Latitude - this.Latitude)) / 2 +
                                  Math.Cos(DegreeToRadian(this.Latitude)) * Math.Cos(DegreeToRadian(geocode.Latitude)) *
                                  (1 - Math.Cos(DegreeToRadian(geocode.Longitude - this.Longitude))) / 2;
            var distance = (2 * earthRadius) * Math.Asin(Math.Sqrt(tempCalculation));

            return distance * kiloToMile;
        }

        private double DegreeToRadian(double degree)
        {
            return degree * (Math.PI / 180);
        }
    }
}
