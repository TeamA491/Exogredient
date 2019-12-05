using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class UserRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public UserRecord(string username, string firstName = null, string lastName = null, string email = null,
                          string phoneNumber = null, string password = null, int disabled = -1, string userType = null, string salt = null,
                          long tempTimestamp = -1, string emailCode = null, long emailCodeTimestamp = -1, int loginFailures = -1,
                          long lastLoginFailTimestamp = -1, int emailCodeFailures = -1, int phoneCodeFailures = -1)
        {
            _data.Add(Constants.UserDAOusernameColumn, username);
            _data.Add(Constants.UserDAOfirstNameColumn, firstName);
            _data.Add(Constants.UserDAOlastNameColumn, lastName);
            _data.Add(Constants.UserDAOemailColumn, email);
            _data.Add(Constants.UserDAOphoneNumberColumn, phoneNumber);
            _data.Add(Constants.UserDAOpasswordColumn, password);
            _data.Add(Constants.UserDAOdisabledColumn, disabled);
            _data.Add(Constants.UserDAOuserTypeColumn, userType);
            _data.Add(Constants.UserDAOsaltColumn, salt);
            _data.Add(Constants.UserDAOtempTimestampColumn, tempTimestamp);
            _data.Add(Constants.UserDAOemailCodeColumn, emailCode);
            _data.Add(Constants.UserDAOemailCodeTimestampColumn, emailCodeTimestamp);
            _data.Add(Constants.UserDAOloginFailuresColumn, loginFailures);
            _data.Add(Constants.UserDAOlastLoginFailTimestampColumn, lastLoginFailTimestamp);
            _data.Add(Constants.UserDAOemailCodeFailuresColumn, emailCodeFailures);
            _data.Add(Constants.UserDAOphoneCodeFailuresColumn, phoneCodeFailures);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
