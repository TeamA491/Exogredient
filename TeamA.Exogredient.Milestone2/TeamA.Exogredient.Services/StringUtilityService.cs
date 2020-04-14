using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using Snowball;
using WeCantSpell.Hunspell;
using System.Linq;

namespace TeamA.Exogredient.Services
{
    public static class StringUtilityService
    {
        
        public static string NormalizeTerm(string term, string dicFilePath, string affFilePath)
        {
            if(term == null)
            {
                return null;
            }

            term = term.ToLower();
            var tokens = term.Split(Constants.termSpliters);
            var normalizedTerm = new List<string>();

            foreach(var token in tokens)
            {
                if(!CheckIfTermInDictionary(token,dicFilePath,affFilePath))
                {
                    normalizedTerm.Add(token);
                }
                else
                {
                    var stemmedTerm = Stem(token);
                    if(stemmedTerm.Length == term.Length)
                    {
                        normalizedTerm.Add(stemmedTerm);
                    }
                    else
                    {
                        normalizedTerm.Add(AutoCorrect(stemmedTerm, dicFilePath, affFilePath));
                    }
                }
            }
            return string.Join(Constants.termJoiner, normalizedTerm.ToArray());
        }

        public static bool CheckIfTermInDictionary(string term, string dicFilePath, string affFilePath)
        {
            var dictionary = WordList.CreateFromFiles(dicFilePath, affFilePath);
            return dictionary.Check(term);
        }

        public static string AutoCorrect(string word, string dicFilePath, string affFilePath)
        {
            var dictionary = WordList.CreateFromFiles(dicFilePath, affFilePath);
            var suggestions = dictionary.Suggest(word).ToArray<string>();

            foreach (var suggestion in suggestions)
            {
                if (suggestion.Length > word.Length)
                {
                    return suggestion;
                }
            }
            return word;
        }
        

        public static string Stem(string word)
        {
            var stemmer = new EnglishStemmer();

            stemmer.Current = word;
            stemmer.Stem();
            return stemmer.Current;
        }

        /// <summary>
        /// Convert a hex string to a byte array
        /// </summary>
        /// <param name="hexString"> hex string to be converted </param>
        /// <returns> byte array of the hex string </returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            // TODO VALIDATE INPUT

            // The length of the byte array of the hex string is hexString.Length / 2
            byte[] bytes = new byte[hexString.Length / 2];
            char[] charArray = hexString.ToCharArray();

            // for index i in the byte array
            for (int i = 0; i < bytes.Length; i++)
            {
                // Create a string of two characters at i*2 and i*2+1 index of hexString
                string temp = "" + charArray[i * 2] + charArray[i * 2 + 1];
                // Convert the hex string to a byte and store at i index of the byte array
                bytes[i] = Convert.ToByte(temp, Constants.HexBaseValue);
            }

            return bytes;
        }

        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">The bytes to convert (byte[])</param>
        /// <returns> hex string of the byte array </returns>
        public static string BytesToHexString(byte[] bytes)
        {
            // Convert the bytes to hex string without "-"
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// Convert a byte array to a UTF8 string.
        /// </summary>
        /// <param name="bytes">The bytes to convert (byte[])</param>
        /// <returns>The utf-8 string (string)</returns>
        public static string BytesToUTF8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Convert a string to a hex string using ASCII encoding.
        /// </summary>
        /// <param name="s"> string to be converted </param>
        /// <returns> hex string of the string </returns>
        public static string ToHexString(string s)
        {
            // TODO VALIDATE INPUT HERE

            // Convert the string into a ASCII byte array
            byte[] bytes = Encoding.ASCII.GetBytes(s);

            // Convert the byte array to hex string
            return BytesToHexString(bytes);
        }

        /// <summary>
        /// Check whether a given string meets the requirement for length.
        /// </summary>
        /// <param name="name">The string the user wants to check length of.</param>
        /// <param name="length">The length that the string must be equal to.</param>
        /// <param name="min">A optional parameter. If this is set then, name's length can be a
        /// range from min to length (inclusive).</param>
        /// <returns>Returns value of bool to represent whether the name met the required constraints.</returns>
        public static bool CheckLength(string name, int length, int min = -1)
        {
            // If the min was left default, compare the length literaly.
            if (min == -1)
            {
                return name.Length == length;
            }
            else
            {
                // Otherwise, treat the length as the max value of a range.
                return name.Length >= min && name.Length <= length;
            }
        }

        /// <summary>
        /// Check whether a given string contains only characters represented in the data.
        /// </summary>
        /// <param name="input">The string that we are checking.</param>
        /// <returns>Returns value of bool to represent whether all the characters
        /// in name meet the specification.</returns>
        public static bool CheckCharacters(string input, List<char> data)
        {
            bool result = true;

            // Convert to lower, check whether the dat contains the character, and AND it to the result (1 false will make the result false).
            foreach (char c in input.ToLower())
            {
                result = result && data.Contains(c);
            }

            return result;
        }

        /// <summary>
        /// Check whether the email is in a valid format (minimally: contains an @ with text on
        /// either side, and that text does not contain "..").
        /// </summary>
        /// <param name="email">The email we are checking</param>
        /// <returns>Returns a bool representing whether the email satisfies
        /// the specifications.</returns>
        public static bool CheckEmailFormatValidity(string email)
        {
            string[] splitResult = email.Split('@');

            // If the email contained an @, continue.
            if (splitResult.Length == 2)
            {
                string first = splitResult[0];
                string second = splitResult[1];

                // If either side of the @ had at least 1 character, continue.
                if (first.Length >= 1 && second.Length >= 1)
                {
                    // If .. is containd on other side return false, otherwise return true.
                    if (first.Contains("..") || second.Contains(".."))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // TODO WHY ONLY CHECKING FOR GMAIL
        /// <summary>
        /// Removes the superfluous elements of an email so it can be properly checked for uniqueness.
        /// </summary>
        /// <param name="email">The email we are checking.</param>
        /// <returns>Returns value of string to represent the
        /// canonicalized email.</returns>
        public static string CanonicalizeEmail(string email)
        {
            // Split the email into the username and domain parts.
            string[] splitResult = email.Split('@');
            string username = splitResult[0].ToLower();
            string domain = splitResult[1].ToLower();

            string transposedUsername = username;

            if (domain.Equals(Constants.GmailHost))
            {
                // Remove dots.
                transposedUsername = transposedUsername.Replace(".", "");

                // Remove plus and everything after.
                int plusIndex = transposedUsername.IndexOf("+");
                if (plusIndex != -1)
                {
                    transposedUsername = transposedUsername.Remove(plusIndex);
                }
            }

            // Remove quotes.
            transposedUsername = transposedUsername.Replace("\"", "");

            return transposedUsername + "@" + domain;
        }

        /// <summary>
        /// Checks if the string <paramref name="input"/> contains context specific
        /// words from our application.
        /// </summary>
        /// <param name="input">The string to test (string)</param>
        /// <returns>bool indicating whether the input contains context specific words.</returns>
        public static bool ContainsContextSpecificWords(string input)
        {
            // Lowercase the string, any combination of capitals and lowercase is considered context specific.
            string lower = input.ToLower();

            foreach (string word in Constants.ContextSpecificWords)
            {
                if (lower.Contains(word))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously check if string <paramref name="input"/> contains an english word, upper or lowercase,
        /// among the top 9000 most popular words that are over 3 characters in length.
        /// </summary>
        /// <param name="input">The input string to test (string)</param>
        /// <returns>Task (bool) indicating whether the string contains an english word.</returns>
        public static async Task<bool> ContainsDictionaryWordsAsync(string input)
        {
            string lineInput = "";

            using (StreamReader reader = new StreamReader(Constants.WordsTxtPath))
            {
                // Asynchronously read every word from the text file.
                while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (input.Contains(lineInput))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the string <paramref name="input"/> contains repetition or sequences of characters (same case).
        /// NOTE: currently accounts for 012, but not 901.
        /// </summary>
        /// <param name="input">The input string to test (string)</param>
        /// <returns>bool whether the input contains a repetition or sequence of characters</returns>
        public static bool ContainsRepetitionOrSequence(string input)
        {
            // The starting pattern count, 1 character is a pattern of 1.
            int patternCount = 1;

            // Flags for detecting if, based on the previous characters, we are currently in a repetition,
            // increasing sequence, or decreasing sequence.
            bool repetition = false;
            bool increasingSequence = false;
            bool decreasingSequence = false;

            // The previous character that was check, initialized to a random character.
            char previousCharacter = '\b';

            // Flag indicating whether this is the first character in the loop.
            bool first = true;

            foreach (char character in input)
            {
                if (first)
                {
                    // If it is the first, it is no longer the first and the previous character can be properly initialized.
                    first = false;
                    previousCharacter = character;
                }
                else
                {
                    if (repetition || increasingSequence || decreasingSequence)
                    {
                        // We are in a pattern. We may now continue the flags and increase the count, or stop the
                        // flags and reset the count.
                        if (repetition)
                        {
                            // Check if the repetition is continuing. If so increment the count.
                            if (character == previousCharacter)
                            {
                                patternCount++;
                            }
                            else
                            {
                                // Otherwise, reset the flag and count.
                                repetition = false;
                                patternCount = 1;
                            }
                        }
                        else if (increasingSequence)
                        {
                            // Position of the previous character in the sequence.
                            int previousPosition = 0;

                            // Determine if the previous char was a number, uppercase letter, or lowercase letter.
                            // Find its corresponding position based on the result.
                            bool number = false;
                            bool upperLetter = false;
                            bool lowerLetter = false;

                            if (Constants.LettersLower.Contains(previousCharacter))
                            {
                                lowerLetter = true;
                                previousPosition = Constants.LettersLowerToPositions[previousCharacter];
                            }
                            else if (Constants.LettersUpper.Contains(previousCharacter))
                            {
                                upperLetter = true;
                                previousPosition = Constants.LettersUpperToPositions[previousCharacter];
                            }
                            else if (Constants.Numbers.Contains(previousCharacter))
                            {
                                number = true;

                                // Characters represented by sequential numbers in every utf-16. Subtracting the character 0 finds
                                // its value if it is a number. Its value is its position in the sequence.
                                previousPosition = previousCharacter - '0';
                            }

                            // Since this is an increasing sequence, the next position is the previous position + 1.
                            int nextPosition = previousPosition + 1;

                            if (number)
                            {
                                // If the next position went past the max digit value, the next position should be the beginning of
                                // the sequence.
                                if (nextPosition > Constants.MaxDigitValue)
                                {
                                    nextPosition = Constants.MinDigitValue;
                                }

                                // If the next position is the character we are currently at, continue the pattern. Otherwise reset
                                // the flag and the count.
                                if (Constants.Numbers[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    increasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                            else if (upperLetter)
                            {
                                // If the next position went past the max alpha value, the next position should be the beginning of
                                // the sequence.
                                if (nextPosition > Constants.MaxAlphaValue)
                                {
                                    nextPosition = Constants.MinAlphaValue;
                                }

                                // If the next position is the character we are currently at, continue the pattern. Otherwise reset
                                // the flag and the count.
                                if (Constants.PositionsToLettersUpper[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    increasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                            else if (lowerLetter)
                            {
                                // If the next position went past the max alpha value, the next position should be the beginning of
                                // the sequence.
                                if (nextPosition > Constants.MaxAlphaValue)
                                {
                                    nextPosition = 1;
                                }

                                // If the next position is the character we are currently at, continue the pattern. Otherwise reset
                                // the flag and the count.
                                if (Constants.PositionsToLettersLower[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    increasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                        }
                        else if (decreasingSequence)
                        {
                            // Same process as above, except the next position is the previous position - 1.
                            int previousPosition = 0;

                            bool number = false;
                            bool upperLetter = false;
                            bool lowerLetter = false;

                            if (Constants.LettersLower.Contains(previousCharacter))
                            {
                                lowerLetter = true;
                                previousPosition = Constants.LettersLowerToPositions[previousCharacter];
                            }
                            else if (Constants.LettersUpper.Contains(previousCharacter))
                            {
                                upperLetter = true;
                                previousPosition = Constants.LettersUpperToPositions[previousCharacter];
                            }
                            else if (Constants.Numbers.Contains(previousCharacter))
                            {
                                number = true;
                                previousPosition = previousCharacter - '0';
                            }

                            int nextPosition = previousPosition - 1;

                            if (number)
                            {
                                if (nextPosition < Constants.MinDigitValue)
                                {
                                    nextPosition = Constants.MaxDigitValue;
                                }

                                if (Constants.Numbers[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    decreasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                            else if (upperLetter)
                            {
                                if (nextPosition < Constants.MinAlphaValue)
                                {
                                    nextPosition = Constants.MaxAlphaValue;
                                }

                                if (Constants.PositionsToLettersUpper[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    decreasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                            else if (lowerLetter)
                            {
                                if (nextPosition < Constants.MinAlphaValue)
                                {
                                    nextPosition = Constants.MaxAlphaValue;
                                }

                                if (Constants.PositionsToLettersLower[nextPosition] == character)
                                {
                                    patternCount++;
                                }
                                else
                                {
                                    decreasingSequence = false;
                                    patternCount = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Set flags for first instance of a pattern.

                        if (previousCharacter == character)
                        {
                            patternCount++;
                            repetition = true;
                        }
                        else
                        {
                            if (Constants.LettersLower.Contains(character))
                            {
                                int previousPosition = 0;

                                if (Constants.LettersLower.Contains(previousCharacter))
                                {
                                    // Both the previous character and current are lowercase letters. Check for sequences.
                                    previousPosition = Constants.LettersLowerToPositions[previousCharacter];

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease > Constants.MaxAlphaValue)
                                    {
                                        nextPositionIncrease = Constants.MinAlphaValue;
                                    }

                                    if (nextPositionDecrease < Constants.MinAlphaValue)
                                    {
                                        nextPositionDecrease = Constants.MaxAlphaValue;
                                    }

                                    if (Constants.PositionsToLettersLower[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (Constants.PositionsToLettersLower[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                            else if (Constants.LettersUpper.Contains(character))
                            {
                                int previousPosition = 0;

                                if (Constants.LettersUpper.Contains(previousCharacter))
                                {
                                    // Both the previous character and current are upercase letters. Check for sequences.

                                    previousPosition = Constants.LettersUpperToPositions[previousCharacter];

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease > Constants.MaxAlphaValue)
                                    {
                                        nextPositionIncrease = Constants.MinAlphaValue;
                                    }

                                    if (nextPositionDecrease < Constants.MinAlphaValue)
                                    {
                                        nextPositionDecrease = Constants.MaxAlphaValue;
                                    }

                                    if (Constants.PositionsToLettersUpper[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (Constants.PositionsToLettersUpper[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                            else if (Constants.Numbers.Contains(character))
                            {
                                int previousPosition = 0;

                                if (Constants.Numbers.Contains(previousCharacter))
                                {
                                    // Both the previous character and current are numbers. Check for sequences.

                                    previousPosition = previousCharacter - '0';

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease > Constants.MaxDigitValue)
                                    {
                                        nextPositionIncrease = Constants.MinDigitValue;
                                    }

                                    if (nextPositionDecrease < Constants.MinDigitValue)
                                    {
                                        nextPositionDecrease = Constants.MaxDigitValue;
                                    }

                                    if (Constants.Numbers[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (Constants.Numbers[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                        }
                    }

                    // Constant check at end of each iteration to possibly return true from this function.
                    if (patternCount == Constants.MaxRepetitionOrSequence)
                    {
                        return true;
                    }

                    // If here, we go to the next iteration.
                    previousCharacter = character;
                }
            }

            // If nothing found, return false.
            return false;
        }

        /// <summary>
        /// Converts a string to UTF8 bytes
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>UTF8 Bytes array</returns>
        public static byte[] ToBytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// Converts a UTF8 bytes array back to a string
        /// </summary>
        /// <param name="b">The byte array to convert</param>
        /// <returns>The original string representation</returns>
        public static string FromBytes(this byte[] b)
        {
            return Encoding.UTF8.GetString(b);
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
        public static string FromBase64URL(this string str)
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

        /// <summary>
        /// Extension function to convert a string to alpha numeric.
        /// </summary>
        /// <remarks>
        /// This implementation ignores whitespace characters.
        /// </remarks>
        /// <param name="str">The string to be converted.</param>
        /// <returns>An alpha numeric representation</returns>
        public static string ToAlphaNumeric(this string str)
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
        public static bool IsAlphaNumeric(this string str)
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
        /// Converts a dictionary to a JSON string representation.
        /// </summary>
        /// <remarks>
        /// All string values are converted to be Alpha-Numeric. This is the avoid
        /// errors and security issues with serializing a dictionary.
        /// </remarks>
        /// <param name="dict">The dictionary to represent as a string.</param>
        /// <returns>A JSON string representation of the dictionary.</returns>
        public static string DictionaryToString(Dictionary<string, string> dict)
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
        public static Dictionary<string, string> StringToDictionary(string dictStr)
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
    }
}
