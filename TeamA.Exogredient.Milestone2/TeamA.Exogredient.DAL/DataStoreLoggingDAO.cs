using System;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;

namespace TeamA.Exogredient.DAL
{
    public class DataStoreLoggingDAO : MasterNOSQLDAO<string>
    {
        public override void Create(object record)
        {
            if (record.GetType() == typeof(LogRecord))
            {
                LogRecord logRecord = (LogRecord)record;
                Session session = MySQLX.GetSession(ConnectionString);

                Schema schema = session.GetSchema(Schema);

                var collection = schema.GetCollection("logs");

                //string document = JsonConvert.SerializeObject(record);

                // Test anon types
                var document = new { timestamp = logRecord.Timestamp,
                                     operation = logRecord.Operation,
                                     identifier = logRecord.Identifier,
                                     ip = logRecord.IPAddress,
                                     errorType = logRecord.ErrorType};


                Result r = collection.Add(document).Execute();
                Console.WriteLine("result: " + r);

            }
            else
            {
                throw new ArgumentException("Record must be of class LogRecord");
            }
        }

        public override void Delete(string uniqueID)
        {
            throw new NotImplementedException();
        }

        public override void Read(string uniqueID)
        {
            throw new NotImplementedException();
        }
    }
}
