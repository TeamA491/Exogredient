﻿using System;
using System.Text;
using System.Collections.Generic;

// TODO: DECIDE HOW TO HOLD EXPIRATION DATES
// TODO: FUNCTION TO CHECK WHETHER WE CAN REFRESH A TOKEN OR NOT
// TODO: SPLIT THIS FUNCTIONALITY FROM THE TokenIsExpired FUNCTION
// TODO: MAAAYBE OVERLOAD FUNCTIONS TO TAKE A JWS TOO. DECIDE WHAT FUNCTIONS GET TO DO THAT.
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
        // TODO: LOAD RSA PRIVATE KEY FROM OS ENVIRONMENT
        private const string SIGNING_ALGORITHM = "RS512";
        private static readonly string PRIVATE_KEY = "TESTING_PRIVATE_RSA_KEY";

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
            // Create the header and convert it to a Base64 string
            Dictionary<string, string> joseHeader = new Dictionary<string, string>{
                { "typ", "JWT" },  // Media type
                { "alg", SIGNING_ALGORITHM }  // Signing algorithm type
            };

            // Base64 encode the header and payload
            string encodedHeader = DictionaryToString(joseHeader).ToBase64();
            string encodedPayload = DictionaryToString(payload).ToBase64();

            // Hash the encoded values using RSA512


            return "";
        }

        /// <summary>
        /// Decrypts a JWS in order to see the encrypted payload.
        /// </summary>
        /// <param name="token">The token to decrypt.</param>
        /// <param name="publicKey">The public key used to decrypt and verify the token.</param>
        /// <returns>The contents inside the decrypted token.</returns>
        public static Dictionary<string, string> DecryptJWS(string token, string publicKey)
        {
            // NOTE: MAKE SURE TO CHECK PERIODS
            // NOTE: CHECK TO MAKE SURE IN BASE64
            // NOTE: CHECK HEADER ALGORITHM, DENY EVERYTHING OTHER THAN `SIGNING_ALGORITHM`

            // NOTE:
            // There were two ways to attack a standards-compliant JWS library to achieve trivial token forgery:
            // 1. Send a header that specifies the "none" algorithm be used
            // 2. Send a header that specifies the "HS256" algorithm when the application normally signs messages with an RSA public key.

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Checks whether a token is expired or not. If the token is expired,
        /// but is still within the refresh period, the token will be regenerated. TODO: CHECK REGENERATION
        /// </summary>
        /// <param name="payload">The JSON data that was encrypted inside the token.</param>
        /// <returns>Whether the JWS is expired and cannot be refreshed.</returns>
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
                // TODO THROW EXCEPTION HERE
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
                // TODO THROW EXCEPTION HERE
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
                    // TODO THROW EXCEPTION 
                }

                string key = p[0];
                string val = p[1];

                // TODO CHECK IF "\"" == '"'
                // Check for condition (3)
                bool keyHasQuotes = key.Length > 2 || key[0] == '"' || key[key.Length - 1] == '"';
                bool valHasQuotes = val.Length > 2 || val[0] == '"' || val[val.Length - 1] == '"';

                if (!keyHasQuotes || !valHasQuotes)
                {
                    // TODO THROW EXCEPTION
                }

                // Check for condition (4)
                if (!p[0].IsAlphaNumeric() || !p[1].IsAlphaNumeric())
                {
                    // TODO THROW EXCEPTION
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
        /// <returns>A Base64 representation of the string</returns>
        private static string ToBase64(this string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
    }
}
