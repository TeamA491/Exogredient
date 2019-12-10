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
using TeamA.Exogredient.Managers;
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

            byte[] res = SecurityService.DecryptRSA(encryptedBytes, UtilityService.HexStringToBytes(Constants.PrivateKey));

            LogRecord record = new LogRecord("fail", "fail", "fail", "fail", "fail");
            LogDAO ldao = new LogDAO();

            string result = await AuthenticationService.VerifyPhoneCodeAsync("9498675309", "7865");

            Console.WriteLine(result);

            //var task = Task.Run(() => VerifyPhoneCodeManager.VerifyPhoneCodeAsync("eli", "0432", "ip", "9499815506", false));

            //if (task.Wait(TimeSpan.FromSeconds(30)))
            //    Console.WriteLine(task.Result);
            //else
            //    Console.WriteLine("Timed out");

            //Console.WriteLine(Encoding.UTF8.GetString(res));

            //await AuthenticationService.SendCallVerificationAsync("eli", "9499815506").ConfigureAwait(false);
            //string result = await AuthenticationService.VerifyPhoneCodeAsync("9499815506", "2247").ConfigureAwait(false);

            //Console.WriteLine(result);
        }
    }
}
