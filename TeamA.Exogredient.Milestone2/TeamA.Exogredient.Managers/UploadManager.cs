using System;
using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;
using System.Drawing;
using TeamA.Exogredient.AppConstants;
using Image = Google.Cloud.Vision.V1.Image;
using System.IO;
using UploadController;
using Microsoft.AspNetCore.Http;

namespace TeamA.Exogredient.Managers
{
    public class UploadManager
    {
        private readonly LoggingManager _loggingManager;
        private readonly GoogleImageAnalysisService _googleImageAnalysisService;
        private readonly StoreService _storeService;
        private readonly UploadService _uploadService;
        private readonly UserManagementService _userManagementService;

        public UploadManager(LoggingManager loggingmanager, GoogleImageAnalysisService googleImageAnalysisService,
                             StoreService storeService, UploadService uploadService, UserManagementService userManagementService)
        {
            _loggingManager = loggingmanager;
            _googleImageAnalysisService = googleImageAnalysisService;
            _storeService = storeService;
            _uploadService = uploadService;
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Verify the upload and store it in the data store.
        /// </summary>
        /// <param name="post">The upload post from the front end.</param>
        /// <param name="failureCount">The exceptions encountered so far.</param>
        /// <returns></returns>
        public async Task<Result<bool>> CreateUploadAsync(UploadPost post, int failureCount)
        {
            var result = false;

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadCreationErrorMessage, result, true);
            }

            try
            {
                // Check if the user making the post exists.
                if (!await _userManagementService.CheckUserExistenceAsync(post.Username).ConfigureAwait(false))
                {
                    // Log the fact user was invalid.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, post.Username, post.IPAddress, Constants.UploadUserDNESystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.UploadUserDNEUserMessage, result, false);
                }


                // Verify the location is within the scope of the application.
                var latLong = LocationUtilityService.GetImageLatitudeAndLongitude(post.Image);
                var latitude = latLong.Item1;
                var longitude = latLong.Item2;

                var withinScope = LocationUtilityService.CheckLocationWithinPolygon(latitude, longitude, Constants.CurrentScopePolygon);

                if (!withinScope)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, post.Username, post.IPAddress, Constants.ImageNotWithinScopeSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.ImageNotWithinScopeUserMessage, result, false);
                }

                // Get the store id of the store the upload is associated with (creates a store if one was found).
                var storeID = await _storeService.FindStoreAsync(latitude, longitude).ConfigureAwait(false);

                if (storeID == Constants.NoStoreFoundCode)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, post.Username, post.IPAddress, Constants.NoStoreFoundSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.NoStoreFoundUserMessage, result, false);
                }

                // Construct the image path.
                var imagePath = Constants.PhotoFolder + "\\" + post.Username + "_" + TimeUtilityService.CurrentUnixTime() + post.FileExtension;

                // Verify the upload.
                var uploadDTO = new UploadDTO(imagePath, post.Image, post.Category, post.Name, (DateTime)post.PostTime, post.Username, post.Description,
                                              post.Rating, post.Price, post.PriceUnit, post.ImageSize);

                var verification = _uploadService.VerifyUpload(uploadDTO, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                               Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                               Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                               Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

                if (!verification.VerificationStatus)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, post.Username, post.IPAddress, Constants.UploadNotValidSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(verification.Message, result, false);
                }

                // Store the photo.
                Directory.CreateDirectory(Constants.PhotoFolder);
                post.Image.Save(imagePath);

                // Create the upload in the data store.
                var uploadRecord = new UploadRecord(post.PostTime, post.Username, storeID, post.Description, post.Rating.ToString(), imagePath,
                                                    post.Price, post.PriceUnit, post.Name, Constants.NoValueInt, Constants.NoValueInt, Constants.NotInProgressStatus, post.Category);

                await _uploadService.CreateUploadAsync(uploadRecord).ConfigureAwait(false);

                result = true;
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.CreateUploadOperation, post.Username, post.IPAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await CreateUploadAsync(post, ++failureCount).ConfigureAwait(false);
            }

            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.CreateUploadOperation, post.Username, post.IPAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.UploadCreationSuccessMessage, result, false);
        }

        /// <summary>
        /// Updates or creates a draft upload in the data store.
        /// </summary>
        /// <param name="post">The post object from the front end.</param>
        /// <param name="id">The id of the upload draft</param>
        /// <param name="failureCount">The exceptions encountered so far.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DraftUploadAsync(UploadPost post, int id, int failureCount)
        {
            var result = false;

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadCreationErrorMessage, result, true);
            }

            try
            {
                if (!await _userManagementService.CheckUserExistenceAsync(post.Username).ConfigureAwait(false))
                {
                    // Log the fact user was invalid.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, post.Username, post.IPAddress, Constants.UploadUserDNESystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.UploadUserDNEUserMessage, result, false);
                }

                var latLong = LocationUtilityService.GetImageLatitudeAndLongitude(post.Image);
                var latitude = latLong.Item1;
                var longitude = latLong.Item2;

                var withinScope = LocationUtilityService.CheckLocationWithinPolygon(latitude, longitude, Constants.CurrentScopePolygon);

                if (!withinScope)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, post.Username, post.IPAddress, Constants.ImageNotWithinScopeSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.ImageNotWithinScopeUserMessage, result, false);
                }

                var storeID = await _storeService.FindStoreAsync(latitude, longitude).ConfigureAwait(false);

                if (storeID == Constants.NoStoreFoundCode)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, post.Username, post.IPAddress, Constants.NoStoreFoundSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.NoStoreFoundUserMessage, result, false);
                }

                var imagePath = Constants.PhotoFolder + "\\" + post.Username + "_" + TimeUtilityService.CurrentUnixTime() + post.FileExtension;

                var uploadDTO = new UploadDTO(imagePath, post.Image, post.Category, post.Name, post.PostTime, post.Username,
                                              post.Description, post.Rating, post.Price, post.PriceUnit, post.ImageSize);
                 

                var verification = _uploadService.VerifyUpload(uploadDTO, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                               Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                               Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                               Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

                if (!verification.VerificationStatus)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, post.Username, post.IPAddress, Constants.UploadNotValidSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(verification.Message, result, false);
                }

                var uploadRecord = new UploadRecord(post.PostTime, post.Username, storeID, post.Description, post.Rating.ToString(), imagePath,
                                                    post.Price, post.PriceUnit, post.Name, Constants.NoValueInt, Constants.NoValueInt, Constants.InProgressStatus, post.Category);

                if (!await _uploadService.CheckUploadsExistenceAsync(new List<int>() { id }))
                {
                    Directory.CreateDirectory(Constants.PhotoFolder);
                    post.Image.Save(imagePath);

                    await _uploadService.CreateUploadAsync(uploadRecord).ConfigureAwait(false);
                }
                else
                {
                    await _uploadService.UpdateUploadAsync(uploadRecord).ConfigureAwait(false);
                }
                
                result = true;
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.DraftUploadOperation, post.Username, post.IPAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await DraftUploadAsync(post, id, ++failureCount).ConfigureAwait(false);
            }

            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                            Constants.DraftUploadOperation, post.Username, post.IPAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.DraftCreationSuccessMessage, result, false);
        }

        public async Task<Result<AnalysisResult>> AnalyzeImageAsync(Image image, string username, string ipAddress, int failureCount)
        {
            var result = new AnalysisResult(new List<string>(), Constants.NoValueString, Constants.NoValueString);

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.AnalyzationFailedMessage, result, true);
            }

            try
            {
                result = await _googleImageAnalysisService.AnalyzeAsync(image, Constants.ExogredientCategories).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.AnalyzeImageOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await AnalyzeImageAsync(image, username, ipAddress, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.AnalyzeImageOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.AnalyzationSuccessMessage, result, false);
        }

        public async Task<Result<UploadObject>> ContinueUploadProgressAsync(string username, int id, string ipAddress, int failureCount)
        {
            var result = new UploadObject(Constants.NoValueDatetime, Constants.NoValueString, Constants.NoValueInt, Constants.NoValueString,
                                          Constants.NoValueInt, Constants.NoValueString, Constants.NoValueDouble, Constants.NoValueString,
                                          Constants.NoValueString, Constants.NoValueInt, Constants.NoValueInt, Constants.NoValueBool,
                                          Constants.NoValueString);

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadRetrievalFailedMessage, result, true);
            }

            try
            {
                result = await _uploadService.ContinueUploadProgressAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.ContinueDraftOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await ContinueUploadProgressAsync(username, id, ipAddress, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.ContinueDraftOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.UploadRetrievalSuccessMessage, result, false);
        }

        /// <summary>
        /// Deletes the information of the upload identified by the <paramref name="id"/>.
        /// </summary>
        /// <param name="username">Username of the user deleting the upload</param>
        /// <param name="id">The id of the upload to delete</param>
        /// <param name="ipAddress">The ip address of the user deleting the upload</param>
        /// <param name="failureCount">The exceptions encountered so far.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteUploadAsync(string username, int id, string ipAddress, int failureCount)
        {
            var result = false;

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadDeletionFailedMessage, result, true);
            }

            try
            {
                // Delete the photo then the record from the data store.
                var objectData = await _uploadService.ContinueUploadProgressAsync(id).ConfigureAwait(false);

                File.Delete(objectData.Photo);

                result = await _uploadService.DeleteUploadsAsync(new List<int>() { id }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.DeleteUploadOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await ContinueUploadProgressAsync(username, id, ipAddress, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.DeleteUploadOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.UploadDeletionSuccessMessage, result, false);
        }
    }
}
