﻿using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns the number of total retreived ingredients that are associated with
        /// storeID and ingredient name if not null.
        /// </summary>
        /// <param name="storeId">The storeID of a store from which to get ingredients</param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <returns>The number of total retreived ingredients of the store</returns>
        public async Task<int> GetTotalIngredientResultsNumberAsync(int storeId, string ingredientName)
        {
            int totalResultsNum;
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                //Retreive ingredient names of a certain StoreID's uploads that's associated with ingredientName.
                var subQuery =
                   $"SELECT {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                   $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                   $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                   $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                   $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} = @STORE_ID " +
                   (ingredientName == null ?
                   "" : $"AND {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} LIKE @INGREDIENT_NAME ") +
                   $"GROUP BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                   $"ORDER BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} ";

                //Count the distinct ingredients names.
                var sqlString =
                    $"SELECT COUNT(*) AS {Constants.StoreDAOTotalResultsNum} FROM ({subQuery}) as x";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    //Inject arguments into the query.
                    if (ingredientName != null)
                    {
                        command.Parameters.AddWithValue("@INGREDIENT_NAME", "%" + ingredientName + "%");
                    }
                    command.Parameters.AddWithValue("@STORE_ID", storeId);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    //Read the number of the distinct ingredients names.
                    totalResultsNum = Convert.ToInt32(dataTable.Rows[0][Constants.StoreDAOTotalResultsNum]);
                }

                return totalResultsNum;
            }
        }

        /// <summary>
        /// Reads the ingredients of a certain page that are associated with
        /// storeId, ingredientName if not null.
        /// </summary>
        /// <param name="storeId">StoreID of a store</param>
        /// <param name="ingredientName">If not null, it filters what ingredients to be returned</param>
        /// <param name="lastIngredientName">
        /// The last ingredient name of of last page that was displayed
        /// (only needed when page changes, otherwise should be null).
        /// </param>
        /// <param name="lastPageResultsNum">
        /// The number of results of last page that was displayed
        /// (only needed when page moved backward).
        /// </param>
        /// <param name="skipPages">The number of pages moved in pagination</param>
        /// <returns>List of IngredientResult objects</returns>
        public async Task<List<IngredientResult>> ReadIngredientsByStoreIdAsync(int storeId,
            string ingredientName, string lastIngredientName, int lastPageResultsNum, int skipPages)
        {
            var ingredients = new List<IngredientResult>();

            // Check if pagination moved backward
            var isSkipPagesNeg = skipPages < 0;

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Read ingredients, avg prices, number of uploads that are associated with ingredientName and pagination data.
                // Retrieve data in opposite order if pagination moved backward.
                var sqlString =
                    $"SELECT {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
                    $"AVG({Constants.UploadDAOTableName}.{Constants.UploadDAOPriceColumn}) AS {Constants.UploadDAOPriceColumn}, " +
                    $"COUNT(*) AS {Constants.UploadDAOUploadNumColumn} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} = @STORE_ID " +
                    (ingredientName == null ?
                    "" : $"AND {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} LIKE @INGREDIENT_NAME ") +
                    (lastIngredientName == null ?
                    "" : $"AND {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"{(isSkipPagesNeg ? $"<" : $">")} @LAST_INGREDIENT_NAME ") +
                    $"GROUP BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"ORDER BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    (isSkipPagesNeg ? $"DESC " : $"ASC ") +
                    $"LIMIT @OFFSET, @COUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    //Inject arguments to the query
                    if (ingredientName != null)
                    {
                        command.Parameters.AddWithValue("@INGREDIENT_NAME", "%" + ingredientName + "%");
                    }
                    if(lastIngredientName != null)
                    {
                        command.Parameters.AddWithValue("@LAST_INGREDIENT_NAME", lastIngredientName);
                    }
                    command.Parameters.AddWithValue("@STORE_ID", storeId);

                    // Offset by (results per page) * (how many pages skipped - 1) from the last result of last page.
                    // If pagination moved backward, we add the number of results of last page - 1
                    // (-1 is to exclude the last result of last page from counting).
                    command.Parameters.AddWithValue
                    (
                        "@OFFSET", isSkipPagesNeg ?
                        (Math.Abs(skipPages) - 1) * Constants.NumOfResultsPerSearchPage + (lastPageResultsNum - 1)
                        : (skipPages - 1) * Constants.NumOfResultsPerSearchPage
                    );
                    command.Parameters.AddWithValue("@COUNT", Constants.NumOfIngredientsPerStorePage);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        //Create IngredientResult with retreived data and add to list.
                        ingredients.Add(new IngredientResult(
                            (string)row[Constants.UploadDAOIngredientNameColumn],
                            (double)row[Constants.UploadDAOPriceColumn],
                            Convert.ToInt32(row[Constants.UploadDAOUploadNumColumn])));
                    }
                    if (isSkipPagesNeg)
                    {
                        // Since moving pagination backward returns list of ingredients backward,
                        // it has to be reversed to be in correct order.
                        ingredients.Reverse();
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
