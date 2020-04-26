using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;


namespace TeamA.Exogredient.Managers
{
    public class IngredientManager
    {
        private readonly UploadService _uploadService;
        private readonly LoggingManager _loggingManager;

        public IngredientManager(UploadService uploadService, LoggingManager loggingManager)
        {
            _uploadService = uploadService;
            _loggingManager = loggingManager;
        }

        public async Task<List<UploadResult>> GetUploadsByIngredientNameandStoreId(string ingredientName, int storeId, int pagination, int failurecount, string username, string ipAddress)
        {
            try
            {
                var uploads = await _uploadService.ReadUploadsByIngredientNameandStoreId(ingredientName, storeId, pagination).ConfigureAwait(false);

               // _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
               //     Constants.GetUploadsByIngredientNameandStoreIdOperation, username, ipAddress).ConfigureAwait(false);

                return uploads;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetUploadsByIngredientNameandStoreIdOperation, username, ipAddress).ConfigureAwait(false);

                failurecount += 1;

                if(failurecount >= Constants.LoggingRetriesAmount)
                {
                    throw e;
                }
                else
                {
                    return await GetUploadsByIngredientNameandStoreId(ingredientName, storeId, pagination, failurecount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }

        public async Task<bool> EditUpvotesonUpload(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementUpvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.EditUpvotesonUploadOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.EditUpvotesonUploadOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
                failurecount += 1;

                if (failurecount >= Constants.LoggingRetriesAmount)
                {
                    throw e;
                }
                else
                {
                    return await EditUpvotesonUpload(votevalue, uploadId, failurecount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }

        public async Task<bool> EditDownvotesonUpload(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementDownvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.EditDownvotesonUploadOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.EditDownvotesonUploadOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
                failurecount += 1;

                if (failurecount >= Constants.LoggingRetriesAmount)
                {
                    throw e;
                }
                else
                {
                    return await EditDownvotesonUpload(votevalue, uploadId, failurecount, username, ipAddress).ConfigureAwait(false);
                }
            }
        }
    }
}
