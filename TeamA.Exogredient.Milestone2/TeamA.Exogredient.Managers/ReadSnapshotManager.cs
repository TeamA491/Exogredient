using System;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;

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
        /// Method to read one snapshot.
        /// There are 3 retries, after the retries fail it will notify system admin.
        /// </summary>
        /// <param name="year">The year to get the snapshot/</param>
        /// <param name="month">The month to get the snapshot.</param>
        /// <returns>A snapshot object with data just for that month.</returns>
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
                    // Increment the amount of tries and then check if the 3 retries are up.
                    // If it is, notify system admin.
                    if (currentNumExceptions >= Constants.MaximumOperationRetries)
                    {
                        await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadOneSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                    }
                }
            }
            return snapshot;
        }

        /// <summary>
        /// Method to read multiple snapshots.
        /// There are 3 retries, after the retries fail it will notify system admin.
        /// </summary>
        /// <param name="year">The year to get all the snapshots.</param>
        /// <returns>A snapshot object with data pertaining to the year.</returns>
        public async Task<SnapShotResult> ReadMultiSnapshotAsync(int year)
        {
            int currentNumExceptions = 0;
            var snapshot = new SnapShotResult(null, null, null, null, null, null, null, null, null, null, null);

            while (currentNumExceptions < 4)
            {
                currentNumExceptions++;
                try
                {
                    snapshot = await _snapshotService.ReadMultiSnapshotAsync(year).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.ReadMultiSnapshotOperation,
                                                  Constants.SystemIdentifier,
                                                  Constants.LocalHost, e.Message).ConfigureAwait(false);
                    // Increment the amount of tries and then check if the 3 retries are up.
                    // If it is, notify system admin.
                    currentNumExceptions++;
                    if (currentNumExceptions >= Constants.MaximumOperationRetries)
                    {
                        await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadMultiSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                    }
                }
            }
            return snapshot;
        }

        /// <summary>
        /// Method to get all the years and months pertaining to that year that has snapshots.
        /// If it fails, there are 3 retries before admin is notified of error.
        /// </summary>
        /// <returns>A json formatted string with the data.</returns>
        public async Task<string> GetYearMonthAsync()
        {
            int currentNumExceptions = 0;
            var snapshotYearMonth = ""; 

            while (currentNumExceptions < 4)
            {
                currentNumExceptions++;
                try
                {
                    snapshotYearMonth = await _snapshotService.GetYearAndMonthAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.ReadMultiSnapshotOperation,
                                                  Constants.SystemIdentifier,
                                                  Constants.LocalHost, e.Message).ConfigureAwait(false);
                    // Increment the amount of tries and then check if the 3 retries are up.
                    // If it is, notify system admin.
                    currentNumExceptions++;
                    if (currentNumExceptions >= Constants.MaximumOperationRetries)
                    {
                        await SystemUtilityService.NotifySystemAdminAsync($"{Constants.ReadMultiSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                    }
                }
            }
            return snapshotYearMonth;
        }

    }
}
