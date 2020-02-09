using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class _authorizationServiceTests
    {
        private static readonly UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
        private static readonly IPAddressDAO _ipDAO = new IPAddressDAO(Constants.SQLConnection);
        private static readonly LogDAO _logDAO = new LogDAO(Constants.SQLConnection);
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.SQLConnection);
        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private static readonly DataStoreLoggingService _dsLog = new DataStoreLoggingService(_logDAO, _maskingService);
        private static readonly FlatFileLoggingService _ffLog = new FlatFileLoggingService(_maskingService);

        private static readonly UserManagementService _userManagementService = new UserManagementService(_userDAO, _ipDAO, _dsLog, _ffLog, _maskingService);
        private readonly AuthorizationService _authorizationService = new AuthorizationService(_userManagementService);

        private readonly Dictionary<string, string> TestPayload = new Dictionary<string, string>
        {
            { Constants.UserTypeKey, "1" },
            { Constants.IdKey, "FMolasses" },
            { Constants.AuthzExpirationField, "00000" },
        };

        [TestMethod]
        public void _authorizationService_GenerateJWS_SuccessGenerateAndDecryptJWS()
        {
            // Arrange
            string jwsToken;
            Dictionary<string, string> payload;

            // Act
            jwsToken = _authorizationService.GenerateJWS(TestPayload);
            payload = _authorizationService.DecryptJWS(jwsToken);

            // Assert
            Assert.AreEqual(TestPayload[Constants.UserTypeKey], payload[Constants.UserTypeKey]);
            Assert.AreEqual(TestPayload[Constants.IdKey], payload[Constants.IdKey]);
            Assert.AreEqual(TestPayload[Constants.AuthzExpirationField], payload[Constants.AuthzExpirationField]);
        }

        [TestMethod]
        public void _authorizationService_RefreshJWS_SuccessTokenRefreshed()
        {
            // Arrange
            string jwsToken;
            string refreshedToken;
            long expectedExpirationTime;
            Dictionary<string, string> payload;

            // Act
            jwsToken = _authorizationService.GenerateJWS(TestPayload);
            refreshedToken = _authorizationService.RefreshJWS(jwsToken,           // Token to refresh
                                                             1);                 // Set expiration 1 minute from now

            payload = _authorizationService.DecryptJWS(refreshedToken);
            expectedExpirationTime = TimeUtilityService.GetEpochFromNow(1);

            // Assert
            Assert.AreEqual(int.Parse(payload[Constants.AuthzExpirationField]), expectedExpirationTime);
        }

        [TestMethod]
        public void _authorizationService_TokenIsExpired_SuccessExpired()
        {
            // Arrange
            string expiredToken;
            string jwsToken;

            // Act
            jwsToken = _authorizationService.GenerateJWS(TestPayload);
            expiredToken = _authorizationService.RefreshJWS(jwsToken,            // Token to refresh
                                                           -20);                // Set back 20 minutes

            // Assert
            Assert.IsTrue(_authorizationService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        public void _authorizationService_TokenIsExpired_FalseExpired()
        {
            // Arrange
            string expiredToken;
            string jwsToken;

            // Act
            jwsToken = _authorizationService.GenerateJWS(TestPayload);
            expiredToken = _authorizationService.RefreshJWS(jwsToken,            // Token to refresh
                                                           20);                 // Set back 20 minutes
                                                                                

            // Assert
            Assert.IsFalse(_authorizationService.TokenIsExpired(expiredToken));
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
        public void _authorizationService_UserHasPermissionForOperation_SuccessHasPermission(int userRole, string operation)
        {
            // Arrange
            bool hasPermission;

            // Act
            hasPermission = _authorizationService.UserHasPermissionForOperation(userRole, operation);

            // Assert
            Assert.IsTrue(hasPermission);
        }

        [TestMethod]
        [DataRow(0, "search")]
        [DataRow(1, "claimBusiness")]
        [DataRow(5, "search")]
        [DataRow(-1, "register")]
        public void _authorizationService_UserHasPermissionForOperation_FailHasPermission(int userRole, string operation)
        {
            // Arrange
            bool hasPermission;

            // Act
            hasPermission = _authorizationService.UserHasPermissionForOperation(userRole, operation);

            // Assert
            Assert.IsFalse(hasPermission);
        }
    }
}
