using System.Threading.Tasks;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public static class LoggingManager
    {
        // timestamp is in format of "HH:mm:ss:ff UTC yyyyMMdd"

        public static async Task<bool> LogAsync(string timestamp, string operation, string identifier,
                                                string ipAddress, string errorType = "null")
        {
            bool ffLoggingResult = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
            bool dsLoggingResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

            int count = 0;

            while (!(ffLoggingResult && dsLoggingResult) && count < Constants.LoggingRetriesAmount)
            {
                if (!ffLoggingResult)
                {
                    ffLoggingResult = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
                }
                if (!dsLoggingResult)
                {
                    dsLoggingResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);
                }
            }

            if (ffLoggingResult && dsLoggingResult)
            {
                return true;
            }
            else
            {
                if (!ffLoggingResult && !dsLoggingResult)
                {
                    await UserManagementService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }
                else
                {
                    // Rollback

                    bool rollbackSuccess = false;

                    if (ffLoggingResult)
                    {
                        rollbackSuccess = await FlatFileLoggingService.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
                    }
                    if (dsLoggingResult)
                    {
                        rollbackSuccess = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);
                    }

                    await UserManagementService.NotifySystemAdminAsync($"{(ffLoggingResult ? "Flat File" : "Data Store")} Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}\n\nRollback status: {(rollbackSuccess ? "successful" : "failed")}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return false;
            }
        }
    }
}
