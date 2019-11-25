using System;
using System.Security.Cryptography;
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
        public void Authenticate_CorrectInputs_ReturnTrue(string userName, string password)
        {
            //Arrange
            if (userDAO.IsUserNameDisabled(userName))
            {
                authenticationService.EnableUserName(userName);
            }

            string hexPassword = ss.ToHexString(password);
            RSAParameters publicKey = SecurityService.GetRSAPublicKey();
            RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
            byte[] key = ss.GenerateAESKey();
            byte[] IV = ss.GenerateAESIV();
            byte[] encryptedKey = ss.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = ss.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = authenticationService.Authenticate("charles971026", encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026", "password")]
        public void Authenticate_IncorrectInputs_ReturnFalse(string userName, string password)
        {
            //Arrange
            if (userDAO.IsUserNameDisabled(userName))
            {
                authenticationService.EnableUserName(userName);
            }
            string hexPassword = ss.ToHexString(password);
            RSAParameters publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = ss.GenerateAESKey();
            byte[] IV = ss.GenerateAESIV();
            byte[] encryptedKey = ss.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = ss.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = authenticationService.Authenticate("charles971026", encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public void DisableUserName_ValidUserName_UserNameDisabled(string userName)
        {

            //Arrange
            bool result;

            //Act
            try
            {
                authenticationService.DisableUserName(userName);
                result = userDAO.IsUserNameDisabled(userName);
            }
            catch(Exception e)
            {
                result = userDAO.IsUserNameDisabled(userName);
            }

            //Assert
            Assert.IsTrue(result);

        }

    }
}
