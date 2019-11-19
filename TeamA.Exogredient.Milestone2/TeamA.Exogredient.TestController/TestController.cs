using System;
using System.Collections.Generic;
using System.Globalization;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TestDAO td = new TestDAO();

            TestRecord tr = new TestRecord(tc: "changed 51");

            DataStoreLoggingDAO dsLoggingDao = new DataStoreLoggingDAO();

            FlatFileLoggingDAO ffLoggingDao = new FlatFileLoggingDAO();



            //TODO: learn what this does
            string timestampOriginal = DateTime.UtcNow.ToString("HH:mm:ss:ff UTC&yyyyMMdd", CultureInfo.InvariantCulture);
            string[] splitResult = timestampOriginal.Split('&');
            string timestamp = splitResult[0];
            string collectionName = "logs_" + splitResult[1];
            string fileName = splitResult[1] + ".CSV";
            string folderName = splitResult[1].Substring(0, 6) + "01";

            timestamp = timestamp.Replace("-", "/");

            LogRecord record = new LogRecord(timestamp, "DELETE THIS", "1111", "localhost");

            Console.WriteLine(folderName);

            // FLAT FILE LOGGING
            ffLoggingDao.Create(record, folderName, fileName);



            // DATA STORE LOGGING
            //dsLoggingDao.Create(record, collectionName);

            //string id = dsLoggingDao.FindIdField(record, collectionName);

            //Console.WriteLine(id);

            //dsLoggingDao.Delete(id, collectionName);



            // SQL TESTING

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            //Console.WriteLine(string.Join("||", (td.ReadByIDs(new List<int>() { 2, 3 })).ToArray()));

            //td.Update(51, tr);

        }
    }
}
