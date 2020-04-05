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

        public async Task<int> GetTotalStoreResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy)
        {
            int totalResultsNum;
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();
                var subQuery =
                    $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
                    $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
                    $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
                    $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
                    $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) AS {Constants.StoreDAODistanceColumn} " +
                    $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                    $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                    $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                    (searchBy==Constants.searchByIngredient ? $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " : $"WHERE {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn} ") +
                    $"LIKE @SEARCH_TERM GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                    $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                    $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";


                var sqlString =
                    $"SELECT COUNT(DISTINCT x.{Constants.StoreDAOStoreIdColumn}) AS {Constants.StoreDAOTotalResultsNum} FROM ({subQuery}) AS x";

                Console.WriteLine(sqlString);
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@SEARCH_TERM", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@LATITUDE", latitude);
                    command.Parameters.AddWithValue("@LONGITUDE", longitude);
                    command.Parameters.AddWithValue("@RADIUS", radius);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    totalResultsNum = Convert.ToInt32(dataTable.Rows[0][Constants.StoreDAOTotalResultsNum]);

                }

            }

            return totalResultsNum;
        }


        public async Task<List<StoreResult>> ReadBySearchTermAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy, double lastStoreData, int lastStoreId, int lastPageResultsNum, int skipPages, string sortOption, bool fromSmallest)
        {
            Console.WriteLine($"skip pages: {skipPages}");
            var stores = new List<StoreResult>();
            var isSkipPagesNeg = skipPages < 0;

            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                var subQuery =
                   $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                   $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
                   $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
                   $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
                   $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
                   $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) AS {Constants.StoreDAODistanceColumn} " +
                   $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
                   $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
                   $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
                   (searchBy==Constants.searchByIngredient ? $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} ":$"WHERE {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn} " ) +
                   $"LIKE @SEARCH_TERM " +
                   $"GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
                   $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
                   $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";

                var sqlString =
                    $"SELECT x.{Constants.StoreDAOStoreIdColumn}, x.{Constants.StoreDAOStoreNameColumn}, " +
                    $"x.{Constants.StoreDAODistanceColumn}, COUNT(*) AS {Constants.StoreDAOIngredientNumColumn} " +
                    $"FROM ({subQuery}) AS x ";
                    

                if (sortOption == Constants.sortByDistance)
                {
                    if (lastStoreData != -1)
                    {
                        sqlString += $"WHERE (x.{Constants.StoreDAODistanceColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    ((fromSmallest ^ isSkipPagesNeg) ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }

                    sqlString += $"GROUP BY x.{Constants.StoreDAOStoreIdColumn} ";
                    sqlString += $"ORDER BY x.{Constants.StoreDAODistanceColumn} " + ((fromSmallest ^ isSkipPagesNeg) ? "ASC, " : "DESC, ") +
                                 $"x.{Constants.StoreDAOStoreIdColumn} " + ((fromSmallest ^ isSkipPagesNeg) ? "ASC " : "DESC ");

                }
                else if (sortOption == Constants.sortByIngredientNum)
                {
                    sqlString += $"GROUP BY x.{Constants.StoreDAOStoreIdColumn} ";

                    if (lastStoreData != -1)
                    {
                        sqlString += $"HAVING ({Constants.StoreDAOIngredientNumColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    ((fromSmallest ^ isSkipPagesNeg) ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }

                    sqlString += $"ORDER BY {Constants.StoreDAOIngredientNumColumn} " + ((fromSmallest ^ isSkipPagesNeg) ? "ASC, " : "DESC, ") +
                                 $"x.{Constants.StoreDAOStoreIdColumn} " + ((fromSmallest ^ isSkipPagesNeg) ? "ASC " : "DESC ");
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
                    command.Parameters.AddWithValue("@OFFSET", isSkipPagesNeg ? (Math.Abs(skipPages)-1)*Constants.NumOfResultsPerSearchPage+(lastPageResultsNum-1) :(skipPages - 1) * Constants.NumOfResultsPerSearchPage);
                    command.Parameters.AddWithValue("@COUNT", Constants.NumOfResultsPerSearchPage);

                    Console.WriteLine($"last page results num: {lastPageResultsNum}");
                    Console.WriteLine(sqlString);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]), (string)row[Constants.StoreDAOStoreNameColumn],
                                                   Convert.ToInt32(row[Constants.StoreDAOIngredientNumColumn]), (double)row[Constants.StoreDAODistanceColumn]));
                    }

                    if (isSkipPagesNeg)
                    {
                        stores.Reverse();
                    }
                }
            }

            return stores;
        }
   
        public async Task<StoreViewData> ReadStoreViewDataByIdAsync(int id)
        {
            StoreViewData storeViewData = null;
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
            {
                // Open the connection.
                connection.Open();

                var sqlString = $"SELECT {Constants.StoreDAOStoreNameColumn}, {Constants.StoreDAOLatitudeColumn}, {Constants.StoreDAOLongitudeColumn}, {Constants.StoreDAOStoreDescriptionColumn}, {Constants.StoreDAOPlaceIdColumn} FROM {Constants.StoreDAOTableName} WHERE {Constants.StoreDAOStoreIdColumn} = @ID;";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@ID", id);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        storeViewData = new StoreViewData(id, (string)row[Constants.StoreDAOStoreNameColumn], (double)row[Constants.StoreDAOLatitudeColumn],
                            (double)row[Constants.StoreDAOLongitudeColumn], (string)row[Constants.StoreDAOStoreDescriptionColumn], (string)row[Constants.StoreDAOPlaceIdColumn]);
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
