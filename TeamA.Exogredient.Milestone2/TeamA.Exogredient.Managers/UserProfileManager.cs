using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class UserProfileManager
    {
        private readonly UploadService _uploadService;
        private readonly StoreManagementService _storeManagementService;
        private readonly SaveListService _saveListService;
        private readonly LoggingManager _loggingManager;
        private readonly UserManagementService _userManagementService;

        public UserProfileManager(UploadService uploadService, StoreManagementService storeManagementService, SaveListService saveListService,
                                  LoggingManager loggingmanager, UserManagementService userManagementService)
        {
            _uploadService = uploadService;
            _storeManagementService = storeManagementService;
            _saveListService = saveListService;
            _loggingManager = loggingmanager;
            _userManagementService = userManagementService;

        }

        public async Task<List<ProfileScoreResult>> GetProfileScore(string username, string ipAddress, int failureCount, Exception ex)
        {
            if(failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            var profileScores = new List<ProfileScoreResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if(!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
                profileScores =  await _uploadService.getUploadVotes(username).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetProfileScoreOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetProfileScore(username, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), 
                                           Constants.GetProfileScoreOperation, username, ipAddress).ConfigureAwait(false);
            return profileScores;
        }

        public async Task<List<UploadResult>> GetRecentUploads(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            var recentUploads = new List<UploadResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if(!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
                recentUploads = await _uploadService.GetRecentByUploader(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetRecentUploadsOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetRecentUploads(username, pagination, ipAddress, ++failureCount, e);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetRecentUploadsOperation, username, ipAddress).ConfigureAwait(false);
            return recentUploads;
        }
        
        public async Task<List<UploadResult>> GetInProgressUploads(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            var inProgessUploads = new List<UploadResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }

                inProgessUploads = await _uploadService.GetInProgressUploadsByUploader(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetInProgressUploadsOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);

                // Retry operation Constant.OperationRetry amount of times when there is exception.
                await GetInProgressUploads(username, pagination, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetInProgressUploadsOperation, username, ipAddress).ConfigureAwait(false);
            return inProgessUploads;
        }

        public async Task<List<SaveListResult>> GetSaveList(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            var saveLists = new List<SaveListResult>();
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
                saveLists = await _saveListService.GetSaveList(username, pagination).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.GetSaveListOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);
                await GetSaveList(username, pagination, ipAddress, ++failureCount, e);
            }


            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.GetSaveListOperation, username, ipAddress).ConfigureAwait(false);
            return saveLists;
        }

        public async Task<bool> DeleteSaveList(string username, int storeId, string ingredient, string ipAddress, int failureCount, Exception ex)
        {
            if (failureCount >= Constants.OperationRetry)
            {
                throw ex;
            }

            bool deleteResult = false;
            try
            {
                // Check that the user exists.
                var userExists = await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
                if (!userExists)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
                deleteResult = await _saveListService.DeleteSaveList(username, storeId, ingredient);
            }
            catch (Exception e)
            {
                // Log everytime we catch an exception.
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                               Constants.DeleteSaveListOperation, username, ipAddress, e.ToString()).ConfigureAwait(false);
                await DeleteSaveList(username, storeId, ingredient, ipAddress, ++failureCount, e).ConfigureAwait(false);
            }

            // Operation successfull, log that operation.
            await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                           Constants.DeleteSaveListOperation, username, ipAddress).ConfigureAwait(false);
            return deleteResult;
        }

    }
}
