using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TestDAO td = new TestDAO();

            TestRecord tr = new TestRecord(tc: "changed 51");

            DataStoreLoggingDAO dsLoggingDao = new DataStoreLoggingDAO();

            AdminFunctionalityService a = new AdminFunctionalityService();

            Directory.Delete(@"C:\Logs", true);

            //await a.NotifySystemAdminAsync("log fail");

            //DATA STORE LOGGING

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
