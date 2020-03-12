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
                    $"LIMIT @START, @END;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Prepare();
                    // Add parameters into the sql string.
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@START", (pagination - 1) * Constants.SaveListPagination);
                    command.Parameters.AddWithValue("@END", pagination * Constants.SaveListPagination);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        saveLists.Add(new SaveListResult((string)row[Constants.SaveListDAOUsername], (string)row[Constants.SaveListDAOIngredient], Convert.ToInt32(row[Constants.SaveListDAOStoreColumn])));
                    }
                }
            }
            return saveLists;
        }


    }
}
