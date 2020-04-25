using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class SystemUtilityServiceTests
    {
        [DataTestMethod]
        [DataRow("123456")]
        [DataRow("123456789")]
        [DataRow("qwerty")]
        [DataRow("password")]
        [DataRow("password1")]
        [DataRow("abc123")]
        public async Task SystemUtilityService_IsCorruptedPassword_IsCorruptedAsyncSuccess(string plaintextPassword)
        {
            // Act
            bool result = await SystemUtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("message", "data", true, 1)]
        public void SystemUtilityService_CreateResult_CreateAccurateResult(string message, string data, bool exceptionOccured)
        {
            // Act
            Result<string> resultObject = SystemUtilityService.CreateResult<string>(message, data, exceptionOccured);

            // Assert: Check that the result we created matches the inputs. 
            bool result;
            if (resultObject.Message == message && resultObject.Data == data &&
                resultObject.ExceptionOccurred == exceptionOccured)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("this is an email")]
        public async Task SystemUtilityService_NotifySystemAdminAsync_SendEmailToSystemAdminSuccess(string body)
        {
            // Act: Check that a successfull message to system admin returns true.
            bool notifyResult = await SystemUtilityService.NotifySystemAdminAsync(body, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
            Assert.IsTrue(notifyResult);
        }
    }
}
