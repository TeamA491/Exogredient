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

        // EMAIL
        public const string SystemEmailAddress = "exogredient.system@gmail.com";
        public const string SystemAdminEmailAddress = "TEAMA.CS491@gmail.com";

        // TWILIO
        public const string TwilioAccountSID = "AC94d03adc3d2da651c16c82932c29b047";
        public const string TwilioPathServiceSID = "VAa9682f046b6f511b9aa1807d4e2949e5";

        // FLAT FILE LOGGING
        public const string LogFolder = @"C:\Logs";
        public const string LogFileType = ".CSV";

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
        public const string UserDAOloginFailuresColmun = "login_failures";                       // INT
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

        public const int SecondsInAnHour = 3600;
        public const int SecondsInAMinute = 60;

        // No < or > to protect from SQL injections.
        public static readonly List<char> AlphaNumericAndSpecialCharacters = new List<char>()
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

    }
}
