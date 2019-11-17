using System;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;

namespace TeamA.Exogredient.DAL
{
    public class DataStoreLoggingDAO : MasterNOSQLDAO<string>
    {
        private string _collection = "logs";
        private string _id = "_id";

        public override void Create(object record)
        {
            if (record.GetType() == typeof(LogRecord))
            {
                LogRecord logRecord = (LogRecord)record;
                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(Schema);

                var collection = schema.GetCollection(_collection);

                // Created anon type to represent json in document store.
                var document = new { timestamp = logRecord.Timestamp,
                                     operation = logRecord.Operation,
                                     identifier = logRecord.Identifier,
                                     ip = logRecord.IPAddress,
                                     errorType = logRecord.ErrorType};


                collection.Add(document).Execute();
                session.Close();
            }
            else
            {
                throw new ArgumentException("Record must be of class LogRecord");
            }
        }

        public override void Delete(string uniqueID)
        {
            Session session = MySQLX.GetSession(ConnectionString);

            Schema schema = session.GetSchema(Schema);

            var collection = schema.GetCollection(_collection);

            collection.Remove($"{_id} = :id").Bind("id", uniqueID).Execute();

            session.Close();
        }

        public override string ReadById(string uniqueID)
        {

            Session session = MySQLX.GetSession(ConnectionString);

            Schema schema = session.GetSchema(Schema);

            var collection = schema.GetCollection(_collection);

            DocResult result = collection.Find($"{_id} = :id").Bind("id", uniqueID).Execute();

            // Prepare string to be returned
            string resultString = ""; 
            while(result.Next())
            {
                // TODO: flesh out columns. make columns into fields.
                resultString += result.Current["_id"] + " ";
                resultString += result.Current["operation"];

            }
            return resultString;
        }
    }
}
