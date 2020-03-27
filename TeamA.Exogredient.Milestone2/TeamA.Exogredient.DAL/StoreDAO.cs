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

        public async Task<int> GetTotalResultsNumberAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy)
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


        public async Task<List<StoreResult>> ReadBySearchTermAsync(string searchTerm, double latitude, double longitude, double radius, string searchBy, double lastStoreData, int lastStoreId, string sortOption, bool fromSmallest)
        {
            var stores = new List<StoreResult>();

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
                    $"FROM ({subQuery}) AS x GROUP BY x.{Constants.StoreDAOStoreIdColumn} ";

                if (sortOption == Constants.sortByDistance)
                {
                    if (lastStoreData != -1)
                    {
                        sqlString += $"WHERE (x.{Constants.StoreDAODistanceColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    (fromSmallest ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }


                    sqlString += $"ORDER BY x.{Constants.StoreDAODistanceColumn} " + (fromSmallest ? "ASC, " : "DESC, ") +
                                 $"x.{Constants.StoreDAOStoreIdColumn} " + (fromSmallest ? "ASC " : "DESC ");

                }
                else if (sortOption == Constants.sortByIngredientNum)
                {
                    if (lastStoreData != -1)
                    {
                        sqlString += $"WHERE (x.{Constants.StoreDAOIngredientNumColumn}, " +
                                    $"x.{Constants.StoreDAOStoreIdColumn}) " +
                                    (fromSmallest ? ">" : "<") + $" (@LAST_STORE_DATA, @LAST_STORE_ID) ";
                    }


                    sqlString += $"ORDER BY {Constants.StoreDAOIngredientNumColumn} " + (fromSmallest ? "ASC, " : "DESC, ") +
                                 $"x.{Constants.StoreDAOStoreIdColumn} " + (fromSmallest ? "ASC " : "DESC ");
                }

                sqlString += $"LIMIT @COUNT;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@SEARCH_TERM", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@LATITUDE", latitude);
                    command.Parameters.AddWithValue("@LONGITUDE", longitude);
                    command.Parameters.AddWithValue("@RADIUS", radius);
                    command.Parameters.AddWithValue("@LAST_STORE_DATA", lastStoreData);
                    command.Parameters.AddWithValue("@LAST_STORE_ID", lastStoreId);
                    command.Parameters.AddWithValue("@COUNT", Constants.NumOfResultsPerSearchPage);

                    Console.WriteLine(sqlString);

                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]), (string)row[Constants.StoreDAOStoreNameColumn],
                                                   Convert.ToInt32(row[Constants.StoreDAOIngredientNumColumn]), (double)row[Constants.StoreDAODistanceColumn]));
                    }
                }
            }

            return stores;
        }

        //public async Task<List<StoreResult>> ReadByIngredientNameAsync(string ingredientName,double latitude, double longitude, double radius, double lastStoreDistance)
        //{
        //    var stores = new List<StoreResult>();

        //    // Get the connection inside a using statement to properly dispose/close.
        //    using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
        //    {
        //        // Open the connection.
        //        connection.Open();
        //        var subQuery =
        //            $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
        //            $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
        //            $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
        //            $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
        //            $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
        //            $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) AS {Constants.StoreDAODistanceColumn} " +
        //            $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
        //            $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
        //            $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
        //            $"WHERE {Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
        //            $"LIKE @INGREDIENT_NAME GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
        //            $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
        //            $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";


        //        var sqlString =
        //            $"SELECT x.{Constants.StoreDAOStoreIdColumn}, x.{Constants.StoreDAOStoreNameColumn}, " +
        //            $"x.{Constants.StoreDAODistanceColumn}, COUNT(*) AS {Constants.StoreDAOIngredientNumColumn} " +
        //            $"FROM ({subQuery}) AS x GROUP BY x.{Constants.StoreDAOStoreIdColumn} " +
        //            $"LIMIT @COUNT;";

        //        using (MySqlCommand command = new MySqlCommand(sqlString, connection))
        //        using (DataTable dataTable = new DataTable())
        //        {
        //            command.Parameters.AddWithValue("@INGREDIENT_NAME", "%"+ingredientName+"%");
        //            command.Parameters.AddWithValue("@LATITUDE", latitude);
        //            command.Parameters.AddWithValue("@LONGITUDE", longitude);
        //            command.Parameters.AddWithValue("@RADIUS", radius);
        //            command.Parameters.AddWithValue("@OFFSET", (lastStoreDistance-1)*Constants.NumOfResultsPerSearchPage);
        //            command.Parameters.AddWithValue("@COUNT", Constants.NumOfResultsPerSearchPage);
        //            var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        //            dataTable.Load(reader);

        //            foreach (DataRow row in dataTable.Rows)
        //            {
        //                stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]), (string)row[Constants.StoreDAOStoreNameColumn],
        //                                           Convert.ToInt32(row[Constants.StoreDAOIngredientNumColumn]), (double)row[Constants.StoreDAODistanceColumn]));
        //            }
        //            //stores.Sort((StoreResult x, StoreResult y) => x.Distance.CompareTo(y.Distance));

        //        }
        //    }

        //    return stores;
        //}

        //public async Task<List<StoreResult>> ReadByStoreNameAsync(string storeName,double latitude, double longitude, double radius, int pagination)
        //{
        //    var stores = new List<StoreResult>();

        //    // Get the connection inside a using statement to properly dispose/close.
        //    using (MySqlConnection connection = new MySqlConnection(_SQLConnection))
        //    {
        //        // Open the connection.
        //        connection.Open();
        //        var subQuery =
        //            $"SELECT {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
        //            $"{Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn}, " +
        //            $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn}, " +
        //            $"(SELECT(3959 * acos(cos(radians(@LATITUDE))* cos(radians({Constants.StoreDAOLatitudeColumn}))* " +
        //            $"cos(radians({Constants.StoreDAOLongitudeColumn}) - radians(@LONGITUDE))+ " +
        //            $"sin(radians(@LATITUDE))* sin(radians({Constants.StoreDAOLatitudeColumn}))))) AS {Constants.StoreDAODistanceColumn} " +
        //            $"FROM {Constants.UploadDAOTableName} INNER JOIN {Constants.StoreDAOTableName} " +
        //            $"ON {Constants.UploadDAOTableName}.{Constants.UploadDAOStoreIdColumn} " +
        //            $"= {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn} " +
        //            $"WHERE {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreNameColumn} " +
        //            $"LIKE @STORE_NAME GROUP BY {Constants.StoreDAOTableName}.{Constants.StoreDAOStoreIdColumn}, " +
        //            $"{Constants.UploadDAOTableName}.{Constants.UploadDAOIngredientNameColumn} " +
        //            $"HAVING {Constants.StoreDAODistanceColumn} <= @RADIUS";


        //        var sqlString =
        //            $"SELECT x.{Constants.StoreDAOStoreIdColumn}, x.{Constants.StoreDAOStoreNameColumn}, " +
        //            $"x.{Constants.StoreDAODistanceColumn}, COUNT(*) AS {Constants.StoreDAOIngredientNumColumn} " +
        //            $"FROM ({subQuery}) AS x GROUP BY x.{Constants.StoreDAOStoreIdColumn} " +
        //            $"ORDER BY x.{Constants.StoreDAODistanceColumn} ASC LIMIT @OFFSET,@COUNT;";


        //        using (MySqlCommand command = new MySqlCommand(sqlString, connection))
        //        using (DataTable dataTable = new DataTable())
        //        {
        //            command.Parameters.AddWithValue("@STORE_NAME", "%" + storeName + "%");
        //            command.Parameters.AddWithValue("@LATITUDE", latitude);
        //            command.Parameters.AddWithValue("@LONGITUDE", longitude);
        //            command.Parameters.AddWithValue("@RADIUS", radius);
        //            command.Parameters.AddWithValue("@OFFSET", (pagination - 1) * Constants.NumOfResultsPerSearchPage);
        //            command.Parameters.AddWithValue("@COUNT", Constants.NumOfResultsPerSearchPage);
        //            var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        //            dataTable.Load(reader);

        //            foreach (DataRow row in dataTable.Rows)
        //            {
        //                stores.Add(new StoreResult(Convert.ToInt32(row[Constants.StoreDAOStoreIdColumn]), (string)row[Constants.StoreDAOStoreNameColumn],
        //                                           Convert.ToInt32(row[Constants.StoreDAOIngredientNumColumn]), (double)row[Constants.StoreDAODistanceColumn]));
        //            }
        //            //stores.Sort((StoreResult x, StoreResult y) => x.Distance.CompareTo(y.Distance));
        //        }
        //    }
        //    return stores;
        //}

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
                        storeViewData = new StoreViewData((string)row[Constants.StoreDAOStoreNameColumn], (double)row[Constants.StoreDAOLatitudeColumn], (double)row[Constants.StoreDAOLongitudeColumn], (string)row[Constants.StoreDAOStoreDescriptionColumn], (string)row[Constants.StoreDAOPlaceIdColumn]);
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
