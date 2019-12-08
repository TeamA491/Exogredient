using System;
using System.Threading.Tasks;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class LogDAO : IMasterNOSQLDAO<string>
    {
        public async Task<bool> CreateAsync(INOSQLRecord record, string yyyymmdd)
        {
            try
            {
                LogRecord temp = (LogRecord)record;
            }
            catch
            {
                throw new ArgumentException("DataStoreLoggingDAO.CreateAsync record argument must be of type LogRecord");
            }

            LogRecord logRecord = (LogRecord)record;

            using (Session session = MySQLX.GetSession(Constants.NOSQLConnection))
            {
                // Create schema if it doesn't exist.
                Schema schema;

                try
                {
                    schema = session.CreateSchema(Constants.LogsSchemaName);
                }
                catch
                {
                    schema = session.GetSchema(Constants.LogsSchemaName);
                }

                var collection = schema.CreateCollection(Constants.LogsCollectionPrefix + yyyymmdd, ReuseExistingObject: true);

                // HACK: hardcoded here for now
                // Created anon type to represent json in document store.
                var document = new
                {
                    timestamp = logRecord.Timestamp,
                    operation = logRecord.Operation,
                    identifier = logRecord.Identifier,
                    ipAddress = logRecord.IPAddress,
                    errorType = logRecord.ErrorType
                };

                await collection.Add(document).ExecuteAsync().ConfigureAwait(false);

                return true;
            }
        }

        public async Task<bool> DeleteAsync(string uniqueId, string yyyymmdd)
        {

            using (Session session = MySQLX.GetSession(Constants.NOSQLConnection))
            {
                Schema schema = session.GetSchema(Constants.LogsSchemaName);

                var collection = schema.GetCollection(Constants.LogsCollectionPrefix + yyyymmdd);

                await collection.Remove($"{Constants.LogsIdField} = :id").Bind("id", uniqueId).ExecuteAsync().ConfigureAwait(false);

                return true;
            }
        }

        // TODO: Can't find id by identifier and timestamp... need operation
        public async Task<string> FindIdFieldAsync(INOSQLRecord record, string yyyymmdd)
        {
            try
            {
                LogRecord temp = (LogRecord)record;
            }
            catch
            {
                throw new ArgumentException("DataStoreLoggingDAO.FindIdFieldAsync record argument must be of type LogRecord");
            }

            LogRecord logRecord = (LogRecord)record;

            using (Session session = MySQLX.GetSession(Constants.NOSQLConnection))
            {
                Schema schema = session.GetSchema(Constants.LogsSchemaName);

                var collection = schema.GetCollection(Constants.LogsCollectionPrefix + yyyymmdd);

                var documentParams = new DbDoc(new { timestamp = logRecord.Timestamp, operation = logRecord.Operation, identifier = logRecord.Identifier, ip = logRecord.IPAddress });

                DocResult result = await collection.Find("timestamp = :timestamp && operation = :operation && identifier = :identifier && ip = :ip").Bind(documentParams).ExecuteAsync().ConfigureAwait(false);

                // Prepare string to be returned
                string resultstring = "";

                while (result.Next())
                {
                    // TODO: flesh out columns. make columns into fields.
                    resultstring = (string)result.Current[Constants.LogsIdField];

                }

                return resultstring;
            }
        }
    }
}
