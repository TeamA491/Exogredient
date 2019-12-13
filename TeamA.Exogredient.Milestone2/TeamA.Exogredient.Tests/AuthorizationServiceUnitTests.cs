using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class AuthorizationServiceUnitTests
    {
        private readonly Dictionary<string, string> TestPayload = new Dictionary<string, string>
        {
            { Constants.UserTypeKey, "1" },
            { Constants.IdKey, "FMolasses" },
            { Constants.AuthzExpirationField, "00000" },
        };

        [TestMethod]
        public void AuthorizationService_GenerateJWS_SuccessGenerateAndDecryptJWS()
        {
            // Arrange
            string jwsToken;
            Dictionary<string, string> payload;

            // Act
            jwsToken = AuthorizationService.GenerateJWS(TestPayload);
            payload = AuthorizationService.DecryptJWS(jwsToken);

            // Assert
            Assert.AreEqual(TestPayload[Constants.UserTypeKey], payload[Constants.UserTypeKey]);
            Assert.AreEqual(TestPayload[Constants.IdKey], payload[Constants.IdKey]);
            Assert.AreEqual(TestPayload[Constants.AuthzExpirationField], payload[Constants.AuthzExpirationField]);
        }

        [TestMethod]
        public void AuthorizationService_RefreshJWS_SuccessTokenRefreshed()
        {
            // Arrange
            string jwsToken;
            string refreshedToken;
            long expectedExpirationTime;
            Dictionary<string, string> payload;

            // Act
            jwsToken = AuthorizationService.GenerateJWS(TestPayload);
            refreshedToken = AuthorizationService.RefreshJWS(jwsToken,           // Token to refresh
                                                             1);                 // Set expiration 1 minute from now

            payload = AuthorizationService.DecryptJWS(refreshedToken);
            expectedExpirationTime = UtilityService.GetEpochFromNow(1);

            // Assert
            Assert.AreEqual(int.Parse(payload[Constants.AuthzExpirationField]), expectedExpirationTime);
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_SuccessExpired()
        {
            // Arrange
            string expiredToken;
            string jwsToken;

            // Act
            jwsToken = AuthorizationService.GenerateJWS(TestPayload);
            expiredToken = AuthorizationService.RefreshJWS(jwsToken,            // Token to refresh
                                                           -20);                // Set back 20 minutes

            // Assert
            Assert.IsTrue(AuthorizationService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_FalseExpired()
        {
            // Arrange
            string expiredToken;
            string jwsToken;

            // Act
            jwsToken = AuthorizationService.GenerateJWS(TestPayload);
            expiredToken = AuthorizationService.RefreshJWS(jwsToken,            // Token to refresh
                                                           20);                 // Set back 20 minutes
                                                                                

            // Assert
            Assert.IsFalse(AuthorizationService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        [DataRow(0, "login")]
        [DataRow(0, "register")]
        [DataRow(1, "search")]
        [DataRow(1, "upload")]
        [DataRow(2, "claimBusiness")]
        [DataRow(2, "createAd")]
        [DataRow(3, "createUser")]
        [DataRow(3, "deleteUser")]
        [DataRow(4, "createSysAdmin")]
        [DataRow(4, "search")]
        public void AuthorizationService_UserHasPermissionForOperation_SuccessHasPermission(int userRole, string operation)
        {
            // Arrange
            bool hasPermission;

            // Act
            hasPermission = AuthorizationService.UserHasPermissionForOperation(userRole, operation);

            // Assert
            Assert.IsTrue(hasPermission);
        }

        [TestMethod]
        [DataRow(0, "search")]
        [DataRow(1, "claimBusiness")]
        [DataRow(5, "search")]
        [DataRow(-1, "register")]
        public void AuthorizationService_UserHasPermissionForOperation_FailHasPermission(int userRole, string operation)
        {
            // Arrange
            bool hasPermission;

            // Act
            hasPermission = AuthorizationService.UserHasPermissionForOperation(userRole, operation);

            // Assert
            Assert.IsFalse(hasPermission);
        }
    }
}
