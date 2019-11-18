using System;
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

            // '{\"ip\": \"localhost\", \"_id\": \"00005dd1cb8d000000000000000b\", \"errorType\": null, \"operation\": \"testing2\", \"timestamp\": \"02:41:12:94 UTC\", \"identifier\": \"2222\"}', ?


            Console.WriteLine(loggingDao.FindID("02:41:12:94 UTC", "testing2", "2222", "localhost", "logs_20191118"));

            // TODO: learn what this does
            //string timestampOriginal = DateTime.UtcNow.ToString("HH:mm:ss:ff UTC&yyyyMMdd", CultureInfo.InvariantCulture);
            //string timestampOriginal2 = DateTime.UtcNow.ToString("HH:mm:ss:ff UTC&yyyyMMdd", CultureInfo.InvariantCulture);
            //string[] splitResult = timestampOriginal.Split('&');
            //string timestamp = splitResult[0];
            //string collectionName = "logs_" + splitResult[1];

            //string[] splitResult2 = timestampOriginal2.Split('&');
            //string timestamp2 = splitResult2[0];
            //string collectionName2 = "logs_" + splitResult[1];
            //Console.WriteLine(collectionName);

            //timestamp = timestamp.Replace("-", "/");

            //LogRecord record = new LogRecord(timestamp, "testing", "1111", "localhost");
            //LogRecord record2 = new LogRecord(timestamp2, "testing2", "2222", "localhost");

            //loggingDao.Create(record, collectionName);
            //loggingDao.Create(record2, collectionName2);

            //loggingDao.Delete("00005dce61b30000000000000003");

            // '{\"ip\": \"localhost\", \"_id\": \"00005dce61b30000000000000007\", \"errorType\": null, \"operation\": \"test operation\", \"timestamp\": \"11/17/2019 7:26:00 AM\", \"identifier\": \"eli\"}', ?


            //string res = loggingDao.ReadById("00005dce61b30000000000000007");
            //Console.WriteLine(res);

            // THIS IS HOW YOU USE A DATA ACCESS OBJECT WITH A RECORD OBJECT

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            //Console.WriteLine(string.Join("||",(td.ReadByIDs(new List<int>() { 2,3 })).ToArray()));

            //td.Update(51, tr);

        }
    }
}
