using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.AppConstants
{
    public static class Constants
    {
        // ENVIRONMENT VARIABLES
        public static readonly string PrivateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY", EnvironmentVariableTarget.User);
        public static readonly string PublicKey = Environment.GetEnvironmentVariable("PUBLIC_KEY", EnvironmentVariableTarget.User);
        public static readonly string TwilioAuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN", EnvironmentVariableTarget.User);
        public static readonly string SystemEmailPassword = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD", EnvironmentVariableTarget.User);
        public static readonly string NOSQLConnection = Environment.GetEnvironmentVariable("NOSQL_CONNECTION", EnvironmentVariableTarget.User);
        public static readonly string SQLConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION", EnvironmentVariableTarget.User);
        public static readonly string FTPpassword = Environment.GetEnvironmentVariable("FTP_PASSWORD", EnvironmentVariableTarget.User);
        public static readonly string AuthzPrivateKey = Environment.GetEnvironmentVariable("AUTHORIZATION_PRIVATE_KEY", EnvironmentVariableTarget.User);
        public static readonly string AuthzPublicKey = Environment.GetEnvironmentVariable("AUTHORIZATION_PUBLIC_KEY", EnvironmentVariableTarget.User);

        // AUTHORIZATION

        public const string AuthzSigningAlgorithm = "RS512";
        public const string AuthzExpirationField = "exp";
        public const string AuthzPublicKeyField = "pk";

        public const string UserTypeKey = "userType";
        public const string IdKey = "id";

        public const string MediaTyp = "typ";
        public const string MediaJWT = "JWT";
        public const string SigningAlg = "alg";

        public const string SHA1 = "SHA1";


        public enum USER_TYPE
        {
            UNREGISTERED = 0,
            REGISTERED = 1,
            STORE_OWNER = 2,
            ADMIN = 3,
            SYS_ADMIN = 4,
        };

        // AUTHENTICATION / USER MANAGEMENT
        public const string GoogleSMTP = "smtp.gmail.com";
        public const int GoogleSMTPPort = 465;

        public const string TwilioCallChannel = "call";

        public const string EmailVerificationSubject = "Exogredient Account Verification";

        public const int EmailCodeLength = 6;

        // FLAT FILE LOGGING
        public static readonly List<string> CsvVulnerabilities = new List<string>()
        {
            "=", "@", "+", "-"
        };

        public const string CsvProtection = @"\t";

        // SECURITY SERVICE
        public const int DefaultSaltLength = 8;
        public const int DefaultHashIterations = 10000;
        public const int DefaultHashLength = 32;

        // USER MANAGEMENT
        public const string NotifySysAdminSubjectFormatString = "MM-dd-yyyy";

        // UTILITY SERVICE
        public const int HoursInADay = 24;
        public const int MonthsInAYear = 12;
        public const int MinutesInAnHour = 60;
        public const int SecondsInAMinute = 60;
        public const int SecondsInAnHour = 3600;
        public const int FebruaryMonthValue = 2;
        public const int LeapDayValue = 29;
        public const int LeapYearOccurrenceYears = 4;
        public const int LeapYearUnoccurenceYears = 100;
        public const int LeapYearReoccurenceYears = 400;

        public const int SecondsStartValue = 0;
        public const int MinuteStartValue = 0;
        public const int HourStartValue = 0;
        public const int DayStartValue = 1;
        public const int MonthStartValue = 1;

        public const string GmailHost = "gmail.com";

        public const string WordsTxtPath = @"..\..\..\..\words.txt";

        public const int MaxDigitValue = 9;
        public const int MaxAlphaValue = 26;

        // BUSINESS RULES
        public const string LoggingFormatString = "HH:mm:ss:ff UTC yyyyMMdd";

        public const string RegistrationOperation = "Registration";
        public const string LogInOperation = "Log In";
        public const string VerifyEmailOperation = "Verify Email Code";
        public const string VerifyPhoneOperation = "Verify Phone Code";
        public const string SendPhoneCodeOperation = "Send Phone Code";
        public const string SendEmailCodeOperation = "Send Email Code";
        public const string UpdatePasswordOperation = "Update Password";

        public const string CustomerUserType = "Customer";
        public const string AnonymousUserType = "Unregistered Customer";
        public const string AnonymousUserIdentifier = "<Unregistered Customer>";

        public const int DisabledStatus = 1;
        public const int EnabledStatus = 0;

        public const long NoValueLong = 0;
        public const int NoValueInt = 0;
        public const string NoValueString = "";

        public const int LoggingRetriesAmount = 3;
        public const int MaxLogInAttempts = 18;
        public const int MaxRegistrationAttempts = 3;
        public const int MaxEmailCodeAttempts = 3;
        public const int MaxPhoneCodeAttempts = 3;
        public static readonly TimeSpan LogInTriesResetTime = new TimeSpan(2, 0, 0);
        public static readonly TimeSpan EmailCodeMaxValidTime = new TimeSpan(0, 15, 0);
        public static readonly TimeSpan MaxTempUserTime = new TimeSpan(1, 0, 0);

        public const string ANSNoAngle = "ANS-NoAngle";
        public const string Numeric = "NUM";

        public static readonly IDictionary<string, List<char>> CharSetsData = new Dictionary<string, List<char>>()
        {
            { ANSNoAngle, ANSNoAngleBrackets },
            { Numeric, Numbers }
        };

        public const int MaximumFirstNameCharacters = 200;
        public const int MinimumFirstNameCharacters = 1;
        public const string FirstNameCharacterType = ANSNoAngle;

        public const int MaximumLastNameCharacters = 200;
        public const int MinimumLastNameCharacters = 1;
        public const string LastNameCharacterType = ANSNoAngle;

        public const int MaximumEmailCharacters = 200;
        public const int MinimumEmailCharacters = 1;
        public const string EmailCharacterType = ANSNoAngle;

        public const int MaximumUsernameCharacters = 200;
        public const int MinimumUsernameCharacters = 1;
        public const string UsernameCharacterType = ANSNoAngle;

        public const int PhoneNumberCharacterLength = 10;
        public const string PhoneNumberCharacterType = Numeric;

        public const int MaximumPasswordCharacters = 2000;
        public const int MinimumPasswordCharacters = 12;
        public const string PasswordCharacterType = ANSNoAngle;

        public const string TwilioExpiredReturnString = "expired";

        // BUSINESS RULES -- MESSAGES
        public const string RegistrationSuccessUserMessage = "Registration Successful!";
        public const string LogInSuccessUserMessage = "Logged in successfully!";
        public const string VerifyEmailSuccessUserMessage = "Email verified! Please select the 'Call Me' option";
        public const string VerifyPhoneSuccessUserMessage = "Phone code verified!";
        public const string SendPhoneCodeSuccessUserMessage = "Phone code sent!";
        public const string SendEmailCodeSuccessUserMessage = "Email code sent!";
        public const string UpdatePasswordSuccessUserMessage = "Updated password!";

        public const string SystemErrorUserMessage = "A system error occurred. Please try again later. A team of highly trained monkeys is currently working on the situation.";

        public const string UsernameDNELogMessage = "Username does not exist";
        public const string InvalidLogInUserMessage = "Id or password was invalid";

        public const string UserDisableLogMessage = "User disabled";
        public const string UserDisableUserMessage = "Your account is disabled, please contact the system administrator.";

        public const string InvalidPasswordLogMessage = "Invalid password entered";

        public const string UsernameExistsLogMessage = "Username taken";
        public const string EmailExistsLogMessage = "Email taken";
        public const string PhoneNumberExistsLogMessage = "Phone number taken";
        public const string UniqueIdExistsRegistrationUserMessage = "Your email, username, or phone number was invalid... please try again";

        public const string InvalidScopeLogMessage = "User not in scope";
        public const string InvalidScopeUserMassage = "You must in California to register";

        public const string InvalidFirstNameLengthLogMessage = "First name length invalid";
        public static readonly string InvalidFirstNameLengthUserMessage = $"Fist name length invalid ({MaximumFirstNameCharacters} max)";
        public const string InvalidFirstNameCharactersLogMessage = "First name characters invalid";
        public const string InvalidFirstNameCharactersUserMessage = "First name characters invalid, < and > not allowed";

        public const string InvalidLastNameLengthLogMessage = "Last name length invalid";
        public static readonly string InvalidLastNameLengthUserMessage = $"Last name length invalid ({MaximumLastNameCharacters} max)";
        public const string InvalidLastNameCharactersLogMessage = "Last name characters invalid";
        public const string InvalidLastNameCharactersUserMessage = "Last name characters invalid, < and > not allowed";

        public const string InvalidEmailLengthLogMessage = "Email length invalid";
        public static readonly string InvalidEmailLengthUserMessage = $"Email length invalid ({MaximumEmailCharacters} max)";
        public const string InvalidEmailCharactersLogMessage = "Email characters invalid";
        public const string InvalidEmailCharactersUserMessage = "Email characters invalid, < and > not allowed";
        public const string InvalidEmailFormatMessage = "Email format invalid";

        public const string InvalidUsernameLengthLogMessage = "Username length invalid";
        public static readonly string InvalidUsernameLengthUserMessage = $"Username length invalid ({MaximumUsernameCharacters} max)";
        public const string InvalidUsernameCharactersLogMessage = "Username characters invalid";
        public const string InvalidUsernameCharactersUserMessage = "Username characters invalid, < and > not allowed";

        public const string InvalidPhoneNumberLengthLogMessage = "Phone number length invalid";
        public static readonly string InvalidPhoneNumberLengthUserMessage = $"Phone number length invalid ({PhoneNumberCharacterLength} only)";
        public const string InvalidPhoneNumberCharactersLogMessage = "Phone number characters invalid";
        public const string InvalidPhoneNumberCharactersUserMessage = "Phone number characters invalid, < and > not allowed";

        public const string InvalidPasswordLengthLogMessage = "Password length invalid";
        public static readonly string InvalidPasswordLengthUserMessage = $"Password length invalid ({MaximumPasswordCharacters} max, {MinimumPasswordCharacters} min)";
        public const string InvalidPasswordCharactersLogMessage = "Password characters invalid";
        public const string InvalidPasswordCharactersUserMessage = "Password characters invalid, < and > not allowed";
        public const string PasswordContextSpecificMessage = "Password contains context specific words";
        public const string PasswordSequencesOrRepetitionsLogMessage = "Password contains sequences or repetitions";
        public const string PasswordSequencesOrRepetitionsUserMessage = "Password contains sequences (111) or repetitions (123).";
        public const string PasswordWordsLogMessage = "Password contains words";
        public const string PasswordWordsUserMessage = "Password contains words from the English language";
        public const string PasswordCorruptedLogMessage = "Password corrupted";
        public const string PasswordCorruptedUserMessage = "Your password has been corrupted";

        public const string MaxEmailTriesReachedLogMessage = "Max email tries reached";
        public static readonly string MaxEmailTriesReachedUserMessage = $"Maximum email code tries reached ({MaxEmailCodeAttempts} max)";

        public const string EmailCodeExpiredLogMessage = "Email code expired";
        public const string EmailCodeExpiredUserMessage = "Email code expired, select the option to re-send.";

        public const string WrongEmailCodeMessage = "Wrong email code input";

        public const string MaxPhoneTriesReachedLogMessage = "Max phone tries reached";
        public static readonly string MaxPhoneTriesReachedUserMessage = $"Maximum pone code tries reached ({MaxPhoneCodeAttempts} max)";

        public const string PhoneCodeExpiredLogMessage = "Phone code expired";
        public const string PhoneCodeExpiredUserMessage = "Phone code expired, select the option to re-send.";

        public const string WrongPhoneCodeMessage = "Wrong phone code input";

        // EMAIL
        public const string SystemEmailAddress = "exogredient.system@gmail.com";
        public const string SystemAdminEmailAddress = "TEAMA.CS491@gmail.com";

        // TWILIO
        public const string TwilioAccountSID = "AC94d03adc3d2da651c16c82932c29b047";
        public const string TwilioPathServiceSID = "VAa9682f046b6f511b9aa1807d4e2949e5";

        // FLAT FILE
        public const string LogFolder = @"C:\Logs";
        public const string LogFileType = ".CSV";
        public const string TokenFile = "token.txt";

        // SQL SCHEMA
        public const string SQLSchemaName = "exogredient";

        // USER TABLE
        public const string UserDAOtableName = "user";
        public const string UserDAOusernameColumn = "username";                                  // VARCHAR(200)
        public const string UserDAOfirstNameColumn = "first_name";                               // VARCHAR(200)
        public const string UserDAOlastNameColumn = "last_name";                                 // VARCHAR(200)
        public const string UserDAOemailColumn = "email";                                        // VARCHAR(200)
        public const string UserDAOphoneNumberColumn = "phone_number";                           // VARCHAR(10)
        public const string UserDAOpasswordColumn = "password";                                  // VARCHAR(2000)
        public const string UserDAOdisabledColumn = "disabled";                                  // TINYINT
        public const string UserDAOuserTypeColumn = "user_type";                                 // VARCHAR(11) -- could be enum/set
        public const string UserDAOsaltColumn = "salt";                                          // VARCHAR(200)
        public const string UserDAOtempTimestampColumn = "temp_timestamp";                       // BIGINT -- unix
        public const string UserDAOemailCodeColumn = "email_code";                               // VARCHAR(6)
        public const string UserDAOemailCodeTimestampColumn = "email_code_timestamp";            // BIGINT -- unix
        public const string UserDAOloginFailuresColumn = "login_failures";                       // INT
        public const string UserDAOlastLoginFailTimestampColumn = "last_login_fail_timestamp";   // BIGINT -- unix
        public const string UserDAOemailCodeFailuresColumn = "email_code_failures";              // INT
        public const string UserDAOphoneCodeFailuresColumn = "phone_code_failures";              // INT

        // IP ADDRESS TABLE
        public const string IPAddressDAOtableName = "ip_address";
        public const string IPAddressDAOIPColumn = "ip";                                         // VARCHAR(15)
        public const string IPAddressDAOtimestampLockedColumn = "timestamp_locked";              // BIGINT -- unix
        public const string IPAddressDAOregistrationFailuresColumn = "registration_failures";    // INT
        public const string IPAddressDAOlastRegFailTimestampColumn = "last_reg_fail_timestamp";  // BIGINT -- unix

        // CORRUPTED PASSWORDS COLLECTION
        public const string CorruptedPassSchemaName = "corrupted_passwords";
        public const string CorruptedPassCollectionName = "passwords";
        public const string CorruptedPassPasswordField = "password";

        // LOGS COLLECTION
        public const string LogsSchemaName = "exogredient_logs";
        public const string LogsCollectionPrefix = "logs_";
        public const string LogsIdField = "_id";
        public const string LogsTimestampField = "timestamp";
        public const string LogsOperationField = "operation";
        public const string LogsIdentifierField = "identifier";
        public const string LogsIPAddressField = "ip";
        public const string LogsErrorTypeField = "errorType";

        // STRING UTILITY HELPER DATA STRUCTURES
        public static readonly IDictionary<int, int> MonthDays = new Dictionary<int, int>()
        {
            { 1, 31 }, { 2, 28 }, { 3, 31 }, { 4, 30 }, { 5, 31 },
            { 6, 30 }, { 7, 31 }, { 8, 31 }, { 9, 30 }, { 10, 31 },
            { 11, 30 }, { 12, 31 }
        };

        // No < or > to protect from SQL injections.
        public static readonly List<char> ANSNoAngleBrackets = new List<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
            'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '~', '`', '@', '#', '$', '%', '^', '&', '!', '*', '(', ')', '_', '-', '+', '=', '{',
            '[', '}', ']', '|', '\\', '"', '\'', ':', ';', '?', '/', '.', ','
        };

        public const int MaxRepetitionOrSequence = 3;

        public static readonly List<string> ContextSpecificWords = new List<string>()
        {
            "exogredient"
        };

        public static readonly List<char> LettersLower = new List<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
            'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        public static readonly List<char> LettersUpper = new List<char>()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q',
            'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        public static readonly List<char> Numbers = new List<char>()
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static readonly IDictionary<char, int> LettersLowerToPositions = new Dictionary<char, int>()
        {
            {'a', 1}, {'b', 2}, {'c', 3}, {'d', 4}, {'e', 5}, {'f', 6}, {'g', 7}, {'h', 8},
            {'i', 9}, {'j', 10}, {'k', 11}, {'l', 12}, {'m', 13}, {'n', 14}, {'o', 15}, {'p', 16},
            {'q', 17}, {'r', 18}, {'s', 19}, {'t', 20}, {'u', 21}, {'v', 22}, {'w', 23}, {'x', 24},
            {'y', 25}, {'z', 26}
        };

        public static readonly IDictionary<char, int> LettersUpperToPositions = new Dictionary<char, int>()
        {
            {'A', 1}, {'B', 2}, {'C', 3}, {'D', 4}, {'E', 5}, {'F', 6}, {'G', 7}, {'H', 8},
            {'I', 9}, {'J', 10}, {'K', 11}, {'L', 12}, {'M', 13}, {'N', 14}, {'O', 15}, {'P', 16},
            {'Q', 17}, {'R', 18}, {'S', 19}, {'T', 20}, {'U', 21}, {'V', 22}, {'W', 23}, {'X', 24},
            {'Y', 25}, {'Z', 26}
        };

        public static readonly IDictionary<int, char> PositionsToLettersLower = new Dictionary<int, char>()
        {
            {1, 'a'}, {2, 'b'}, {3, 'c'}, {4, 'd'}, {5, 'e'}, {6, 'f'}, {7, 'g'}, {8, 'h'},
            {9, 'i'}, {10, 'j'}, {11, 'k'}, {12, 'l'}, {13, 'm'}, {14, 'n'}, {15, 'o'}, {16, 'p'},
            {17, 'q'}, {18, 'r'}, {19, 's'}, {20, 't'}, {21, 'u'}, {22, 'v'}, {23, 'w'}, {24, 'x'},
            {25, 'y'}, {26, 'z'}
        };

        public static readonly IDictionary<int, char> PositionsToLettersUpper = new Dictionary<int, char>()
        {
            {1, 'A'}, {2, 'B'}, {3, 'C'}, {4, 'D'}, {5, 'E'}, {6, 'F'}, {7, 'G'}, {8, 'H'},
            {9, 'I'}, {10, 'J'}, {11, 'K'}, {12, 'L'}, {13, 'M'}, {14, 'N'}, {15, 'O'}, {16, 'P'},
            {17, 'Q'}, {18, 'R'}, {19, 'S'}, {20, 'T'}, {21, 'U'}, {22, 'V'}, {23, 'W'}, {24, 'X'},
            {25, 'Y'}, {26, 'Z'}
        };


        // EXCEPTION MESSAGES -- Authorization
        public const string UserTypeIdNotProvided = "UserType or ID was not provided.";
        public const string JWSthreeSegments = "JWS must have 3 segments separated by periods.";
        public const string IncorrectEncryption = "Incorrect encryption algorithm.";
        public const string PubKeyNotFound = "Public key not found in the JWS payload!";
        public const string JWSNotVerified = "JWS could not be verified!";
        public const string ExpirationNotSpecified = "Expiration time is not specified!";
        public const string ExpirationNotNumeric = "Expiration time is not a number!";
        public const string DictionaryMissingBrackets = "Dictionary doesn't have proper surrounding brackets.";
        public const string InvalidCommaColon = "Invalid comma and / or colon formatting.";
        public const string InvalidKeyValue = "Invalid key/value pair.";
        public const string KeyValueNoDoubleQuotes = "Key or value isn't surrounded by double quotes.";
        public const string KeyValueNotAlphaNum = "Key or value is not alpha-numeric (excluding white-space).";

        // EXCEPTION MESSAGES -- Data Store Logging
        public const string TimestampFormatIncorrect = "Timestamp Format Incorrect";

        // EXCEPTION MESSAGES -- User Management
        public const string UsernameDNE = "The username doesn't exsit.";
        public const string UserLocked = "This user is locked!";
    }
}
