using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Exceptions;

namespace TeamA.Exogredient.Managers
{
    public class UserProfileManager
    {
        private readonly UploadService _uploadService;
        private readonly StoreService _storeService;
        private readonly SaveListService _saveListService;
        private readonly LoggingManager _loggingManager;
        private readonly UserManagementService _userManagementService;

        public UserProfileManager(UploadService uploadService, StoreService storeManagementService, SaveListService saveListService,
                                  LoggingManager loggingmanager, UserManagementService userManagementService)
        {
            _uploadService = uploadService;
            _storeService = storeManagementService;
            _saveListService = saveListService;
            _loggingManager = loggingmanager;
            _userManagementService = userManagementService;

        }

        /// <summary>
        /// Retrieve the profile score for a username and perform business logic to check existence.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">Username of profile score to fetch.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>List of profileScoreResults.</returns>
        public async Task<List<ProfileScoreResult>> GetProfileScoreAsync(string username, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if(failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // List of profile scores to return.
            var profileScores = new List<ProfileScoreResult>();
            
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if(!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                profileScores =  await _uploadService.getUploadVotesAsync(username).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetProfileScoreOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetProfileScoreAsync(username, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), 
                                           Constants.GetProfileScoreOperation, username, ipAddress).ConfigureAwait(false);
            return profileScores;
        }

        /// <summary>
        /// Retrieve the recent uploads for a username and perform business logic to check existence.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">Username of profile score to fetch.</param>
        /// <param name="pagination">Pagination for the operation. Starts at 0.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>List of UploadResult</returns>
        public async Task<List<UploadResult>> GetRecentUploadsAsync(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // List of uploads to return.
            var recentUploads = new List<UploadResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if(!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                recentUploads = await _uploadService.GetRecentByUploaderAsync(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetRecentUploadsOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetRecentUploadsAsync(username, pagination, ipAddress, ++failureCount, e);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetRecentUploadsOperation, username, ipAddress).ConfigureAwait(false);
            return recentUploads;
        }

        /// <summary>
        /// Retrieve the in progress uploads for a username and perform business logic to check existence.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">Username of profile score to fetch.</param>
        /// <param name="pagination">Pagination for the operation. Starts at 0.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>List of UploadResult</returns>
        public async Task<List<UploadResult>> GetInProgressUploadsAsync(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // List of uploads to return.
            var inProgessUploads = new List<UploadResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                inProgessUploads = await _uploadService.GetInProgressUploadsByUploaderAsync(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetInProgressUploadsOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetInProgressUploadsAsync(username, pagination, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetInProgressUploadsOperation, username, ipAddress).ConfigureAwait(false);
            return inProgessUploads;
        }

        /// <summary>
        /// Retrieve the save lists for a username and perform business logic to check existence.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">Username of profile score to fetch.</param>
        /// <param name="pagination">Pagination for the operation. Starts at 0.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>List of SaveListResults.</returns>
        public async Task<List<SaveListResult>> GetSaveListAsync(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // List of uploads to return.
            var saveLists = new List<SaveListResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                saveLists = await _saveListService.GetSaveListAsync(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetSaveListOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);
                
                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetSaveListAsync(username, pagination, ipAddress, ++failureCount, e);
            }


            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetSaveListOperation, username, ipAddress).ConfigureAwait(false);
            return saveLists;
        }

        /// <summary>
        /// Delete the save list for a user and performs business logic to check existence.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">Username of profile score to fetch.</param>
        /// <param name="storeId">Id of the store savelist is associate with.</param>
        /// <param name="ingredient">Name of the ingredient saved.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>bool that represents whether the operation passed.</returns>
        public async Task<bool> DeleteSaveListAsync(string username, int storeId, string ingredient, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // Condition to return.
            bool deleteResult = false;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                deleteResult = await _saveListService.DeleteSaveListAsync(username, storeId, ingredient);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.DeleteSaveListOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);
                
                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await DeleteSaveListAsync(username, storeId, ingredient, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.DeleteSaveListOperation, username, ipAddress).ConfigureAwait(false);
            return deleteResult;
        }

        /// <summary>
        /// Delete an upload and perform business logic to check for user existence and permission.
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="ids">Ids of uploads to delete.</param>
        /// <param name="performingUser">User that is trying to delete the upload.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>bool that represents whether the operation passed.</returns>
        public async Task<bool> DeleteUploadsAsync(List<string> ids, string performingUser, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // Condition to return.
            bool deleteResult = false;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(performingUser).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Check that all the Ids exists.
                if (!await _uploadService.CheckUploadsExistenceAsync(ids).ConfigureAwait(false))
                {
                    throw new ArgumentException(Constants.UploadIdsDNE);
                }

                // Get user type.
                var userType = await _userManagementService.GetUserType(performingUser).ConfigureAwait(false);

                // Check is user is admin. if true then let him perform
                if (userType.Equals(Constants.AdminUserType))
                {
                    // Let execution continue.
                }
                else if (userType.Equals(Constants.CustomerUserType) || userType.Equals(Constants.StoreOwnerUserType))
                {
                    // Check if use is allowed to perform operation
                    if (!await _uploadService.CheckUploadOwnerAsync(ids, performingUser).ConfigureAwait(false))
                    {
                        throw new NotAuthorizedException(Constants.UserNotAllowed);
                    }
                }
                else
                {
                    throw new NotAuthorizedException(Constants.UserNotAllowed);
                }

                // Perform operation.
                deleteResult =  await _uploadService.DeleteUploadsAsync(ids).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.DeleteUploadOperation, performingUser, ipAddress, e.ToString()).ConfigureAwait(false);
                
                // Retry operation Constant.OperationRetry amount of times when there is exception
                await DeleteUploadsAsync(ids, performingUser, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Log the successful operations.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
               Constants.DeleteUploadOperation, performingUser, ipAddress).ConfigureAwait(false);

            return deleteResult;
        }

        /// <summary>
        /// Get the save list pagination length for a user and performs business logic to check for user existence. 
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">User to retrieve pagination length from.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>int that represents pagination size.</returns>
        public async Task<int> GetSaveListPaginationSizeAsync(string username, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // Pagination size to return.
            int paginationSize = 0;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                paginationSize = await _saveListService.GetPaginationSizeAsync(username).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetSaveListPagination, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetSaveListPaginationSizeAsync(username, ipAddress, failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                   Constants.GetSaveListPagination, username, ipAddress).ConfigureAwait(false);

            return paginationSize;
        }

        /// <summary>
        /// Get the in progress upload pagination length for a user and performs business logic to check for user existence. 
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">User to retrieve pagination length from.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>int that represents pagination size.</returns>
        public async Task<int> GetInProgressUploadPaginationSizeAsync(string username, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // Pagination size to return.
            int paginationSize = 0;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                // Perform operation.
                paginationSize = await _uploadService.GetInProgressPaginationSizeAsync(username).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetInProgressUploadPagination, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetInProgressUploadPaginationSizeAsync(username, ipAddress, failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                   Constants.GetInProgressUploadPagination, username, ipAddress).ConfigureAwait(false);

            return paginationSize;
        }

        /// <summary>
        /// Get the recent upload pagination length for a user and performs business logic to check for user existence. 
        /// Retries operation when there is an exception and logs all actions.
        /// </summary>
        /// <param name="username">User to retrieve pagination length from.</param>
        /// <param name="ipAddress">Ipaddress for logging.</param>
        /// <param name="failureCount">Current failure count of the operation.</param>
        /// <param name="ex">Exception that is thrown.</param>
        /// <returns>int that represents pagination size.</returns>
        public async Task<int> GetRecentUploadPaginationSizeAsync(string username, string ipAddress, int failureCount, Exception ex)
        {
            // Escape condition for recursive call if exception is thrown.
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            // Pagination size to return.
            int paginationSize = 0;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
                paginationSize = await _uploadService.GetRecentPaginationSizeAsync(username).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetRecentUploadPagination, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetRecentUploadPaginationSizeAsync(username, ipAddress, failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                   Constants.GetRecentUploadPagination, username, ipAddress).ConfigureAwait(false);

            return paginationSize;
        }


    }
}
