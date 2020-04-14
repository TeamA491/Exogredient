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

        /// <summary>
        /// Returns the number of total stores that are associated with
        /// searchTerm, latitude, longitude, radius, and searchBy 
        /// </summary>
        /// <param name="searchTerm">Term to search for</param>
        /// <param name="latitude">Latitdue of the center of search</param>
        /// <param name="longitude">Longitude of the center of search</param>
        /// <param name="radius">The radius from the center of search</param>
        /// <param name="searchBy">Method of search. It must be either "ingredient" or "store"</param>
        /// <returns>The number of retrieved stores</returns>
        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm, double latitude,
            double longitude, double radius, string searchBy)
        {
            int totalResultsNum;

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Get storeIDs of stores that have any upload that is associated with searchTerm 
                // or stores that have the searchTerm in their name
                // depending on searchBy.
                var subQuery =
                    $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
                    $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
                    $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) " +
                    $"AS {Constants.StoreDAODistanceColumn} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    (searchBy==Constants.searchByIngredient ?
                    $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} "
                    : $"WHERE {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn} ") +
                    $"LIKE @SEARCH_TERM GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";

                // Count the number of distinct storeIDs retrieved.
                var sqlString =
                    $"SELECT COUNT(DISTINCT x.{Constants.StoreDAOStoreIdColumn}) " +
                    $"AS {Constants.StoreDAOTotalResultsNum} FROM ({subQuery}) AS x";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    // Inject arguments to query.
                    command.Parameters.AddWithValue("@SEARCH_TERM", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@LATITUDE", latitude);
                    command.Parameters.AddWithValue("@LONGITUDE", longitude);
                    command.Parameters.AddWithValue("@RADIUS", radius);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    // Get the number of distinct storeIDs retrieved.
                    totalResultsNum = Convert.ToInt32(dataTable.Rows[0][Constants.StoreDAOTotalResultsNum]);

                }

            }

            return totalResultsNum;
        }

        /// <summary>
        /// Returns the stores of a certain page that are associated with
        /// searchTerm, latitude, longitude, radius, and searchBy.
        /// </summary>
        /// <param name="searchTerm"> Term to search for</param>
        /// <param name="latitude">Latitdue of the center of search</param>
        /// <param name="longitude">Longitude of the center of search</param>
        /// <param name="radius">The radius from the center of search</param>
        /// <param name="searchBy">Method of search. Must be either "ingredient" or "store"</param>
        /// <param name="lastStoreData">
        /// The data (distance or num of ingredients) of the last store of last page that was displayed
        /// (only needed when page changes, otherwise shoud be -1).
        /// </param>
        /// <param name="lastStoreId">
        /// The storeID of the last store of last page that was displayed
        /// (only needed when page changes, otherwise should be 0).
        /// </param>
        /// <param name="lastPageResultsNum">
        /// The number of results of last page that was displayed
        /// (only needed when page moved backward, otherwise it's ignored).
        /// </param>
        /// <param name="skipPages">The number of pages moved in pagination</param>
        /// <param name="sortOption">The sort option. Must either "distance" or "ingredientNum"</param>
        /// <param name="fromSmallest">
        /// If true, results are sorted from smallest(ascending).
        /// Otherwise, from biggest(descending).
        /// </param>
        /// <returns>The list of StoreResult objects</returns>
        public async Task<List<StoreResult>> ReadBySearchTermAsync(string searchTerm,
            double latitude, double longitude, double radius, string searchBy,
            double lastStoreData, int lastStoreId, int lastPageResultsNum, int skipPages,
            string sortOption, bool fromSmallest)
        {
            var stores = new List<StoreResult>();

            // Check if pagination moved backward
            var isSkipPagesNeg = skipPages < 0;

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Get storeIDs, store names, ingredients of stores 
                // either by searchTerm (could be store name or ingredient name)
                // depending on searchBy.
                var subQuery =
                   $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                   $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
                   $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
                   $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
                   $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
                   $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) " +
                   $"AS {Constants.StoreDAODistanceColumn} " +
                   $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                   $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                   $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                   (searchBy==Constants.searchByIngredient ?
                   $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} "
                   :$"WHERE {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn} " ) +
                   $"LIKE @SEARCH_TERM " +
                   $"GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                   $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                   $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";

                // Get storeIDs, store names, and number of ingredients of stores.
                var sqlString =
                    $"SELECT x.{Constants.StoreDAOStoreIdColumn}, x.{Constants.StoreDAOStoreNameColumn}, " +
                    $"x.{Constants.StoreDAODistanceColumn}, COUNT(*) AS {Constants.StoreDAOIngredientNumColumn} " +
                    $"FROM ({subQuery}) AS x ";
                    

                if (sortOption == Constants.sortByDistance)
                {
                    // If this method is called due to pagination change
                    // (there is no data from the last store of last page if search or sort performed).
                    if (lastStoreData != -1)
                    {
                        // If sorted by ascending order, get results greater than last store's distance.
                        // If sorted by descending order, get results smaller than last store's distance.
                        // (storeID is there to distinguish some results that have the same distance)
                        // If isSkipPagesNeg is true, it reverses rules mentioned above.
                        sqlString += $"WHERE (x.{Constants.StoreDAODistanceColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    ((fromSmallest ^ isSkipPagesNeg) ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }

                    sqlString += $"GROUP BY x.{Constants.StoreDAOStoreIdColumn} ";

                    // If sorted by ascending order, sort results in ascending order.
                    // If sorted by descending order, sort results in descending order.
                    // (storeID is there to distinguish some results that have the same distance)
                    // If isSkipPagesNeg is true, it reverses rules mentioned above.
                    sqlString +=
                        $"ORDER BY x.{Constants.StoreDAODistanceColumn} " +
                        ((fromSmallest ^ isSkipPagesNeg) ? "ASC, " : "DESC, ") +
                        $"x.{Constants.StoreDAOStoreIdColumn} " +
                        ((fromSmallest ^ isSkipPagesNeg) ? "ASC " : "DESC ");

                }
                else if (sortOption == Constants.sortByIngredientNum)
                {
                    // Group by storeID first in order to get each store's number of ingredients
                    // that will be used to filter results.
                    sqlString += $"GROUP BY x.{Constants.StoreDAOStoreIdColumn} ";

                    // If this method is called due to pagination change
                    // (there is no data from the last store of last page if search or sort performed).
                    if (lastStoreData != -1)
                    {
                        // If sorted by ascending order, get results greater than last store's number of ingredients.
                        // If sorted by descending order, get results smaller than last store's number of ingredients.
                        // (storeID is there to distinguish some results that have the same number of ingredients)
                        // If isSkipPagesNeg is true, it reverses rules mentioned above. 
                        sqlString += $"HAVING ({Constants.StoreDAOIngredientNumColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    ((fromSmallest ^ isSkipPagesNeg) ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }

                    // If sorted by ascending order, sort results in ascending order.
                    // If sorted by descending order, sort results in descending order.
                    // (storeID is there to distinguish some results that have the same number of ingredients)
                    // If isSkipPagesNeg is true, it reverses rules mentioned above.
                    sqlString +=
                        $"ORDER BY {Constants.StoreDAOIngredientNumColumn} " +
                        ((fromSmallest ^ isSkipPagesNeg) ? "ASC, " : "DESC, ") +
                        $"x.{Constants.StoreDAOStoreIdColumn} " +
                        ((fromSmallest ^ isSkipPagesNeg) ? "ASC " : "DESC ");
                }

                sqlString += $"LIMIT @OFFSET, @COUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@SEARCH_TERM", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@LATITUDE", latitude);
                    command.Parameters.AddWithValue("@LONGITUDE", longitude);
                    command.Parameters.AddWithValue("@RADIUS", radius);
                    command.Parameters.AddWithValue("@LAST_STORE_DATA", lastStoreData);
                    command.Parameters.AddWithValue("@LAST_STORE_ID", lastStoreId);

                    // Offset by (results per page) * (how many pages skipped - 1) from the last result of last page.
                    // If pagination moved backward, we add the number of results of last page - 1
                    // (-1 is to exclude the last result of last page from counting).
                    command.Parameters.AddWithValue
                    (
                        "@OFFSET", isSkipPagesNeg ?
                        (Math.Abs(skipPages)-1)*Constants.NumOfResultsPerSearchPage+(lastPageResultsNum-1)
                        :(skipPages - 1) * Constants.NumOfResultsPerSearchPage
                    );
                    command.Parameters.AddWithValue("@COUNT", Constants.NumOfResultsPerSearchPage);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Create storeResult with retreived data and add to list.
                        stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]),
                                                   (string)row[Constants.StoreDAOStoreNameColumn],
                                                   Convert.ToInt32(row[Constants.StoreDAOIngredientNumColumn]),
                                                   (double)row[Constants.StoreDAODistanceColumn]));
                    }

                    if (isSkipPagesNeg)
                    {
                        // Since moving pagination backward returns list of stores backward,
                        // it has to be reversed to be in correct order.
                        stores.Reverse();
                    }
                }
            }

            return stores;
        }

        /// <summary>
        /// Gets the data of a store by its storeID.
        /// </summary>
        /// <param name="id">The storeID of a store</param>
        /// <returns>The StoreViewData object of the store</returns>
        public async Task<StoreViewData> ReadStoreViewDataByIdAsync(int id)
        {
            StoreViewData storeViewData = null;

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                // Read store name, latitude/longitude, store description, and google place id of a store whose storeID == id.
                var sqlString =
                    $"SELECT {Constants.StoreDAOStoreNameColumn}, " +
                    $"{Constants.StoreDAOLatitudeColumn}, {Constants.StoreDAOLongitudeColumn}, " +
                    $"{Constants.StoreDAOStoreDescriptionColumn}, {Constants.StoreDAOPlaceIdColumn} " +
                    $"FROM {Constants.StoreDAOTableName} " +
                    $"WHERE {Constants.StoreDAOStoreIdColumn} = @ID;";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    // Inject argument to query.
                    command.Parameters.AddWithValue("@ID", id);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Create StoreViewData with retrieved data and id.
                        storeViewData = new StoreViewData(id,
                            (string)row[Constants.StoreDAOStoreNameColumn],
                            (double)row[Constants.StoreDAOLatitudeColumn],
                            (double)row[Constants.StoreDAOLongitudeColumn],
                            (string)row[Constants.StoreDAOStoreDescriptionColumn],
                            (string)row[Constants.StoreDAOPlaceIdColumn]);
                    }
                }
            }
            return storeViewData;
        }

        public Task<bool> UpdateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
