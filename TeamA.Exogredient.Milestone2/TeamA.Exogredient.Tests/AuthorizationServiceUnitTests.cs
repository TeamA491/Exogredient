using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// TODO IMPORT KEYS
// TODO MANAGER
// TODO ADD MORE TESTS AND SCENARIOS

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class AuthorizationServiceUnitTests
    {
        private const string PublicKeyTest = "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnzyis1ZjfNB0bBgKFMSv\nvkTtwlvBsaJq7S5wA+kzeVOVpVWwkWdVha4s38XM/pa/yr47av7+z3VTmvDRyAHc\naT92whREFpLv9cj5lTeJSibyr/Mrm/YtjCZVWgaOYIhwrXwKLqPr/11inWsAkfIy\ntvHWTxZYEcXLgAXFuUuaS3uF9gEiNQwzGTU1v0FqkqTBr4B8nW3HCN47XUu0t8Y0\ne+lf4s4OxQawWD79J9/5d3Ry0vbV3Am1FtGJiJvOwRsIfVChDpYStTcHTCMqtvWb\nV6L11BWkpzGXSW4Hv43qa+GSYOD2QU68Mb59oSk2OB+BtOLpJofmbGEGgvmwyCI9\nMwIDAQAB\n-----END PUBLIC KEY-----";
        private const string PrivateKeyTest = "-----BEGIN RSA PRIVATE KEY-----\nMIIEogIBAAKCAQEAnzyis1ZjfNB0bBgKFMSvvkTtwlvBsaJq7S5wA+kzeVOVpVWw\nkWdVha4s38XM/pa/yr47av7+z3VTmvDRyAHcaT92whREFpLv9cj5lTeJSibyr/Mr\nm/YtjCZVWgaOYIhwrXwKLqPr/11inWsAkfIytvHWTxZYEcXLgAXFuUuaS3uF9gEi\nNQwzGTU1v0FqkqTBr4B8nW3HCN47XUu0t8Y0e+lf4s4OxQawWD79J9/5d3Ry0vbV\n3Am1FtGJiJvOwRsIfVChDpYStTcHTCMqtvWbV6L11BWkpzGXSW4Hv43qa+GSYOD2\nQU68Mb59oSk2OB+BtOLpJofmbGEGgvmwyCI9MwIDAQABAoIBACiARq2wkltjtcjs\nkFvZ7w1JAORHbEufEO1Eu27zOIlqbgyAcAl7q+/1bip4Z/x1IVES84/yTaM8p0go\namMhvgry/mS8vNi1BN2SAZEnb/7xSxbflb70bX9RHLJqKnp5GZe2jexw+wyXlwaM\n+bclUCrh9e1ltH7IvUrRrQnFJfh+is1fRon9Co9Li0GwoN0x0byrrngU8Ak3Y6D9\nD8GjQA4Elm94ST3izJv8iCOLSDBmzsPsXfcCUZfmTfZ5DbUDMbMxRnSo3nQeoKGC\n0Lj9FkWcfmLcpGlSXTO+Ww1L7EGq+PT3NtRae1FZPwjddQ1/4V905kyQFLamAA5Y\nlSpE2wkCgYEAy1OPLQcZt4NQnQzPz2SBJqQN2P5u3vXl+zNVKP8w4eBv0vWuJJF+\nhkGNnSxXQrTkvDOIUddSKOzHHgSg4nY6K02ecyT0PPm/UZvtRpWrnBjcEVtHEJNp\nbU9pLD5iZ0J9sbzPU/LxPmuAP2Bs8JmTn6aFRspFrP7W0s1Nmk2jsm0CgYEAyH0X\n+jpoqxj4efZfkUrg5GbSEhf+dZglf0tTOA5bVg8IYwtmNk/pniLG/zI7c+GlTc9B\nBwfMr59EzBq/eFMI7+LgXaVUsM/sS4Ry+yeK6SJx/otIMWtDfqxsLD8CPMCRvecC\n2Pip4uSgrl0MOebl9XKp57GoaUWRWRHqwV4Y6h8CgYAZhI4mh4qZtnhKjY4TKDjx\nQYufXSdLAi9v3FxmvchDwOgn4L+PRVdMwDNms2bsL0m5uPn104EzM6w1vzz1zwKz\n5pTpPI0OjgWN13Tq8+PKvm/4Ga2MjgOgPWQkslulO/oMcXbPwWC3hcRdr9tcQtn9\nImf9n2spL/6EDFId+Hp/7QKBgAqlWdiXsWckdE1Fn91/NGHsc8syKvjjk1onDcw0\nNvVi5vcba9oGdElJX3e9mxqUKMrw7msJJv1MX8LWyMQC5L6YNYHDfbPF1q5L4i8j\n8mRex97UVokJQRRA452V2vCO6S5ETgpnad36de3MUxHgCOX3qL382Qx9/THVmbma\n3YfRAoGAUxL/Eu5yvMK8SAt/dJK6FedngcM3JEFNplmtLYVLWhkIlNRGDwkg3I5K\ny18Ae9n7dHVueyslrb6weq7dTkYDi3iOYRW8HRkIQh06wEdbxt0shTzAJvvCQfrB\njg/3747WSsf/zBTcHihTRBdAv6OmdhV4/dD5YBfLAkLrd+mX7iE=\n-----END RSA PRIVATE KEY-----";
        // This is a token generated online, using the test payload and keys above...
        private const string GeneratedJWSToken = "eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VyVHlwZSI6IjEiLCJpZCI6IkZNb2xhc3NlcyIsImV4cCI6IjAwMDAwIiwicGsiOiItLS0tLUJFR0lOIFBVQkxJQyBLRVktLS0tLVxuTUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUFuenlpczFaamZOQjBiQmdLRk1TdlxudmtUdHdsdkJzYUpxN1M1d0Era3plVk9WcFZXd2tXZFZoYTRzMzhYTS9wYS95cjQ3YXY3K3ozVlRtdkRSeUFIY1xuYVQ5MndoUkVGcEx2OWNqNWxUZUpTaWJ5ci9Ncm0vWXRqQ1pWV2dhT1lJaHdyWHdLTHFQci8xMWluV3NBa2ZJeVxudHZIV1R4WllFY1hMZ0FYRnVVdWFTM3VGOWdFaU5Rd3pHVFUxdjBGcWtxVEJyNEI4blczSENONDdYVXUwdDhZMFxuZStsZjRzNE94UWF3V0Q3OUo5LzVkM1J5MHZiVjNBbTFGdEdKaUp2T3dSc0lmVkNoRHBZU3RUY0hUQ01xdHZXYlxuVjZMMTFCV2twekdYU1c0SHY0M3FhK0dTWU9EMlFVNjhNYjU5b1NrMk9CK0J0T0xwSm9mbWJHRUdndm13eUNJOVxuTXdJREFRQUJcbi0tLS0tRU5EIFBVQkxJQyBLRVktLS0tLSJ9.lh4ajZXkTvlGYWmfl6Kqm-ILLiyYMo1eb4zWyvy-RWLbbfYkm5GJzNoVyTvuTvl03jLQ82qIDk30Kix9dE8e3QBVPVKgerpNGAZptrm0BX2oavezlpJJZFY9iZTBbzWpzGFcFItAd-WrQyvKr0zL7aztv3rj6h-CF1etch2fw0_5Qr9hcbrqMJ_0RiY8HRlRL_zthD2BBhTpdY2BA0LyGH0KcePHx4G1gRFFZGkOlnSy9h6pXfT6Pmcf2UyD2uIi582TCeH96GK7SM8HQNggj70OxyEPUtAap1i3ybEegmdacWnaPxVMkVuRV5fbyTSu_bSr5XttQtEpXinYUi_EYg";
        private readonly Dictionary<string, string> TestPayload = new Dictionary<string, string>
        {
            { Constants.UserTypeKey, "1" },
            { Constants.IdKey, "FMolasses" },
            { Constants.AuthzExpirationField, "00000" },
            { Constants.AuthzPublicKeyField, PublicKeyTest }
        };

        [TestMethod]
        public void AuthorizationService_GenerateJWS_SuccessGenerateJWS()
        {
            // Arrange
            string jwsToken;

            // Act
            jwsToken = AuthorizationService.GenerateJWS(TestPayload, PublicKeyTest, PrivateKeyTest);

            // Assert
            Assert.AreEqual(jwsToken, GeneratedJWSToken);
        }

        [TestMethod]
        public void AuthorizationService_DecryptJWS_SuccessDecryptJWS()
        {
            // Arrange
            Dictionary<string, string> payload;

            // Act
            payload = AuthorizationService.DecryptJWS(GeneratedJWSToken);

            // Assert
            Assert.AreEqual(TestPayload[Constants.UserTypeKey],             // Check user type
                            payload[Constants.UserTypeKey]);

            Assert.AreEqual(TestPayload[Constants.IdKey],                   // Check user id
                            payload[Constants.IdKey]);

            Assert.AreEqual(TestPayload[Constants.AuthzExpirationField],    // Check expiration
                            payload[Constants.AuthzExpirationField]);
        }

        [TestMethod]
        public void AuthorizationService_RefreshJWS_SuccessTokenRefreshed()
        {
            // Arrange
            string refreshedToken;
            long expectedExpirationTime;

            // Act
            refreshedToken = AuthorizationService.RefreshJWS(GeneratedJWSToken, // Token to refresh
                                                             1,                 // Set expiration 1 minute from now
                                                             PublicKeyTest,     // Custom public key
                                                             PrivateKeyTest);   // Custom private key

            Dictionary<string, string> payload = AuthorizationService.DecryptJWS(refreshedToken);
            expectedExpirationTime = UtilityService.GetEpochFromNow(1);

            // Assert
            Assert.AreEqual(payload[Constants.AuthzExpirationField], expectedExpirationTime);
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_SuccessExpired()
        {
            // Arrange
            string expiredToken;

            // Act
            expiredToken = AuthorizationService.RefreshJWS(GeneratedJWSToken,  // Token to refresh
                                                           -20,                // Set back 20 minutes
                                                           PublicKeyTest,      // Custom public key
                                                           PrivateKeyTest);    // Custom private key

            // Assert
            Assert.IsTrue(AuthorizationService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        public void AuthorizationService_TokenIsExpired_FalseExpired()
        {
            // Arrange
            string expiredToken;

            // Act
            expiredToken = AuthorizationService.RefreshJWS(GeneratedJWSToken,  // Token to refresh
                                                           20,                 // Set back 20 minutes
                                                           PublicKeyTest,      // Custom public key
                                                           PrivateKeyTest);    // Custom private key

            // Assert
            Assert.IsTrue(AuthorizationService.TokenIsExpired(expiredToken));
        }

        [TestMethod]
        [DataRow(0, "login")]
        [DataRow(0, "signup")]
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
