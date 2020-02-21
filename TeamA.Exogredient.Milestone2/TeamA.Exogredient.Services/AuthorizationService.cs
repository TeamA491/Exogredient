using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TeamA.Exogredient.AppConstants;

// NOTE JWT TOKEN MUST BE IN THE AUTHORIZATION HEADER FOR EACH REQUEST
namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// The AuthorizationService will authorize users through a
    /// JSON Web Token (JWT). Authorization is based on a role system,
    /// where each user is assigned a role. Each role has access to
    /// only certain operations. The JWT is encrypted and decrypted using the RSA
    /// algorithm.
    /// </summary>
    public class AuthorizationService
    {
        private readonly byte[] keyPair;

        public AuthorizationService()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                // Holds both the private key and public key
                keyPair = rsa.ExportCspBlob(true);
            }
        }

        /// <summary>
        /// Generates a JWT with RSA512 using a private key loaded from the environment.
        /// </summary>
        /// <remarks>
        /// A JWT is compromised of 3 dot-separated, base64 strings.
        /// Jose_Header.JWT_Payload.JWT_Signature
        /// </remarks>
        /// <param name="payload">The data to be encrypted.</param>
        /// <returns>The JWT.</returns>
        public string GenerateJWT(Dictionary<string, string> payload)
        {
            // Make sure we have the proper parameters inside the dictionary
            if (!payload.ContainsKey(Constants.UserTypeKey) || !payload.ContainsKey(Constants.IdKey))
                throw new ArgumentException("UserType or ID was not provided.");

            // Create the header and convert it to a Base64 string
            Dictionary<string, string> joseHeader = new Dictionary<string, string>{
                { Constants.MediaType, Constants.MediaJWT },  // Media type
                { Constants.SigningAlgKey, Constants.SIGNING_ALGORITHM }  // Signing algorithm type
            };

            // If the expiration date wasn't already specified, then create one
            if (!payload.ContainsKey(Constants.EXPIRATION_FIELD))
            {
                payload.Add(Constants.EXPIRATION_FIELD, TimeUtilityService.GetEpochFromNow().ToString());
            }

            // Base64 encode the header and payload
            string encodedHeader = StringUtilityService.DictionaryToString(joseHeader).ToBase64URL();
            string encodedPayload = StringUtilityService.DictionaryToString(payload).ToBase64URL();

            // The signature will be the hash of the header and payload
            string stringToSign = encodedHeader + '.' + encodedPayload;

            // Create the signature
            string signature = GetPKCSSignature(stringToSign).ToBase64URL();

            return string.Format("{0}.{1}.{2}", encodedHeader, encodedPayload, signature);
        }

        /// <summary>
        /// Generates the signature, for the third part of the JWT token.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <returns></returns>
        public string GetPKCSSignature(string data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Hash the data
                byte[] hash = SecurityService.HashWithSHA512AsBytes(data);

                // Set the private key
                rsa.ImportCspBlob(keyPair);

                // Prepare to sign the hash
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                RSAFormatter.SetHashAlgorithm(Constants.HASHING_ALGORITHM);

                // Create the signature from the hash
                byte[] signedHash = RSAFormatter.CreateSignature(hash);

                return signedHash.FromBytes();
            }
        }

        /// <summary>
        /// Decrypts a JWT in order to verify it and see the payload.
        /// </summary>
        /// <param name="jwt">The token to decrypt.</param>
        /// <returns>The contents inside the token.</returns>
        public Dictionary<string, string> DecryptJWT(string jwt)
        {
            string[] segments = jwt.Split('.');

            // Make sure we have the proper JWT format of 3 tokens delimited by periods
            if (segments.Length != 3)
                throw new InvalidTokenException("JWT must have 3 segments separated by periods.");

            string encodedHeader = segments[0];
            string encodedPayload = segments[1];

            // Convert header back to dictionary format
            string decodedHeader = encodedHeader.FromBase64URL();
            Dictionary<string, string> headerJSON = StringUtilityService.StringToDictionary(decodedHeader);

            // Convert payload back to dictionary format
            string decodedPayload = encodedPayload.FromBase64URL();
            Dictionary<string, string> payloadJSON = StringUtilityService.StringToDictionary(decodedPayload);

            // Make sure that we are using the correct encryption algorithm in the header
            if (headerJSON[Constants.SigningAlgKey] != Constants.SIGNING_ALGORITHM)
                throw new InvalidTokenException("Incorrect encryption algorithm.");

            string strToVerify = encodedHeader + '.' + encodedPayload;

            // Make sure the signature is correct
            if (VerifyPKCSSignature(strToVerify))
                return payloadJSON;
            else
                throw new InvalidTokenException("JWT could not be verified!");
        }

        /// <summary>
        /// Verifies that a JWT signature is correct and untampered with.
        /// </summary>
        /// <param name="data">The signature to check.</param>
        /// <returns></returns>
        public bool VerifyPKCSSignature(string data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Hash the data
                byte[] hash = SecurityService.HashWithSHA512AsBytes(data);

                // Set the public key
                rsa.ImportCspBlob(keyPair);

                // Prepare to sign the hash
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                RSAFormatter.SetHashAlgorithm(Constants.HASHING_ALGORITHM);

                // Create the signature from the hash
                byte[] signedHash = RSAFormatter.CreateSignature(hash);

                // Prepare to verify the signed hash
                RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                RSADeformatter.SetHashAlgorithm(Constants.HASHING_ALGORITHM);

                return RSADeformatter.VerifySignature(hash, signedHash);
            }
        }

        /// <summary>
        /// Determines whether a user has the permissions to perform a certain operarion.
        /// </summary>
        /// <param name="userRole">The role of the current user.</param>
        /// <param name="operation">The operation the user is trying to access.</param>
        /// <returns>Whether the user can perform the operation.</returns>
        public bool UserHasPermissionForOperation(int userRole, string operation)
        {
            // Make sure the operation exists
            if (!Constants.UserOperations.ContainsKey(operation))
                return false;

            // Make sure the user type exists in the enum
            if (!Enum.IsDefined(typeof(Constants.USER_TYPE), userRole))
                return false;

            // Check if the operation requires a higher user role
            if (Constants.UserOperations[operation] > userRole)
                return false;

            return true;
        }
    }

    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() { }

        public InvalidTokenException(string message)
            : base(message) { }

        public InvalidTokenException(string message, Exception inner)
            : base(message, inner) { }
    }
}
