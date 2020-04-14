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
    public class SaveListDAO : IMasterSQLCRD<string>
    {
        private readonly string _SQLConnection;

        public SaveListDAO(string connection)
        {
            _SQLConnection = connection;
        }

        public Task<bool> CreateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public Task<IDataObject> ReadByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a save list from the data store.
        /// </summary>
        /// <param name="username">Username of save list to delete.</param>
        /// <param name="storeId">StoreId of the save list to delete.</param>
        /// <param name="ingredient">Ingredient of the save list.</param>
        /// <returns>bool reprsenting whether the operation passed.</returns>
        public async Task<bool> DeleteByPKAsync(string username, int storeId, string ingredient)
        {
            // Get connection inside using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Construct sql string for deleting a save list 
                string sqlString = $"DELETE FROM {Constants.SaveListDAOTableName} " +
                                   $"WHERE {Constants.SaveListDAOIngredient} = @INGREDIENT " +
                                   $"AND {Constants.SaveListDAOStoreColumn} = @STOREID " +
                                   $"AND {Constants.SaveListDAOUsername} = @USERNAME;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add values from the parameters.
                    command.Parameters.AddWithValue("@INGREDIENT", ingredient);
                    command.Parameters.AddWithValue("@STOREID", storeId);
                    command.Parameters.AddWithValue("@USERNAME", username);

                    // Execute command.
                    int result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    // Throw exception if the command doesn't delete a row.
                    if(result == 0)
                    {
                        throw new ArgumentException(Constants.SaveListDNE);
                    }
                } 
            }
            return true;
        }

        /// <summary>
        /// Get the save list for a user.
        /// </summary>
        /// <param name="username">User to get save list from.</param>
        /// <param name="pagination">Specific pagination of the save list.</param>
        /// <returns>List of SaveListResult.</returns>
        public async Task<List<SaveListResult>> ReadyByUsernameAsync(string username, int pagination)
        {
            var saveLists = new List<SaveListResult>();

            // Get connection inside using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrieve all saveLists for a user.
                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.SaveListDAOTableName} " +
                    $"WHERE {Constants.SaveListDAOUsername} = @USERNAME " +
                    $"LIMIT @OFFSET, @AMOUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                { 
                    using (DataTable dataTable = new DataTable())
                    {
                        // Add parameters into the sql string.
                        command.Parameters.AddWithValue("@USERNAME", username);
                        command.Parameters.AddWithValue("@OFFSET", pagination * Constants.SaveListPagination);
                        command.Parameters.AddWithValue("@AMOUNT", Constants.SaveListPagination);

                        // Execute command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                        dataTable.Load(reader);
                        foreach (DataRow row in dataTable.Rows)
                        {
                            saveLists.Add(new SaveListResult((string)row[Constants.SaveListDAOUsername], (string)row[Constants.SaveListDAOIngredient], Convert.ToInt32(row[Constants.SaveListDAOStoreColumn])));
                        }
                    }
                }
            }
            return saveLists;
        }

        /// <summary>
        /// Get the pagination size for a user.
        /// </summary>
        /// <param name="username">User to get the pagination size from.</param>
        /// <returns>int representing the pagination size.</returns>
        public async Task<int> GetPaginationSizeAsync(string username)
        {

            // Get connection inside using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrieve count of all saveLists for a user.
                var sqlString =
                    $"SELECT COUNT(*) " +
                    $"FROM {Constants.SaveListDAOTableName} " +
                    $"WHERE {Constants.SaveListDAOUsername} = @USERNAME;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Add paramters to command.
                    command.Parameters.AddWithValue("@USERNAME", username);

                    // Execute command and convert object to int.
                    var saveListcount = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false));

                    // Perform logic to account for needed extra pagination.
                    var paginationSize = saveListcount / Constants.SaveListPagination;
                    if(paginationSize == 0)
                    {
                        return 1;
                    }
                    else if((paginationSize % Constants.SaveListPagination) == 0)
                    {
                        return paginationSize;
                    }
                    else
                    {
                        return paginationSize + 1;
                    }                    
                }
            }
        }
    }
}
