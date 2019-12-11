using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class UnitTestUserDAO
    {
        readonly Dictionary<string, UserRecord> User;


        public UnitTestUserDAO()
        {
            User = new Dictionary<string, UserRecord>();
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="record"> a record object that contains the user's information </param>
        /// <returns> true or throw exception </returns>
        public bool Create(ISQLRecord record)
        {
            // Cast the record object to UserRecord.
            UserRecord userRecord;
            try
            {
                userRecord = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserCreateInvalidArgument);
            }

            // Get the data of the user.
            IDictionary<string, object> recordData = userRecord.GetData();

            foreach (KeyValuePair<string, object> pair in recordData)
            {
                if (pair.Value is string)
                {
                    if (pair.Value == null)
                    {
                        // If the data is string and null, throw an exception.
                        throw new NoNullAllowedException(Constants.UserRecordNoNull);
                    }
                }

                if (pair.Value is int || pair.Value is long)
                {
                    if (pair.Value.Equals(-1))
                    {
                        // If the data is int and -1, throw an exception.
                        throw new NoNullAllowedException(Constants.UserRecordNoNull);
                    }
                }
            }

            User.Add((string)recordData[Constants.UserDAOusernameColumn], userRecord);

            return true;
        }

        public bool DeleteByIds(List<string> idsOfRows)
        {
            foreach (string username in idsOfRows)
            {
                if (!CheckUserExistence(username))
                {
                    throw new ArgumentException(Constants.UserDeleteDNE);
                }
                User.Remove(username);
            }

            return true;
        }

        public IDataObject ReadById(string id)
        {
            if (!CheckUserExistence(id))
            {
                throw new ArgumentException(Constants.UserDeleteDNE);
            }

            IDictionary<string, object> data = User[id].GetData();

            return new UserObject((string)data[Constants.UserDAOusernameColumn], (string)data[Constants.UserDAOfirstNameColumn],
                                  (string)data[Constants.UserDAOlastNameColumn], (string)data[Constants.UserDAOemailColumn],
                                  (string)data[Constants.UserDAOphoneNumberColumn], (string)data[Constants.UserDAOpasswordColumn],
                                  (int)data[Constants.UserDAOdisabledColumn], (string)data[Constants.UserDAOuserTypeColumn],
                                  (string)data[Constants.UserDAOsaltColumn], (long)data[Constants.UserDAOtempTimestampColumn],
                                  (string)data[Constants.UserDAOemailCodeColumn], (long)data[Constants.UserDAOemailCodeTimestampColumn],
                                  (int)data[Constants.UserDAOloginFailuresColumn], (long)data[Constants.UserDAOlastLoginFailTimestampColumn],
                                  (int)data[Constants.UserDAOemailCodeFailuresColumn], (int)data[Constants.UserDAOphoneCodeFailuresColumn]);
        }

        public bool Update(ISQLRecord record)
        {
            UserRecord userRecord;
            try
            {
                userRecord = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserUpdateInvalidArgument);
            }

            IDictionary<string, object> newRecordData = userRecord.GetData();
            if(!CheckUserExistence( (string)newRecordData[Constants.UserDAOusernameColumn]))
            {
                throw new ArgumentException(Constants.UserUpdateDNE);
            }
            IDictionary<string, object> existingRecordData = User[(string)newRecordData[Constants.UserDAOusernameColumn]].GetData();

            foreach (KeyValuePair<string, object> pair in newRecordData)
            {
                if (pair.Value is int && (int)pair.Value != -1)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
                else if (pair.Value is long && (long)pair.Value != (long)-1)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
                else if (!(pair.Value is int || pair.Value is long) && pair.Value != null)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
            }

            return true;
        }

        public bool CheckUserExistence(string username)
        {
            return User.ContainsKey(username);
        }

        /*
        public bool CheckDataExistence(string colName, object data)
        {
            Dictionary<string, UserRecord>.ValueCollection users = User.Values;

            foreach (UserRecord user in users)
            {
                if (user.GetData()[colName].Equals(data)) return true;
            }
            return false;
        }
        */

        public bool CheckPhoneNumberExistence(string phoneNumber)
        {
            Dictionary<string, UserRecord>.ValueCollection users = User.Values;

            foreach (UserRecord user in users)
            {
                if ((string)user.GetData()[Constants.UserDAOphoneNumberColumn] == phoneNumber) return true;
            }
            return false;
        }

        public bool CheckEmailExistence(string email)
        {
            Dictionary<string, UserRecord>.ValueCollection users = User.Values;

            foreach (UserRecord user in users)
            {
                if ((string)user.GetData()[Constants.UserDAOemailColumn] == email) return true;
            }
            return false;
        }
    }
}   
