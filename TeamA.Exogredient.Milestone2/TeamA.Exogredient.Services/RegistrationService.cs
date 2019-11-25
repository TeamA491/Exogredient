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
            //TODO
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
            //string lineInput = "";

            //using (StreamReader reader = new StreamReader(@"..\..\..\..\words.txt"))
            //{
            //    while ((lineInput = await reader.ReadLineAsync()) != null)
            //    {
            //        if (plaintextPassword.Contains(lineInput))
            //        {
            //            return false;
            //        }
            //    }
            //}

            //List<string> passwordHashes = await _corruptedPasswordsDAO.ReadAsync();
            //string passwordSha1 = _securityService.HashWithSHA1(plaintextPassword);

            //foreach (string hash in passwordHashes)
            //{
            //    if (passwordSha1.Equals(hash))
            //    {
            //        return false;
            //    }
            //}


            // TEST: parentheses and curly braces and double quote and single quotes
            string pattern = @"(a{4,}|b{4,}|c{4,}|d{4,}|e{4,}|f{4,}|g{4,}|h{4,}|i{4,}|j{4,}|k{4,}" +
                             @"|l{4,}|m{4,}|n{4,}|o{4,}|p{4,}|q{4,}|r{4,}|s{4,}|t{4,}|u{4,}|v{4,}" +
                             @"|w{4,}|x{4,}|y{4,}|z{4,}|1{4,}|2{4,}|3{4,}|4{4,}|5{4,}|6{4,}|7{4,}" +
                             @"|8{4,}|9{4,}|0{4,}|~{4,}|`{4,}|@{4,}|#{4,}|\${4,}|%{4,}|\^{4,}|&{4,}" +
                             @"|!{4,}|\*{4,}|\({4,}|\){4,}|_{4,}|-{4,}|\+{4,}|={4,}|{{4,}|\[{4,}|}{4,}" +
                             @"|]{4,}|\|{4,}|\\{4,}|""{4,}|'{4,}|:{4,}|;{4,}|\?{4,}|\/{4,}|\.{4,}|,{4,}";

            Regex rgx = new Regex(pattern);

            if (rgx.IsMatch(plaintextPassword))
            {
                Console.WriteLine($"hit: {plaintextPassword}");
            }
            else
            {
                Console.WriteLine("fail");
            }

            return false;
        }

        public string GenerateSalt()
        {
            //return execute random generator
            return "";
        }

        public string GenerateDigest(string salt, string plaintextPassword)
        {
            //return sha2(salt, plaintextPassword);
            return "";
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
