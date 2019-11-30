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
        UserDAO userDAO = new UserDAO();


        [DataTestMethod]
        [DataRow("charles971026", "correctpassword")]
        public void AuthenticationService_Authenticate_CorrectInputs(string userName, string password)
        {
            //Arrange
            if (await userDAO.IsUserNameDisabledAsync(userName))
            {
                await authenticationService.EnableUserNameAsync(userName);
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
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026", "wrongpassword")]
        public void AuthenticationService_Authenticate_IncorrectPassword(string userName, string password)
        {
            //Arrange
            if (await userDAO.IsUserNameDisabledAsync(userName))
            {
                await authenticationService.EnableUserNameAsync(userName);
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
        [DataRow("charles9710", "correctpassword")]
        public void AuthenticationService_Authenticate_IncorrectUserName(string userName, string password)
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
        public void AuthenticationService_DisableUserName_ValidUserName(string userName)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                await authenticationService.DisableUserNameAsync(userName);
                result = await userDAO.IsUserNameDisabledAsync(userName);
            }
            catch
            {
                result = await userDAO.IsUserNameDisabledAsync(userName);
            }

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public void AuthenticationService_EnableUserName_ValidUserName(string userName)
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

        [DataTestMethod]
        [DataRow("testuser","newpassword")]
        public void AuthenticationService_ChangePassword_ValidUserName(string userName, string password)
        {
            //Arrange
            string storedPassword;
            string saltString;

            //Act=
            authenticationService.ChangePassword(userName, password);
            userDAO.GetStoredPasswordAndSalt(userName, out storedPassword, out saltString);
            byte[] saltBytes = SecurityService.HexStringToBytes(saltString);
            string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);

            //Assert
            Assert.IsTrue(storedPassword.Equals(hashedPassword));
        }

    }
}
