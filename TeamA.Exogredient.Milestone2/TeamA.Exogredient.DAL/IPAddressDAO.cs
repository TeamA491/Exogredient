using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for the data store containing IP Address information.
    /// </summary>
    public class IPAddressDAO : IMasterSQLDAO<string>
    {
        /// <summary>
        /// Asynchronously creates the <paramref name="record"/> in the data store.
        /// </summary>
        /// <param name="record">The record to insert (ISQLRecord)</param>
        /// <returns>Task(bool) whether the function executed without exception.</returns>
        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            // Try casting the record to an IPAddressRecord, throw an argument exception if it fails.
            try
            {
                IPAddressRecord temp = (IPAddressRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.IPCreateInvalidArgument);
            }

            // Get the data stored in the record.
            IPAddressRecord ipRecord = (IPAddressRecord)record;
            IDictionary<string, object> recordData = ipRecord.GetData();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                // Open the connection
                connection.Open();

                // Construct the sql string .. start by inserting into the table name
                string sqlString = $"INSERT INTO {Constants.IPAddressDAOtableName} (";

                // Loop through the data.
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check for null values in the data (string == null, numeric == -1), and throw a NoNullAllowedException
                    // if one is found.
                    if (pair.Value is string)
                    {
                        if (pair.Value == null)
                        {
                            throw new NoNullAllowedException(Constants.IPRecordNoNull);
                        }
                    }
                    if (pair.Value is int || pair.Value is long)
                    {
                        if (pair.Value.Equals(-1))
                        {
                            throw new NoNullAllowedException(Constants.IPRecordNoNull);
                        }
                    }

                    // Otherwise add the key to the string (column name).
                    sqlString += $"{pair.Key},";
                }

                // Remove the last comma, add the VALUES keyword
                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += ") VALUES (";

                // Loop through the data once again, but instead of constructing the string with user input, use
                // @PARAM0, @PARAM1 parameters to prevent against sql injections from user input.
                int count = 0;
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    sqlString += $"@PARAM{count},";
                    count++;
                }

                // Remove the last comma and add the last ) and ;
                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += ");";

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    count = 0;

                    // Loop through the data again to add the parameter values to the corresponding @PARAMs in the string.
                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                        count++;
                    }

                    // Asynchronously execute the non query.
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                return true;
            }
        }

        /// <summary>
        /// Delete all the objects referenced by the <paramref name="idsOfRows"/>.
        /// </summary>
        /// <param name="idsOfRows">The list of ids of rows to delete (List(string))</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Loop through the ids of rows.
                foreach (string ipAddress in idsOfRows)
                {
                    // Check if the ip exists in the table, throw an argument exception if it doesn't exist.
                    if (!await CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
                    {
                        throw new ArgumentException(Constants.IPDeleteDNE);
                    }

                    // Construct the sql string for deleteing where the ip column equals the @IPADDRESS parameter.
                    string sqlString = $"DELETE {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = @IPADDRESS;";

                    // Get the command object inside a using statement to properly dispose/close.
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        // Add the value of the ip address to the parameter and execute the non query asynchronously.
                        command.Parameters.AddWithValue("@IPADDRESS", ipAddress);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Read the information in the data store pointed to by the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the row to read (string)</param>
        /// <returns>Task (IDataObject) the information represented as an object</returns>
        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            // Check if the id exists in the table, and throw an argument exception if it doesn't.
            if(!await CheckIPExistenceAsync(id).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.IPReadDNE);
            }

            // Object to return -- IPAddressObject.
            IPAddressObject result;

            // Get the connection inside of a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to get the record where the id column equals the id parameter.
                string sqlString = $"SELECT * FROM {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = @ID;";

                // Get the command and data table objects inside using statements to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    // Add the value to the id parameter, execute the reader asynchronously, load the reader into
                    // the data table, and get the first row (the result).
                    command.Parameters.AddWithValue("@ID", id);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);
                    DataRow row = dataTable.Rows[0];

                    // Construct the IPAddressObject by casting the values of the columns to their proper data types.
                    result = new IPAddressObject((string)row[Constants.IPAddressDAOIPColumn],
                                                 (long)row[Constants.IPAddressDAOtimestampLockedColumn],
                                                 (int)row[Constants.IPAddressDAOregistrationFailuresColumn],
                                                 (long)row[Constants.IPAddressDAOlastRegFailTimestampColumn]);
                }
            }

            return result;
        }

        /// <summary>
        /// Update the <paramref name="record"/> in the data store based on the values that are not null inside it.
        /// </summary>
        /// <param name="record">The record containing the information to update (ISQLRecord)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public async Task<bool> UpdateAsync(ISQLRecord record)
        {
            // Try casting the record to an IPAddressRecord, throw an argument exception if it fails.
            try
            {
                IPAddressRecord temp = (IPAddressRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.IPUpdateInvalidArgument);
            }

            // Get the record data.
            IPAddressRecord ipRecord = (IPAddressRecord)record;
            IDictionary<string, object> recordData = ipRecord.GetData();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to update the table name where..
                string sqlString = $"UPDATE {Constants.IPAddressDAOtableName} SET ";

                // Loop through the record data.
                int count = 0;
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check if the value at the ip column is contained within the table, throw an argument
                    // exception if it doesn't exist.
                    if (pair.Key == Constants.IPAddressDAOIPColumn)
                    {
                        if (!await CheckIPExistenceAsync((string)pair.Value).ConfigureAwait(false))
                        {
                            throw new ArgumentException(Constants.IPUpdateDNE);
                        }
                    }

                    // Update only the values where the record value is not null (string == null, numeric == -1).
                    // Again, use parameters to prevent against sql injections.
                    if (pair.Key != Constants.IPAddressDAOIPColumn)
                    {
                        if (pair.Value is int || pair.Value is long)
                        {
                            if (!pair.Value.Equals(-1))
                            {
                                sqlString += $"{pair.Key} = @PARAM{count},";
                            }
                        }
                        else if (pair.Value != null)
                        {
                            sqlString += $"{pair.Key} = @PARAM{count},";
                        }
                    }

                    count++;
                }

                // Remove the last comma and identify the record by its ip column.
                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += $" WHERE {Constants.IPAddressDAOIPColumn} = '{recordData[Constants.IPAddressDAOIPColumn]}';";

                // Get the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Loop through the record data again to add values to the parameters.
                    count = 0;
                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != Constants.IPAddressDAOIPColumn)
                        {
                            command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                        }

                        count++;
                    }

                    // Execute the non query asynchronously.
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                return true;
            }
        }

        /// <summary>
        /// Check if the <paramref name="ipAddress"/> exists.
        /// </summary>
        /// <param name="ipAddress"> ip address to be checked </param>
        /// <returns> true if ip address exists, otherwise false </returns>
        public async Task<bool> CheckIPExistenceAsync(string ipAddress)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the ip column matches the ipAddress,
                // then check if at least 1 row exists. Use a parameter to protect against sql injections.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = @IPADDRESS);";

                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@IPADDRESS", ipAddress);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }
    }
}
