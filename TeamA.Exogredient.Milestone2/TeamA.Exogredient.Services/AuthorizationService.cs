using System;
using System.Text;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

// TODO USE SECURITY SERVICE FOR HASHING

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
        private const string EXPIRATION_FIELD = "exp";
        private const string PUBLIC_KEY_FIELD = "pk";

        private const string ENV_PRIVATE_KEY = "AUTHORIZATION_PRIVATE_KEY";
        private const string ENV_PUBLIC_KEY = "AUTHORIZATION_PUBLIC_KEY";
        private static readonly string PRIVATE_KEY = Environment.GetEnvironmentVariable(ENV_PRIVATE_KEY, EnvironmentVariableTarget.Process);
        private static readonly string PUBLIC_KEY = Environment.GetEnvironmentVariable(ENV_PUBLIC_KEY, EnvironmentVariableTarget.Process);

        private static readonly UserDAO _userDAO;

        public enum USER_TYPE {
            UNREGISTERED    = 0,
            REGISTERED      = 1,
            STORE_OWNER     = 2,
            ADMIN           = 3,
            SYS_ADMIN       = 4,
        };

        static AuthorizationService()
        {
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
            // Make sure we have the proper parameters inside the dictionary
            if (!payload.ContainsKey("userType") || !payload.ContainsKey("id"))
                // TODO THROW PROPER EXCEPTION
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
                payload.Add(EXPIRATION_FIELD, Get20MinFromNow().ToString());
            }

            // Add the public key to the payload
            payload.Add(PUBLIC_KEY_FIELD, PUBLIC_KEY);

            // Base64 encode the header and payload
            string encodedHeader = DictionaryToString(joseHeader).ToBase64();
            string encodedPayload = DictionaryToString(payload).ToBase64();

            // The signature will be the hash of the header and payload
            string stringToSign = encodedHeader + '.' + encodedPayload;

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(PRIVATE_KEY);

            // This object will let us create a signature
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            RSAFormatter.SetHashAlgorithm("SHA1");  // We care more about speed here, so we use SHA1
            SHA1Managed SHhash = new SHA1Managed();

            // Hash the encoded values using RSA512
            byte[] hashedString = SHhash.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            // Sign the hash with the private key
            string signature = RSAFormatter.CreateSignature(hashedString).ToBase64();

            // Release resources
            SHhash.Dispose();

            return string.Format("{0}.{1}.{2}", encodedHeader, encodedPayload, signature);
        }

        /// <summary>
        /// Create a token for a logged-in user.
        /// </summary>
        /// <param name="userName"> logged-in username </param>
        /// <returns> string of token that represents the user type and unique ID of the username </returns>
        public static async Task<string> CreateTokenAsync(string username)
        {
            UserRecord user = (UserRecord)await _userDAO.ReadByIdAsync(username);

            // Get the user type of the username.
            string userType = user.UserType;

            // Craete a dictionary that represents the user type and unique ID.
            Dictionary<string, string> userInfo = new Dictionary<string, string>()
            {
                {"userType", userType},
                {"id", username }
            };

            return GenerateJWS(userInfo);
        }

        /// <summary>
        /// Decrypts a JWS in order to see the encrypted payload.
        /// </summary>
        /// <param name="token">The token to decrypt.</param>
        /// <returns>The contents inside the decrypted token.</returns>
        public static Dictionary<string, string> DecryptJWS(string token)
        {
            string[] segments = token.Split('.');

            // Make sure we have the proper JWS format of 3 tokens delimited by periods
            if (segments.Length != 3)
                throw new ArgumentException("JWS must have 3 segments separated by periods.");

            string encodedHeader = segments[0];
            string encodedPayload = segments[1];
            string encodedSignature = segments[2];

            // Convert header back to dictionary format
            string decodedHeader = segments[0].FromBase64();
            Dictionary<string, string> headerJSON = StringToDictionary(decodedHeader);

            // Convert payload back to dictionary format
            string decodedPayload = segments[1].FromBase64();
            Dictionary<string, string> payloadJSON = StringToDictionary(decodedPayload);

            // Make sure that we are using the correct encryption algorithm in the header
            if (headerJSON["alg"] != SIGNING_ALGORITHM)
                // TODO THROW PROPER EXCEPTION
                throw new ArgumentException("Incorrect encryption algorithm.");

            if (!payloadJSON.ContainsKey(PUBLIC_KEY_FIELD))
                // TODO THROW PROPER EXCEPTION
                throw new ArgumentException("Public key not found in the JWS payload!");

            string publicKey = payloadJSON[PUBLIC_KEY_FIELD];
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(publicKey);

            // Create this object in order to verify that the JWS was untampered with
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            RSADeformatter.SetHashAlgorithm("SHA1");
            SHA1Managed SHhash = new SHA1Managed();

            // Sign the hash with the private key
            string strToVerify = encodedHeader + '.' + encodedPayload;
            // Hash the encoded values using RSA512
            byte[] hashedString = SHhash.ComputeHash(Encoding.UTF8.GetBytes(strToVerify));

            // Release resources
            SHhash.Dispose();

            // Verify that the JWS is correct and untampered with
            if (RSADeformatter.VerifySignature(hashedString, System.Convert.FromBase64String(encodedSignature)))
            {
                return payloadJSON;
            }
            else
            {
                throw new ArgumentException("JWS could not be verified!");
            }
        }

        /// <summary>
        /// Refreshes a token to be active for 20 more minutes.
        /// </summary>
        /// <param name="jws">The token that needs to be refreshed.</param>
        /// <returns>A new token that has been refreshed and active for 20 more minutes.</returns>
        public static string RefreshJWS(string jws)
        {
            Dictionary<string, string> payload = DecryptJWS(jws);

            // Refresh the token for an additional 20 minutes
            payload[EXPIRATION_FIELD] = Get20MinFromNow().ToString();

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

            // Make sure we are dealing with number first
            if (!isNumeric)
                throw new ArgumentException("Expiration time is not a number!");

            return GetEpochTime() > expTime;
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
            else
            {
                // Remove the first and last brackets
                dictStr = dictStr.Remove(0, 1)
                                 .Remove(dictStr.Length - 1, 1);
            }

            // String should look like this now:
            // "key1:value1,key2:value2"

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

                // TODO CHECK IF "\"" == '"'
                // Check for condition (3)
                bool keyHasQuotes = key.Length > 2 || key[0] == '"' || key[key.Length - 1] == '"';
                bool valHasQuotes = val.Length > 2 || val[0] == '"' || val[val.Length - 1] == '"';

                if (!keyHasQuotes || !valHasQuotes)
                {
                    throw new ArgumentException("Key or value isn't surrounded by double quotes.");
                }

                // Check for condition (4)
                if (!p[0].IsAlphaNumeric() || !p[1].IsAlphaNumeric())
                {
                    throw new ArgumentException("Key or value is not alpha-numeric (excluding white-space).");
                }

                dict.Add(key, val);
            }

            return dict;
        }

        /// <summary>
        /// Gets the current epoch time.
        /// </summary>
        /// <returns>A long representing the current epoch time.</returns>
        private static long GetEpochTime()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Gets the UTC epoch time 20 minutes from when it is called.
        /// </summary>
        /// <returns>Epoch time representing 20 minutes from now.</returns>
        private static long Get20MinFromNow()
        {
            DateTime curTime = DateTime.UtcNow;
            return ((DateTimeOffset)curTime.AddMinutes(20)).ToUnixTimeSeconds();
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
        private static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Extension function to convert a bytes array to a Base64 encoding.
        /// </summary>
        /// <param name="bytes">The bytes array to be converted.</param>
        /// <returns>A Base64 representation of the bytes array.</returns>
        private static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a Base64 encoded string back to it's original format.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>The original representation of the string.</returns>
        private static string FromBase64(this string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
