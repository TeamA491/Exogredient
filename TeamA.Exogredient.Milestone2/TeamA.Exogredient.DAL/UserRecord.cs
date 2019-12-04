using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;


namespace TeamA.Exogredient.DAL
{
    public class UserRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

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

        public UserRecord(string username, string firstName = null, string lastName = null, string email = null,
            string phoneNumber = null, string password = null, int disabled = -1, string userType = null, string salt = null,
            long tempTimestamp = -1, string emailCode = null, long emailCodeTimestamp = -1, int loginFailures = -1,
            long lastLoginFailTimestamp = -1, int emailCodeFailures = -1, int phoneCodeFailures = -1)
        {
            _data.Add(Constants.UserDAOusernameColumn, username);
            Username = username;

            _data.Add(Constants.UserDAOfirstNameColumn, firstName);
            FirstName = firstName;

            _data.Add(Constants.UserDAOlastNameColumn, lastName);
            LastName = lastName;

            _data.Add(Constants.UserDAOemailColumn, email);
            Email = email;

            _data.Add(Constants.UserDAOphoneNumberColumn, phoneNumber);
            PhoneNumber = phoneNumber;

            _data.Add(Constants.UserDAOpasswordColumn, password);
            Password = password;

            _data.Add(Constants.UserDAOdisabledColumn, disabled);
            Disabled = disabled;

            _data.Add(Constants.UserDAOuserTypeColumn, userType);
            UserType = userType;

            _data.Add(Constants.UserDAOsaltColumn, salt);
            Salt = salt;

            _data.Add(Constants.UserDAOtempTimestampColumn, tempTimestamp);
            TempTimestamp = tempTimestamp;

            _data.Add(Constants.UserDAOemailCodeColumn, emailCode);
            EmailCode = emailCode;

            _data.Add(Constants.UserDAOemailCodeTimestampColumn, emailCodeTimestamp);
            EmailCodeTimestamp = emailCodeTimestamp;

            _data.Add(Constants.UserDAOloginFailuresColumn, loginFailures);
            LogInFailures = loginFailures;

            _data.Add(Constants.UserDAOlastLoginFailTimestampColumn, lastLoginFailTimestamp);
            LastLoginFailTimestamp = lastLoginFailTimestamp;

            _data.Add(Constants.UserDAOemailCodeFailuresColumn, emailCodeFailures);
            EmailCodeFailures = emailCodeFailures;

            _data.Add(Constants.UserDAOphoneCodeFailuresColumn, phoneCodeFailures);
            PhoneCodeFailures = phoneCodeFailures;
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
