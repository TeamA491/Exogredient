using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// Performs a complete loggging action to both the data store and a flat file.
    /// </summary>
    public static class LoggingService
    {
        /// <summary>
        /// Asynchronously logs to the data store and flat file.
        /// </summary>
        /// <param name="timestamp">The timestamp of the operation.</param>
        /// <param name="operation">The type of the operation.</param>
        /// <param name="identifier">The identifier of the operation's performer.</param>
        /// <param name="ipAddress">The ip address of the operation's performer.</param>
        /// <param name="errorType">The type of error that occurred during the operation (default = no error)</param>
        /// <returns>Task (bool) whether the logging operation was successful.</returns>
        public static async Task<bool> LogAsync(string timestamp, string operation, string identifier,
                                                string ipAddress, string errorType = Constants.NoError)
        {
            // Attempt logging to both the data store and the flat file and track the results.
            bool ffLoggingResult = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
            bool dsLoggingResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

            int count = 0;

            // Retry whichever one failed, a maximum number of times.
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
                count++;
            }

            // If both succeeded we are finished.
            if (ffLoggingResult && dsLoggingResult)
            {
                return true;
            }
            else
            {
                // Otherwise, if both failed notify the system admin.
                if (!ffLoggingResult && !dsLoggingResult)
                {
                    await UserManagementService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }
                else
                {
                    // Otherwise rollback whichever one succeeded and notify the system admin.

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
