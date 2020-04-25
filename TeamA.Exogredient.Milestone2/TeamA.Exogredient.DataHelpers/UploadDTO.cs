using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadDTO
    {
        public Bitmap Image { get; }
        public string Name { get; }
        public DateTime Time { get; }
        public string Username { get; }
        public string Description { get; }
        public string Category { get; }
        public int Rating { get; }
        public float Price { get; }
        public string PriceUnit { get; }
        public float Latitude { get; }
        public float Longitude { get; }

        public UploadDTO(Bitmap image, string name, DateTime time, string username,
                         string description, string category, int rating, float price, string priceUnit,
                         float latitude, float longitude)
        {
            Image = image;
            Name = name;
            Time = time;
            Username = username;
            Description = description;
            Category = category;
            Rating = rating;
            Price = price;
            PriceUnit = priceUnit;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
