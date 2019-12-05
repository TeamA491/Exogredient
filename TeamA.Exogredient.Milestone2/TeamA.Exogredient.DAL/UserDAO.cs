using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    sqlString += $"'{pair.Value}',";
                }

                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += ");";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
                    
                return true;
            }
        }

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            using (MySqlConnection connection = new MySqlConnection(Constants.SQLConnection))
            {
                connection.Open();

                foreach (string userName in idsOfRows)
                {
                    string sqlString = $"DELETE {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = '{userName}';";
                    using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                    {
                        await command.ExecuteNonQueryAsync();
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

                string sqlString = $"SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = '{id}';";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                using (DataTable dataTable = new DataTable())
                {
                    var reader = await command.ExecuteReaderAsync();
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

                foreach (KeyValuePair<string, object> pair in recordData)
                {
                    if (pair.Key != Constants.UserDAOusernameColumn)
                    {
                        if (pair.Value is int || pair.Value is long)
                        {
                            if (!pair.Value.Equals(-1))
                            {
                                sqlString += $"{pair.Key} = '{pair.Value}',";
                            }
                        }
                        else if (pair.Value != null)
                        {
                            sqlString += $"{pair.Key} = '{pair.Value}',";
                        }
                    }

                }
                sqlString = sqlString.Remove(sqlString.Length - 1);
                sqlString += $" WHERE {Constants.UserDAOusernameColumn} = '{recordData[Constants.UserDAOusernameColumn]}';";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    await command.ExecuteNonQueryAsync();
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
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOusernameColumn} = '{username}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
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

                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOphoneNumberColumn} = '{phoneNumber}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
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

                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.UserDAOtableName} WHERE {Constants.UserDAOemailColumn} = '{email}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    result = reader.GetBoolean(0);
                }

                return result;
            }
        }
    }
}
