using System;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TeamA.Exogredient.Managers
{
    public class ReadSnapshotManager
    {
        private readonly LoggingManager _loggingManager;
        private readonly SnapshotService _snapshotService;

        public ReadSnapshotManager(LoggingManager loggingManager, SnapshotService snapshotService)
        {
            _loggingManager = loggingManager;
            _snapshotService = snapshotService;
        }

        public async Task<SnapShotResult> ReadOneSnapshotAsync(int currentNumExceptions, int year, int month)
        {
            bool readOneSnapshotSuccess = false;
            var snapshot = new SnapShotResult(null, null, null, null, null, null, null, null, null, null, null);
            try 
            { 
                snapshot = await _snapshotService.ReadOneSnapshotAsync(2020, 4).ConfigureAwait(false);
                readOneSnapshotSuccess = true;
            }
            catch (Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.ReadOneSnapshotOperation,
                                              Constants.SystemIdentifier,
                                              Constants.LocalHost, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadOneSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

            }

            return snapshot;
        }

        public async Task<List<SnapShotResult>> ReadMultiSnapshotAsync(int currentNumExceptions, int year)
        {
            bool readMultiSnapshotSuccess = false;
            var snapshotList = new List<SnapShotResult>();
            try
            {
                snapshotList = await _snapshotService.ReadMultiSnapshotAsync(2020).ConfigureAwait(false);
                readMultiSnapshotSuccess = true;
            }
            catch (Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.ReadMultiSnapshotOperation,
                                              Constants.SystemIdentifier,
                                              Constants.LocalHost, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadMultiSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

            }
            return snapshotList;
        }


    }
}
