using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using Google.Cloud.Vision.V1;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class ImageAnalysisManager
    {
        private readonly GoogleImageAnalysisService _googleImageAnalysisService;
        private readonly LoggingManager _loggingManager;

        public ImageAnalysisManager(GoogleImageAnalysisService googleImageAnalysisService, LoggingManager loggingManager)
        {
            _googleImageAnalysisService = googleImageAnalysisService;
            _loggingManager = loggingManager;
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
    }
}
