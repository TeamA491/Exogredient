using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
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

            UserManagementService a = new UserManagementService();

            CorruptedPasswordsDAO c = new CorruptedPasswordsDAO();

            RegistrationService r = new RegistrationService();

            AuthenticationService authn = new AuthenticationService();


            //await a.NotifySystemAdminAsync("hi");

            //await authn.SendCallVerificationAsync("5622537764");

            //await authn.SendEmailVerificationAsync("jnguyen7539@gmail.com");

            //Console.WriteLine(await r.CheckPasswordSecurityAsync("abcdefg"));

            //Console.WriteLine(",,,,");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync(",,,,"));
            //Console.WriteLine();
            //Console.WriteLine("1234567");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("1234567"));
            //Console.WriteLine();
            //Console.WriteLine("lasdkfjlafdsjabcdef");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("lasdkfjlafdsjabcdef"));
            //Console.WriteLine();
            //Console.WriteLine("123");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("123"));
            //Console.WriteLine();
            //Console.WriteLine("7890123");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("7890123"));
            //Console.WriteLine();
            //Console.WriteLine("zyx");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("zyx"));
            //Console.WriteLine();
            //Console.WriteLine("AGTY");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("AGTY"));
            //Console.WriteLine();
            //Console.WriteLine("dslfj;laf111111;lkdjflaj");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("dslfj;laf111111;lkdjflaj"));
            //Console.WriteLine();
            //Console.WriteLine("87baudjey");
            //Console.WriteLine(await r.CheckPasswordSecurityAsync("87baudjey"));
            //Console.WriteLine();

            //Console.WriteLine(DateTime.Now.ToString("0:HH:mm:ss.ffff"));
            //Console.WriteLine(DateTime.Now.ToString("0:HH:mm:ss.ffff"));

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
