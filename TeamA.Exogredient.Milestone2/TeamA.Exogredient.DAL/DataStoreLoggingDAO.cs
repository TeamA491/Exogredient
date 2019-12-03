using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;

namespace TeamA.Exogredient.DAL
{
    public class DataStoreLoggingDAO : MasterNOSQLDAO<string>
    {
        // ID field name.
        private readonly string _id = "_id";

        private readonly string _collectionPrefix = "logs_";

        private readonly string _schema = "exogredient_logs";

        public async override Task<bool> CreateAsync(object record, string yyyymmdd)
        {
            try
            {
                LogRecord logRecord = (LogRecord)record;
                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(_schema);

                var collection = schema.CreateCollection(_collectionPrefix + yyyymmdd, ReuseExistingObject: true);

                // Created anon type to represent json in document store.
                var document = new
                {
                    timestamp = logRecord.Timestamp,
                    operation = logRecord.Operation,
                    identifier = logRecord.Identifier,
                    ip = logRecord.IPAddress,
                    errorType = logRecord.ErrorType
                };

                await collection.Add(document).ExecuteAsync();
                session.Close();

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async override Task<bool> DeleteAsync(string uniqueId, string yyyymmdd)
        {
            try
            {
                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(_schema);

                var collection = schema.GetCollection(_collectionPrefix + yyyymmdd);

                await collection.Remove($"{_id} = :id").Bind("id", uniqueId).ExecuteAsync();

                session.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // TODO: Can't find id by identifier and timestamp... need operation
        public async override Task<string> FindIdFieldAsync(object record, string yyyymmdd)
        {
            try
            {
                LogRecord logRecord = (LogRecord)record;

                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(_schema);

                var collection = schema.GetCollection(_collectionPrefix + yyyymmdd);

                var documentParams = new DbDoc(new { timestamp = logRecord.Timestamp, operation = logRecord.Operation, identifier = logRecord.Identifier, ip = logRecord.IPAddress });

                DocResult result = await collection.Find("timestamp = :timestamp && operation = :operation && identifier = :identifier && ip = :ip").Bind(documentParams).ExecuteAsync();

                // Prepare string to be returned
                string resultstring = "";
                while (result.Next())
                {
                    // TODO: flesh out columns. make columns into fields.
                    resultstring = (string)result.Current["_id"];

                }

                session.Close();

                return resultstring;
            }
            catch (Exception e)
            {
                // HACK !!!!
                throw e;
            }
        }
    }
}
