using System;
using System.Security.Cryptography;
using System.Text;
using TeamA.Exogredient.Services;
namespace TeamA.Exogredient.AuthenticationController
{
    class AuthenticationController
    {
        static void Main(string[] args)
        {
            /*
            SecurityService ss = new SecurityService();
            byte[] salt = ss.GenerateSalt(100);
            string saltHex = ss.BytesToHexString(salt);
            string hexPassword = ss.ToHexString("password123");
            string hashedPassword = ss.HashPassword(hexPassword, salt, 100, 32);
            Console.WriteLine("Salt: " + saltHex);
            Console.WriteLine("Hashed password: " + hashedPassword);
            */
            /*
            SecurityService ss = new SecurityService();
            string hexPassword = ss.ToHexString("password");
            byte[] key = ss.GenerateAESKey();
            RSAParameters publicKey = SecurityService.GetRSAPublicKey();
            RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
            byte[] encryptedKey = ss.EncryptRSA(key, publicKey);
            byte[] IV = ss.GenerateAESIV();
            byte[] encryptedPassword = ss.EncryptAES(hexPassword, key, IV);
            AuthenticationService authenticationService = new AuthenticationService();
            bool result = authenticationService.Authenticate("charles971026", encryptedPassword, encryptedKey, IV);
            Console.WriteLine(result);
            */
            


        }
    }
}
