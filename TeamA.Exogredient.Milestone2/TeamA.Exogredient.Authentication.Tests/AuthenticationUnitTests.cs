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

            string hexPassword = SecurityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = authenticationService.Authenticate("charles971026", encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026", "password")]
        public void Authenticate_IncorrectPassword_ReturnFalse(string userName, string password)
        {
            //Arrange
            if (userDAO.IsUserNameDisabled(userName))
            {
                authenticationService.EnableUserName(userName);
            }
            string hexPassword = SecurityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = authenticationService.Authenticate(userName, encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("charles9710", "password")]
        public void Authenticate_IncorrectUserName_ReturnFalse(string userName, string password)
        {
            //Arrange
            string hexPassword = SecurityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = authenticationService.Authenticate(userName, encryptedPassword, encryptedKey, IV);

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
            catch
            {
                result = userDAO.IsUserNameDisabled(userName);
            }

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public void EnableUserName_ValidUserName_UserNameEnabled(string userName)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                authenticationService.EnableUserName(userName);
                result = userDAO.IsUserNameDisabled(userName);
            }
            catch
            {
                result = userDAO.IsUserNameDisabled(userName);
            }

            //Assert
            Assert.IsFalse(result);
        }
    }
}
