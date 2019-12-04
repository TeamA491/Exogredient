using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;


namespace TeamA.Exogredient.DAL
{
    public class UserRecord
    {
        private readonly IDictionary<string, string> _data = new Dictionary<string, string>();

        public UserRecord(string username, string firstName = null, string lastName = null, string email = null,
            string phoneNumber = null, string password = null, string disabled = null, string userType = null, string salt = null,
            string tempTimestamp = null, string emailCode = null, string emailCodeTimestamp = null, string loginFailures = null,
            string lastLoginFailTimestamp = null, string emailCodeFailures = null, string phoneCodeFailures = null)
        {
            _data.Add(Constants.UserDAOusernameColumn, username);
            _data.Add(Constants.UserDAOfirstNameColumn, firstName);
            _data.Add(Constants.UserDAOlastNameColumn, lastName);
            _data.Add(Constants.UserDAOemailColumn, email);
            _data.Add(Constants.UserDAOphoneNumberColumn, phoneNumber);
            _data.Add(Constants.UserDAOpasswordColumn, password);
            _data.Add(Constants.UserDAOpasswordColumn, disabled);
            _data.Add(Constants.UserDAOuserTypeColumn, userType);
            _data.Add(Constants.UserDAOsaltColumn, salt);
            _data.Add(Constants.UserDAOtempTimestampColumn, tempTimestamp);
            _data.Add(Constants.UserDAOemailCodeColumn, emailCode);
            _data.Add(Constants.UserDAOemailCodeTimestampColumn, emailCodeTimestamp);
            _data.Add(Constants.UserDAOloginFailuresColmun, loginFailures);
            _data.Add(Constants.UserDAOlastLoginFailTimestampColumn, lastLoginFailTimestamp);
            _data.Add(Constants.UserDAOemailCodeFailuresColumn, emailCodeFailures);
            _data.Add(Constants.UserDAOphoneCodeFailuresColumn, phoneCodeFailures);
        }

        public IDictionary<string, string> GetData()
        {
            return _data;
        }
    }
}
