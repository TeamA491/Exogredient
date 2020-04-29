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
        public string Name { get; }
        public DateTime Time { get; }
        public string Username { get; }
        public string Description { get; }
        public string Category { get; }
        public int Rating { get; }
        public double Price { get; }
        public string PriceUnit { get; }

        public UploadDTO(string imagePath, string category, string name, DateTime time,
                         string username, string description, int rating, double price, string priceUnit)
        {
            ImagePath = imagePath;
            Category = category;
            Name = name;
            Time = time;
            Username = username;
            Description = description;
            Rating = rating;
            Price = price;
            PriceUnit = priceUnit;
        }
    }
}
