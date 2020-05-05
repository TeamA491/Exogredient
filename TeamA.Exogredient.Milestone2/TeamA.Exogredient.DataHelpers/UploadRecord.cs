using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public UploadRecord(DateTime postTimeDate, string uploader, int storeID, string description,
                            string rating, string photo, double price, string priceUnit, string ingredientName,
                            int upvote, int downvote, int inProgress, string category)
        {
            _data.Add(Constants.UploadDAOPostTimeDateColumn, postTimeDate);
            _data.Add(Constants.UploadDAOUploaderColumn, uploader);
            _data.Add(Constants.UploadDAOStoreIdColumn, storeID);
            _data.Add(Constants.UploadDAODescriptionColumn, description);
            _data.Add(Constants.UploadDAORatingColumn, rating);
            _data.Add(Constants.UploadDAOPhotoColumn, photo);
            _data.Add(Constants.UploadDAOPriceColumn, price);
            _data.Add(Constants.UploadDAOPriceUnitColumn, priceUnit);
            _data.Add(Constants.UploadDAOIngredientNameColumn, ingredientName);
            _data.Add(Constants.UploadDAOUpvoteColumn, upvote);
            _data.Add(Constants.UploadDAODownvoteColumn, downvote);
            _data.Add(Constants.UploadDAOInProgressColumn, inProgress);
            _data.Add(Constants.UploadDAOCategoryColumn, category);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
