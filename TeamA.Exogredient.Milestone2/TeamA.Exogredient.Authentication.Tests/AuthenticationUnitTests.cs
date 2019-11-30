using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Authentication.Tests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        AuthenticationService authenticationService = new AuthenticationService();
        SecurityService ss = new SecurityService();
        UserDAO userDAO = new UserDAO();


        [DataTestMethod]
        [DataRow("charles971026", "password123")]
        public async Task Authenticate_CorrectInputs_ReturnTrue(string userName, string password)
        {
            //Arrange
            if (await userDAO.IsUserNameDisabledAsync(userName))
            {
                await authenticationService.EnableUserNameAsync(userName);
            }

            string hexPassword = ss.ToHexString(password);
            RSAParameters publicKey = SecurityService.GetRSAPublicKey();
            RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
            byte[] key = ss.GenerateAESKey();
            byte[] IV = ss.GenerateAESIV();
            byte[] encryptedKey = ss.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = ss.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = await authenticationService.AuthenticateAsync("charles971026", encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026", "password")]
        public async Task Authenticate_IncorrectInputs_ReturnFalse(string userName, string password)
        {
            //Arrange
            if (await userDAO.IsUserNameDisabledAsync(userName))
            {
                await authenticationService.EnableUserNameAsync(userName);
            }
            string hexPassword = ss.ToHexString(password);
            RSAParameters publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = ss.GenerateAESKey();
            byte[] IV = ss.GenerateAESIV();
            byte[] encryptedKey = ss.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = ss.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = await authenticationService.AuthenticateAsync("charles971026", encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public async Task DisableUserName_ValidUserName_UserNameDisabled(string userName)
        {

            //Arrange
            bool result;

            //Act
            try
            {
                await authenticationService.DisableUserNameAsync(userName);
                result = await userDAO.IsUserNameDisabledAsync(userName);
            }
            catch(Exception e)
            {
                result = await userDAO.IsUserNameDisabledAsync(userName);
            }

            //Assert
            Assert.IsTrue(result);

        }

    }
}
