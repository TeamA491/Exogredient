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
            //await AuthenticationService.SendCallVerificationAsync("9499815506");
            //string result = await AuthenticationService.VerifyPhoneCodeAsync("9499815506", "0738");
            //Console.WriteLine(result);

            //DataStoreLoggingDAO ds = new DataStoreLoggingDAO();
            //LogRecord record = new LogRecord("test", "test", "test", "test", "test");

            //await ds.CreateAsync(record, "20191201");

            //UserRecord record = new UserRecord("eli", "first", "last", "email", "dkkdk", "password", 1, "kddk", "salt", StringUtilityService.CurrentUnixTime(), "", 0, 0, 0, 0, 0);
            //UserDAO dao = new UserDAO();

            //await dao.CreateAsync(record);
        }
    }
}
