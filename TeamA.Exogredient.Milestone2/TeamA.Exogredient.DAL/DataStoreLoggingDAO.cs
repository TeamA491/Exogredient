using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class DataStoreLoggingDAO : IMasterNOSQLDAO<string>
    {
        public async Task<bool> CreateAsync(INOSQLRecord record, string yyyymmdd)
        {
            try
            {
                LogRecord logRecord = (LogRecord)record;
                Session session = MySQLX.GetSession(Constants.NOSQLConnection);

                Schema schema = session.GetSchema(Constants.LogsSchemaName);

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

                await collection.Add(document).ExecuteAsync();
                session.Close();

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<bool> DeleteAsync(string uniqueId, string yyyymmdd)
        {
            try
            {
                Session session = MySQLX.GetSession(Constants.NOSQLConnection);

                Schema schema = session.GetSchema(Constants.LogsSchemaName);

                var collection = schema.GetCollection(Constants.LogsCollectionPrefix + yyyymmdd);

                await collection.Remove($"{Constants.LogsIdField} = :id").Bind("id", uniqueId).ExecuteAsync();

                session.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // TODO: Can't find id by identifier and timestamp... need operation
        public async Task<string> FindIdFieldAsync(INOSQLRecord record, string yyyymmdd)
        {
            try
            {
                LogRecord logRecord = (LogRecord)record;

                Session session = MySQLX.GetSession(Constants.NOSQLConnection);

                Schema schema = session.GetSchema(Constants.LogsSchemaName);

                var collection = schema.GetCollection(Constants.LogsCollectionPrefix + yyyymmdd);

                var documentParams = new DbDoc(new { timestamp = logRecord.Timestamp, operation = logRecord.Operation, identifier = logRecord.Identifier, ip = logRecord.IPAddress });

                DocResult result = await collection.Find("timestamp = :timestamp && operation = :operation && identifier = :identifier && ip = :ip").Bind(documentParams).ExecuteAsync();

                // Prepare string to be returned
                string resultstring = "";
                while (result.Next())
                {
                    // TODO: flesh out columns. make columns into fields.
                    resultstring = (string)result.Current[Constants.LogsIdField];

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
