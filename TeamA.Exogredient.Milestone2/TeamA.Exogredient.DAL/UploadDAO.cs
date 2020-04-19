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

        private readonly string SQLConnection;
        public UploadDAO(string connection)
        {
            SQLConnection = connection;
        }

        public Task<bool> CreateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public Task<IDataObject> ReadByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ISQLRecord record)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, int>> GetUsersWithUploadsAsync(Dictionary<String, int> affectedUploadsDict)
        {
            Dictionary<string, int> topUpvotedUserDict = new Dictionary<String, int>();
            // Get the connection inside a using statement to properly dispose/close.
            using (MySqlConnection connection = new MySqlConnection(SQLConnection))
            {
                // Open the connection.
                connection.Open();

                foreach (var upload in affectedUploadsDict)
                {
                    string user;
                    string uploadID = upload.Key;
                    int upvoteAmount = upload.Value;

                    string sqlString = $"SELECT {Constants.UploadDAOUploaderColumn} FROM {Constants.UploadDAOTableName} WHERE {Constants.UploadDAOUploadIdColumn} = @UPLOADID;";

                    // Open the command inside a using statement to properly dispose/close.
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        // Add the value to the parameter, execute the reader asyncrhonously, read asynchronously, then get the boolean result.
                        command.Parameters.AddWithValue("@UPLOADID", uploadID);
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        await reader.ReadAsync().ConfigureAwait(false);
                        user = reader.GetString(0);
                    }
                    topUpvotedUserDict.Add(user, upvoteAmount);
                }
                return topUpvotedUserDict;
            }
        }

    }
}
