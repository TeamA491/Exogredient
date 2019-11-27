using System;
using System.Collections.Generic;
using System.Globalization;
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

            DataStoreLoggingDAO dsLoggingDao = new DataStoreLoggingDAO();

            AdminFunctionalityService a = new AdminFunctionalityService();

            CorruptedPasswordsDAO c = new CorruptedPasswordsDAO();

            RegistrationService r = new RegistrationService();

            AuthenticationService authn = new AuthenticationService();

            Console.WriteLine(authn.SendSMSCode("5622537764"));

            //Console.WriteLine(await r.CheckPasswordSecurityAsync("abcdefg"));

            //await r.CheckPasswordSecurityAsync("aaa");
            //await r.CheckPasswordSecurityAsync("AAA");
            //await r.CheckPasswordSecurityAsync("111");
            //await r.CheckPasswordSecurityAsync("~~~");
            //await r.CheckPasswordSecurityAsync("```");
            //await r.CheckPasswordSecurityAsync("!!!");
            //await r.CheckPasswordSecurityAsync("@@@");
            //await r.CheckPasswordSecurityAsync("###");
            //await r.CheckPasswordSecurityAsync("$$$");
            //await r.CheckPasswordSecurityAsync("%%%");
            //await r.CheckPasswordSecurityAsync("^^^");
            //await r.CheckPasswordSecurityAsync("&&&");
            //await r.CheckPasswordSecurityAsync("***");
            //await r.CheckPasswordSecurityAsync("(((");
            //await r.CheckPasswordSecurityAsync(")))");
            //await r.CheckPasswordSecurityAsync("___");
            //await r.CheckPasswordSecurityAsync("---");
            //await r.CheckPasswordSecurityAsync("+++");
            //await r.CheckPasswordSecurityAsync("===");
            //await r.CheckPasswordSecurityAsync("{{{{");
            //await r.CheckPasswordSecurityAsync("[[[[");
            //await r.CheckPasswordSecurityAsync("}}}}");
            //await r.CheckPasswordSecurityAsync("]]]]");
            //await r.CheckPasswordSecurityAsync("||||");
            //await r.CheckPasswordSecurityAsync("\\\\\\\\");
            //await r.CheckPasswordSecurityAsync("::::");
            //await r.CheckPasswordSecurityAsync(";;;;");
            //await r.CheckPasswordSecurityAsync("''''");
            //await r.CheckPasswordSecurityAsync("\"\"\"\"");
            //await r.CheckPasswordSecurityAsync("????");
            //await r.CheckPasswordSecurityAsync("////");
            //await r.CheckPasswordSecurityAsync("????");
            //await r.CheckPasswordSecurityAsync("....");
            //await r.CheckPasswordSecurityAsync(",,,,");
            //await r.CheckPasswordSecurityAsync("1234567");
            //await r.CheckPasswordSecurityAsync("lasdkfjlafdsjabcdef");
            //await r.CheckPasswordSecurityAsync("123");
            //await r.CheckPasswordSecurityAsync("7890123");
            //await r.CheckPasswordSecurityAsync("zyx");

            // READ CORRUPTED PASSWORDS

            //List<string> result = await c.ReadAsync();

            //foreach (string s in result)
            //{
            //    Console.WriteLine(s);
            //}

            // NOTIFY SYSTEM ADMIN

            //await a.NotifySystemAdminAsync("log fail");

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
