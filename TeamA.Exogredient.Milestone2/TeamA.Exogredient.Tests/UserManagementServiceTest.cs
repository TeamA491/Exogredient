using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserManagementServiceTest
    {
        UserDAO userDAO = new UserDAO();

        [DataTestMethod]
        [DataRow("charles971026")]
        public async Task UserManagementService_DisableUserName_ValidUserName(string userName)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                await UserManagementService.DisableUserNameAsync(userName);
                result = await userDAO.CheckIfUserDisabledAsync(userName);
            }
            catch
            {
                result = await userDAO.CheckIfUserDisabledAsync(userName);
            }

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public async Task UserManagementService_EnableUserName_ValidUserName(string userName)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                await UserManagementService.EnableUserNameAsync(userName);
                result = await userDAO.CheckIfUserDisabledAsync(userName);
            }
            catch
            {
                result = await userDAO.CheckIfUserDisabledAsync(userName);
            }

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("testuser", "newpassword")]
        public async Task UserManagementService_ChangePassword_ValidUserName(string userName, string password)
        {
            //Arrange


            //Act
            await UserManagementService.ChangePasswordAsync(userName, password);
            Tuple<string, string> result = await userDAO.GetStoredPasswordAndSaltAsync(userName);

            string storedPassword = result.Item1;
            string saltString = result.Item2;
            byte[] saltBytes = StringUtilityService.HexStringToBytes(saltString);
            string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);

            //Assert
            Assert.IsTrue(storedPassword.Equals(hashedPassword));
        }
    }
}
