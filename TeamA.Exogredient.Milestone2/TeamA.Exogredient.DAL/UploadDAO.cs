using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class UploadDAO : IMasterSQLDAO<string>
    {
        private string _SQLConnection;

        public UploadDAO(string connection)
        {
            _SQLConnection = connection;
        }

        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IngredientResult>> ReadIngredientsByStoreIdAsync(int storeId, string ingredientName, string lastIngredientName)
        {
            var ingredients = new List<IngredientResult>();

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                var sqlString =
                    $"SELECT {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
                    $"AVG({Constants.UploadDAOTableName}.{Constants.UploadDAOPriceColumn}) AS {Constants.UploadDAOPriceColumn}, " +
                    $"COUNT(*) AS {Constants.UploadDAOUploadNumColumn} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} = @STORE_ID " +
                    (ingredientName == null ? "" : $"AND {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} LIKE @INGREDIENT_NAME ") +
                    (lastIngredientName == null ? "" : $"AND {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} > @LAST_INGREDIENT_NAME ") +
                    $"GROUP BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"ORDER BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} ASC " +
                    $"LIMIT @COUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    if (ingredientName != null)
                    {
                        command.Parameters.AddWithValue("@INGREDIENT_NAME", "%" + ingredientName + "%");
                    }
                    if(lastIngredientName != null)
                    {
                        command.Parameters.AddWithValue("@LAST_INGREDIENT_NAME", lastIngredientName);
                    }
                    command.Parameters.AddWithValue("@LAST_INGREDIENT_NAME", lastIngredientName);
                    command.Parameters.AddWithValue("@STORE_ID", storeId);
                    command.Parameters.AddWithValue("@COUNT", Constants.NumOfIngredientsPerStorePage);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    Console.WriteLine(sqlString);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        ingredients.Add(new IngredientResult((string)row[Constants.UploadDAOIngredientNameColumn], (double)row[Constants.UploadDAOPriceColumn],
                                                             Convert.ToInt32(row[Constants.UploadDAOUploadNumColumn])));
                    }
                }

                return ingredients;
            }
        }

        public async Task<bool> UpdateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }

    }
}
