using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for accessing the logs in the data store.
    /// </summary>
    public class UnitTestLogDAO
    {
        // Datastore for logs.
        Dictionary<string, Dictionary<int, LogRecord>> Logs;
        // Counter for unique ID
        int counter;

        public UnitTestLogDAO()
        {
            Logs = new Dictionary<string, Dictionary<int,LogRecord>>();
            counter = 0;
        }

        /// <summary>
        /// Asynchronously inserts the <paramref name="record"/>'s data into the data store defined
        /// by the <paramref name="groupName"/>.
        /// </summary>
        /// <param name="record">The data to insert into the data store (INOSQLRecord)</param>
        /// <param name="groupName">The name of the group to create (string)</param>
        /// <returns>Task(bool) whether the function executed without exception</returns>
        public bool Create(INOSQLRecord record, string groupName)
        {
            // Convert the record to LogRecord.
            LogRecord logRecord;
            try
            {
                logRecord = (LogRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.LogCreateInvalidArgument);
            }

            // If the group name already exists
            if (Logs.ContainsKey(groupName))
            {
                // Add the log and the unique ID to the group name in the datastore.
                Logs[groupName].Add(counter,logRecord);
                counter++;
            }
            else
            {
                // Create a log dictionary.
                Dictionary<int, LogRecord> logDictionary = new Dictionary<int, LogRecord>();
                // Add the log and the unique ID to the group name in the datastore.
                logDictionary.Add(counter, logRecord);
                Logs.Add(groupName, logDictionary);
                counter++;
            }

            return true;
            
        }

        /// <summary>
        /// Asynchronously deletes the record defined by the <paramref name="uniqueId"/> from
        /// the data store further defined by the <paramref name="groupName"/>.
        /// </summary>
        /// <param name="uniqueId">The id of the record in the data store (string)</param>
        /// <param name="groupName">The name of the group the record is stored in (string)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public bool Delete(string uniqueId, string groupName)
        {
            // Get the logs of the group name.
            Dictionary<int, LogRecord> oneDayLogs = Logs[groupName];
            foreach(int id in oneDayLogs.Keys)
            {
                if (id.ToString().Equals(uniqueId))
                {
                    // If the uniqueId is equal to the id of the log, delete that log.
                    oneDayLogs.Remove(id);
                    return true;
                }
            }
            // If not found, throw an exception.
            throw new ArgumentException(Constants.LogDeleteDNE);
        }

        /// <summary>
        /// Find the id of the record that was inserted into the data store.
        /// </summary>
        /// <param name="record">The record to find (INOSQLRecord)</param>
        /// <param name="groupName">The name of the group where the record is located (string)</param>
        /// <returns>Task (string), the id of the record</returns>
        public string FindIdField(INOSQLRecord record, string groupName)
        {
            // Convert the record to LogRecord.
            LogRecord logRecord;
            try
            {
                logRecord = (LogRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.LogFindInvalidArgument);
            }
            foreach(KeyValuePair<int,LogRecord> pair in Logs[groupName])
            {
                if (pair.Value.Equals(logRecord))
                {
                    // If the log matches the given LogRecord, return its unique ID.
                    return pair.Key.ToString();
                }
            }
            // If the specific log is not found, throw an exception.
            throw new ArgumentException(Constants.LogFindDNE);
        }
    }
}
