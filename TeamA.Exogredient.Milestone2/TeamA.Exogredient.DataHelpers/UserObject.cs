namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents an item stored in the User table.
    /// </summary>
    public class UserObject : IDataObject
    {
        public string Username { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Password { get; }
        public int Disabled { get; }
        public string UserType { get; }
        public string Salt { get; }
        public long TempTimestamp { get; }
        public string EmailCode { get; }
        public long EmailCodeTimestamp { get; }
        public int LogInFailures { get; }
        public long LastLoginFailTimestamp { get; }
        public int EmailCodeFailures { get; }
        public int PhoneCodeFailures { get; }

        /// <summary>
        /// Constructs a UserObject by initializing its public fields.
        /// </summary>
        /// <param name="username">The username stored in the table (string)</param>
        /// <param name="firstName">The first name stored in the table (string)</param>
        /// <param name="lastName">The last name stored in the table (string)</param>
        /// <param name="email">The email address stored in the table (string)</param>
        /// <param name="phoneNumber">The phone number stored in the table (string)</param>
        /// <param name="password">The password digest stored in the table (string)</param>
        /// <param name="disabled">The disabled status of the user stored in the table (int)</param>
        /// <param name="userType">The user type of the user stored in the table (string)</param>
        /// <param name="salt">The salt used to hash the password stored in the table (string)</param>
        /// <param name="tempTimestamp">The timestamp that the temporary user was created at stored in the table (long)</param>
        /// <param name="emailCode">The email code associated with the user stored in the table (string)</param>
        /// <param name="emailCodeTimestamp">The timestamp the email code was sent at stored in the table (long)</param>
        /// <param name="loginFailures">The amount of failures the user currently has for logging in stored in the table (int)</param>
        /// <param name="lastLoginFailTimestamp">The timestamp of the user's last login failure stored in the table (long)</param>
        /// <param name="emailCodeFailures">The amount of failures the user has for entering their email code stored in the table (int)</param>
        /// <param name="phoneCodeFailures">The amount of failures the user has for entering their phone code stored in the table (int)</param>
        public UserObject(string username, string firstName, string lastName, string email,
                          string phoneNumber, string password, int disabled, string userType, string salt,
                          long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
                          long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Disabled = disabled;
            UserType = userType;
            Salt = salt;
            TempTimestamp = tempTimestamp;
            EmailCode = emailCode;
            EmailCodeTimestamp = emailCodeTimestamp;
            LogInFailures = loginFailures;
            LastLoginFailTimestamp = lastLoginFailTimestamp;
            EmailCodeFailures = emailCodeFailures;
            PhoneCodeFailures = phoneCodeFailures;
        }
    }
}
