﻿using System;
using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;
using System.Drawing;
using TeamA.Exogredient.AppConstants;
using Image = Google.Cloud.Vision.V1.Image;

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

        public async Task<Result<bool>> CreateUploadAsync(string imagePath, string category, string username, string ipAddress, DateTime postTime,
                                                          string name, string description, int rating, double price, string priceUnit, int failureCount)
        {
            var result = false;

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadCreationErrorMessage, result, true);
            }

            try
            {
                if (postTime.Equals(null))
                {
                    postTime = Constants.NoValueDatetime;
                }

                if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    // Log the fact user was invalid.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, username, ipAddress, Constants.UploadUserDNESystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.UploadUserDNEUserMessage, result, false);
                }

                var image = new Bitmap(imagePath);

                var latLong = LocationUtilityService.GetImageLatitudeAndLongitude(image);
                var latitude = latLong.Item1;
                var longitude = latLong.Item2;

                var withinScope = LocationUtilityService.CheckLocationWithinPolygon(latitude, longitude, Constants.CurrentScopePolygon);

                if (!withinScope)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, username, ipAddress, Constants.ImageNotWithinScopeSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.ImageNotWithinScopeUserMessage, result, false);
                }

                var storeID = await _storeService.FindStoreAsync(latitude, longitude).ConfigureAwait(false);

                if (storeID == Constants.NoStoreFoundCode)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, username, ipAddress, Constants.NoStoreFoundSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.NoStoreFoundUserMessage, result, false);
                }

                var uploadDTO = new UploadDTO(imagePath, category, name, (DateTime)postTime, username, description, rating, price, priceUnit);

                var verification = _uploadService.VerifyUpload(uploadDTO, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                               Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                               Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                               Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

                if (!verification.VerificationStatus)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.CreateUploadOperation, username, ipAddress, Constants.UploadNotValidSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(verification.Message, result, false);
                }

                var uploadRecord = new UploadRecord((DateTime)postTime, username, storeID, description, rating.ToString(), imagePath,
                                                    price, priceUnit, name, Constants.NoValueInt, Constants.NoValueInt, Constants.NotInProgressStatus, category);

                await _uploadService.CreateUploadAsync(uploadRecord).ConfigureAwait(false);

                result = true;
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.CreateUploadOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await CreateUploadAsync(imagePath, category, username, ipAddress, postTime, name, description, rating, price, priceUnit, ++failureCount).ConfigureAwait(false);
            }

            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                            Constants.CreateUploadOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.UploadCreationSuccessMessage, result, false);
        }

        public async Task<Result<bool>> DraftUploadAsync(string imagePath, string category, string username, string ipAddress, DateTime? postTime = null,
                                                         string name = Constants.NoValueString, string description = Constants.NoValueString, int rating = Constants.NoValueInt,
                                                         double price = Constants.NoValueDouble, string priceUnit = Constants.NoValueString, int failureCount = Constants.NoValueInt)
        {
            var result = false;

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadCreationErrorMessage, result, true);
            }

            try
            {
                if (postTime.Equals(null))
                {
                    postTime = Constants.NoValueDatetime;
                }

                if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    // Log the fact user was invalid.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, username, ipAddress, Constants.UploadUserDNESystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.UploadUserDNEUserMessage, result, false);
                }

                var image = new Bitmap(imagePath);

                var latLong = LocationUtilityService.GetImageLatitudeAndLongitude(image);
                var latitude = latLong.Item1;
                var longitude = latLong.Item2;

                var withinScope = LocationUtilityService.CheckLocationWithinPolygon(latitude, longitude, Constants.CurrentScopePolygon);

                if (!withinScope)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, username, ipAddress, Constants.ImageNotWithinScopeSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.ImageNotWithinScopeUserMessage, result, false);
                }

                var storeID = await _storeService.FindStoreAsync(latitude, longitude).ConfigureAwait(false);

                if (storeID == Constants.NoStoreFoundCode)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, username, ipAddress, Constants.NoStoreFoundSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.NoStoreFoundUserMessage, result, false);
                }

                var uploadDTO = new UploadDTO(imagePath, category, name, (DateTime)postTime, username, description, rating, price, priceUnit);

                var verification = _uploadService.VerifyUpload(uploadDTO, Constants.MaximumPhotoCharacters, Constants.MinimumPhotoCharacters, Constants.MinimumImageSizeMB, Constants.MaximumImageSizeMB, Constants.ValidImageExtensions,
                                                               Constants.IngredientNameMaximumCharacters, Constants.IngredientNameMinimumCharacters, Constants.MaximumIngredientPrice,
                                                               Constants.DescriptionMaximumCharacters, Constants.DescriptionMinimumCharacters, Constants.ExogredientCategories,
                                                               Constants.ExogredientPriceUnits, Constants.ValidTimeBufferMinutes, Constants.MaximumRating, Constants.MinimumRating);

                if (!verification.VerificationStatus)
                {
                    // Log the fact that scope was violated.
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                   Constants.DraftUploadOperation, username, ipAddress, Constants.UploadNotValidSystemMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(verification.Message, result, false);
                }

                var uploadRecord = new UploadRecord((DateTime)postTime, username, storeID, description, rating.ToString(), imagePath,
                                                    price, priceUnit, name, Constants.NoValueInt, Constants.NoValueInt, Constants.InProgressStatus, category);

                await _uploadService.CreateUploadAsync(uploadRecord).ConfigureAwait(false);

                result = true;
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.DraftUploadOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await DraftUploadAsync(imagePath, category, username, ipAddress, postTime, name, description, rating, price, priceUnit, ++failureCount).ConfigureAwait(false);
            }

            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                            Constants.DraftUploadOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.DraftCreationSuccessMessage, result, false);
        }

        public async Task<Result<AnalysisResult>> AnalyzeImageAsync(string username, string ipAddress, Image image, int failureCount)
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
                await AnalyzeImageAsync(username, ipAddress, image, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.AnalyzeImageOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.AnalyzationSuccessMessage, result, false);
        }

        public async Task<Result<UploadObject>> AnalyzeImageAsync(string username, string ipAddress, int id, int failureCount)
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
                result = await _uploadService.ContinueProgressUploadAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.ContinueDraftOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await AnalyzeImageAsync(username, ipAddress, id, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.ContinueDraftOperation, username, ipAddress).ConfigureAwait(false);

            return SystemUtilityService.CreateResult(Constants.UploadRetrievalSuccessMessage, result, false);
        }
    }
}