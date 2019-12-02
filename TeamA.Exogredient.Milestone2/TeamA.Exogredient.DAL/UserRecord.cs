using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;


namespace TeamA.Exogredient.DAL
{
    public class UserRecord
    {
        IDictionary<string, string> data = new Dictionary<string, string>();

        public UserRecord(string username, string firstName = null, string lastName = null, string email = null, 
            string phoneNumber = null, string password = null, string salt = null,
            string disabled = null, string userType = null, string tempTimestamp = null, string emailCode = null,
            string emailCodeTimestamp = null)
        {
            data.Add(Constants.UserDAOusernameColumn, username);
            data.Add(Constants.UserDAOfirstNameColumn, firstName);
            data.Add(Constants.UserDAOlastNameColumn, lastName);
            data.Add(Constants.UserDAOemailColumn, email);
            data.Add(Constants.UserDAOphoneNumberColumn, phoneNumber);
            data.Add(Constants.UserDAOpasswordColumn, password);
            data.Add(Constants.UserDAOpasswordColumn, disabled);
            data.Add(Constants.UserDAOuserTypeColumn, userType);
            data.Add(Constants.UserDAOsaltColumn, salt);
            data.Add(Constants.UserDAOtempTimestampColumn, tempTimestamp);
            data.Add(Constants.UserDAOemailCodeColumn, emailCode);
            data.Add(Constants.UserDAOemailCodeTimestampColumn, emailCodeTimestamp);
        }

        public IDictionary<string, string> GetData()
        {
            return data;
        }
    }
}
