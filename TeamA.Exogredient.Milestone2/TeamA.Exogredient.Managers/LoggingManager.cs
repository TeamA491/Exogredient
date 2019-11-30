using System;
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

        private UserManagementService _userManagementService;
        private FlatFileLoggingService _flatFileLoggingService;
        private DataStoreLoggingService _dataStoreLoggingService;

        public LoggingManager()
        {
            _userManagementService = new UserManagementService();
            _flatFileLoggingService = new FlatFileLoggingService();
            _dataStoreLoggingService = new DataStoreLoggingService();
        }

        public async Task<bool> LogAsync(string timestamp, string operation, string identifier,
                                         string ipAddress, string errorType = "null")
        {
            bool ffLoggingResult = await _flatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
            bool dsLoggingResult = await _dataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);

            int count = 0;

            while (!(ffLoggingResult && dsLoggingResult) && count < _loggingRetriesAmount)
            {
                if (!ffLoggingResult)
                {
                    ffLoggingResult = await _flatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                }
                if (!dsLoggingResult)
                {
                    dsLoggingResult = await _dataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
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
                    await _userManagementService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}");
                }
                else
                {
                    // Rollback

                    bool rollbackSuccess = false;

                    if (ffLoggingResult)
                    {
                        rollbackSuccess = await _flatFileLoggingService.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }
                    if (dsLoggingResult)
                    {
                        rollbackSuccess = await _dataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
                    }

                    await _userManagementService.NotifySystemAdminAsync($"{(ffLoggingResult ? "Flat File" : "Data Store")} Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}\n\nRollback status: {(rollbackSuccess ? "successful" : "failed")}");
                }

                return false;
            }
        }
    }
}
