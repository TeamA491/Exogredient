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

        public async Task<bool> DeleteByPK(string username, int storeId, string ingredient)
        {
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Construct sql string for deleting a save list 
                string sqlString = $"DELETE FROM {Constants.SaveListDAOTableName} " +
                                   $"WHERE {Constants.SaveListDAOIngredient} = @INGREDIENT " +
                                   $"AND {Constants.SaveListDAOStoreColumn} = @STOREID " +
                                   $"AND {Constants.SaveListDAOUsername} = @USERNAME;";

                // Add values from the parameters and execute the command.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    command.Parameters.AddWithValue("@INGREDIENT", ingredient);
                    command.Parameters.AddWithValue("@STOREID", storeId);
                    command.Parameters.AddWithValue("@USERNAME", username);
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


        public async Task<List<SaveListResult>> ReadyByUsername(string username, int pagination)
        {
            var saveLists = new List<SaveListResult>();
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

        public async Task<int> GetPaginationSize(string username)
        {
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
                    command.Parameters.AddWithValue("@USERNAME", username);
                    var saveListcount = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false));

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
