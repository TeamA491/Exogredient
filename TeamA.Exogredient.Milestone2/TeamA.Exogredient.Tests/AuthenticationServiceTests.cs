using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        [DataTestMethod]
        [DataRow("9499815506", "eli")]
        public async Task AuthenticationService_SendCallVerificationAsync_CallSuccessful(string phoneNumber, string username)
        {
            try
            {
                // Arrange: Create user 
                UserRecord user = new UserRecord(username, "eli", "test@gamil.com", phoneNumber, "asdasd", Constants.EnabledStatus, Constants.CustomerUserType,
                                        "123123", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

                await UserManagementService.CreateUserAsync(false, user, "system", "localhost").ConfigureAwait(false);
            }
            catch
            { }

            bool result = await AuthenticationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

            Assert.IsTrue(result);

            await UserManagementService.DeleteUserAsync(username, "system", "localhost").ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("elithegolfer@gmail.com", "eli")]
        public async Task AuthenticationService_SendEmailVerificationAsync_EmailSuccessful(string emailAddress, string username)
        {
            try
            {
                // Arrange: Create user 
                UserRecord user = new UserRecord(username, "eli", emailAddress, "5625555555", "asdasd", Constants.EnabledStatus, Constants.CustomerUserType,
                                        "123123", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);
                await UserManagementService.CreateUserAsync(false, user).ConfigureAwait(false);
            }
            catch
            { }

            bool result = await AuthenticationService.SendEmailVerificationAsync("eli", emailAddress).ConfigureAwait(false);

            Assert.IsTrue(result);

            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }
    }
}
