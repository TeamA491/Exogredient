using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadDTO
    {
        public string ImagePath { get; }
        public Bitmap Image { get; }
        public string Name { get; }
        public DateTime Time { get; }
        public string Username { get; }
        public string Description { get; }
        public string Category { get; }
        public int Rating { get; }
        public double Price { get; }
        public string PriceUnit { get; }
        public int ImageSize { get;  }

        public UploadDTO(string imagePath, Bitmap image, string category, string name, DateTime time,
                         string username, string description, int rating, double price, string priceUnit, int imageSize)
        {
            ImagePath = imagePath;
            Image = image;
            Category = category;
            Name = name;
            Time = time;
            Username = username;
            Description = description;
            Rating = rating;
            Price = price;
            PriceUnit = priceUnit;
            ImageSize = imageSize;
        }
    }
}
