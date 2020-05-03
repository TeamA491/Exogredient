using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace UploadController
{
    public class UploadPost
    {
        public Bitmap Image { get; set; }
        public string Category { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }
        public DateTime PostTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public double Price { get; set; }
        public string PriceUnit { get; set; }
        public string FileExtension { get; set; }
        public int ImageSize { get; set; }

        public UploadPost(Bitmap image, string category, string username, string ipAddress, DateTime postTime,
                          string name, string description, int rating, double price, string priceUnit, string fileExtension, int imageSize)
        {
            Image = image;
            Category = category;
            Username = username;
            IPAddress = ipAddress;
            PostTime = postTime;
            Name = name;
            Description = description;
            Rating = rating;
            Price = price;
            PriceUnit = priceUnit;
            FileExtension = fileExtension;
            ImageSize = imageSize;
        }
    }
}
