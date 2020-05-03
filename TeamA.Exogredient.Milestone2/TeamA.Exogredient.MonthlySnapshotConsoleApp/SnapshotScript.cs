using System;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.Managers;
using System.Threading.Tasks;

namespace TeamA.Exogredient.SnapshotCreateConsoleApp
{
    class SnapshotScript
    {
        public static async Task Main(string[] args)
        {
            var mapDAO = new MapDAO(Constants.MapSQLConnection);
            var logDAO = new LogDAO(Constants.NOSQLConnection);
            var userDAO = new UserDAO(Constants.SQLConnection);
            var uploadDAO = new UploadDAO(Constants.SQLConnection);
            var snapshotDAO = new SnapshotDAO(Constants.NOSQLConnection);

            var maskService = new MaskingService(mapDAO);
            var ffLoggingService = new FlatFileLoggingService(maskService);

            var dsLoggingService = new DataStoreLoggingService(logDAO, maskService);
            var snapshotService = new SnapshotService(logDAO, userDAO, uploadDAO, snapshotDAO);

            var loggingManager = new LoggingManager(ffLoggingService, dsLoggingService);
            var createSnapManager = new CreateSnapshotManager(loggingManager, snapshotService);
            
            // Script called at the beginning of every month.
            // Get the year and month according the Utc time.
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            // We want to create a snapshot of the previous month so we have to subtract 1 to the month if it is not 1 (January).
            // If it is 1, then change month to 12, and subtract 1 from year instead.
            if (month == 1)
            {
                year = year - 1;
                month = 12;
            }
            else
            {
                month = month - 1;
            }

            var tries = 0;
            var createSnapshotSuccess = false;

            // If the createsnapshot was a failure, there are 3 extra tries. It can stop if it was successful though.
            while (!createSnapshotSuccess && tries < 4) {
                createSnapshotSuccess = await createSnapManager.CreateSnapshotAsync(tries, year, month).ConfigureAwait(false);
                if(!createSnapshotSuccess)
                {
                    tries++;
                }
            }
            
        }
    }
}
