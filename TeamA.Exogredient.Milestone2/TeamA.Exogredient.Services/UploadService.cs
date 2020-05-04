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

        public async Task<bool> CreateUploadAsync(UploadRecord uploadRecord)
        {
            return await _uploadDAO.CreateAsync(uploadRecord).ConfigureAwait(false);
        }

        public async Task<UploadObject> ContinueUploadProgressAsync(int id)
        {
            return (UploadObject)await _uploadDAO.ReadByIdAsync(id).ConfigureAwait(false);
        }

        public VerifyUploadResult VerifyUpload(UploadDTO dto, int maxPhotoChars, int minPhotoChars, double minimumImageSizeMB, double maximumImageSizeMB, List<string> validExtensions,
                                               int ingredientNameMaxChars, int ingredientNameMinChars, double maxIngredientPrice, int descriptionMaxChars,
                                               int descriptionMinChars, List<string> validCategories, List<string> validPriceUnits, int validTimeBufferMinutes,
                                               int maxRating, int minRating)
        {
            var validPhotoPath = StringUtilityService.CheckLength(dto.ImagePath, maxPhotoChars, minPhotoChars);

            if (!validPhotoPath)
            {
                return new VerifyUploadResult(Constants.ImagePathInvalidMessage, false);
            }

            var sizeMB = dto.ImageSize * Constants.ToMBConversionFactor;
            var validSize = sizeMB >= minimumImageSizeMB && sizeMB <= maximumImageSizeMB;

            if (!validSize)
            {
                return new VerifyUploadResult(Constants.ImageNotWithinSizeMessage, false);
            }

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

            var validCat = false;

            foreach (var cat in validCategories)
            {
                validCat = validCat || (dto.Category.ToLower().Equals(cat.ToLower()));
            }

            if (!validCat)
            {
                return new VerifyUploadResult(Constants.CategoryNotValidMessage, false);
            }

            if (!dto.Name.Equals(Constants.NoValueString))
            {
                var validNameLength = StringUtilityService.CheckLength(dto.Name, ingredientNameMaxChars, ingredientNameMinChars);

                if (!validNameLength)
                {
                    return new VerifyUploadResult(Constants.IngredientNameLengthInvalidMessage, false);
                }
            }

            if (!dto.Price.Equals(Constants.NoValueDouble))
            {
                var validPrice = dto.Price > Constants.NoValueDouble && dto.Price <= maxIngredientPrice;

                if (!validPrice)
                {
                    return new VerifyUploadResult(Constants.PriceInvalidMessage, false);
                }
            }

            if (!dto.Description.Equals(Constants.NoValueString))
            {
                var validDescriptionLength = StringUtilityService.CheckLength(dto.Description, descriptionMaxChars, descriptionMinChars);

                if (!validDescriptionLength)
                {
                    return new VerifyUploadResult(Constants.DescriptionLengthInvalidMessage, false);
                }
            }
            
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
            
            if (!dto.Time.Equals(Constants.NoValueDatetime))
            {
                var validTime = (DateTime.Now - dto.Time).TotalMinutes <= validTimeBufferMinutes;

                if (!validTime)
                {
                    return new VerifyUploadResult(Constants.TimeNotValidMessage, false);
                }
            }
            
            if (!dto.Rating.Equals(Constants.NoValueInt))
            {
                var validRating = dto.Rating >= minRating && dto.Rating <= maxRating;

                if (!validRating)
                {
                    return new VerifyUploadResult(Constants.InvalidRatingMessage, false);
                }
            }

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
