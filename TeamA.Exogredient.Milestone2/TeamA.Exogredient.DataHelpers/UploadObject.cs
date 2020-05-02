using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadObject : IDataObject
    {
        public DateTime PostTimeDate { get; }
        public string Uploader { get; }
        public int StoreID { get; }
        public string Description { get; }
        public int Rating { get; }
        public string Photo { get; } // ImagePath
        public double Price { get; }
        public string PriceUnit { get; }
        public string IngredientName { get; }
        public int Upvote { get; }
        public int Downvote { get; }
        public bool InProgress { get; }
        public string Category { get; }

        public UploadObject(DateTime postTimeDate, string uploader, int storeID, string description,
                            int rating, string photo, double price, string priceUnit, string ingredientName,
                            int upvote, int downvote, bool inProgress, string category)
        {
            PostTimeDate = postTimeDate;
            Uploader = uploader;
            StoreID = storeID;
            Description = description;
            Rating = rating;
            Photo = photo;
            Price = price;
            PriceUnit = priceUnit;
            IngredientName = ingredientName;
            Upvote = upvote;
            Downvote = downvote;
            InProgress = inProgress;
            Category = category;
        }
    }
}
