using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class MapDAO : IMasterSQLCRD<string>
    {
        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            // Try casting the record to a MapRecord, throw an argument exception if it fails.
            try
            {
                MapRecord temp = (MapRecord)record;
            }
            catch
            {
                // TODO: exception message
                throw new ArgumentException();
            }

            MapRecord mapRecord = (MapRecord)record;
            IDictionary<string, object> recordData = mapRecord.GetData();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.MapSQLConnection))
            {
                connection.Open();

                // Construct the sql string .. start by inserting into the table name
                string sqlString = $"INSERT INTO {Constants.MapDAOTableName} (";

                foreach (KeyValuePair<string, object> pair in recordData)
                {
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

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.MapSQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Loop through the ids of rows.
                foreach (string hash in idsOfRows)
                {
                    // Check if the hash exists in the table, throw an argument exception if it doesn't exist.
                    if (!await CheckHashExistenceAsync(hash).ConfigureAwait(false))
                    {
                        // TODO: exception message
                        throw new ArgumentException();
                    }

                    // Construct the sql string for deleteing where the hash column equals the @HASH parameter.
                    string sqlString = $"DELETE FROM {Constants.MapDAOTableName} WHERE {Constants.MapDAOHashColumn} = @HASH;";

                    // Get the command object inside a using statement to properly dispose/close.
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        // Add the value of the hash to the parameter and execute the non query asynchronously.
                        command.Parameters.AddWithValue("@HASH", hash);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                return true;
            }
        }

        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            // Check if the id exists in the table, and throw an argument exception if it doesn't.
            if (!await CheckHashExistenceAsync(id).ConfigureAwait(false))
            {
                // TODO: exception message
                throw new ArgumentException();
            }

            // Object to return -- MapObject
            MapObject result;

            // Get the connection inside of a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.MapSQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to get the record where the id column equals the id parameter.
                string sqlString = $"SELECT * FROM {Constants.MapDAOTableName} WHERE {Constants.MapDAOHashColumn} = @ID;";

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

                    // Construct the MapObject by casting the values of the columns to their proper data types.
                    result = new MapObject((string)row[Constants.MapDAOHashColumn], (string)row[Constants.MapDAOActualColumn]);
                }
            }

            return result;
        }

        public async Task<bool> CheckHashExistenceAsync(string hash)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(Constants.MapSQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Construct the sql string to select all from the table where the username column matches the hash,
                // then check if at least 1 row exists. Use a parameter to protect against sql injections.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.MapDAOTableName} WHERE {Constants.MapDAOHashColumn} = @HASH);";

                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@HASH", hash);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }
    }
}
