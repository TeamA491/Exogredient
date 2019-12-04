using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class AuthenticationUnitTests
    {
        private readonly UserDAO _userDAO = new UserDAO();


        [DataTestMethod]
        [DataRow("charles971026", "correctpassword")]
        public async Task AuthenticationService_Authenticate_CorrectInputs(string username, string password)
        {
            //Arrange
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            if (user.Disabled == 1)
            {
                await UserManagementService.EnableUserAsync(username);
            }

            string hexPassword = StringUtilityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = await AuthenticationService.AuthenticateAsync(username, encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026", "wrongpassword")]
        public async Task AuthenticationService_Authenticate_IncorrectPassword(string username, string password)
        {
            //Arrange
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            if (user.Disabled == 1)
            {
                await UserManagementService.EnableUserAsync(username);
            }

            string hexPassword = StringUtilityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = await AuthenticationService.AuthenticateAsync(username, encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("charles9710", "correctpassword")]
        public async Task AuthenticationService_Authenticate_IncorrectUserName(string userName, string password)
        {
            //Arrange
            string hexPassword = StringUtilityService.ToHexString(password);
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();
            byte[] encryptedKey = SecurityService.EncryptRSA(key, publicKey);
            byte[] encryptedPassword = SecurityService.EncryptAES(hexPassword, key, IV);

            //Act
            bool result = await AuthenticationService.AuthenticateAsync(userName, encryptedPassword, encryptedKey, IV);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
