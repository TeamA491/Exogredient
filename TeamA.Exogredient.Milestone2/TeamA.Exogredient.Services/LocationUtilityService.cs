using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class LocationUtilityService
    {
        public static Tuple<double, double> GetImageLatitudeAndLongitude(Bitmap image)
        {
            PropertyItem latitudeProperty = image.GetPropertyItem(0x0002);

            // Typecasts are NOT redundant
            var latDegrees = (double)(BitConverter.ToUInt32(latitudeProperty.Value, 0)) / (double)(BitConverter.ToUInt32(latitudeProperty.Value, 4));
            var latMinutes = (double)(BitConverter.ToUInt32(latitudeProperty.Value, 8)) / (double)(BitConverter.ToUInt32(latitudeProperty.Value, 12));
            var latSeconds = (double)(BitConverter.ToUInt32(latitudeProperty.Value, 16)) / (double)(BitConverter.ToUInt32(latitudeProperty.Value, 20));
            var latitudeDD = latDegrees + (latMinutes / Constants.MinutesInAnHour) + (latSeconds / Constants.SecondsInAnHour);

            PropertyItem longitudeProperty = image.GetPropertyItem(0x0004);

            // Typecasts are NOT redundant
            var longDegrees = (double)(BitConverter.ToUInt32(longitudeProperty.Value, 0)) / (double)(BitConverter.ToUInt32(longitudeProperty.Value, 4));
            var longMinutes = (double)(BitConverter.ToUInt32(longitudeProperty.Value, 8)) / (double)(BitConverter.ToUInt32(longitudeProperty.Value, 12));
            var longSeconds = (double)(BitConverter.ToUInt32(longitudeProperty.Value, 16)) / (double)(BitConverter.ToUInt32(longitudeProperty.Value, 20));
            var longitudeDD = longDegrees + (longMinutes / Constants.MinutesInAnHour) + (longSeconds / Constants.SecondsInAnHour);

            return Tuple.Create(latitudeDD, longitudeDD);
        }

        public static List<Tuple<double, double>> GetStorePolygon(double latitude, double longitude)
        {
            var result = new List<Tuple<double, double>>();

            result.Add(Tuple.Create(latitude - Constants.LatDegree400ft, longitude + Constants.LongDegree400ft));
            result.Add(Tuple.Create(latitude + Constants.LatDegree400ft, longitude + Constants.LongDegree400ft));
            result.Add(Tuple.Create(latitude + Constants.LatDegree400ft, longitude - Constants.LongDegree400ft));
            result.Add(Tuple.Create(latitude - Constants.LatDegree400ft, longitude - Constants.LongDegree400ft));

            return result;
        }

        public static bool CheckLocationWithinPolygon(double latitude, double longitude, List<Tuple<double, double>> polygon)
        {
            var angle = 0.0;
            var numCoords = polygon.Count;
            
            for (int i = 0; i < numCoords; i++)
            {
                var point1_lat = polygon[i].Item1 - latitude;
                var point1_long = polygon[i].Item2 - longitude;
                var point2_lat = polygon[(i + 1) % numCoords].Item1 - latitude;
                var point2_long = polygon[(i + 1) % numCoords].Item2 - longitude;

                angle += Angle2D(point1_lat, point1_long, point2_lat, point2_long);
            }

            if (Math.Abs(angle) < Constants.Pi)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static double Angle2D(double y1, double x1, double y2, double x2)
        {
            var theta1 = Math.Atan2(y1, x1);
            var theta2 = Math.Atan2(y2, x2);
            var dtheta = theta2 - theta1;

            while (dtheta > Constants.Pi)
            {
                dtheta -= 2 * Constants.Pi;
            }

            while (dtheta < -Constants.Pi)
            {
                dtheta += 2 * Constants.Pi;
            }

            return dtheta;
        }
    }
}
