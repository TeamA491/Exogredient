using System;
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
            LogRecord record = new LogRecord(DateTime.UtcNow, "test operation", "eli", "localhost");

            //loggingDao.Create(record);

            //loggingDao.Delete("00005dce61b30000000000000003");

            // '{\"ip\": \"localhost\", \"_id\": \"00005dce61b30000000000000007\", \"errorType\": null, \"operation\": \"test operation\", \"timestamp\": \"11/17/2019 7:26:00 AM\", \"identifier\": \"eli\"}', ?


            string res = loggingDao.ReadById("00005dce61b30000000000000007");
            Console.WriteLine(res);

            // THIS IS HOW YOU USE A DATA ACCESS OBJECT WITH A RECORD OBJECT

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            //Console.WriteLine(string.Join("||",(td.ReadByIDs(new List<int>() { 2,3 })).ToArray()));

            //td.Update(51, tr);

        }
    }
}
