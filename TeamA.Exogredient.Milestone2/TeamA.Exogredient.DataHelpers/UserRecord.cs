using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents a record that is meant to be stored in the User table.
    /// </summary>
    public class UserRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// Constructs a UserRecord, the username is the minimum field required as it serves
        /// as identification.
        /// </summary>
        /// <param name="username">The username of the user to be stored in the table (string)</param>
        /// <param name="firstName">The first name of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="lastName">The last name of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="email">The email address of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="phoneNumber">The phone number of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="password">The hashed password of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="disabled">The disabled status of the user to be stored in the table,
        /// if left default it will not be changed during an update (int)</param>
        /// <param name="userType">The user type of the user to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="salt">The salt used in hashing the password to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="tempTimestamp">The timestamp the temporary user was created if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (long)</param>
        /// <param name="emailCode">The email code of the user if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
        /// <param name="emailCodeTimestamp">The timestamp the email code was sent if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (long)</param>
        /// <param name="loginFailures">The amount of login failures currently for the user to be stored in the table,
        /// if left default it will not be changed during an update (int)</param>
        /// <param name="lastLoginFailTimestamp">The timestamp of the user's last login failure if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (long)</param>
        /// <param name="emailCodeFailures">The amount of email code failures currently for the user if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (int)</param>
        /// <param name="phoneCodeFailures">The amount of phone code failures currently for the user if not NOVALUE to be stored in the table,
        /// if left default it will not be changed during an update (string)</param>
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

        /// <summary>
        /// Gets the internal data of this object.
        /// </summary>
        /// <returns>IDictionary of (string, object)</returns>
        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
