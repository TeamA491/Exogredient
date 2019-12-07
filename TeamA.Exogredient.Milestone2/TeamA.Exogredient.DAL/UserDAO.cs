using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class UserDAO : IMasterSQLDAO<string>
    {
        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            try
            {
                UserRecord temp = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException("UserDAO.CreateAsync record argument must be of type UserRecord");
            }

            UserRecord userRecord = (UserRecord)record;
            IDictionary<string, object> recordData = userRecord.GetData();

            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                connection.Open();

                string sqlString = $"INSERT INTO {Constants.UserDAOtableName} (";

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    if (pair.Value is string)
                    {
                        if (pair.Value == null)
                        {
                            throw new NoNullAllowedException("All columns in UserRecord must be not null.");
                        }
                    }

                    if (pair.Value is int || pair.Value is long)
                    {
                        if (pair.Value.Equals(-1))
                        {
                            throw new NoNullAllowedException("All columns in UserRecord must be not null.");
                        }
                    }

                    sqlString += $"{pair.Key},";
                }

                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += ") VALUES (";


                // SQL Injection Prevention:
                int count = 0;

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    sqlString += $"@PARAM{count},";
                    count++;
                }

                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += ");";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    count = 0;

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                        count++;
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                    
                return true;
            }
        }

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                connection.Open();

                foreach (string username in idsOfRows)
                {
                    string sqlString = $"DELETE {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @USERNAME;";

                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@USERNAME", username);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                return true;
            }
        }

        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            // TODO: check if user exists first
            UserObject result;

            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                connection.Open();

                string sqlString = $"SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @ID;";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@ID", id);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    dataTable.Load(reader);
                    DataRow row = dataTable.Rows[0];

                    result = new UserObject((string)row[Constants.UserDAOusernameColumn], (string)row[Constants.UserDAOfirstNameColumn],
                                            (string)row[Constants.UserDAOlastNameColumn], (string)row[Constants.UserDAOemailColumn],
                                            (string)row[Constants.UserDAOphoneNumberColumn], (string)row[Constants.UserDAOpasswordColumn],
                                            (bool)row[Constants.UserDAOdisabledColumn] ? 1 : 0, (string)row[Constants.UserDAOuserTypeColumn],
                                            (string)row[Constants.UserDAOsaltColumn], (long)row[Constants.UserDAOtempTimestampColumn],
                                            (string)row[Constants.UserDAOemailCodeColumn], (long)row[Constants.UserDAOemailCodeTimestampColumn],
                                            (int)row[Constants.UserDAOloginFailuresColumn], (long)row[Constants.UserDAOlastLoginFailTimestampColumn],
                                            (int)row[Constants.UserDAOemailCodeFailuresColumn], (int)row[Constants.UserDAOphoneCodeFailuresColumn]);
                }
            }

            return result;
        }

        public async Task<bool> UpdateAsync(ISQLRecord record)
        {
            try
            {
                UserRecord temp = (UserRecord)record;
            }
            catch
            {
                throw new ArgumentException("UserDAO.UpdateAsync record argument must be of type UserRecord");
            }

            UserRecord userRecord = (UserRecord)record;

            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                connection.Open();
                
                IDictionary<string, object> recordData = userRecord.GetData();

                string sqlString = $"UPDATE {Constants.UserDAOtableName} SET ";
                
                int count = 0;

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    if (pair.Key != Constants.UserDAOusernameColumn)
                    {
                        if (pair.Value is int || pair.Value is long)
                        {
                            if (!pair.Value.Equals(-1))
                            {
                                sqlString += $"{pair.Key} = @PARAM{count},";
                            }
                        }
                        else if (pair.Value != null)
                        {
                            sqlString += $"{pair.Key} = @PARAM{count},";
                        }
                    }

                    count++;
                }

                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += $" WHERE {Constants.UserDAOusernameColumn} = '{recordData[Constants.UserDAOusernameColumn]}';";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    count = 0;

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != Constants.UserDAOusernameColumn)
                        {
                            command.Parameters.AddWithValue($"@PARAM{count}", pair.Value);
                        }

                        count++;
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                return true;
            }
        }

        /// <summary>
        /// Check if the username exists.
        /// </summary>
        /// <param name="username"> username to be checked </param>
        /// <returns> true if username exists, otherwise false </returns>
        public async Task<bool> CheckUserExistenceAsync(string username)
        {
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                bool result;

                // Connect to the database.
                connection.Open();

                // Check if the username exists in the table.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = @USERNAME);";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    command.Parameters.AddWithValue("@USERNAME", username);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        /// <summary>
        /// Check if the phone number exists.
        /// </summary>
        /// <param name="phoneNumber"> phone number to be checked </param>
        /// <returns> true if phone number exists, otherwise false </returns>
        public async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                bool result;

                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOphoneNumberColumn} = @PHONENUMBER);";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    command.Parameters.AddWithValue("@PHONENUMBER", phoneNumber);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }

        /// <summary>
        /// Check if the email exists.
        /// </summary>
        /// <param name="email"> email to be checked </param>
        /// <returns> true if email exists, otherwise false </returns>
        public async Task<bool> CheckEmailExistenceAsync(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                bool result;

                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOemailColumn} = @EMAIL);";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    command.Parameters.AddWithValue("@EMAIL", email);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }
    }
}
