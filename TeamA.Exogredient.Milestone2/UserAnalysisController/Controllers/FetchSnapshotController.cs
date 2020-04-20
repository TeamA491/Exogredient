using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

namespace UserAnalysisController.Controllers
{
    [Route("api/[controller]")]
    public class FetchSnapshotController : Controller
    {
        [HttpGet("FetchSingle/{year}/{month}")]
        public async Task<IActionResult> GetSingleSnapshotAsync(int year, int month)
        {
            int tries = 0;

            var uploaddao = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.NOSQLConnection);
            var userdao = new UserDAO(Constants.SQLConnection);
            var snapshotdao = new SnapshotDAO(Constants.NOSQLConnection);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            var snapshotService = new SnapshotService(logdao, userdao, uploaddao, snapshotdao);

            var readsnapshotManager = new ReadSnapshotManager(loggingManager, snapshotService);

            var snapshot = await readsnapshotManager.ReadOneSnapshotAsync(tries, 2020, 4).ConfigureAwait(false);
            Console.WriteLine(snapshot._month);

            try
            {
                // HACK ONLY FOR TESTING PURPOSE. MUST BE REMOVED IN PRODUCTION.
                if (month <= 0 || month > 12)
                {
                    throw new Exception("error");
                }

                return Ok(await readsnapshotManager.ReadOneSnapshotAsync(tries, year, month).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("FetchMulti/{year}")]
        public async Task<IActionResult> GetMultiSnapshotAsync(int year)
        {
            int tries = 0;

            var uploaddao = new UploadDAO(Constants.SQLConnection);
            var logdao = new LogDAO(Constants.NOSQLConnection);
            var userdao = new UserDAO(Constants.SQLConnection);
            var snapshotdao = new SnapshotDAO(Constants.NOSQLConnection);
            var mapdao = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(mapdao);
            var ffLogging = new FlatFileLoggingService(mask);
            var dsLogging = new DataStoreLoggingService(logdao, mask);
            var loggingManager = new LoggingManager(ffLogging, dsLogging);
            var snapshotService = new SnapshotService(logdao, userdao, uploaddao, snapshotdao);

            var readsnapshotManager = new ReadSnapshotManager(loggingManager, snapshotService);

            var snapshots = await readsnapshotManager.ReadMultiSnapshotAsync(tries, 2020).ConfigureAwait(false);

            foreach (var snaps in snapshots)
            {
                Console.WriteLine(snaps._month);
            }

            try
            {
                return Ok(await readsnapshotManager.ReadMultiSnapshotAsync(tries, year).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}