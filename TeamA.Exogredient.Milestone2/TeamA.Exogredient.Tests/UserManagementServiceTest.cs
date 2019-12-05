using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserManagementServiceTest
    {
        private readonly UserDAO _userDAO = new UserDAO();

        [DataTestMethod]
        [DataRow("charles971026")]
        public async Task UserManagementService_DisableUserAsync_ValidUserName(string username)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                await UserManagementService.DisableUserAsync(username);
                UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
                result = (user.Disabled == 1);
            }
            catch
            {
                UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
                result = (user.Disabled == 1);
            }

            //Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("charles971026")]
        public async Task UserManagementService_EnableUserName_ValidUserName(string username)
        {
            //Arrange
            bool result;

            //Act
            try
            {
                await UserManagementService.EnableUserAsync(username);
                UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
                result = (user.Disabled == 1);
            }
            catch
            {
                UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
                result = (user.Disabled == 1);
            }

            //Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("testuser", "newpassword")]
        public async Task UserManagementService_ChangePassword_ValidUserName(string username, string password)
        {
            //Arrange


            //Act
            await UserManagementService.ChangePasswordAsync(username, password);
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            string storedPassword = user.Password;
            string saltString = user.Salt;
            byte[] saltBytes = UtilityService.HexStringToBytes(saltString);
            string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);

            //Assert
            Assert.IsTrue(storedPassword.Equals(hashedPassword));
        }
    }
}
