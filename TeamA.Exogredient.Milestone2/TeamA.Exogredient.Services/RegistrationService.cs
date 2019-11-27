using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// Class <c>RegistrationService</c> Provides functionality to allow for user registration.
    /// </summary>
    public class RegistrationService
    {
        private UserDAO _userDAO;
        private CorruptedPasswordsDAO _corruptedPasswordsDAO;
        private SecurityService _securityService;

        // No < or > to protect from SQL injections.
        private List<char> _alphaNumericSpecialCharacters = new List<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
            'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '~', '`', '@', '#', '$', '%', '^', '&', '!', '*', '(', ')', '_', '-', '+', '=', '{',
            '[', '}', ']', '|', '\\', '"', '\'', ':', ';', '?', '/', '.', ','
        };

        private List<char> _numericalCharacters = new List<char>()
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };

        /// <summary>
        /// Constructor initializes the UserDAO object to provide
        /// the interface with the usertable.
        /// </summary>
        public RegistrationService()
        {
            _userDAO = new UserDAO();
            _corruptedPasswordsDAO = new CorruptedPasswordsDAO();
            _securityService = new SecurityService();
        }

        /// <summary>
        /// Determines whether the user is within the project scope.
        /// </summary>
        /// <param name="answer">The user's selected scope answer.</param>
        /// <returns>Returns the value of bool that determines whether the 
        /// user is allowed to proceed.</returns>
        public bool CheckScope(bool answer)
        {
            return answer == true;
        }

        /// <summary>
        /// Check whether a given string meets the requirement for length.
        /// </summary>
        /// <param name="name">The string the user wants to check length of.</param>
        /// <param name="length">The length that the string must be equal to.</param>
        /// <param name="min">A optional parameter. If this is set then, name's length can be a 
        /// range from min to length (inclusive).</param>
        /// <returns>Returns value of bool to represent whether the name met the required constraints.</returns>
        public bool CheckLength(string name, int length, int min = -1)
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
        public bool CheckIfANSCharacters(string name)
        {

            bool result = true;

            foreach (char c in name.ToLower())
            {
                result = result && _alphaNumericSpecialCharacters.Contains(c);
            }

            return result;
        }

        /// <summary>
        /// Check whether a given string contains only numerical characters.
        /// </summary>
        /// <param name="name">The string that we are checking.</param>
        /// <returns>Returns value of bool to represent whether all the characters
        /// in name meet the specification.</returns>
        public bool CheckIfNumericCharacters(string name)
        {
            bool result = true;

            foreach (char c in name)
            {
                result = result && _numericalCharacters.Contains(c);
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
        public bool EmailFormatValidityCheck(string email)
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
        public string CanonicalizingEmail(string email)
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

        /// <summary>
        /// Checks if a canonicalized email exists in the database.
        /// </summary>
        /// <param name="canonEmail">Email that already has been canonicalized.</param>
        /// <returns>Returns the value of bool to represent whether
        /// an canonicalized email is unique.</returns>
        public async Task<bool> CheckEmailUniquenessAsync(string canonEmail)
        {
            //return await _userDAO.CheckEmailUniquenessAsync(canonEmail);
            return false;
        }

        /// <summary>
        /// Checks if a phone number already exists in the database.
        /// </summary>
        /// <param name="phoneNumber">The phone number we are checking.</param>
        /// <returns>Returns the value of bool to represent whether
        /// a phone number is unique.</returns>
        public async Task<bool> CheckPhoneUniquenessAsync(string phoneNumber)
        {
            //return await _userDAO.CheckPhoneUniquenessAsync(phoneNumber);
            return false;
        }

        /// <summary>
        /// Checks if a username already exists in the database.
        /// </summary>
        /// <param name="username">The username we are checking.</param>
        /// <returns>Returns the value of bool to represent whether
        /// an username is unique.</returns>
        public async Task<bool> CheckUsernameUniquenessAsync(string username)
        {
            //return await _userDAO.CheckUsernameUniquenessAsync(username);
            return false;
        }

        public async Task<bool> CheckPasswordSecurityAsync(string plaintextPassword)
        {
            // Test if password contains context specific words.

            if (plaintextPassword.ToLower().Contains("exogredient"))
            {
                return false;
            }


            // Check if password contains an english word, upper or lowercase, among the top 9000 most popular
            // words that are over 3 characters in length. Done 2nd because fastest IO test.

            string lineInput = "";

            using (StreamReader reader = new StreamReader(@"..\..\..\..\words.txt"))
            {
                while ((lineInput = await reader.ReadLineAsync()) != null)
                {
                    if (plaintextPassword.Contains(lineInput))
                    {
                        return false;
                    }
                }
            }

            // Check if password has been corrupted in previous breaches. Done second to last because
            // it is the slowest IO check.

            List<string> passwordHashes = await _corruptedPasswordsDAO.ReadAsync();
            string passwordSha1 = _securityService.HashWithSHA1(plaintextPassword);

            foreach (string hash in passwordHashes)
            {
                if (passwordSha1.Equals(hash))
                {
                    return false;
                }
            }

            // Check password for repeated consecutive alphanumeric and special characters, 4 or more.
            string pattern = @"(a{3,}|b{3,}|c{3,}|d{3,}|e{3,}|f{3,}|g{3,}|h{3,}|i{3,}|j{3,}|k{3,}" +
                             @"|l{3,}|m{3,}|n{3,}|o{3,}|p{3,}|q{3,}|r{3,}|s{3,}|t{3,}|u{3,}|v{3,}" +
                             @"|w{3,}|x{3,}|y{3,}|z{3,}|1{3,}|2{3,}|3{3,}|4{3,}|5{3,}|6{3,}|7{3,}" +
                             @"|8{3,}|9{3,}|0{3,}|~{3,}|`{3,}|@{3,}|#{3,}|\${3,}|%{3,}|\^{3,}|&{3,}" +
                             @"|!{3,}|\*{3,}|\({3,}|\){3,}|_{3,}|-{3,}|\+{3,}|={3,}|{{3,}|\[{3,}|}{3,}" +
                             @"|]{3,}|\|{3,}|\\{3,}|""{3,}|'{3,}|:{3,}|;{3,}|\?{3,}|\/{3,}|\.{3,}|,{3,}" +
                             @"|A{3,}|B{3,}|C{3,}|D{3,}|E{3,}|F{3,}|G{3,}|H{3,}|I{3,}|J{3,}|K{3,}" +
                             @"|L{3,}|M{3,}|N{3,}|O{3,}|P{3,}|Q{3,}|R{3,}|S{3,}|T{3,}|U{3,}|V{3,}" +
                             @"|W{3,}|X{3,}|Y{3,}|Z{3,})";

            // Check password for sequential sequences of numbers or letters in both forward and reverse order.
            string sequences = @"(123|234|345|456|567|678|789|890|901|012|987|876|765|654|543|432|321|210|109|098" +
                               @"|zyx|yxw|xwv|wvu|vut|uts|tsr|srq|rqp|qpo|pon|onm|nml|mlk|lkj|kji|jih" +
                               @"|ihg|hgf|gfe|fed|edc|dcb|cba|baz|azy|abc|bcd|cde|def|efg|fgh|ghi|hij" +
                               @"|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz|yza|zab)";

            // Last check as it's the slowest check.

            Regex regexPattern = new Regex(pattern);
            Regex regexSequence = new Regex(sequences);

            if (regexPattern.IsMatch(plaintextPassword) || regexSequence.IsMatch(plaintextPassword))
            {
                return false;
            }

            return true;
        }

        bool GenerateTempUser(string firstName, string lastName, string phoneNumber,
            string username, string email, string digestPlusSalt)
        {
            //return async bool _UserDAO.GenerateTempUser(string username);
            return false;
        }

        bool DeleteTempUser(string username)
        {
            //return async bool _UserDAO.DeleteTempUser(string username)
            return false;
        }

        bool MakeTempUserPerm(string username)
        {
            //return async bool _UserDAO.MakeTempUserPerm(string username)
            return false;
        }
    }
}
