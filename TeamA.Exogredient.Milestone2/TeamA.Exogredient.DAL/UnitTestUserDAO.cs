using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for the data store containing User information.
    /// </summary>
    public class UnitTestUserDAO
    {
        // Datastore for User.
        readonly Dictionary<string, UserRecord> User;

        public UnitTestUserDAO()
        {
            User = new Dictionary<string, UserRecord>();
        }

        /// <summary>
        /// Asynchronously creates the <paramref name="record"/> in the data store.
        /// </summary>
        /// <param name="record">The record to insert (ISQLRecord)</param>
        /// <returns>(bool) whether the function executed without exception.</returns>
        public bool Create(ISQLRecord record)
        {
            // Try casting the record to a UserRecord, throw an argument exception if it fails.
            UserRecord userRecord;
            try
            {
                userRecord = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserCreateInvalidArgument);
            }

            // Get the data stored in the record.
            IDictionary<string, object> recordData = userRecord.GetData();

            foreach (KeyValuePair<string, object> pair in recordData)
            {
                // Check for null values in the data (string == null, numeric == -1), and throw a NoNullAllowedException
                // if one is found.
                if (pair.Value is int)
                {
                    if ((int)pair.Value == -1)
                    {
                        throw new NoNullAllowedException(Constants.UserRecordNoNull);
                    }
                }
                if (pair.Value is string)
                {
                    if (pair.Value == null)
                    {
                        throw new NoNullAllowedException(Constants.UserRecordNoNull);
                    }
                }
                if (pair.Value is long)
                {
                    if ((long)pair.Value == -1)
                    {
                        throw new NoNullAllowedException(Constants.UserRecordNoNull);
                    }
                }
            }

            // Add the user record to the datastore.
            User.Add((string)recordData[Constants.UserDAOusernameColumn], userRecord);

            return true;
        }

        /// <summary>
        /// Delete all the objects referenced by the <paramref name="idsOfRows"/>.
        /// </summary>
        /// <param name="idsOfRows">The list of ids of rows to delete (List(string))</param>
        /// <returns>(bool) whether the function executed without exception.</returns>
        public bool DeleteByIds(List<string> idsOfRows)
        {
            foreach (string username in idsOfRows)
            {
                // If the username doesn't exist, throw an exception.
                if (!CheckUserExistence(username))
                {
                    throw new ArgumentException(Constants.UserDeleteDNE);
                }
                User.Remove(username);
            }

            return true;
        }

        /// <summary>
        /// Read the information in the adata store pointed to by the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the row to read (string)</param>
        /// <returns>Task (IDataObject) the information represented as an object</returns>
        public IDataObject ReadById(string id)
        {
            // If the username doesn't exist, throw an exception.
            if (!CheckUserExistence(id))
            {
                throw new ArgumentException(Constants.UserDeleteDNE);
            }

            // Get the data of the user.
            IDictionary<string, object> data = User[id].GetData();

            // Return the data in UserObject.
            return new UserObject((string)data[Constants.UserDAOusernameColumn], (string)data[Constants.UserDAOfirstNameColumn],
                                  (string)data[Constants.UserDAOlastNameColumn], (string)data[Constants.UserDAOemailColumn],
                                  (string)data[Constants.UserDAOphoneNumberColumn], (string)data[Constants.UserDAOpasswordColumn],
                                  (int)data[Constants.UserDAOdisabledColumn], (string)data[Constants.UserDAOuserTypeColumn],
                                  (string)data[Constants.UserDAOsaltColumn], (long)data[Constants.UserDAOtempTimestampColumn],
                                  (string)data[Constants.UserDAOemailCodeColumn], (long)data[Constants.UserDAOemailCodeTimestampColumn],
                                  (int)data[Constants.UserDAOloginFailuresColumn], (long)data[Constants.UserDAOlastLoginFailTimestampColumn],
                                  (int)data[Constants.UserDAOemailCodeFailuresColumn], (int)data[Constants.UserDAOphoneCodeFailuresColumn]);
        }

        /// <summary>
        /// Update the <paramref name="record"/> in the data store based on the values that are not null inside it.
        /// </summary>
        /// <param name="record">The record containing the information to update (ISQLRecord)</param>
        /// <returns>(bool) whether the function executed without exception.</returns>
        public bool Update(ISQLRecord record)
        {
            // Try casting the record to a UserRecord, throw an argument exception if it fails.
            UserRecord userRecord;
            try
            {
                userRecord = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.UserUpdateInvalidArgument);
            }

            // Get the data of the new record.
            IDictionary<string, object> newRecordData = userRecord.GetData();
            // Get the username to update.
            string username = (string)newRecordData[Constants.UserDAOusernameColumn];

            // If the username doesn't exist, throw an exception.
            if (!CheckUserExistence(username))
            {
                throw new ArgumentException(Constants.UserUpdateDNE);
            }

            // Get the existing data of the user.
            IDictionary<string, object> existingRecordData = User[username].GetData();

            foreach (KeyValuePair<string, object> pair in newRecordData)
            {
                // Update only the values where the record value is not null (string == null, numeric == -1).
                if (pair.Value is int)
                {
                    if ((int)pair.Value != -1)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
                if (pair.Value is string)
                {
                    if (pair.Value != null)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
                if (pair.Value is long)
                {
                    if ((long)pair.Value != -1)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the <paramref name="username"/> exists.
        /// </summary>
        /// <param name="username"> username to be checked </param>
        /// <returns> true if username exists, otherwise false </returns>
        public bool CheckUserExistence(string username)
        {
            return User.ContainsKey(username);
        }

        /// <summary>
        /// Check if the <paramref name="phoneNumber"/> exists.
        /// </summary>
        /// <param name="phoneNumber"> phone number to be checked </param>
        /// <returns> true if phone number exists, otherwise false </returns>
        public bool CheckPhoneNumberExistence(string phoneNumber)
        {
            Dictionary<string, UserRecord>.ValueCollection users = User.Values;

            foreach (UserRecord user in users)
            {
                if ((string)user.GetData()[Constants.UserDAOphoneNumberColumn] == phoneNumber) return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the <paramref name="email"/> exists.
        /// </summary>
        /// <param name="email"> email to be checked </param>
        /// <returns> true if email exists, otherwise false </returns>
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
