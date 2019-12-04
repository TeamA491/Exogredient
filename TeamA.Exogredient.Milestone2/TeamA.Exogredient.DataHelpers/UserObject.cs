using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
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
