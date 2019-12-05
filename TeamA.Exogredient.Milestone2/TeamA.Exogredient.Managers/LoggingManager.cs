using System;
using System.Collections.Generic;
using System.Text;
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
            bool ffLoggingResult = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
            bool dsLoggingResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);

            int count = 0;

            while (!(ffLoggingResult && dsLoggingResult) && count < Constants.LoggingRetiesAmount)
            {
                if (!ffLoggingResult)
                {
                    ffLoggingResult = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                }
                if (!dsLoggingResult)
                {
                    dsLoggingResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
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
                    await UserManagementService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}");
                }
                else
                {
                    // Rollback

                    bool rollbackSuccess = false;

                    if (ffLoggingResult)
                    {
                        rollbackSuccess = await FlatFileLoggingService.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }
                    if (dsLoggingResult)
                    {
                        rollbackSuccess = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }

                    await UserManagementService.NotifySystemAdminAsync($"{(ffLoggingResult ? "Flat File" : "Data Store")} Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}\n\nRollback status: {(rollbackSuccess ? "successful" : "failed")}");
                }

                return false;
            }
        }
    }
}
