using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadResult
    {
        public int UploadId { get; }
        public int StoreId { get; }
        public string IngredientName { get; }
        public string Uploader { get; }
        public string PostTimeDate { get; }
        public string Description { get; }
        public string Rating { get; }
        public string Photo { get; }
        public double Price { get;  }
        public int Upvote { get; }
        public int Downvote { get; }
        public bool InProgress { get;}

        public UploadResult(int uploadId, int storeId, string ingredientName, string uploader, string postTimeDate, string description, string rating, 
                            string photo, double price, int upvote, int downvote, bool inProgress)
        {
            UploadId = uploadId;
            StoreId = storeId;
            IngredientName = ingredientName;
            Uploader = uploader;
            PostTimeDate = postTimeDate;
            Description = description;
            Rating = rating;
            Photo = photo;
            Price = price;
            Upvote = upvote;
            Downvote = downvote;
            InProgress = inProgress;
        }
    }
}
