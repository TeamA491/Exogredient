using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class StoreDAO: IMasterSQLDAO<int>
    {
        private readonly string _SQLConnection;

        public StoreDAO(string connection)
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

        public Task<IDataObject> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        //TODO Replace column names with constants
        public async Task<Dictionary<SearchResultStoreObject,Geocode>> ReadByIngredientAsync(string ingredient)
        {
            var stores = new Dictionary<SearchResultStoreObject,Geocode>();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                var sqlString =
                    $"SELECT store.store_id, store.store_name, store.latitude, store.longitude, COUNT(*) as upload_num " +
                    $"FROM upload INNER JOIN store ON upload.store_id = store.store_id " +
                    $"WHERE upload.ingredient_name = @INGREDIENT_NAME GROUP BY store.store_id;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@INGREDIENT_NAME", ingredient);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stores.Add(new SearchResultStoreObject((int)row["store_id"], (string)row["store_name"],3),
                                   new Geocode((double)row["latitude"],(double)row["longitude"]));
                    }

                }
            }

            return stores;

        }

        public Task<bool> UpdateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
