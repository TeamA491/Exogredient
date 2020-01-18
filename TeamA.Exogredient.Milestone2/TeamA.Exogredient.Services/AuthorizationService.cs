using System;
using System.Text;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

// NOTE JWS TOKEN MUST BE IN THE AUTHORIZATION HEADER FOR EACH REQUEST
namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// The AuthorizationService will authorize users through a
    /// JSON Web Signature (JWS). Authorization is based on a role system,
    /// where each user is assigned a role. Each role has access to
    /// only certain operations. The JWS is encrypted and decrypted using the RSA
    /// algorithm.
    /// </summary>
    public static class AuthorizationService
    {
        private const string SIGNING_ALGORITHM = "RS512";
        private const string HASHING_ALGORITHM = "SHA512";
        private const string EXPIRATION_FIELD = "exp";
        private const string PUBLIC_KEY_FIELD = "pk";

        private static readonly byte[] keyPair;
        private static readonly UserDAO _userDAO;

        static AuthorizationService()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                // Holds both the private key and public key
                keyPair = rsa.ExportCspBlob(true);
            }

            _userDAO = new UserDAO();
        }

        /// <summary>
        /// Generates a JWS with RSA512 using a private key loaded from the environment.
        /// </summary>
        /// <remarks>
        /// A JWS is compromised of 3 dot-separated, base64 strings.
        /// Jose_Header.JWS_Payload.JWS_Signature
        /// </remarks>
        /// <param name="payload">The data to be encrypted.</param>
        /// <returns>The JWS.</returns>
        public static string GenerateJWS(Dictionary<string, string> payload)
        {
            // TODO CHECK PUBLIC KEY AND PRIVATE KEY, CHECK THEIR LENGTHS AND IF THEY INCLUDE ----BEGIN... ---END... ETC

            // Make sure we have the proper parameters inside the dictionary
            if (!payload.ContainsKey(Constants.UserTypeKey) || !payload.ContainsKey(Constants.IdKey))
                throw new ArgumentException("UserType or ID was not provided.");

            // Create the header and convert it to a Base64 string
            Dictionary<string, string> joseHeader = new Dictionary<string, string>{
                { "typ", "JWT" },  // Media type
                { "alg", SIGNING_ALGORITHM }  // Signing algorithm type
            };

            // If the expiration date wasn't already specified, then create one
            if (!payload.ContainsKey(EXPIRATION_FIELD))
            {
                // Add a 20 min expiration
                payload.Add(EXPIRATION_FIELD, UtilityService.GetEpochFromNow().ToString());
            }

            // Base64 encode the header and payload
            string encodedHeader = DictionaryToString(joseHeader).ToBase64URL();
            string encodedPayload = DictionaryToString(payload).ToBase64URL();

            // The signature will be the hash of the header and payload
            string stringToSign = encodedHeader + '.' + encodedPayload;

            // Create the signature
            string signature = GetPKCSSignature(stringToSign).ToBase64URL();

            return string.Format("{0}.{1}.{2}", encodedHeader, encodedPayload, signature);
        }

        /// <summary>
        /// Generates the signature, for the third part of the JWS token.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="privateKey">The private key to sign it with.</param>
        /// <returns></returns>
        public static string GetPKCSSignature(string data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Hash the data
                byte[] hash;
                using (SHA512 sha256 = SHA512.Create())
                {
                    hash = sha256.ComputeHash(data.ToBytes());
                }

                // Set the private key
                rsa.ImportCspBlob(keyPair);

                // Prepare to sign the hash
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                RSAFormatter.SetHashAlgorithm(HASHING_ALGORITHM);

                // Create the signature from the hash
                byte[] signedHash = RSAFormatter.CreateSignature(hash);

                return signedHash.FromBytes();
            }
        }

        /// <summary>
        /// Create a token for a logged-in user.
        /// </summary>
        /// <param name="username"> logged-in username </param>
        /// <returns> string of token that represents the user type and unique ID of the username </returns>
        public static async Task<string> CreateTokenAsync(string username)
        {
            UserObject rawUser = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = (UserObject)await maskingService.UnMaskAsync(rawUser).ConfigureAwait(false);

            // Get the user type of the username.
            string userType = user.UserType;

            // Craete a dictionary that represents the user type and unique ID.
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {Constants.UserTypeKey, userType},
                {Constants.IdKey, username }
            };

            return GenerateJWS(userInfo);
        }

        /// <summary>
        /// Decrypts a JWS in order to see the encrypted payload.
        /// </summary>
        /// <param name="jws">The token to decrypt.</param>
        /// <returns>The contents inside the decrypted token.</returns>
        public static Dictionary<string, string> DecryptJWS(string jws)
        {
            string[] segments = jws.Split('.');

            // Make sure we have the proper JWS format of 3 tokens delimited by periods
            if (segments.Length != 3)
                throw new InvalidTokenException("JWS must have 3 segments separated by periods.");

            string encodedHeader = segments[0];
            string encodedPayload = segments[1];

            // Convert header back to dictionary format
            string decodedHeader = segments[0].FromBase64URL();
            Dictionary<string, string> headerJSON = StringToDictionary(decodedHeader);

            // Convert payload back to dictionary format
            string decodedPayload = segments[1].FromBase64URL();
            Dictionary<string, string> payloadJSON = StringToDictionary(decodedPayload);

            // Make sure that we are using the correct encryption algorithm in the header
            if (headerJSON["alg"] != SIGNING_ALGORITHM)
                throw new InvalidTokenException("Incorrect encryption algorithm.");

            string strToVerify = encodedHeader + '.' + encodedPayload;

            // Make sure the signature is correct
            if (VerifyPKCSSignature(strToVerify))
                return payloadJSON;
            else
                throw new InvalidTokenException("JWS could not be verified!");
        }

        /// <summary>
        /// Verifies that a JWS signature is correct and untampered with.
        /// </summary>
        /// <param name="data">The signature to check.</param>
        /// <param name="publicKey">The public key to use.</param>
        /// <returns></returns>
        public static bool VerifyPKCSSignature(string data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Hash the data
                byte[] hash;
                using (SHA512 sha256 = SHA512.Create())
                {
                    hash = sha256.ComputeHash(data.ToBytes());
                }

                // Set the public key
                rsa.ImportCspBlob(keyPair);

                // Prepare to sign the hash
                RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
                RSAFormatter.SetHashAlgorithm(HASHING_ALGORITHM);

                // Create the signature from the hash
                byte[] signedHash = RSAFormatter.CreateSignature(hash);

                // Prepare to verify the signed hash
                RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                RSADeformatter.SetHashAlgorithm(HASHING_ALGORITHM);

                return RSADeformatter.VerifySignature(hash, signedHash);
            }
        }

        /// <summary>
        /// Refreshes a token to be active for 20 more minutes.
        /// </summary>
        /// <param name="jws">The token that needs to be refreshed.</param>
        /// <param name="publicKey">An optional public key to be used instead of the one
        /// loaded from the environment. The public key will be sent along with the token payload.</param>
        /// <param name="privateKey">An optional private key to be used instead of the one
        /// loaded from the environment.</param>
        /// <returns>A new token that has been refreshed and active for 20 more minutes.</returns>
        public static string RefreshJWS(string jws, int minutes = Constants.TOKEN_EXPIRATION_MIN)
        {
            Dictionary<string, string> payload = DecryptJWS(jws);

            // Refresh the token for an additional 20 minutes
            payload[EXPIRATION_FIELD] = UtilityService.GetEpochFromNow(minutes).ToString();

            return GenerateJWS(payload);
        }

        /// <summary>
        /// Checks whether the current token is expired or not.
        /// </summary>
        /// <param name="jws">The token to check.</param>
        /// <returns>Whether the current token is past it's 20 minute lifetime.</returns>
        public static bool TokenIsExpired(string jws)
        {
            Dictionary<string, string> payload = DecryptJWS(jws);

            // Check if the expiration key exists first
            if (!payload.ContainsKey(EXPIRATION_FIELD))
                throw new ArgumentException("Expiration time is not specified!");

            long expTime;
            bool isNumeric = long.TryParse(payload[EXPIRATION_FIELD], out expTime);

            // Make sure we are dealing with a number first
            if (!isNumeric)
                throw new ArgumentException("Expiration time is not a number!");

            return UtilityService.CurrentUnixTime() > expTime;
        }

        /// <summary>
        /// Determines whether a user has the permissions to perform a certain operarion.
        /// </summary>
        /// <param name="userRole">The role of the current user.</param>
        /// <param name="operation">The operation the user is trying to access.</param>
        /// <returns>Whether the user can perform the operation.</returns>
        public static bool UserHasPermissionForOperation(int userRole, string operation)
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

        /// <summary>
        /// Converts a dictionary to a JSON string representation.
        /// </summary>
        /// <remarks>
        /// All string values are converted to be Alpha-Numeric. This is the avoid
        /// errors and security issues with serializing a dictionary.
        /// </remarks>
        /// <param name="dict">The dictionary to represent as a string.</param>
        /// <returns>A JSON string representation of the dictionary.</returns>
        private static string DictionaryToString(Dictionary<string, string> dict)
        {
            List<string> body = new List<string>();  // Holds all the key value pairs in string representation
     
            // Construct a List of serialized key/value pairs
            foreach (KeyValuePair<string, string> entry in dict)
            {
                // Create a string representation of the key/value pair
                string serializedKeyVal = string.Format("\"{0}\":\"{1}\"",
                                                        entry.Key.ToAlphaNumeric(),
                                                        entry.Value.ToAlphaNumeric());

                body.Add(serializedKeyVal);
            }

            // Construct the final JSON string in valid form
            return "{" + string.Join(",", body) + "}";
        }

        /// <summary>
        /// Converts a JSON string to a dictionary.
        /// </summary>
        /// <param name="dictStr">The string to parse into a dictionary.</param>
        /// <returns>Dictionary representation of the string.</returns>
        private static Dictionary<string, string> StringToDictionary(string dictStr)
        {
            // The passed in string is assumed to be in proper JSON format, with each
            // key/value pair being alpha-numeric
            // Example: "{\"key1\":\"value1\",\"key2\":\"value2\"}"
            // .... Defining a proper format for the string:
            // ........ (1) Has correct surrounding brackets.
            // ........ (2) Correct comma count and placements.
            // ........ (3) Each key and value have double quotes around them.
            // ........ (4) Each key and value are alpha-numeric.
            // If all conditions are not true, an error will be thrown.

            // Check for condition (1)
            if (dictStr.Length < 2 || dictStr[0] != '{' || dictStr[dictStr.Length - 1] != '}')
            {
                throw new ArgumentException("Dictionary doesn't have proper surrounding brackets.");
            }

            // Remove the first and last brackets
            dictStr = dictStr.Remove(0, 1);
            dictStr = dictStr.Remove(dictStr.Length - 1, 1);

            // String should look like this now:
            // "\"key1\":\"value1\",\"key2\":\"value2\""

            // Count the commas and colons in the string...
            int commaCount = 0, colonCount = 0;
            foreach (char c in dictStr)
            {
                if (c == ',') commaCount++;
                if (c == ':') colonCount++;
            }

            // Check for condition (2)
            // For every comma, there are 2 key/value pairs...
            // For every key/value pair, there is 1 colon to separate the key and value
            // So that means ((colonCount - 1) = commaCount) in a valid JSON string
            // NOTE: If a comma or colon appears in the key or value, then it violates condition (4)
            if (colonCount - 1 != commaCount)
            {
                throw new ArgumentException("Invalid comma and / or colon formatting.");
            }

            // Determine key/value pairs and their correctness
            string[] pairs = dictStr.Split(',');
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (string pair in pairs)
            {
                string[] p = pair.Split(':');
                // If we don't have a key and value pair, then it's not correct
                if (p.Length != 2)
                {
                    throw new ArgumentException("Invalid key/value pair.");
                }

                string key = p[0];
                string val = p[1];

                // Check for condition (3)
                bool keyHasQuotes = key.Length > 2 && key[0] == '"' && key[key.Length - 1] == '"';
                bool valHasQuotes = val.Length > 2 && val[0] == '"' && val[val.Length - 1] == '"';

                if (!keyHasQuotes || !valHasQuotes)
                {
                    throw new ArgumentException("Key or value isn't surrounded by double quotes.");
                }

                // Remove the double quote at the beginning
                key = key.Remove(0, 1);
                val = val.Remove(0, 1);

                // Remove the double quote at the end
                key = key.Remove(key.Length - 1);
                val = val.Remove(val.Length - 1);

                // Check for condition (4)
                if (!(key.IsAlphaNumeric() && val.IsAlphaNumeric()))
                {
                    throw new ArgumentException("Key or value is not alpha-numeric (excluding white-space).");
                }

                dict.Add(key, val);
            }

            return dict;
        }

        /// <summary>
        /// Extension function to convert a string to alpha numeric.
        /// </summary>
        /// <remarks>
        /// This implementation ignores whitespace characters.
        /// </remarks>
        /// <param name="str">The string to be converted.</param>
        /// <returns>An alpha numeric representation</returns>
        private static string ToAlphaNumeric(this string str)
        {
            if (str == null)
                return "";

            // Traverse the string backwards, because when we delete
            // characters from the string going forward, there will be an offset
            // and the for loop will eventually go out of bounds of the string
            for (int i = str.Length - 1; i >= 0; i--)
            {
                bool isCharacter = char.IsLetter(str[i]);
                bool isNumber = char.IsNumber(str[i]);

                // Check if the char is a valid character, valid number, or space
                // and delete the character if not
                if (!isCharacter && !isNumber && str[i] != ' ')
                    str = str.Remove(i, 1);
            }

            return str;
        }

        /// <summary>
        /// Determines whether a string is alpha-numeric or not.
        /// </summary>
        /// <remarks>
        /// This implementation ignores whitespace characters.
        /// </remarks>
        /// <param name="str">The string to check.</param>
        /// <returns>Whether the string is alpha-numeric or not.</returns>
        private static bool IsAlphaNumeric(this string str)
        {
            if (str == null)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                bool isCharacter = char.IsLetter(str[i]);
                bool isNumber = char.IsNumber(str[i]);

                // Check if the char is a valid character, valid number, or space
                if (!isCharacter && !isNumber && str[i] != ' ')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Extension function to convert a string to a Base64 encoding.
        /// </summary>
        /// <param name="str">The string to be converted.</param>
        /// <returns>A Base64 representation of the string.</returns>
        public static string ToBase64URL(this string str)
        {
            string b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            // Make sure it's URL safe
            b64 = b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return b64;
        }

        /// <summary>
        /// Extension function to convert a bytes array to a Base64 encoding.
        /// </summary>
        /// <param name="bytes">The bytes array to be converted.</param>
        /// <returns>A Base64 representation of the bytes array.</returns>
        public static string ToBase64URL(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).ToBase64URL();
        }

        /// <summary>
        /// Converts a Base64 encoded string back to it's original format.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>The original representation of the string.</returns>
        private static string FromBase64URL(this string str)
        {
            string oldStr = str.Replace('_', '/').Replace('-', '+');

            // Base64URL encoded string must be a multiple of 4
            // otherwise it's missing padding at the end
            int missingPadding = str.Length % 4;
            if (missingPadding == 1)
                oldStr += "===";        // Missing 3 characters
            else if (missingPadding == 2)
                oldStr += "==";         // Missing 2 characters
            else if (missingPadding == 3)
                oldStr += "=";          // Missing 1 character

            return Convert.FromBase64String(oldStr).FromBytes();
        }

        public static byte[] ToBytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static string FromBytes(this byte[] b)
        {
            return Encoding.UTF8.GetString(b);
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
