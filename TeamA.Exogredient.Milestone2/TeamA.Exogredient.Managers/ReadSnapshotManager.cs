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

        /// <summary>
        /// Manager method to read one snapshot.
        /// There are 3 retries, after the retries fail it will notify system admin.
        /// </summary>
        /// <param name="year">The year to get the snapshot/</param>
        /// <param name="month">The month to get the snapshot.</param>
        /// <returns>The snapshot object.</returns>
        public async Task<SnapShotResult> ReadOneSnapshotAsync(int year, int month)
        {
            int currentNumExceptions = 0;
            var snapshot = new SnapShotResult(null, null, null, null, null, null, null, null, null, null, null);

            while (currentNumExceptions < 4)
            {
                currentNumExceptions++;
                try
                {
                    snapshot = await _snapshotService.ReadOneSnapshotAsync(year, month).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.ReadOneSnapshotOperation,
                                                  Constants.SystemIdentifier,
                                                  Constants.LocalHost, e.Message).ConfigureAwait(false);
                    if (currentNumExceptions >= Constants.MaximumOperationRetries)
                    {
                        await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadOneSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                    }
                }
            }
            return snapshot;
        }

        /// <summary>
        /// Manager method to read multiple snapshots.
        /// There are 3 retries, after the retries fail it will notify system admin.
        /// </summary>
        /// <param name="year">The year to get all the snapshots.</param>
        /// <returns>A string formatted result with all the snapshots data.</returns>
        public async Task<String> ReadMultiSnapshotAsync(int year)
        {
            int currentNumExceptions = 0;
            string snapshots = "";

            while (currentNumExceptions < 4)
            {
                currentNumExceptions++;
                try
                {
                    snapshots = await _snapshotService.ReadMultiSnapshotAsync(year).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.ReadMultiSnapshotOperation,
                                                  Constants.SystemIdentifier,
                                                  Constants.LocalHost, e.Message).ConfigureAwait(false);
                    currentNumExceptions++;
                    if (currentNumExceptions >= Constants.MaximumOperationRetries)
                    {
                        await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadMultiSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                    }
                }
            }
            return snapshots;
        }

    }
}
