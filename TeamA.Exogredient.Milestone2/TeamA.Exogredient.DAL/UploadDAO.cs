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

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            throw new NotImplementedException();
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

        public async Task<List<ProfileScoreResult>> ReadUploadVotes(string username)
        {
            // List to store upload's votes.
            var votes = new List<ProfileScoreResult>();

            using (MySqlConnection connection = new MySqlConnection(_SQLConnection)) 
            { 
                connection.Open();

                // SQL command to retrive all upvotes and downvotes from a user.
                var sqlString =
                    $"SELECT {Constants.UploadDAOUpvoteColumn}, {Constants.UploadDAODownvoteColumn} " +
                    $"FROM {Constants.UploadDAOTableName} " + 
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME;";


                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Prepare();
                    // Add paramters of username into the command.
                    command.Parameters.AddWithValue("@USERNAME", username);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        votes.Add(new ProfileScoreResult(Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn])));
                    }

                }

            }
            return votes;
        }

        public async Task<List<UploadResult>> ReadRecentByUploader(string username, int pagination)
        {
            var uploads = new List<UploadResult>();
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrive all uploads from a user.
                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadNotInprogress} " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @START, @END;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Prepare();
                    // Add paramters of username into the command.
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@START", (pagination - 1) * Constants.RecentUploadPagination);
                    command.Parameters.AddWithValue("@END", pagination * Constants.RecentUploadPagination);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                    (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                    (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                    }

                }

            }
            return uploads;
        }

        public async Task<List<UploadResult>> ReadSavedUploadsByUploader(string username, int pagination)
        {
            var uploads = new List<UploadResult>();
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                connection.Open();

                // SQL command to retrive all uploads from a user.
                var sqlString =
                    $"SELECT * " +
                    $"FROM {Constants.UploadDAOTableName} " +
                    $"WHERE {Constants.UploadDAOUploaderColumn} = @USERNAME AND {Constants.UploadDAOInProgressColumn} = {Constants.UploadInprogress} " +
                    $"ORDER BY {Constants.UploadDAOPostTimeDateColumn} ASC " +
                    $"LIMIT @START, @END;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Prepare();
                    // Add paramters of into the sql string.
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@START", (pagination - 1) * Constants.SavedUploadPagination);
                    command.Parameters.AddWithValue("@END", pagination * Constants.SavedUploadPagination);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        uploads.Add(new UploadResult(Convert.ToInt32(row[Constants.UploadDAOUploadIdColumn]), Convert.ToInt32(row[Constants.UploadDAOStoreIdColumn]), (string)row[Constants.UploadDAOIngredientNameColumn],
                                    (string)row[Constants.UploadDAOUploaderColumn], (string)row[Constants.UploadDAOPostTimeDateColumn].ToString(), (string)row[Constants.UploadDAODescriptionColumn], (string)row[Constants.UploadDAORatingColumn],
                                    (string)row[Constants.UploadDAOPhotoColumn], Convert.ToDouble(row[Constants.UploadDAOPriceColumn]), Convert.ToInt32(row[Constants.UploadDAOUpvoteColumn]), Convert.ToInt32(row[Constants.UploadDAODownvoteColumn]), Convert.ToBoolean(row[Constants.UploadDAOInProgressColumn])));
                    }

                }

            }
            return uploads;
        }
            



    }
}
