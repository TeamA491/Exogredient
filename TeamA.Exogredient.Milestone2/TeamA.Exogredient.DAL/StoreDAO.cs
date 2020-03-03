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
        public async Task<List<StoreResult>> ReadByIngredientAsync(string ingredient,
            double latitude, double longitude, double radius)
        {
            var stores = new List<StoreResult>();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();
                

                var sqlString =
                    $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
                    $"COUNT(*) AS {Constants.StoreDAOUploadNumColumn}, " +
                    $"(SELECT(3959 * acos(cos(radians({latitude}))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
                    $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians({longitude}))+ " +
                    $"sin(radians({latitude}))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) as {Constants.StoreDAODistance} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}" +
                    $" WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"LIKE @INGREDIENT_NAME GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    $"HAVING {Constants.StoreDAODistance} <= {radius}";
                    /*
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
                    */

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@INGREDIENT_NAME", "%"+ingredient+"%");
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]), (string)row[Constants.StoreDAOStoreNameColumn],
                                                   Convert.ToInt32(row[Constants.StoreDAOUploadNumColumn]), (double)row[Constants.StoreDAODistance]));
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
