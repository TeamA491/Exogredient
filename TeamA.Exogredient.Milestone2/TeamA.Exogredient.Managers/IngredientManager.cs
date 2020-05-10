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
        private readonly UserManagementService _userManagementService;

        public IngredientManager(UploadService uploadService, LoggingManager loggingManager, UserManagementService userManagementService)
        {
            _uploadService = uploadService;
            _loggingManager = loggingManager;
            _userManagementService = userManagementService;
        }
        /// <summary>
        /// Retrieve a list of uploads based off an ingredient's name and store id. 
        /// </summary>
        /// <param name="ingredientName"> The name of the ingredient used for searching uploads</param>
        /// <param name="storeId"> The id of store used for searching uploads.</param>
        /// <param name="pagination">Pagination for the operation. starts at 0. </param>
        /// <param name="failurecount">Count of how many times current operation has failed. </param>
        /// <param name="username">username of person doing operation used for logging. </param>
        /// <param name="ipAddress">ip address of system requesting operation for logging. </param>
        /// <returns> A list of uploadsresults </returns>
        public async Task<List<UploadResult>> GetUploadsByIngredientNameandStoreId(string ingredientName, int storeId, int pagination, int failurecount, string username, string ipAddress)
        {
            try
            {
                var uploads = await _uploadService.ReadUploadsByIngredientNameandStoreId(ingredientName, storeId, pagination).ConfigureAwait(false);

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
        /// <summary>
        /// Alter the upvote value for an upload. 
        /// </summary>
        /// <param name="votevalue">The value going to be added to the current upvote value. Can be negative.</param>
        /// <param name="uploadId">Id of upload used for searching.</param>
        /// <param name="failurecount">Current number of times operation has failed.</param>
        /// <param name="username">Username of person requesting operation for logging.</param>
        /// <param name="ipAddress">IP address of system requesting operation for logging.</param>
        /// <returns>A boolean for the successful completion of the operation. </returns>
        public async Task<bool> EditUpvotesonUpload(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementUpvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UpvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UpvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
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
        /// <summary>
        /// Alter the upvote value for an upload. 
        /// </summary>
        /// <param name="votevalue">The value going to be added to the current downvote value.</param>
        /// <param name="uploadId">Id of upload used for searching.</param>
        /// <param name="failurecount">Current number of times operation has failed.</param>
        /// <param name="username">Username of person requesting operation for logging.</param>
        /// <param name="ipAddress">IP address of system requesting operation for logging.</param>
        /// <returns>A boolean for the successful completion of the operation. </returns>
        public async Task<bool> EditDownvotesonUpload(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementDownvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.DownvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.DownvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
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
        /// <summary>
        /// Alter the upvote value for an upload. 
        /// </summary>
        /// <param name="votevalue">The value going to be added to the current upvote value. Can be negative.</param>
        /// <param name="uploadId">Id of upload used for searching.</param>
        /// <param name="failurecount">Current number of times operation has failed.</param>
        /// <param name="username">Username of person requesting operation for logging.</param>
        /// <param name="ipAddress">IP address of system requesting operation for logging.</param>
        /// <returns>A boolean for the successful completion of the operation. </returns>
        public async Task<bool> UndoUpvote(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementUpvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UndoUpvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UndoUpvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
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
        /// <summary>
        /// Alter the upvote value for an upload. 
        /// </summary>
        /// <param name="votevalue">The value going to be added to the current downvote value. Can be negative.</param>
        /// <param name="uploadId">Id of upload used for searching.</param>
        /// <param name="failurecount">Current number of times operation has failed.</param>
        /// <param name="username">Username of person requesting operation for logging.</param>
        /// <param name="ipAddress">IP address of system requesting operation for logging.</param>
        /// <returns>A boolean for the successful completion of the operation. </returns>
        public async Task<bool> UndoDownvote(int votevalue, int uploadId, int failurecount, string username, string ipAddress)
        {
            try
            {
                bool result = await _uploadService.IncrementUpvotesonUpload(votevalue, uploadId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UndoDownvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.UndoDownvoteOperation + "/" + uploadId, username, ipAddress).ConfigureAwait(false);
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
        /// <summary>
        /// Retrieves the pagination size for the ingredient view. 
        /// </summary>
        /// <param name="ingredientName"> The name of the ingredients used for searching.</param>
        /// <param name="storeId"> Store id used to find the correct store. </param>
        /// <param name="failurecount"> Current number of times this operation has failed.</param>
        /// <param name="username"> Username of person requesting operation for logging.</param>
        /// <param name="ipAddress"> IP address of system requesting operation for logging.</param>
        /// <returns> Integer holding the number of a certain ingredient at a specific store. </returns>
        public async Task<int> GetIngredientViewPaginationSize(string ingredientName, int storeId, int failurecount, string username, string ipAddress)
        {
            try
            {
                var uploads = await _uploadService.GetIngredientViewPaginationSize(ingredientName, storeId).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                  Constants.GetIngredientViewPaginationSizeOperation, username, ipAddress).ConfigureAwait(false);

                return uploads;
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                    Constants.GetIngredientViewPaginationSizeOperation, username, ipAddress).ConfigureAwait(false);

                failurecount += 1;

                if (failurecount >= Constants.LoggingRetriesAmount)
                {
                    throw e;
                }
                else
                {
                    return await _uploadService.GetIngredientViewPaginationSize(ingredientName, storeId).ConfigureAwait(false);
                }
            }
        }
    }
}
