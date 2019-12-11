using MySqlX.XDevAPI;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.CorruptedPasswordsConsoleApp
{
    /// <summary>
    /// Class containg the script to create the corrupted password collection and schema,
    /// while populating it with data from a text file containing corrupted passwords
    /// hashed with SHA1.
    /// </summary>
    public class CorruptedPasswordsScript
    {
        /// <summary>
        /// Executes the reading from the file of corrupted passwords and storing them in the correspondin
        /// data store.
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Main()
        {
            DAO ds = new DAO();

            string line;

            // Put the file in this location, or change this to your specific path!!
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\pwned-passwords-sha1-ordered-by-count-v5.txt");

            // Read every line in the file.
            while ((line = await file.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                // Only take the first part (the digest) to the left of the : in the line. Store in the
                await ds.CreateAsync(line.Split(':')[0]).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Abstract class that defines the schema name, connection name, and create function of NOSQLDAOs
    /// in this context.
    /// </summary>
    public abstract class MasterNOSQLDAO
    {
        protected static readonly string ConnectionString = Constants.NOSQLConnection;

        protected const string Schema = Constants.CorruptedPassSchemaName;

        public abstract Task<bool> CreateAsync(string password);

    }

    /// <summary>
    /// Object for creating records in the corrupted passwords dao.
    /// </summary>
    public class DAO : MasterNOSQLDAO
    {
        /// <summary>
        /// Creates the schema and collection if necessary, then stores the <paramref name="password"/> in the
        /// collection.
        /// </summary>
        /// <param name="password">The password digest to be stored in the collection (string)</param>
        /// <returns>Task(bool) whether the process completed</returns>
        public override async Task<bool> CreateAsync(string password)
        {
            using (Session session = MySQLX.GetSession(ConnectionString))
            {
                // Create the schema if it doesn't exist, otherwise get the schema.
                Schema schema;

                try
                {
                    schema = session.CreateSchema(Schema);
                }
                catch
                {
                    schema = session.GetSchema(Schema);
                }

                // Create the collection if it doesn't exist, otherwise get the collection.
                var collection = schema.CreateCollection(Constants.CorruptedPassCollectionName, ReuseExistingObject: true);

                // Created the json string to store in the collection.
                string document = $@"{{""{Constants.CorruptedPassPasswordField}"": ""{password}""}}";

                // Add the json to the colleciton asynchronously.
                await collection.Add(document).ExecuteAsync().ConfigureAwait(false);

                return true;
            }
        }
    }
}
