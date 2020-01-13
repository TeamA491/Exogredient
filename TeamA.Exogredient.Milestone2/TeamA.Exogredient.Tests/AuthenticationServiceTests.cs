using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;
using System.Threading.Tasks;

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
                await UserManagementService.CreateUserAsync(false, "eli", "test", "test", phoneNumber, "test", 0, "test", "test").ConfigureAwait(false);
            }
            catch
            { }

            bool result = await AuthenticationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

            Assert.IsTrue(result);

            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("elithegolfer@gmail.com", "eli")]
        public async Task AuthenticationService_SendEmailVerificationAsync_EmailSuccessful(string emailAddress, string username)
        {
            try
            {
                await UserManagementService.CreateUserAsync(false, "eli", "test", emailAddress, "test", "test", 0, "test", "test").ConfigureAwait(false);
            }
            catch
            { }

            bool result = await AuthenticationService.SendEmailVerificationAsync("eli", emailAddress).ConfigureAwait(false);

            Assert.IsTrue(result);

            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }
    }
}
