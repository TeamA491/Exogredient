using System;

namespace TeamA.Exogredient.AppConstants
{
    public static class Constants
    {
        public static readonly string PrivateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY", EnvironmentVariableTarget.User);
        public static readonly string PublicKey = Environment.GetEnvironmentVariable("PUBLIC_KEY", EnvironmentVariableTarget.User);
        public static readonly string TwilioAuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN", EnvironmentVariableTarget.User);
        public static readonly string SystemEmailPassword = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD", EnvironmentVariableTarget.User);
        public static readonly string NOSQLConnection = Environment.GetEnvironmentVariable("NOSQL_CONNECTION", EnvironmentVariableTarget.User);
        public static readonly string SQLConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION", EnvironmentVariableTarget.User);
        public static readonly string FTPpassword = Environment.GetEnvironmentVariable("FTP_PASSWORD", EnvironmentVariableTarget.User);

        public const string UserDAOusernameColumn = "username";                       // VARCHAR(200)
        public const string UserDAOfirstNameColumn = "first_name";                    // VARCHAR(200)
        public const string UserDAOlastNameColumn = "last_name";                      // VARCHAR(200)
        public const string UserDAOemailColumn = "email";                             // VARCHAR(200)
        public const string UserDAOphoneNumberColumn = "phone_number";                // VARCHAR(12)
        public const string UserDAOpasswordColumn = "password";                       // VARCHAR(2000)
        public const string UserDAOdisabledColumn = "disabled";                       // VARCHAR(1)
        public const string UserDAOuserTypeColumn = "user_type";                      // VARCHAR(11)
        public const string UserDAOsaltColumn = "salt";                               // VARCHAR(200)
        public const string UserDAOtempTimestampColumn = "temp_timestamp";            // VARCHAR(23)
        //00:00:00 mm-dd-yyyy UTC
        public const string UserDAOemailCodeColumn = "email_code";                    // VARCHAR(6)
        public const string UserDAOemailCodeTimestampColumn = "email_code_timestamp"; // VARCHAR(23)
        //00:00:00 mm-dd-yyyy UTC

        //public const string UserDAOusernameColumn = "username";
        //public const string UserDAOusernameColumn = "username";
    }
}
