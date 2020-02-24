using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;
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
                    $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
                    $"{Constants.StoreDAOTableName}.{Constants.StoreDAOLatitudeColumn}, " +
                    $"{Constants.StoreDAOTableName}.{Constants.StoreDAOLongitudeColumn}, " +
                    $"COUNT(*) as {Constants.StoreDAOUploadNumColumn} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"= @INGREDIENT_NAME GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn};";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@INGREDIENT_NAME", ingredient);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stores.Add(new SearchResultStoreObject((int)row[Constants.StoreDAOStoreIdColumn], (string)row[Constants.StoreDAOStoreNameColumn], Convert.ToInt32(row[Constants.StoreDAOUploadNumColumn])),
                                   new Geocode((double)row[Constants.StoreDAOLatitudeColumn],(double)row[Constants.StoreDAOLongitudeColumn]));
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
