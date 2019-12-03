using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class StringUtilityService
    {
        private static readonly IDictionary<int, int> _monthDays = Constants.MonthDays;
        private static readonly List<char> _alphaNumericAndSpecialCharacters = Constants.AlphaNumericAndSpecialCharacters;
        private const int _secondsInAnHour = Constants.SecondsInAnHour;
        private const int _secondsInAMinute = Constants.SecondsInAMinute;
           
        // NIST checking
        private const int _maxRepetitionOrSequence = Constants.MaxRepetitionOrSequence;
        private static readonly List<string> _contextSpecificWords = Constants.ContextSpecificWords;
        private static readonly List<char> _lettersLower = Constants.LettersLower;
        private static readonly List<char> _lettersUpper = Constants.LettersUpper;
        private static readonly List<char> _numbers = Constants.Numbers;
        private static readonly IDictionary<char, int> _lettersLowerToPositions = Constants.LettersLowerToPositions;
        private static readonly IDictionary<char, int> _lettersUpperToPositions = Constants.LettersUpperToPositions;
        private static readonly IDictionary<int, char> _positionsToLettersLower = Constants.PositionsToLettersLower;
        private static readonly IDictionary<int, char> _positionsToLettersUpper = Constants.PositionsToLettersUpper;

        private static readonly CorruptedPasswordsDAO _corruptedPasswordsDAO;

        /// <summary>
        /// Constructor initializes the CorruptedPasswordsDAO object to provide
        /// the interface with the usertable.
        /// </summary>
        static StringUtilityService()
        {
            _corruptedPasswordsDAO = new CorruptedPasswordsDAO();
        }

        public static long CurrentUnixTime()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public static long TimespanToSeconds(TimeSpan span)
        {
            long result = 0;

            int inputHours = span.Hours;
            int inputMinutes = span.Minutes;
            int inputSeconds = span.Seconds;

            for (int i = 0; i < inputHours; i++)
            {
                result += _secondsInAnHour;
            }

            for (int i = 0; i < inputMinutes; i++)
            {
                result += _secondsInAMinute;
            }

            result += inputSeconds;

            return result;
        }

        // Change To Epoch Time
        public static bool CurrentTimePastDatePlusTimespan(string date, TimeSpan span)
        {
            int lockedHour = Int32.Parse(date.Substring(0, 2));
            int lockedMinute = Int32.Parse(date.Substring(3, 2));
            int lockedSecond = Int32.Parse(date.Substring(6, 2));
            int lockedMonth = Int32.Parse(date.Substring(9, 2));
            int lockedDay = Int32.Parse(date.Substring(12, 2));
            int lockedYear = Int32.Parse(date.Substring(15, 4));

            int inputHours = span.Hours;
            int inputMinutes = span.Minutes;
            int inputSeconds = span.Seconds;

            int resultHour = lockedHour;
            int resultMinute = lockedMinute;
            int resultSecond = lockedSecond;
            int resultMonth = lockedMonth;
            int resultDay = lockedDay;
            int resultYear = lockedYear;

            // Get result date time

            for (int i = 0; i < inputHours; i++)
            {
                resultHour++;

                if (resultHour > 23)
                {
                    resultHour = 00;

                    resultDay++;
                }

                if (resultDay > _monthDays[resultMonth])
                {
                    if (resultMonth == 2 && resultYear % 4 == 0 && resultDay == 29)
                    {
                        if (resultYear % 100 == 0 && resultYear % 400 != 0)
                        {
                            // Not a leap year.
                            resultDay = 01;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = 01;

                        resultMonth++;
                    }
                }

                if (resultMonth > 12)
                {
                    resultMonth = 01;

                    resultYear++;
                }
            }

            for (int i = 0; i < inputMinutes; i++)
            {
                resultMinute++;

                if (resultMinute > 59)
                {
                    resultMinute = 00;

                    resultHour++;
                }

                if (resultHour > 23)
                {
                    resultHour = 00;

                    resultDay++;
                }

                if (resultDay > _monthDays[resultMonth])
                {
                    if (resultMonth == 2 && resultYear % 4 == 0 && resultDay == 29)
                    {
                        if (resultYear % 100 == 0 && resultYear % 400 != 0)
                        {
                            // Not a leap year.
                            resultDay = 01;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = 01;

                        resultMonth++;
                    }
                }

                if (resultMonth > 12)
                {
                    resultMonth = 01;

                    resultYear++;
                }
            }

            for (int i = 0; i < inputSeconds; i++)
            {
                resultSecond++;

                if (resultSecond > 59)
                {
                    resultSecond = 00;

                    resultMinute++;
                }

                if (resultMinute > 59)
                {
                    resultMinute = 00;

                    resultHour++;
                }

                if (resultHour > 23)
                {
                    resultHour = 00;

                    resultDay++;
                }

                if (resultDay > _monthDays[resultMonth])
                {
                    if (resultMonth == 2 && resultYear % 4 == 0 && resultDay == 29)
                    {
                        if (resultYear % 100 == 0 && resultYear % 400 != 0)
                        {
                            // Not a leap year.
                            resultDay = 01;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = 01;

                        resultMonth++;
                    }
                }

                if (resultMonth > 12)
                {
                    resultMonth = 01;

                    resultYear++;
                }
            }

            // Compare datetimes
            DateTime resultTime = new DateTime(resultYear, resultMonth, resultDay, resultHour, resultMinute, resultSecond, DateTimeKind.Utc);

            int compareResult = DateTime.Compare(resultTime, DateTime.UtcNow);

            if (compareResult < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert a hex string to a byte array
        /// </summary>
        /// <param name="hexString"> hex string to be converted </param>
        /// <returns> byte array of the hex string </returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            // The length of the byte array of the hex string is hexString.Length / 2
            byte[] bytes = new byte[hexString.Length / 2];
            char[] charArray = hexString.ToCharArray();

            // for index i in the byte array
            for (int i = 0; i < bytes.Length; i++)
            {
                // Create a string of two characters at i*2 and i*2+1 index of hexString
                string temp = "" + charArray[i * 2] + charArray[i * 2 + 1];
                // Convert the hex string to a byte and store at i index of the byte array
                bytes[i] = Convert.ToByte(temp, 16);
            }

            return bytes;
        }

        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns> hex string of the byte array </returns>
        public static string BytesToHexString(byte[] bytes)
        {
            // Convert the bytes to hex string without "-"
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// Convert a string to a hex string using ASCII encoding.
        /// </summary>
        /// <param name="s"> string to be converted </param>
        /// <returns> hex string of the string </returns>
        public static string ToHexString(string s)
        {
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
            if (min == -1)
            {
                return name.Length == length;
            }
            else
            {
                return name.Length >= min && name.Length <= length;
            }
        }

        /// <summary>
        /// Check whether a given string contains only Alphanumeric and Special characters.
        /// </summary>
        /// <param name="name">The string that we are checking.</param>
        /// <returns>Returns value of bool to represent whether all the characters
        /// in name meet the specification.</returns>
        public static bool CheckIfANSCharacters(string name)
        {

            bool result = true;

            foreach (char c in name.ToLower())
            {
                result = result && _alphaNumericAndSpecialCharacters.Contains(c);
            }

            return result;
        }

        /// <summary>
        /// Check whether a given string contains only numerical characters.
        /// </summary>
        /// <param name="name">The string that we are checking.</param>
        /// <returns>Returns value of bool to represent whether all the characters
        /// in name meet the specification.</returns>
        public static bool CheckIfNumericCharacters(string name)
        {
            bool result = true;

            foreach (char c in name)
            {
                result = result && _numbers.Contains(c);
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
        public static bool EmailFormatValidityCheck(string email)
        {
            string[] splitResult = email.Split('@');

            if (splitResult.Length == 2)
            {
                string first = splitResult[0];
                string second = splitResult[1];

                if (first.Length >= 1 && second.Length >= 1)
                {
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

        /// <summary>
        /// Breaks email address up into two parts, the local-part 
        /// and the domain.
        /// First we convert to lowercase.
        /// Then we check if the domain is "gmail.com".
        /// If it is, then the local-part is checked for "+[anything]", ".", and """", 
        /// and they are then removed.
        /// Finally all double quotes are removed
        /// </summary>
        /// <param name="email">The email we are checking.</param>
        /// <returns>Returns value of string to represent the 
        /// canonicalized email.</returns>
        public static string CanonicalizingEmail(string email)
        {
            string[] splitResult = email.Split('@');
            string username = splitResult[0].ToLower();
            string domain = splitResult[1].ToLower();

            string transposedUsername = username;

            if (domain.Equals("gmail.com"))
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

        public static bool ContainsContextSpecificWords(string plaintextPassword)
        {
            string lowerPassword = plaintextPassword.ToLower();

            foreach (string word in _contextSpecificWords)
            {
                if (lowerPassword.Contains(word))
                {
                    return true;
                }
            }

            return false;
        }


        // Check if password contains an english word, upper or lowercase, among the top 9000 most popular
        // words that are over 3 characters in length. Done 2nd because fastest IO test.
        public static async Task<bool> ContainsDictionaryWordsAsync(string plaintextPassword)
        {
            string lineInput = "";

            using (StreamReader reader = new StreamReader(@"..\..\..\..\words.txt"))
            {
                while ((lineInput = await reader.ReadLineAsync()) != null)
                {
                    if (plaintextPassword.Contains(lineInput))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Check if password has been corrupted in previous breaches. Done second to last because
        // it is the slowest IO check.
        public static async Task<bool> IsCorruptedPassword(string plaintextPassword)
        {
            List<string> passwordHashes = await _corruptedPasswordsDAO.ReadAsync();
            string passwordSha1 = SecurityService.HashWithSHA1(plaintextPassword);

            foreach (string hash in passwordHashes)
            {
                if (passwordSha1.Equals(hash))
                {
                    return true;
                }
            }

            return false;
        }

        // NOTE: does not account for 901. but will return tru for 012
        public static bool ContainsRepetitionOrSequence(string plaintextPassword)
        {
            // Repetition and sequence checking.
            int patternCount = 1;

            bool repetition = false;
            bool increasingSequence = false;
            bool decreasingSequence = false;

            char previousCharacter = '\b';

            bool first = true;

            foreach (char character in plaintextPassword)
            {
                if (first)
                {
                    first = false;
                    previousCharacter = character;
                }
                else
                {
                    // Continue flags, or stop them.
                    if (repetition || increasingSequence || decreasingSequence)
                    {
                        if (repetition)
                        {
                            if (character == previousCharacter)
                            {
                                patternCount++;
                            }
                            else
                            {
                                repetition = false;
                                patternCount = 1;
                            }
                        }
                        else if (increasingSequence)
                        {
                            int previousPosition = 0;
                            bool number = false;
                            bool upperLetter = false;
                            bool lowerLetter = false;

                            if (_lettersLower.Contains(previousCharacter))
                            {
                                lowerLetter = true;
                                previousPosition = _lettersLowerToPositions[previousCharacter];
                            }
                            else if (_lettersUpper.Contains(previousCharacter))
                            {
                                upperLetter = true;
                                previousPosition = _lettersUpperToPositions[previousCharacter];
                            }
                            else if (_numbers.Contains(previousCharacter))
                            {
                                number = true;
                                // Characters represented by sequential numbers in every utf-16
                                previousPosition = previousCharacter - '0';
                            }

                            int nextPosition = previousPosition + 1;

                            if (number)
                            {
                                if (nextPosition == 10)
                                {
                                    nextPosition = 1;
                                }

                                if (_numbers[nextPosition] == character)
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
                                if (nextPosition == 27)
                                {
                                    nextPosition = 1;
                                }

                                if (_positionsToLettersUpper[nextPosition] == character)
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
                                if (nextPosition == 27)
                                {
                                    nextPosition = 1;
                                }

                                if (_positionsToLettersLower[nextPosition] == character)
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
                            int previousPosition = 0;
                            bool number = false;
                            bool upperLetter = false;
                            bool lowerLetter = false;

                            if (_lettersLower.Contains(previousCharacter))
                            {
                                lowerLetter = true;
                                previousPosition = _lettersLowerToPositions[previousCharacter];
                            }
                            else if (_lettersUpper.Contains(previousCharacter))
                            {
                                upperLetter = true;
                                previousPosition = _lettersUpperToPositions[previousCharacter];
                            }
                            else if (_numbers.Contains(previousCharacter))
                            {
                                number = true;
                                // Characters represented by sequential numbers in every utf-16
                                previousPosition = previousCharacter - '0';
                            }

                            int nextPosition = previousPosition - 1;

                            if (number)
                            {
                                if (nextPosition == 0)
                                {
                                    nextPosition = 9;
                                }

                                if (_numbers[nextPosition] == character)
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
                                if (nextPosition == 0)
                                {
                                    nextPosition = 26;
                                }

                                if (_positionsToLettersUpper[nextPosition] == character)
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
                                if (nextPosition == 0)
                                {
                                    nextPosition = 26;
                                }

                                if (_positionsToLettersLower[nextPosition] == character)
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
                            if (_lettersLower.Contains(character))
                            {
                                int previousPosition = 0;

                                if (_lettersLower.Contains(previousCharacter))
                                {
                                    previousPosition = _lettersLowerToPositions[previousCharacter];

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease == 27)
                                    {
                                        nextPositionIncrease = 1;
                                    }

                                    if (nextPositionDecrease == 0)
                                    {
                                        nextPositionDecrease = 26;
                                    }

                                    if (_positionsToLettersLower[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (_positionsToLettersLower[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                            else if (_lettersUpper.Contains(character))
                            {
                                int previousPosition = 0;

                                if (_lettersUpper.Contains(previousCharacter))
                                {
                                    previousPosition = _lettersUpperToPositions[previousCharacter];

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease == 27)
                                    {
                                        nextPositionIncrease = 1;
                                    }

                                    if (nextPositionDecrease == 0)
                                    {
                                        nextPositionDecrease = 26;
                                    }

                                    if (_positionsToLettersUpper[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (_positionsToLettersUpper[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                            else if (_numbers.Contains(character))
                            {
                                int previousPosition = 0;

                                if (_numbers.Contains(previousCharacter))
                                {
                                    previousPosition = previousCharacter - '0';

                                    int nextPositionIncrease = previousPosition + 1;
                                    int nextPositionDecrease = previousPosition - 1;

                                    if (nextPositionIncrease == 10)
                                    {
                                        nextPositionIncrease = 1;
                                    }

                                    if (nextPositionDecrease < 1)
                                    {
                                        nextPositionDecrease = 9;
                                    }

                                    if (_numbers[nextPositionIncrease] == character)
                                    {
                                        patternCount++;
                                        increasingSequence = true;
                                    }

                                    if (_numbers[nextPositionDecrease] == character)
                                    {
                                        patternCount++;
                                        decreasingSequence = true;
                                    }
                                }
                            }
                        }
                    }

                    // Constant check at end of each iteration to possibly return false from this function.
                    if (patternCount == _maxRepetitionOrSequence)
                    {
                        return true;
                    }

                    // If here, we go to the next iteration.
                    previousCharacter = character;
                }
            }

            return false;
        }
    }
}
