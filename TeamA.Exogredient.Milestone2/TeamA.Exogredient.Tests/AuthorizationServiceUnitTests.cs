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
        public void AuthorizationService_GenerateJWT_SuccessGenerateAndDecryptJWS()
        {
            // Arrange
            string jwtToken;
            Dictionary<string, string> payload;
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            jwtToken = authorizationService.GenerateJWT(TestPayload);
            payload = authorizationService.DecryptJWT(jwtToken);

            // Assert
            Assert.AreEqual(TestPayload[Constants.UserTypeKey], payload[Constants.UserTypeKey]);
            Assert.AreEqual(TestPayload[Constants.IdKey], payload[Constants.IdKey]);
            Assert.AreEqual(TestPayload[Constants.AuthzExpirationField], payload[Constants.AuthzExpirationField]);
        }

        [TestMethod]
        public void AuthorizationService_RefreshJWS_SuccessTokenRefreshed()
        {
            // Arrange
            string jwtToken;
            string refreshedToken;
            long expectedExpirationTime;
            Dictionary<string, string> payload;
            SessionService sessionService = new SessionService();
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            jwtToken = authorizationService.GenerateJWT(TestPayload);
            refreshedToken = sessionService.RefreshJWT(jwtToken,           // Token to refresh
                                                             1);                 // Set expiration 1 minute from now

            payload = authorizationService.DecryptJWT(refreshedToken);
            expectedExpirationTime = UtilityService.GetEpochFromNow(1);

            // Assert
            Assert.AreEqual(int.Parse(payload[Constants.AuthzExpirationField]), expectedExpirationTime);
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_SuccessExpired()
        {
            // Arrange
            string jwtToken;
            string expiredToken;
            SessionService sessionService = new SessionService();
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            jwtToken = authorizationService.GenerateJWT(TestPayload);
            expiredToken = sessionService.RefreshJWT(jwtToken,            // Token to refresh
                                                           -20);                // Set back 20 minutes

            // Assert
            Assert.IsTrue(sessionService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_FalseExpired()
        {
            // Arrange
            string jwtToken;
            string expiredToken;
            SessionService sessionService = new SessionService();
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            jwtToken = authorizationService.GenerateJWT(TestPayload);
            expiredToken = sessionService.RefreshJWT(jwtToken,            // Token to refresh
                                                           20);                 // Set back 20 minutes
                                                                                

            // Assert
            Assert.IsFalse(sessionService.TokenIsExpired(expiredToken));
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
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            hasPermission = authorizationService.UserHasPermissionForOperation(userRole, operation);

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
            AuthorizationService authorizationService = new AuthorizationService();

            // Act
            hasPermission = authorizationService.UserHasPermissionForOperation(userRole, operation);

            // Assert
            Assert.IsFalse(hasPermission);
        }
    }
}
