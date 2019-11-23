using System.Collections.Generic;

// TODO: LOAD RSA PRIVATE KEY FROM OS ENVIRONMENT
// TODO: DECIDE HOW TO HOLD EXPIRATION DATES
// TODO: BUILD JWT PAYLOAD DATA
// TODO: DECIDE WHETHER CLASS IS STATIC OR NOT
namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// The AuthorizationService will authorize users through a
    /// JSON web token (JWT). Authorization is based on a role system,
    /// where each user is assigned a role. Each role has access to
    /// only certain operations. The JWT is encrypted and decrypted using the RSA
    /// algorithm.
    /// </summary>
    public static class AuthorizationService
    {
        /// <summary>
        /// Decrypts a JWT in order to see the encrypted payload.
        /// </summary>
        /// <param name="token">The token to decrypt.</param>
        /// <param name="publicKey">The public key used to decrypt and verify the token.</param>
        /// <returns>The contents inside the decrypted token.</returns>
        public static Dictionary<string, string> DecryptJWT(string token, string publicKey)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Checks whether a token is expired or not. If the token is expired,
        /// but is still within the refresh period, the token will be regenerated. TODO: CHECK REGENERATION
        /// </summary>
        /// <param name="payload">The JSON data that was encrypted inside the token.</param>
        /// <returns>Whether the JWT is expired and cannot be refreshed.</returns>
        public static bool TokenIsExpired(Dictionary<string, string> payload)
        {
            return false;
        }

        /// <summary>
        /// Determines whether a user has the permissions to perform a certain operarion.
        /// </summary>
        /// <param name="userRole">The role of the current user.</param>
        /// <param name="operation">The operation the user is trying to access.</param>
        /// <returns>Whether the user can perform the operation.</returns>
        public static bool HasPermission(string userRole, string operation)
        {
            return true;
        }

        /// <summary>
        /// Generates a JWT using a private key loaded from the environment.
        /// </summary>
        /// <param name="payload">The data to be encrypted.</param>
        /// <returns>The JWT.</returns>
        public static string GenerateJWT(Dictionary<string, string> payload)
        {
            return "";
        }
    }
}
