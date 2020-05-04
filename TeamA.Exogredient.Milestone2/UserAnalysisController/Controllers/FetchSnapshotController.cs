using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

namespace UserAnalysisController.Controllers
{
    [Route("api/[controller]")]
    public class FetchSnapshotController : Controller
    {
        /// <summary>
        /// Method to get snapshot specific to the year and month value and format it to send up.
        /// </summary>
        /// <param name="year">The year to get the snapshot.</param>
        /// <param name="month">The month to get the snapshot.</param>
        /// <returns>The snapshot object.</returns>
        [HttpGet("FetchSingle/{year}/{month}")]
        public async Task<IActionResult> GetSingleSnapshotAsync(int year, int month)
        {
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

            try
            {
                return Ok(await readsnapshotManager.ReadOneSnapshotAsync(year, month).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Method to get multiple snapshot specific to the year.
        /// It will also format the data in the multiple snapshot and store it as one snapshot object.
        /// </summary>
        /// <param name="year">The year to get all the snapshots.</param>
        /// <returns>The snapshot object with the filtered data.</returns>
        [HttpGet("FetchMulti/{year}")]
        public async Task<IActionResult> GetMultiSnapshotAsync(int year)
        {
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

            try
            {
                return Ok(await readsnapshotManager.ReadMultiSnapshotAsync(year).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}