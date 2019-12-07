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

            byte[] res = SecurityService.DecryptRSA(encryptedBytes, UtilityService.HexStringToBytes(Constants.PrivateKey));

            Console.WriteLine(Encoding.UTF8.GetString(res));

            //await AuthenticationService.SendCallVerificationAsync("9499815506").ConfigureAwait(false);
            //bool result = await AuthenticationService.VerifyPhoneCodeAsync("9499815506", "8932").ConfigureAwait(false);

            //Console.WriteLine(result);
        }
    }
}
