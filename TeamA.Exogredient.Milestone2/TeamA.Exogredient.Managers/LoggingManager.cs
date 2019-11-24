﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class LoggingManager
    {
        // timestamp is in format of "HH:mm:ss:ff UTC yyyyMMdd", CultureInfo.InvariantCulture);

        private readonly int _loggingRetriesAmount = 3;

        public async Task<bool> LogAsync(string timestamp, string operation, string identifier,
                                         string ipAddress, string errorType = "null")
        {
            FlatFileLoggingService ffLogger = new FlatFileLoggingService();
            DataStoreLoggingService dsLogger = new DataStoreLoggingService();

            bool ffLoggingResult = await ffLogger.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
            bool dsLoggingResult = await dsLogger.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);

            int count = 0;

            while (!(ffLoggingResult && dsLoggingResult) && count < _loggingRetriesAmount)
            {
                if (!ffLoggingResult)
                {
                    ffLoggingResult = await ffLogger.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                }
                if (!dsLoggingResult)
                {
                    dsLoggingResult = await dsLogger.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
                }
            }

            if (ffLoggingResult && dsLoggingResult)
            {
                return true;
            }
            else
            {
                AdminFunctionalityService adminService = new AdminFunctionalityService();

                if (!ffLoggingResult && !dsLoggingResult)
                {
                    await adminService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}");
                }
                else
                {
                    // Rollback

                    bool rollbackSuccess = false;

                    if (ffLoggingResult)
                    {
                        rollbackSuccess = await ffLogger.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }
                    if (dsLoggingResult)
                    {
                        rollbackSuccess = await dsLogger.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }

                    await adminService.NotifySystemAdminAsync($"{(ffLoggingResult ? "Flat File" : "Data Store")} Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}\n\nRollback status: {(rollbackSuccess ? "successful" : "failed")}");
                }

                return false;
            }
        }
    }
}
