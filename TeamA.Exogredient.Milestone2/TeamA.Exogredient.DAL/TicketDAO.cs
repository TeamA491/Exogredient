using System;
using System.Data;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for the data store containing Ticket information
    /// </summary>
    public class TicketDAO : IMasterSQLDAO<uint>
    {
        private string _SQLConnection;

        public TicketDAO(string connection)
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
            // Check if it is a TicketRecord or not
            TicketRecord ticketRecord;
            try
            {
                ticketRecord = (TicketRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.TicketCreateInvalidArgument);
            }

            IDictionary<string, object> recordData = ticketRecord.GetData();

            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();
                StringBuilder sqlString = new StringBuilder($"INSERT INTO {Constants.TicketDAOTableName} (");

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check for null values in the data (string == null, numeric == -1)
                    if (pair.Value is int || pair.Value is long)
                    {
                        if ((int)pair.Value < 0)
                        {
                            throw new NoNullAllowedException(Constants.TicketRecordNoNull);
                        }
                    }
                    else if (pair.Value is string)
                    {
                        if (pair.Value == null)
                        {
                            throw new NoNullAllowedException(Constants.TicketRecordNoNull);
                        }
                    }

                    // Otherwise add the key to the string (column name).
                    sqlString.Append($"{pair.Key},");
                }

                // Remove the last comma, add the VALUES keyword
                sqlString.Length--;
                sqlString.Append(") VALUES (");

                // Loop through the data once again, but instead of constructing the string with user input, use
                // @PARAM0, @PARAM1 parameters to prevent against sql injections from user input.
                int count = 0;
                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    sqlString.Append($"@PARAM{count},");
                    count++;
                }

                // Remove the last comma and add the last ) and ;
                sqlString.Length--;
                sqlString.Append(");");

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection))
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
        public async Task<bool> DeleteByIdsAsync(List<uint> idsOfRows)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Loop through the ids of rows.
                foreach (uint ticketID in idsOfRows)
                {
                    if (!await CheckTicketExistenceAsync(ticketID).ConfigureAwait(false))
                        throw new ArgumentException(Constants.TicketDeleteDNE);

                    string sqlString = $"DELETE FROM {Constants.TicketDAOTableName} WHERE {Constants.TicketDAOTicketIDColumn} = @TICKETID;";
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        // Add the value of the ticket id to the parameter and execute the non query asynchronously
                        command.Parameters.AddWithValue("@TICKETID", ticketID);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Asynchronously read the information in the data store pointed to by the <paramref name="ticketID"/>.
        /// </summary>
        /// <param name="ticketID">The ticket ID of the row to read (string)</param>
        /// <returns>Task (IDataObject) the information represented as an object</returns>
        public async Task<IDataObject> ReadByIdAsync(uint ticketID)
        {
            if (!await CheckTicketExistenceAsync(ticketID).ConfigureAwait(false))
                throw new ArgumentException(Constants.TicketReadDNE);

            // Object to return -- TicketObject
            TicketObject result;

            // Get the connection inside of a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Construct the sql string to get the record where the id column equals the id parameter.
                string sqlString = $"SELECT * FROM {Constants.TicketDAOTableName} WHERE {Constants.TicketDAOTicketIDColumn} = @TICKETID;";

                // Get the command and data table objects inside using statements to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    // Add the value to the id parameter, execute the reader asynchronously, load the reader into
                    // the data table, and get the first row (the result).
                    command.Parameters.AddWithValue("@TICKETID", ticketID);
                    DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);
                    DataRow row = dataTable.Rows[0];

                    // Construct the TicketObject by casting the values of the columns to their proper data types.
                    result = new TicketObject((uint)row[Constants.TicketDAOSubmitTimestampColumn],
                                              (string)row[Constants.TicketDAOCategoryColumn],
                                              (string)row[Constants.TicketDAOStatusColumn],
                                              (string)row[Constants.TicketDAOFlagColorColumn],
                                              (string)row[Constants.TicketDAODescriptionColumn],
                                              (bool)row[Constants.TicketDAOIsReadColumn]);
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
            TicketRecord ticketRecord;
            try
            {
                ticketRecord = (TicketRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.TicketUpdateInvalidArgument);
            }

            ticketRecord = (TicketRecord)record;
            IDictionary<string, object> recordData = ticketRecord.GetData();

            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                int count = 0;
                StringBuilder sqlString = new StringBuilder($"UPDATE {Constants.TicketDAOTableName} SET ");

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    // Check if the value at the ticket id column is contained within the table, throw an argument
                    // exception if it doesn't exist.
                    if (pair.Key == Constants.TicketDAOTicketIDColumn)
                    {
                        if (!await CheckTicketExistenceAsync((uint)pair.Value).ConfigureAwait(false))
                            throw new ArgumentException(Constants.TicketUpdateDNE);
                    }

                    // Update only the values where the record value is not null (string == null, numeric == -1).
                    // Again, use parameters to prevent against sql injections.
                    if (pair.Key != Constants.TicketDAOTicketIDColumn)
                    {
                        if (pair.Value is int || pair.Value is long)
                        {
                            if ((int)pair.Value != -1)
                            {
                                sqlString.Append($"{pair.Key} = @PARAM{count},");
                            }
                        }
                        else if (pair.Value is string)
                        {
                            if (pair.Value != null)
                            {
                                sqlString.Append($"{pair.Key} = @PARAM{count},");
                            }
                        }
                    }

                    count++;
                }

                // Remove the last comma and identify the record by its ticket id column
                sqlString.Length--;
                sqlString.Append($" WHERE {Constants.TicketDAOTicketIDColumn} = '{recordData[Constants.TicketDAOTicketIDColumn]}';");

                using (MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection))
                {
                    // Loop through the record data again to add values to the parameters.
                    count = 0;
                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != Constants.TicketDAOTicketIDColumn)
                        {
                            if (pair.Value is int || pair.Value is long)
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
                        }

                        count++;
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                return true;
            }
        }

        /// <summary>
        /// Check if the <paramref name="ticketID"/> exists.
        /// </summary>
        /// <param name="ticketID">ticket id to be checked </param>
        /// <returns> true if ticket id exists, otherwise false </returns>
        public async Task<bool> CheckTicketExistenceAsync(uint ticketID)
        {
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Check if the row exists for the ticket id
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.TicketDAOTableName} WHERE {Constants.TicketDAOTicketIDColumn} = @TICKETID);";
                bool result;

                // Open the command inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                    command.Parameters.AddWithValue("@TICKETID", ticketID);
                    DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public async Task<List<TicketRecord>> FilterTicketsAsync(Dictionary<Constants.TicketSearchFilter, object> filterParams)
        {
            // Make sure we have at least one filter parameter
            if (filterParams.Count == 0)
                throw new ArgumentException(Constants.TicketFilterNeedsParameter);

            List<string> queryConditions = new List<string>();

            // Go through all the search params
            foreach (KeyValuePair<Constants.TicketSearchFilter, object> filter in filterParams)
            {
                if (filter.Key == Constants.TicketSearchFilter.Category)
                {
                    // Make sure we are using a correct Enum value
                    if (!(filter.Value is Constants.TicketCategories))
                        throw new ArgumentException(Constants.TicketIncorrectEnum);

                    queryConditions.Add(
                        $"`{Constants.TicketDAOCategoryColumn}` = `{filter.Value}`"
                    );
                }
                else if (filter.Key == Constants.TicketSearchFilter.DateFrom)
                {
                    // Make sure we are using an positive int
                    if (!(filter.Value is int i && i > 0))
                        throw new ArgumentException(Constants.TicketIncorrectInt);

                    queryConditions.Add(
                        $"`{Constants.TicketDAOSubmitTimestampColumn}` >= {filter.Value}"
                    );
                }
                else if (filter.Key == Constants.TicketSearchFilter.DateTo)
                {
                    // Make sure we are using an positive int
                    if (!(filter.Value is int i && i > 0))
                        throw new ArgumentException(Constants.TicketIncorrectInt);

                    queryConditions.Add(
                        $"`{Constants.TicketDAOSubmitTimestampColumn}` <= {filter.Value}"
                    );
                }
                else if (filter.Key == Constants.TicketSearchFilter.FlagColor)
                {
                    // Make sure we are using a correct Enum value
                    if (!(filter.Value is Constants.TicketFlagColors))
                        throw new ArgumentException(Constants.TicketIncorrectEnum);

                    queryConditions.Add(
                        $"`{Constants.TicketDAOFlagColorColumn}` = `{filter.Value}`"
                    );
                }
                else if (filter.Key == Constants.TicketSearchFilter.ReadStatus)
                {
                    // Make sure we are using a correct Enum value
                    if (filter.Value is Constants.TicketReadStatuses ticketReadStatus)
                    {
                        // If we want to see both read and unread, then skip this condition
                        if (ticketReadStatus == Constants.TicketReadStatuses.All)
                            continue;
                    }
                    else
                        throw new ArgumentException(Constants.TicketIncorrectEnum);

                    bool isRead = ticketReadStatus == Constants.TicketReadStatuses.Read;
                    queryConditions.Add(
                        $"`{Constants.TicketDAOIsReadColumn}` = `{isRead}`"
                    );
                }
                else if (filter.Key == Constants.TicketSearchFilter.Status)
                {
                    // Make sure we are using a correct Enum value
                    if (!(filter.Value is Constants.TicketStatuses))
                        throw new ArgumentException(Constants.TicketIncorrectEnum);

                    queryConditions.Add(
                        $"`{Constants.TicketDAOStatusColumn}` = `{filter.Value}`"
                    );
                }
            }

            // Construct the query string
            string sqlString = $"SELECT * FROM `{Constants.TicketDAOTableName}` WHERE " +
                                string.Join(" AND ", queryConditions) +
                                ";";

            // Fetch the data from the database

            // Temp
            List<TicketRecord> tickets = new List<TicketRecord>();
            return tickets;
        }

        public async Task<List<TicketRecord>> GetAllTickets()
        {
            string sqlString = $"SELECT * FROM `{Constants.TicketDAOTableName}`;";
            return new List<TicketRecord>();
        }
    }
}
