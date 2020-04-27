﻿﻿using System;
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

        /// <summary>
        /// Delete a list of uploads using ids.
        /// </summary>
        /// <param name="ids">list of ids of uploads to delete.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> DeleteByIdsAsync(List<string> ids)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();
                var sqlString = $"DELETE FROM {Constants.UploadDAOTableName} WHERE {Constants.UploadDAOUploadIdColumn} IN (";

                // Number of ids to delete.
                var idsToDelete = ids.Count;

                // Loop through the ids
                for (var i = 0; i < idsToDelete; i++)
                {
                    // Construcet the sql string for deleting an all the uploads.
                    sqlString += $"@UPLOAD_ID{i},";
                }

                // delete the trailing comma and add a semicolon to complete sql string.
                sqlString = sqlString.TrimEnd(new char[] { ',' });
                sqlString += ");";

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Loop through the idsOfRows and replace them with the parameters.
                    for (var i = 0; i < idsToDelete; i++)
                    {
                        command.Parameters.AddWithValue($"@UPLOAD_ID{i}", ids[i]);
                    }

                    // Result is the number of rows affected.
                    var result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    // Return false when no rows are affected.
                    return result != 0;
                }
            }
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

        // <summary>
        /// get all the upload's upvote and downvote for a user.
        /// </summary>
        /// <param name="username">User to retrieve votes from.</param>
        /// <returns>List of ProfileScoreResult.</returns>
        public async Task<List<ProfileScoreResult>> ReadUploadVotesAsync(string username)
        {
            // List to store upload's votes.
            var votes = new List<ProfileScoreResult>();

            // Open connection in using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrive all upvotes and downvotes from a user.
                var sqlString =
                    $"SELECT {Constants.UploadDAOUpvoteColumn}, {Constants.UploadDAODownvoteColumn} " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME;";


                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        // Add paramters of username into the command.
                        command.Parameters.AddWithValue("@USERNAME", username);

                        // Execute command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        dataTable.Load(reader);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Add objects to vote list and convert then to the appropriate type.
                            votes.Add(new ProfileScoreResult(Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn])));
                        }
                    }
                }
            }
            return votes;
        }

        /// <summary>
        /// Add an upvote to an Upload.
        /// </summary>
        public async Task<bool> IncrementUpvotesonUpload(int voteValue, int uploadId)
        {
            // Open connection.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                //SQL command for increasing upvotes
                var sqlString =
                    $"UPDATE {Constants.UploadDAOTableName}" +
                    $"SET {Constants.UploadDAOUpvoteColumn} = {Constants.UploadDAOUpvoteColumn} + @VOTEVALUE" +
                    $"WHERE {Constants.UploadDAOUploadIdColumn} = @UPLOADID;";

                using (MySqlCommand command = new MySqlCommand(sqlString,connection))
                {
                    using (DataTable datatable = new DataTable())
                    {
                        // Add the parameters to the sql string. 
                        command.Parameters.AddWithValue("@UPLOADID", uploadId);
                        command.Parameters.AddWithValue("@VOTEVALUE", voteValue);

                        // Execute the command
                        var result = await command.ExecuteReaderAsync().ConfigureAwait(false);

                        // The command should return the number of rows affected. Returns true if only 1 row is changed. 
                        return Convert.ToInt32(result) == 1;
                    }
                }

            }
        }

        /// <summary>
        /// Add a downvote to an Upload.
        /// </summary>
        /// <param name="voteValue"> The number added to the current number of Downvotes. </param>
        /// <param name="uploadId"> Used to identify the specific upload being changed. </param>
        /// <returns> A boolean showing whether or not the function executed properly. </returns>
        public async Task<bool> IncrementDownvotesonUpload(int voteValue, int uploadId)
        {
            // Open connection.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                //SQL command for increasing upvotes
                var sqlString =
                    $"UPDATE {Constants.UploadDAOTableName}" +
                    $"SET {Constants.UploadDAODownvoteColumn} = {Constants.UploadDAODownvoteColumn} + @VOTEVALUE" +
                    $"WHERE {Constants.UploadDAOUploadIdColumn} = @UPLOADID;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable datatable = new DataTable())
                    {
                        // Add the parameters to the sql string. 
                        command.Parameters.AddWithValue("@UPLOADID", uploadId);
                        command.Parameters.AddWithValue("@VOTEVALUE", voteValue);

                        // Execute the command
                        var result = await command.ExecuteReaderAsync().ConfigureAwait(false);

                        // The command should return the number of rows affected. Returns true if only 1 row is changed. 
                        return Convert.ToInt32(result) == 1;
                    }
                }

            }
        }

        ///<summary> Return all uploads based on ingredientname and storeId.</summary>
        ///<param name="ingredientName"> The name of the ingredient.</param>
        ///<param name="storeId"> Store Id.</param>
        public async Task<List<UploadResult>> ReadUploadsByIngredientNameandStoreId(string ingredientName, int storeId, int pagination)
        {
            var uploads = new List<UploadResult>();

            //Open Connection properly.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOIngredientNameColumn} = @INGREDIENTNAME AND {Constants.UploadDAOStoreIdColumn} = @STOREID " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @OFFSET, @AMOUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable datatable = new DataTable())
                    {
                        command.Parameters.AddWithValue("@INGREDIENTNAME", ingredientName);
                        command.Parameters.AddWithValue("@STOREID", storeId);
                        command.Parameters.AddWithValue("@OFFSET", pagination * Constants.IngredientViewPagination);
                        command.Parameters.AddWithValue("@AMOUNT", Constants.IngredientViewPagination);

                        // Execute the command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        datatable.Load(reader);

                        foreach(DataRow row in datatable.Rows)
                        {
                            uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                        (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                        (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                        }
                    }
                }
            }
            return uploads;
        }

        public async Task<List<UploadResult>> GetIngredientsfromStore(int storeId, int pagination)
        {
            var uploads = new List<UploadResult>();

            //Open Connection properly.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOStoreIdColumn} = @STOREID " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @OFFSET, @AMOUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable datatable = new DataTable())
                    {
                        command.Parameters.AddWithValue("@STOREID", storeId);
                        command.Parameters.AddWithValue("@OFFSET", pagination * Constants.IngredientViewPagination);
                        command.Parameters.AddWithValue("@AMOUNT", Constants.IngredientViewPagination);

                        // Execute the command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        datatable.Load(reader);

                        foreach (DataRow row in datatable.Rows)
                        {
                            uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                        (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                        (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                        }
                    }
                }
            }
            return uploads;
        }

        /// <summary>
        /// Get the recent upload's by a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> ReadRecentByUploaderAsync(string username, int pagination)
        {
            // List of upload to return.
            var uploads = new List<UploadResult>();

            // Open connection in using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrive all uploads from a user.
                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadNotInprogress} " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @OFFSET, @AMOUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        // Add paramters of username into the command.
                        command.Parameters.AddWithValue("@USERNAME", username);
                        command.Parameters.AddWithValue("@OFFSET", pagination * Constants.RecentUploadPagination);
                        command.Parameters.AddWithValue("@AMOUNT", Constants.RecentUploadPagination);

                        // Execute the command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        dataTable.Load(reader);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Add the result to the upload list and convert them to correct type.
                            uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                        (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                        (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                        }

                    }
                }
            }
            return uploads;
        }

        /// <summary>
        /// Get the in progress uplaods for a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> ReadInProgressUploadsByUploaderAsync(string username, int pagination)
        {
            // List of uploads to return.
            var uploads = new List<UploadResult>();

            // Open the connection with using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrive all uploads from a user.
                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadInprogress} " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @OFFSET, @AMOUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        // Add paramters of into the sql string.
                        command.Parameters.AddWithValue("@USERNAME", username);
                        command.Parameters.AddWithValue("@OFFSET", pagination * Constants.SavedUploadPagination);
                        command.Parameters.AddWithValue("@AMOUNT", Constants.SavedUploadPagination);

                        // Execute command.
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        dataTable.Load(reader);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Add result to the upload list and convert object to correct type.
                            uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                        (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                        (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                        }

                    }
                }
            }
            return uploads;
        }

        /// <summary>
        /// Test whether a user is an owner of a list of uploads.
        /// </summary>
        /// <param name="ids">Ids of the uploads to check.</param>
        /// <param name="owner">user to test.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadsOwnerAsync(List<string> ids, string owner)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Sql string to check upload owner.
                var sqlString = $"SELECT COUNT(*) FROM {Constants.UploadDAOTableName} WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOUploadIdColumn} IN (";

                // Store the number of ids to check.
                var idsToCount = ids.Count;

                // Loop through the ids
                for (var i = 0; i < idsToCount; i++)
                {
                    // Construcet the sql string for deleting an all the uploads.
                    sqlString += $"@UPLOAD_ID{i},";
                }

                // delete the trailing comma and add a semicolon to complete sql string.
                sqlString = sqlString.TrimEnd(new char[] { ',' });
                sqlString += ");";

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Replace the username parameter.
                    command.Parameters.AddWithValue("@USERNAME", owner);

                    // Loop through the idsOfRows and replace them with the parameters for upload ID.
                    for (var i = 0; i < idsToCount; i++)
                    {
                        command.Parameters.AddWithValue($"@UPLOAD_ID{i}", ids[i]);
                    }

                    // Execute Command. Result contains the number of rows counted.
                    var result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                    // Return false when the rows returned are not equal to size of list ids.
                    return Convert.ToInt32(result) == idsToCount;
                }
            }
        }

        /// <summary>
        /// Test whether an upload exists.
        /// </summary>
        /// <param name="ids">Ids of the uploads.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadsExistenceAsync(List<string> ids)
        {
            // Get the connnection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // Sql string to check uploads existence.
                var sqlString = $"SELECT COUNT(*) FROM {Constants.UploadDAOTableName} WHERE {Constants.UploadDAOUploadIdColumn} IN (";

                // Store the size of the ids list.
                var idsToCount = ids.Count;

                // Loop through the ids
                for (var i = 0; i < idsToCount; i++)
                {
                    // Construcet the sql string for deleting an all the uploads.
                    sqlString += $"@UPLOAD_ID{i},";
                }

                // delete the trailing comma and add a semicolon to complete sql string.
                sqlString = sqlString.TrimEnd(new char[] { ',' });
                sqlString += ");";

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {

                    // Loop through the idsOfRows and replace them with the parameters.
                    for (var i = 0; i < idsToCount; i++)
                    {
                        command.Parameters.AddWithValue($"@UPLOAD_ID{i}", ids[i]);
                    }

                    // Result reeturn the number of rows affected.
                    var result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                    // Return false when the rows returned are not equal to size of list ids.
                    return Convert.ToInt32(result) == idsToCount;
                }
            }
        }

        /// <summary>
        /// Get the pagination size for in progress uploads of a user.
        /// </summary>
        /// <param name="username">User to retreive information from.</param>
        /// <returns>Int representing the size of the pagination.</returns>
        public async Task<int> GetInProgressPaginationSizeAsync(string username)
        {
            // Open the connection with using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrieve count of all saveLists for a user.
                var sqlString =
                    $"SELECT COUNT(*) " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadInprogress};";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Replace username parameter.
                    command.Parameters.AddWithValue("@USERNAME", username);

                    // Execute command.
                    var inProgresscount = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false));

                    // Perform logic to account for needed extra pagination.
                    var paginationSize = inProgresscount / Constants.SavedUploadPagination;
                    if (paginationSize == 0)
                    {
                        return 1;
                    }
                    else if ((paginationSize % Constants.SavedUploadPagination) == 0)
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

        /// <summary>
        /// Get the pagination size for recent uploads of a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <returns>int representing size of the pagination.</returns>
        public async Task<int> GetRecentPaginationSizeAsync(string username)
        {
            // Open connection with using to properly dispose.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrieve count of all saveLists for a user.
                var sqlString =
                    $"SELECT COUNT(*) " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadNotInprogress};";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Replace username parameter.
                    command.Parameters.AddWithValue("@USERNAME", username);

                    // Execute command.
                    var inProgresscount = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false));

                    // Perform logic to account for needed extra pagination.
                    var paginationSize = inProgresscount / Constants.RecentUploadPagination;
                    if (paginationSize == 0)
                    {
                        return 1;
                    }
                    else if ((paginationSize % Constants.RecentUploadPagination) == 0)
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
