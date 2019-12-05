using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using System.Text;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main(string[] args)
        {
            string password = "password";
            string passHex = UtilityService.ToHexString(password);
            byte[] passBytes = UtilityService.HexStringToBytes(passHex);

            byte[] pubBytes = UtilityService.HexStringToBytes(Constants.PublicKey);

            byte[] encryptedBytes = SecurityService.EncryptRSA(passBytes, pubBytes);

            byte[] result = SecurityService.DecryptRSA(encryptedBytes, UtilityService.HexStringToBytes(Constants.PrivateKey));

            Console.WriteLine(Encoding.UTF8.GetString(result));


            //await AuthenticationService.SendCallVerificationAsync("9499815506");
            //bool result = await AuthenticationService.VerifyPhoneCodeAsync("9499815506", "8932");
            //Console.WriteLine(result);
            //Console.WriteLine(result);

            //DataStoreLoggingDAO ds = new DataStoreLoggingDAO();
            //LogRecord record = new LogRecord("test", "test", "test", "test", "test");

            //await ds.CreateAsync(record, "20191201");

            UserRecord record = new UserRecord("test2", "first", "last", "email2", "sdf", "sdf", 1, "sdf", "salt", UtilityService.CurrentUnixTime(), "", 0, 0, 0, 0, 0);
            UserDAO dao = new UserDAO();

            //await dao.CreateAsync(record);

            //int result = await dao.GetEmailCodeFailureCountAsync("eli");

            //Console.WriteLine(result);

            //UserObject r = (UserObject)await dao.ReadByIdAsync("eli");

            //Console.WriteLine(r.Username);
            //Console.WriteLine(r.Password);
            //Console.WriteLine(r.Salt);
            //Console.WriteLine(r.EmailCode);
            //Console.WriteLine(r.EmailCodeFailures);
            //Console.WriteLine(r.EmailCodeTimestamp);
            //Console.WriteLine(r.TempTimestamp);
            //Console.WriteLine(r.LastLoginFailTimestamp);
            //Console.WriteLine(r.UserType);
            //Console.WriteLine(r.Disabled);
            //Console.WriteLine(r.Email);
        }
    }
}
