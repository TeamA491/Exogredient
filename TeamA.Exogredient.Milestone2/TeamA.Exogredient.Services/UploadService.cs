using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.AppConstants;
using System.Linq;

namespace TeamA.Exogredient.Services
{
    public class UploadService
    {
        private readonly UploadDAO _uploadDAO;
        public UploadService(UploadDAO uploadDAO)
        {
            _uploadDAO = uploadDAO;
        }

        /// <summary>
        /// Stores the <paramref name="uploadRecord"/> in the data store.
        /// </summary>
        /// <param name="uploadRecord">The record to store.</param>
        /// <returns></returns>
        public async Task<bool> CreateUploadAsync(UploadRecord uploadRecord)
        {
            return await _uploadDAO.CreateAsync(uploadRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns information of the upload identified by the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the upload to get the information of</param>
        /// <returns></returns>
        public async Task<UploadObject> ContinueUploadProgressAsync(int id)
        {
            return (UploadObject)await _uploadDAO.ReadByIdAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies an <paramref name="dto"/>'s information corresponds with various restrictions
        /// </summary>
        /// <param name="dto">The DTO conatining the upload information</param>
        /// <param name="maxPhotoChars">Maximum characters for a photo path</param>
        /// <param name="minPhotoChars">Minimum characters for a photo path</param>
        /// <param name="minimumImageSizeMB">Minimum photo size in MB</param>
        /// <param name="maximumImageSizeMB">Maximum photo size in MB</param>
        /// <param name="validExtensions">List of valid image extensions</param>
        /// <param name="ingredientNameMaxChars">Maximum characters for ingredient name</param>
        /// <param name="ingredientNameMinChars">Minimum characters for ingredient name</param>
        /// <param name="maxIngredientPrice">Maximum value for ingredient price</param>
        /// <param name="descriptionMaxChars">Maximum characters for description</param>
        /// <param name="descriptionMinChars">Minimum characters for description</param>
        /// <param name="validCategories">List of valid categories</param>
        /// <param name="validPriceUnits">List of valid price units</param>
        /// <param name="validTimeBufferMinutes">Allowable time passage from now to post time date</param>
        /// <param name="maxRating">Maximum rating allowed</param>
        /// <param name="minRating">Minimum rating allowed</param>
        /// <returns></returns>
        public VerifyUploadResult VerifyUpload(UploadDTO dto, int maxPhotoChars, int minPhotoChars, double minimumImageSizeMB, double maximumImageSizeMB, List<string> validExtensions,
                                               int ingredientNameMaxChars, int ingredientNameMinChars, double maxIngredientPrice, int descriptionMaxChars,
                                               int descriptionMinChars, List<string> validCategories, List<string> validPriceUnits, int validTimeBufferMinutes,
                                               int maxRating, int minRating)
        {
            // Validate length of image path.
            var validPhotoPath = StringUtilityService.CheckLength(dto.ImagePath, maxPhotoChars, minPhotoChars);

            if (!validPhotoPath)
            {
                return new VerifyUploadResult(Constants.ImagePathInvalidMessage, false);
            }

            // Validate image size in MBs.
            var sizeMB = dto.ImageSize * Constants.ToMBConversionFactor;
            var validSize = sizeMB >= minimumImageSizeMB && sizeMB <= maximumImageSizeMB;

            if (!validSize)
            {
                return new VerifyUploadResult(Constants.ImageNotWithinSizeMessage, false);
            }

            // Validate image extension.
            var fileExtension = "." + dto.ImagePath.Split('.').Last();
            var validExt = false;

            foreach (var ext in validExtensions)
            {
                validExt = validExt || (fileExtension.ToLower().Equals(ext.ToLower()));
            }

            if (!validExt)
            {
                return new VerifyUploadResult(Constants.ExtensionNotValidMessage, false);
            }

            // Validate category.
            var validCat = false;

            foreach (var cat in validCategories)
            {
                validCat = validCat || (dto.Category.ToLower().Equals(cat.ToLower()));
            }

            if (!validCat)
            {
                return new VerifyUploadResult(Constants.CategoryNotValidMessage, false);
            }

            // Validate name length, if it has a value.
            if (!dto.Name.Equals(Constants.NoValueString))
            {
                var validNameLength = StringUtilityService.CheckLength(dto.Name, ingredientNameMaxChars, ingredientNameMinChars);

                if (!validNameLength)
                {
                    return new VerifyUploadResult(Constants.IngredientNameLengthInvalidMessage, false);
                }
            }

            // Validate price value, if it has a value.
            if (!dto.Price.Equals(Constants.NoValueDouble))
            {
                var validPrice = dto.Price > Constants.NoValueDouble && dto.Price <= maxIngredientPrice;

                if (!validPrice)
                {
                    return new VerifyUploadResult(Constants.PriceInvalidMessage, false);
                }
            }

            // Validate description length, if it has a value.
            if (!dto.Description.Equals(Constants.NoValueString))
            {
                var validDescriptionLength = StringUtilityService.CheckLength(dto.Description, descriptionMaxChars, descriptionMinChars);

                if (!validDescriptionLength)
                {
                    return new VerifyUploadResult(Constants.DescriptionLengthInvalidMessage, false);
                }
            }

            // Validate price unit, if it has a value.
            if (!dto.PriceUnit.Equals(Constants.NoValueString))
            {
                var validUnit = false;

                foreach (var unit in validPriceUnits)
                {
                    validUnit = validUnit || (dto.PriceUnit.ToLower().Equals(unit.ToLower()));
                }

                if (!validUnit)
                {
                    return new VerifyUploadResult(Constants.PriceUnitNotValidMessage, false);
                }
            }

            // Validate timestamp, if it has a value.
            if (!dto.Time.Equals(Constants.NoValueDatetime))
            {
                var validTime = (DateTime.Now - dto.Time).TotalMinutes <= validTimeBufferMinutes;

                if (!validTime)
                {
                    return new VerifyUploadResult(Constants.TimeNotValidMessage, false);
                }
            }

            // Validate rating, if it has a value.
            if (!dto.Rating.Equals(Constants.NoValueInt))
            {
                var validRating = dto.Rating >= minRating && dto.Rating <= maxRating;

                if (!validRating)
                {
                    return new VerifyUploadResult(Constants.InvalidRatingMessage, false);
                }
            }

            // Success.
            return new VerifyUploadResult(Constants.NoValueString, true);
        }

        /// <summary>
        /// Delete the uploads using a list of uploads.
        /// </summary>
        /// <param name="ids">Ids of the upload to delete.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUploadsAsync(List<int> ids)
        {
            return await _uploadDAO.DeleteByIdsAsync(ids).ConfigureAwait(false);   
        }


        /// <summary>
        /// Get all the upload vote's for a user.
        /// </summary>
        /// <param name="username">User to get votes from.</param>
        /// <returns>List of ProfileScoreResult.</returns>
        public async Task<List<ProfileScoreResult>> getUploadVotesAsync(string username)
        {
            return await _uploadDAO.ReadUploadVotesAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Edit the upvote value for an upload. 
        /// </summary>
        /// <param name="voteValue"> The number that is going to be added to the current upvote value.</param>
        /// <param name="uploadId"> The id used to find the specific upload.</param>
        /// <returns> A bool for the successful completion of the operation.</returns>
        public async Task<bool> IncrementUpvotesonUpload(int voteValue, int uploadId)
        {
            return await _uploadDAO.IncrementUpvotesonUpload(voteValue, uploadId).ConfigureAwait(false);
        }
        /// <summary>
        /// Edit the downvote value for an upload. 
        /// </summary>
        /// <param name="voteValue"> The number that is going to be added to the current downvote value.</param>
        /// <param name="uploadId"> The id used to find the specific upload.</param>
        /// <returns> A bool for the successful completion of the operation.</returns>
        public async Task<bool> IncrementDownvotesonUpload(int voteValue, int uploadId)
        {
            return await _uploadDAO.IncrementDownvotesonUpload(voteValue, uploadId).ConfigureAwait(false);
        }
        /// <summary>
        /// Return all uploads based on ingredientname and storeId.
        /// </summary>
        /// <param name="ingredientName"></param>
        /// <param name="storeId"></param>
        /// <param name="pagination"></param>
        /// <returns> A list of uploads associated with the ingredientName and storeId.</returns>
        public async Task<List<UploadResult>> ReadUploadsByIngredientNameandStoreId(string ingredientName, int storeId, int pagination)
        {
            return await _uploadDAO.ReadUploadsByIngredientNameandStoreId(ingredientName, storeId, pagination).ConfigureAwait(false);
        }
        /// <summary>
        /// Get the pagination size for ingredient view. 
        /// </summary>
        /// <param name="ingredientName"> The name of the ingredient</param>
        /// <param name="storeId"> The store id of the store.</param>
        /// <returns> An integer holding the number of a certain ingredient at a specific store. </returns>
        public async Task<int> GetIngredientViewPaginationSize(string ingredientName, int storeId)
        {
            return await _uploadDAO.ReadIngredientViewPaginationSize(ingredientName, storeId).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the recent uploads by a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> GetRecentByUploaderAsync(string username, int pagination)
        {
            return await _uploadDAO.ReadRecentByUploaderAsync(username, pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the In Progress upload for a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> GetInProgressUploadsByUploaderAsync(string username, int pagination)
        {
            return await _uploadDAO.ReadInProgressUploadsByUploaderAsync(username, pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Check that a user is the owner of a list of uploads.
        /// </summary>
        /// <param name="ids">Ids of the uploads to check.</param>
        /// <param name="owner">User to perform operation on.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadOwnerAsync(List<int> ids, string owner)
        {
            return await _uploadDAO.CheckUploadsOwnerAsync(ids, owner).ConfigureAwait(false);
        }

        public async Task<bool> UpdateUploadAsync(UploadRecord uploadRecord)
        {
            return await _uploadDAO.UpdateAsync(uploadRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Check that an upload exists.
        /// </summary>
        /// <param name="ids">Ids of the upload to check.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadsExistenceAsync(List<int> ids)
        {
            return await _uploadDAO.CheckUploadsExistenceAsync(ids).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the pagination size for in progress uploads of a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <returns>int representing pagination size.</returns>
        public async Task<int> GetInProgressPaginationSizeAsync(string username)
        {
            return await _uploadDAO.GetInProgressPaginationSizeAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the pagination size for recent uploads of a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <returns>int representing pagination size.</returns>
        public async Task<int> GetRecentPaginationSizeAsync(string username)
        {
            return await _uploadDAO.GetRecentPaginationSizeAsync(username).ConfigureAwait(false);
        }

        public async Task<bool> ReportUpload(UploadRecord uploadRecord)
        {
            throw new NotImplementedException();
        }
    }
}
