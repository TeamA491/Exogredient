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
    /// DAO for the data store containing User information.
    /// </summary>
    public class UserDAO : IMasterSQLDAO<string>
    {
        private string _SQLConnection;
        public UserDAO(string connection)
        {
            _SQLConnection = connection;
        }
        /// <summary>
        /// Asynchronously creates the <paramref name="record"/> in the data store.
        /// </summary>
        /// <param name="record">The record to insert (ISQLRecord)</param>
        /// <returns>Task(bool) whether the function executed without exception.</returns>
        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            // Try casting the record to a UserRecord, throw an argument exception if it fails.
            try
            {
                UserRecord temp = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserCreateInvalidArgument);
            }

            // Get the data stored in the record.
            UserRecord userRecord = (UserRecord)record;
            IDictionary<string, object> recordData = userRecord.GetData();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection
                connection.Open();

                // Construct the sql string .. start by inserting into the table name
                string sqlString = $"INSERT INTO {Constants.UserDAOtableName} (";

                // Loop through the data.
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check for null values in the data (string == null, numeric == -1), and throw a NoNullAllowedException
                    // if one is found.
                    if (pair.Value is int)
                    {
                        if ((int)pair.Value == -1)
                        {
                            throw new NoNullAllowedException(Constants.UserRecordNoNull);
                        }
                    }
                    if (pair.Value is string)
                    {
                        if (pair.Value == null)
                        {
                            throw new NoNullAllowedException(Constants.UserRecordNoNull);
                        }
                    }
                    if (pair.Value is long)
                    {
                        if ((long)pair.Value == -1)
                        {
                            throw new NoNullAllowedException(Constants.UserRecordNoNull);
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
        /// <returns>Task (bool) whether the function executed without exception.</returns>
        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Loop through the ids of rows.
                foreach (string username in idsOfRows)
                {
                    // Check if the username exists in the table, throw an argument exception if it doesn't exist.
                    if (!await CheckUserExistenceAsync(username).ConfigureAwait(false))
                    {
                        throw new ArgumentException(Constants.UserDeleteDNE);
                    }

                    // Construct the sql string for deleteing where the username column equals the @USERNAME parameter.
                    string sqlString = $"DELETE FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @USERNAME;";

                    // Get the command object inside a using statement to properly dispose/close.
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        // Add the value of the username to the parameter and execute the non query asynchronously.
                        command.Parameters.AddWithValue("@USERNAME", username);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Asynchronously read the information in the data store pointed to by the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the row to read (string)</param>
        /// <returns>Task (IDataObject) the information represented as an object</returns>
        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            // Check if the id exists in the table, and throw an argument exception if it doesn't.
            if (!await CheckUserExistenceAsync(id).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.UserReadDNE);
            }

            // Object to return -- UserObject
            UserObject result;

            // Get the connection inside of a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Construct the sql string to get the record where the id column equals the id parameter.
                string sqlString = $"SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @ID;";

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

                    // Construct the UserObject by casting the values of the columns to their proper data types.
                    result = new UserObject((string)row[Constants.UserDAOusernameColumn], (string)row[Constants.UserDAOnameColumn], (string)row[Constants.UserDAOemailColumn],
                                            (string)row[Constants.UserDAOphoneNumberColumn], (string)row[Constants.UserDAOpasswordColumn],
                                            (bool)row[Constants.UserDAOdisabledColumn] ? Constants.DisabledStatus : Constants.EnabledStatus, (string)row[Constants.UserDAOuserTypeColumn],
                                            (string)row[Constants.UserDAOsaltColumn], (long)row[Constants.UserDAOtempTimestampColumn],
                                            (string)row[Constants.UserDAOemailCodeColumn], (long)row[Constants.UserDAOemailCodeTimestampColumn],
                                            (int)row[Constants.UserDAOloginFailuresColumn], (long)row[Constants.UserDAOlastLoginFailTimestampColumn],
                                            (int)row[Constants.UserDAOemailCodeFailuresColumn], (int)row[Constants.UserDAOphoneCodeFailuresColumn]);
                }
            }

            return result;
        }

        /// <summary>
        /// Update the <paramref name="record"/> in the data store based on the values that are not null inside it.
        /// </summary>
        /// <param name="record">The record containing the information to update (ISQLRecord)</param>
        /// <returns>Task (bool) whether the function executed without exception.</returns>
        public async Task<bool> UpdateAsync(ISQLRecord record)
        {
            // Try casting the record to a UserRecord, throw an argument exception if it fails.
            try
            {
                UserRecord temp = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserUpdateInvalidArgument);
            }

            // Get the record data.
            UserRecord userRecord = (UserRecord)record;
            IDictionary<string, object> recordData = userRecord.GetData();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to update the table name where..
                string sqlString = $"UPDATE {Constants.UserDAOtableName} SET ";

                // Loop through the record data.
                int count = 0;
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check if the value at the username column is contained within the table, throw an argument
                    // exception if it doesn't exist.
                    if (pair.Key == Constants.UserDAOusernameColumn)
                    {
                        if (!await CheckUserExistenceAsync((string)pair.Value).ConfigureAwait(false))
                        {
                            throw new ArgumentException(Constants.UserUpdateDNE);
                        }
                    }

                    // Update only the values where the record value is not null (string == null, numeric == -1).
                    // Again, use parameters to prevent against sql injections.
                    if (pair.Key != Constants.UserDAOusernameColumn)
                    {
                        if (pair.Value is int)
                        {
                            if ((int)pair.Value != -1)
                            {
                                sqlString += $"{pair.Key} = @PARAM{count},";
                            }
                        }
                        if (pair.Value is string)
                        {
                            if (pair.Value != null)
                            {
                                sqlString += $"{pair.Key} = @PARAM{count},";
                            }
                        }
                        if (pair.Value is long)
                        {
                            if ((long)pair.Value != -1)
                            {
                                sqlString += $"{pair.Key} = @PARAM{count},";
                            }
                        }
                    }

                    count++;
                }

                // Remove the last comma and identify the record by its username column.
                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += $" WHERE {Constants.UserDAOusernameColumn} = '{recordData[Constants.UserDAOusernameColumn]}';";

                // Get the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Loop through the record data again to add values to the parameters.
                    count = 0;
                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != Constants.UserDAOusernameColumn)
                        {
                            if (pair.Value is int)
                            {
                                if ((int)pair.Value != -1)
                                {
                                    command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                                }
                            }
                            if (pair.Value is string)
                            {
                                if (pair.Value != null)
                                {
                                    command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                                }
                            }
                            if (pair.Value is long)
                            {
                                if ((long)pair.Value != -1)
                                {
                                    command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                                }
                            }
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
        /// Check if the <paramref name="username"/> exists.
        /// </summary>
        /// <param name="username"> username to be checked </param>
        /// <returns> true if username exists, otherwise false </returns>
        public async Task<bool> CheckUserExistenceAsync(string username)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the username column matches the username,
                // then check if at least 1 row exists. Use a parameter to protect against sql injections.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @USERNAME);";

                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@USERNAME", username);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        /// <summary>
        /// Check if the <paramref name="phoneNumber"/> exists.
        /// </summary>
        /// <param name="phoneNumber"> phone number to be checked </param>
        /// <returns> true if phone number exists, otherwise false </returns>
        public async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the phone number column matches the phoneNumber,
                // then check if at least 1 row exists. Use a parameter to protect against sql injections.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOphoneNumberColumn} = @PHONENUMBER);";

                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@PHONENUMBER", phoneNumber);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        /// <summary>
        /// Check if the <paramref name="email"/> exists.
        /// </summary>
        /// <param name="email"> email to be checked </param>
        /// <returns> true if email exists, otherwise false </returns>
        public async Task<bool> CheckEmailExistenceAsync(string email)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the email column matches the email,
                // then check if at least 1 row exists. Use a parameter to protect against sql injections.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOemailColumn} = @EMAIL);";

                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@EMAIL", email);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        public async Task<String> ReadUserType(String user)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the email column matches the email,
                string sqlString = $"SELECT {Constants.UserDAOuserTypeColumn} FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @USERNAME";

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@USERNAME", user);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    reader.Read();
                    return reader.GetString(0);
                }
            }

        }
    }
}
