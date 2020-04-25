using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;
using System.Drawing;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class CreateUploadManager
    {
        private readonly LoggingManager _loggingManager;
        private readonly StoreService _storeService;
        private readonly UploadService _uploadService;

        public CreateUploadManager(LoggingManager loggingmanager, StoreService storeService, UploadService uploadService)
        {
            _loggingManager = loggingmanager;
            _storeService = storeService;
            _uploadService = uploadService;
        }

        public async Task<Result<bool>> CreateUploadAsync(string username, string ipAddress, Bitmap image, string name, string description, string category, int rating,
                                                          float price, string priceUnit, int failureCount)
        {
            var result = SystemUtilityService.CreateResult("", false, false);

            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                return SystemUtilityService.CreateResult(Constants.UploadCreationErrorMessage, false, true);
            }

            try
            {
                var longLat = LocationUtilityService.GetImageLatitudeAndLongitude(image);
                var withinScope = LocationUtilityService.CheckLocationWithinPolygon(longLat.Item1, longLat.Item2, Constants.CurrentScopePolygon);

                if (!withinScope)
                {
                    result = SystemUtilityService.CreateResult(Constants.UploadImageNotWithinScopeMessage, false, false);
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                // Log exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                               Constants.CreateUploadOperation, username, ipAddress, ex.ToString()).ConfigureAwait(false);

                // Recursively retry the operation until the maximum amount of retries is reached.
                await CreateUploadAsync(username, ipAddress, image, name, description, category, rating, price, priceUnit, ++failureCount).ConfigureAwait(false);
            }


            // Log the fact that the operation was successful.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.CreateUploadOperation, username, ipAddress).ConfigureAwait(false);



            return result;
        }
    }
}
