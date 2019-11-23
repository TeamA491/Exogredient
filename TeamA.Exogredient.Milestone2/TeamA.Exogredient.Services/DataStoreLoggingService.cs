using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class DataStoreLoggingService
    {

        // change string
        public void LogToDataStore(string operation, string timestamp, string username,
                        string ipAddress, string errorType)
        {
            // TODO: change timestamp to string instead of DateTime

            // Create record to represent log that is inserted into datastore and flatfile.
            // timestamp is in format of "HH:mm:ss:ff UTC&yyyyMMdd", CultureInfo.InvariantCulture);

            string[] timeResult = timestamp.Split('&');
            LogRecord record = new LogRecord(timeResult[0], operation, username, ipAddress, errorType);

            DataStoreLoggingDAO dsLoggingDao = new DataStoreLoggingDAO();

            // Extract collection name from timestamp
            string collectionName = "logs_" + timeResult[1];

            dsLoggingDao.Create(record, collectionName);
        }

    }
}
