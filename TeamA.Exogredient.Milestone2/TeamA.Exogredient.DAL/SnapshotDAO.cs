using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class SnapshotDAO 
    {
        private string NOSQLConnection;
        public SnapshotDAO(string connection)
        {
            NOSQLConnection = connection;
        }

        /// <summary>
        /// Method to store a snapshot.
        /// </summary>
        /// <param name="year">Year needed to be specific for storing.</param>
        /// <param name="month">Month needed to be specific for storing.</param>
        /// <param name="snapshot">The list of string(data) that pertains to the snapshot.</param>
        /// <returns>A bool result of true or false depending on if it succeeded of not.</returns>
        public async Task<bool> StoreSnapshotAsync(string year, string month, List<string> snapshot)
        {
            using (Session session = MySQLX.GetSession(NOSQLConnection))
            {
                Schema schema = session.GetSchema(Constants.SnapshotSchemaName);

                if (!schema.ExistsInDatabase())
                {
                    session.CreateSchema(Constants.SnapshotSchemaName);
                }

                string specificMonth = year + month;
                

                var collection = schema.CreateCollection(Constants.SnapshotCollectionPrefix, true);

                // Create json string to insert into the data store.
                string document = $@"{{""{Constants.SnapshotMonth}"": ""{specificMonth}"", " +
                                  $@"""{Constants.SnapshotOperationsDict}"": ""{snapshot[0]}"", " +
                                  $@"""{Constants.SnapshotUsersDict}"": ""{snapshot[1]}"", " +
                                  $@"""{Constants.SnapshotTopCityDict}"": ""{snapshot[2]}"", " +
                                  $@"""{Constants.SnapshotTopUserUploadedDict}"": ""{snapshot[3]}"", " +
                                  $@"""{Constants.SnapshotTopUploadedIngredientDict}"": ""{snapshot[4]}"", " +
                                  $@"""{Constants.SnapshotTopUploadedstoreDict}"": ""{snapshot[5]}"", " +
                                  $@"""{Constants.SnapshotTopSearchedIngredientDict}"": ""{snapshot[6]}"", " +
                                  $@"""{Constants.SnapshotTopSearchedStoreDict}"": ""{snapshot[7]}"", " +
                                  $@"""{Constants.SnapshotTopUpvotedUserDict}"": ""{snapshot[8]}"", " +
                                  $@"""{Constants.SnapshotTopDownvotedUserDict}"": ""{snapshot[9]}""}}";

                await collection.Add(document).ExecuteAsync().ConfigureAwait(false);


                return true;
            }
        }

        /// <summary>
        /// Method to read a snapshot pertaining to a specific month.
        /// </summary>
        /// <param name="year">Year needed to get specific snapshot.</param>
        /// <param name="month">Month needed to get specific snapshot.</param>
        /// <returns>A SnapShotResult object that has all the information.</returns>
        public async Task<SnapShotResult> ReadMonthlySnapshotAsync(string year, string month)
        {
            using (Session session = MySQLX.GetSession(NOSQLConnection))
            {               
                
                Schema schema = session.GetSchema(Constants.SnapshotSchemaName);

                string specificMonth = year + month;

                var collection = schema.GetCollection(Constants.SnapshotCollectionPrefix);

                DocResult result = await collection.Find($"{Constants.SnapshotMonth} = :_month").Bind("_month", specificMonth).ExecuteAsync().ConfigureAwait(false);

                result.Next();
                
                var snapshot = new SnapShotResult((string)result.Current[Constants.SnapshotMonth], (string)result.Current[Constants.SnapshotOperationsDict], (string)result.Current[Constants.SnapshotUsersDict], (string)result.Current[Constants.SnapshotTopCityDict],
                                (string)result.Current[Constants.SnapshotTopUserUploadedDict], (string)result.Current[Constants.SnapshotTopUploadedIngredientDict], (string)result.Current[Constants.SnapshotTopUploadedstoreDict],
                                (string)result.Current[Constants.SnapshotTopSearchedIngredientDict], (string)result.Current[Constants.SnapshotTopSearchedStoreDict], (string)result.Current[Constants.SnapshotTopUpvotedUserDict], (string)result.Current[Constants.SnapshotTopDownvotedUserDict]);
                
                return snapshot;
            }
        }

        /// <summary>
        /// Method to read all of the snapshots in specific year.
        /// </summary>
        /// <param name="year">Year needed to get all snapshots pertaining to it.</param>
        /// <returns>A list of SnapShotResult objects.</returns>
        public async Task<List<SnapShotResult>> ReadYearlySnapshotAsync(string year)
        {
            using (Session session = MySQLX.GetSession(NOSQLConnection))
            {

                Schema schema = session.GetSchema(Constants.SnapshotSchemaName);

                var collection = schema.GetCollection(Constants.SnapshotCollectionPrefix);

                DocResult result = await collection.Find($"{Constants.SnapshotMonth} like :_month").Bind("_month", year + "%").ExecuteAsync().ConfigureAwait(false);

                var snapshotList = new List<SnapShotResult>();

                while (result.Next())
                {
                    var snapshot = new SnapShotResult((string)result.Current[Constants.SnapshotMonth], (string)result.Current[Constants.SnapshotOperationsDict], (string)result.Current[Constants.SnapshotUsersDict], (string)result.Current[Constants.SnapshotTopCityDict],
                                (string)result.Current[Constants.SnapshotTopUserUploadedDict], (string)result.Current[Constants.SnapshotTopUploadedIngredientDict], (string)result.Current[Constants.SnapshotTopUploadedstoreDict],
                                (string)result.Current[Constants.SnapshotTopSearchedIngredientDict], (string)result.Current[Constants.SnapshotTopSearchedStoreDict], (string)result.Current[Constants.SnapshotTopUpvotedUserDict], (string)result.Current[Constants.SnapshotTopDownvotedUserDict]);
                    snapshotList.Add(snapshot);
                }

                return snapshotList;
            }
        }

    }
}
