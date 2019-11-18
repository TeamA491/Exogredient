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

            DataStoreLoggingDAO loggingDao = new DataStoreLoggingDAO();

            //string id = loggingDao.FindID("20:34:23:53 UTC", "DELETE THIS", "1111", "localhost", "logs_20191118");

            //loggingDao.Delete(id, "logs_20191118");

            //TODO: learn what this does
            //string timestampOriginal = DateTime.UtcNow.ToString("HH:mm:ss:ff UTC&yyyyMMdd", CultureInfo.InvariantCulture);
            //string[] splitResult = timestampOriginal.Split('&');
            //string timestamp = splitResult[0];
            //string collectionName = "logs_" + splitResult[1];
            //Console.WriteLine(collectionName);

            //timestamp = timestamp.Replace("-", "/");

            //LogRecord record = new LogRecord(timestamp, "DELETE THIS", "1111", "localhost");

            //loggingDao.Create(record, collectionName);

            //THIS IS HOW YOU USE A DATA ACCESS OBJECT WITH A RECORD OBJECT

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            //Console.WriteLine(string.Join("||", (td.ReadByIDs(new List<int>() { 2, 3 })).ToArray()));

            //td.Update(51, tr);

        }
    }
}
