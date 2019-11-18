using System;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
//HACK: to use MySQL.Data nuget, install Connector/NET here: https://dev.mysql.com/downloads/connector/net/

namespace TeamA.Exogredient.DAL
{
    public class DataStoreLoggingDAO : MasterNOSQLDAO<string>
    {
        // ID field name.
        private string _id = "_id";

        public override void Create(object record, string collectionName)
        {
            if (record.GetType() == typeof(LogRecord))
            {
                LogRecord logRecord = (LogRecord)record;
                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(Schema);

                var collection = schema.CreateCollection(collectionName, ReuseExistingObject: true);

                // Created anon type to represent json in document store.
                var document = new
                {
                    timestamp = logRecord.Timestamp,
                    operation = logRecord.Operation,
                    identifier = logRecord.Identifier,
                    ip = logRecord.IPAddress,
                    errorType = logRecord.ErrorType
                };

                collection.Add(document).Execute();
                session.Close();
            }
            else
            {
                throw new ArgumentException("Record must be of class LogRecord");
            }
        }

        public override void Delete(string uniqueID, string collectionName)
        {
            Session session = MySQLX.GetSession(ConnectionString);

            Schema schema = session.GetSchema(Schema);

            var collection = schema.GetCollection(collectionName);

            collection.Remove($"{_id} = :id").Bind("id", uniqueID).Execute();

            session.Close();
        }

        // TODO: Can't find id by identifier and timestamp... need operation
        public string FindID(string timestampArgument, string operationArgument, string identifierArgument,
                             string ipArgument, string collectionName)
        {
            Session session = MySQLX.GetSession(ConnectionString);

            Schema schema = session.GetSchema(Schema);

            var collection = schema.GetCollection(collectionName);

            var documentParams = new DbDoc(new { timestamp = timestampArgument, operation = operationArgument, identifier = identifierArgument, ip = ipArgument });

            DocResult result = collection.Find("timestamp = :timestamp && operation = :operation && identifier = :identifier && ip = :ip").Bind(documentParams).Execute();

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
    }
}
