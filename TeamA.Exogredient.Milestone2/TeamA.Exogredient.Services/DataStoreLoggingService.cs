using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// This service stores or deletes a log in the data store by using the DAL.
    /// </summary>
    public static class DataStoreLoggingService
    {
        private static readonly LogDAO _dsLoggingDAO;

        /// <summary>
        /// Initiates the object and its dependencies.
        /// </summary>
        static DataStoreLoggingService()
        {
            _dsLoggingDAO = new LogDAO();
        }

        /// <summary>
        /// Asynchronously stores a log in the data store.
        /// </summary>
        /// <param name="timestamp">timestamp of the log (string)</param>
        /// <param name="operation">operation of the log (string)</param>
        /// <param name="identifier">identifier of the performer of the operation (string)</param>
        /// <param name="ipAddress">ip address of the performer of the operation (string)</param>
        /// <param name="errorType">the error type that occurred during the operation (string)</param>
        /// <returns>Task (bool) whether the log creation was successful</returns>
        public static async Task<bool> LogToDataStoreAsync(string timestamp, string operation, string identifier,
                                                           string ipAddress, string errorType)
        {
            try
            {
                // Currently the timestamp is expected to be in the following format: "HH:mm:ss:ff UTC yyyyMMdd".
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                // Create the log record to be stored, mostly just the parameters to this function apart from the timestamp.
                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                MaskingService maskingService = new MaskingService(new MapDAO());

                LogRecord resultRecord = (LogRecord)await maskingService.MaskAsync(logRecord, false).ConfigureAwait(false);

                // The name of the collection/table should be a derivative of the "yyyyMMdd" part of the timestamp.
                // Asynchronously call the Log DAO's function to create the log record in the collection denoted by the name (second parameter).
                return await _dsLoggingDAO.CreateAsync(resultRecord, splitResult[2]).ConfigureAwait(false);
            }
            catch
            {
                // Any exception results in a failed creation.
                return false;
            }
        }

        /// <summary>
        /// Asynchronously delete a previously logged item from the data store.
        /// </summary>
        /// <param name="timestamp">the timestamp that was logged (string)</param>
        /// <param name="operation">the operation that was logged (string)</param>
        /// <param name="identifier">the identifier that was logged (string)</param>
        /// <param name="ipAddress">the ip address that was logged (string)</param>
        /// <param name="errorType">the error type that was logged (string)</param>
        /// <returns>Task (bool) whether the log deletion was successful</returns>
        public static async Task<bool> DeleteLogFromDataStoreAsync(string timestamp, string operation, string identifier,
                                                                   string ipAddress, string errorType)
        {
            try
            {
                // Currently the timestamp is expected to be in the following format: "HH:mm:ss:ff UTC yyyyMMdd".
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                // Create the log record to be found, the parameters to this function should make it unique among the data store,
                // i.e no unique operation should have the same timestamp.
                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                MaskingService maskingService = new MaskingService(new MapDAO());

                LogRecord resultRecord = (LogRecord)await maskingService.MaskAsync(logRecord, false).ConfigureAwait(false);

                // Asynchronously find the id field of the log in the data store, passing the collection/table name.
                string id = await _dsLoggingDAO.FindIdFieldAsync(resultRecord, splitResult[2]).ConfigureAwait(false);

                // Asynchronously delete the log by id in the data store.
                await _dsLoggingDAO.DeleteAsync(id, splitResult[2]).ConfigureAwait(false);

                return true;
            }
            catch
            {
                // Any exceptions result in a failed deletion.
                return false;
            }
        }
    }
}
