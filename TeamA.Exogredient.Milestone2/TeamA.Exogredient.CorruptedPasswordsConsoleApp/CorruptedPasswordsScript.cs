﻿using MySqlX.XDevAPI;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.CorruptedPasswordsConsoleApp
{
    public class CorruptedPasswordsScript
    {
        public static async Task Main(string[] args)
        {
            DataStoreLoggingDAO ds = new DataStoreLoggingDAO();

            string line;

            // Change this to your specific path!!
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Eli\Desktop\pwned-passwords-sha1-ordered-by-count-v5.txt");

            while ((line = file.ReadLine()) != null)
            {
                await ds.CreateAsync(line.Split(':')[0]).ConfigureAwait(false);
            }
        }
    }

    public abstract class MasterNOSQLDAO<T>
    {
        // HACK: Change this to your specific password
        protected static readonly string ConnectionString = Constants.NOSQLConnection;

        protected const string Schema = Constants.CorruptedPassSchemaName;

        public abstract Task<bool> CreateAsync(string password);

    }

    public class DataStoreLoggingDAO : MasterNOSQLDAO<string>
    {

        public override async Task<bool> CreateAsync(string password)
        {
            using (Session session = MySQLX.GetSession(ConnectionString))
            {
                Schema schema;

                try
                {
                    schema = session.CreateSchema(Schema);
                }
                catch
                {
                    schema = session.GetSchema(Schema);
                }

                var collection = schema.CreateCollection(Constants.CorruptedPassCollectionName, ReuseExistingObject: true);

                // Created anon type to represent json in document store.
                var document = new
                {
                    password
                };

                await collection.Add(document).ExecuteAsync().ConfigureAwait(false);

                return true;
            }
        }
    }
}
