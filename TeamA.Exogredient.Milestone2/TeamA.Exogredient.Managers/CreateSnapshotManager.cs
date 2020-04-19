using System;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Managers
{
    public class CreateSnapshotManager
    {
        private readonly LoggingManager _loggingManager;
        private readonly SnapshotService _snapshotService;

        public CreateSnapshotManager(LoggingManager loggingManager, SnapshotService snapshotService)
        {
            _loggingManager = loggingManager;
            _snapshotService = snapshotService;
        }

        public async Task<Result<bool>> CreateSnapshotAsync(int currentNumExceptions, int year, int month)
        {
            bool createSnapshotSuccess = false;
            try
            {
                createSnapshotSuccess = await _snapshotService.CreateSnapShotAsync(2020, 4, 30).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.CreateSnapshotOperation, 
                                              Constants.SystemIdentifier, 
                                              Constants.LocalHost, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.CreateSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }

            return SystemUtilityService.CreateResult(Constants.WrongEmailCodeMessage, createSnapshotSuccess, false, currentNumExceptions);
        }

    }
}
