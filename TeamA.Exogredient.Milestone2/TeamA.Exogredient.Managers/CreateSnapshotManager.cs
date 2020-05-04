using System;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        /// <summary>
        /// Method to create a snapshot with given year and month.
        /// </summary>
        /// <param name="currentNumExceptions"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<bool> CreateSnapshotAsync(int currentNumExceptions, int year, int month)
        {
            bool createSnapshotSuccess = false;
            try
            {
                // Get the amount of days in the specific month.
                var amountOfDays = _snapshotService.GetDaysInMonth(year, month);

                // Get all the logs pertaining to the specific month.
                var logResults = await _snapshotService.GetLogsInMonthAsync(year, month, amountOfDays).ConfigureAwait(false);

                var snapshot = new List<string>();

                // Calling the snapshot service methods to format the data in logresults to dictionaries.
                var operationsDict = _snapshotService.GetOperationDict(logResults, amountOfDays);
                var usersDict = await _snapshotService.GetUsersDictAsync().ConfigureAwait(false);
                var cityDict = await _snapshotService.GetCityDictAsync(logResults).ConfigureAwait(false);
                var userUploadedDict = _snapshotService.GetUserUploadedDict(logResults);
                var uploadedIngredientDict = _snapshotService.GetUploadedIngredientDict(logResults);
                var uploadedStoreDict = _snapshotService.GetUploadedStoreDict(logResults);
                var searchedIngredientDict = _snapshotService.GetSearchedIngredientDict(logResults);
                var searchedStoreDict = _snapshotService.GetSearchedStoreDict(logResults);
                var upvotedUserDict = await _snapshotService.GetUpvotedUserDictAsync(logResults).ConfigureAwait(false);
                var downvotedUserDict = await _snapshotService.GetDownvotedUserDictAsync(logResults).ConfigureAwait(false);

                // Finalizing the data and then adding it to the snapshot List.
                snapshot.Add(_snapshotService.FormatOperationsDict(operationsDict));
                snapshot.Add(_snapshotService.FormatStringIntDict(usersDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(cityDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(userUploadedDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(uploadedIngredientDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(uploadedStoreDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(searchedIngredientDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(searchedStoreDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(upvotedUserDict));
                snapshot.Add(_snapshotService.FinalizeStringIntDictForSnap(downvotedUserDict));

                // Call the method to create the snapshot in the snapshot service.
                createSnapshotSuccess = await _snapshotService.CreateSnapShotAsync(year, month, snapshot).ConfigureAwait(false);

                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                              Constants.CreateSnapshotOperation,
                              Constants.SystemIdentifier,
                              Constants.LocalHost).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.CreateSnapshotOperation, 
                                              Constants.SystemIdentifier, 
                                              Constants.LocalHost, e.Message).ConfigureAwait(false);

                if (currentNumExceptions >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.CreateSnapshotOperation} failed a maximum number of times for {Constants.LocalHost}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }
            }
            return createSnapshotSuccess;
        }

    }
}
