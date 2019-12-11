using System.Threading.Tasks;
using System.Collections.Generic;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for the data store containing the corrupted passwords from
    /// previous breaches.
    /// </summary>
    public class CorruptedPasswordDAO : IMasterNOSQLDAOReadOnly
    {
        /// <summary>
        /// Asynchronously reads all data from the data store and returns it as a list.
        /// </summary>
        /// <returns>List(string) containing all passwords from the data store.</returns>
        public async Task<List<string>> ReadAsync()
        {
            // Get the session inside a using statement to properly dispose/close.
            using (Session session = MySQLX.GetSession(Constants.NOSQLConnection))
            {
                // Get the schema and collection.
                Schema schema = session.GetSchema(Constants.CorruptedPassSchemaName);
                var collection = schema.GetCollection(Constants.CorruptedPassCollectionName);

                // Get the doc result of everything in the data store.
                DocResult result = await collection.Find().ExecuteAsync().ConfigureAwait(false);

                List<string> resultList = new List<string>();

                // Loop through doc result and all values from the password field to the result list.
                while (result.Next())
                {
                    string temp = (string)result.Current[Constants.CorruptedPassPasswordField];

                    resultList.Add(temp);
                }

                return resultList;
            }
        }
    }
}
