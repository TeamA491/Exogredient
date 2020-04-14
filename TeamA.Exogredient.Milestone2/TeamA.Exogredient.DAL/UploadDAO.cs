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
        private readonly string _SQLConnection;

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
                sqlString = sqlString.TrimEnd(new char[] {','});
                sqlString += ");";

                // Get the command object inside a using statement to properly dispose/close.
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    // Loop through the idsOfRows and replace them with the parameters.
                    for(var i = 0; i < idsToDelete; i++)
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


        public async Task<List<IngredientResult>> ReadIngredientsByStoreIdAsync(int storeId, string ingredientName)
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
                    $"GROUP BY {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn};";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    if (ingredientName != null)
                    {
                        command.Parameters.AddWithValue("@INGREDIENT_NAME", "%" + ingredientName + "%");
                    }
                    command.Parameters.AddWithValue("@STORE_ID", storeId);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

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

        /// <summary>
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
