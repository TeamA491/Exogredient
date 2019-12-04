using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class UserDAO : MasterSQLDAO<string>
    {
        // Table name.
        private const string _tableName = Constants.UserDAOtableName;

        // Column names.
        private const string _username = Constants.UserDAOusernameColumn;
        private const string _firstName = Constants.UserDAOfirstNameColumn;
        private const string _lastName = Constants.UserDAOlastNameColumn;
        private const string _email = Constants.UserDAOemailColumn;
        private const string _phoneNumber = Constants.UserDAOphoneNumberColumn;
        private const string _password = Constants.UserDAOpasswordColumn;
        private const string _disabled = Constants.UserDAOdisabledColumn;
        private const string _userType = Constants.UserDAOuserTypeColumn;
        private const string _salt = Constants.UserDAOsaltColumn;
        private const string _tempTimestamp = Constants.UserDAOtempTimestampColumn;
        private const string _emailCode= Constants.UserDAOemailCodeColumn;
        private const string _emailCodeTimestamp = Constants.UserDAOemailCodeTimestampColumn;
        private const string _loginFailures = Constants.UserDAOloginFailuresColmun;
        private const string _lastLoginFailTimestamp = Constants.UserDAOlastLoginFailTimestampColumn;
        private const string _emailCodeFailures = Constants.UserDAOemailCodeFailuresColumn;
        private const string _phoneCodeFailures = Constants.UserDAOphoneCodeFailuresColumn;

        /// <summary>
        /// Get the hashed password and the salt stored in the database corresponding to the username.
        /// </summary>
        /// <param name="userName"> the username of the password and salt </param>
        public async Task<Tuple<string, string>> GetStoredPasswordAndSaltAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT {_password},{_salt}  FROM {_tableName} WHERE {_username} = '{userName}';";
                string storedPassword = "";
                string salt = "";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    storedPassword = reader.GetString(0);
                    salt = reader.GetString(1);
                    reader.Close();
                }

                return Tuple.Create(storedPassword, salt);
            }
            finally
            {
                connection.Close();
            }
        }


        public override async Task<bool> CreateAsync(object record)
        {
            if (record.GetType() == typeof(UserRecord))
            {
                UserRecord userRecord = (UserRecord)record;
                IDictionary<string, string> recordData = userRecord.GetData();

                MySqlConnection connection = new MySqlConnection(ConnectionString);
                try
                {
                    connection.Open();
                    string sqlString = $"INSERT INTO {_tableName} (";

                    foreach (KeyValuePair<string, string> pair in recordData)
                    {
                        if (pair.Value == null)
                        {
                            throw new NoNullAllowedException("All columns must be not null");
                        }
                        sqlString += $"{pair.Key},";
                    }

                    sqlString = sqlString.Remove(sqlString.Length - 1);
                    sqlString += ") VALUES (";

                    foreach (KeyValuePair<string, string> pair in recordData)
                    {
                        sqlString += $"'{pair.Value}',";
                    }

                    sqlString = sqlString.Remove(sqlString.Length - 1);
                    sqlString += ");";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();

                    return true;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                throw new ArgumentException("UserDAO.CreateAsync argument must be of type UserRecord");
            }
        }

        /// <summary>
        /// Get the user type of the username.
        /// </summary>
        /// <param name="userName"> username whose user type is returned </param>
        /// <returns> the user type of the username </returns>
        public async Task<string> GetUserTypeAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                // Get the user type of the username
                string sqlString = $"SELECT {_userType} FROM {_tableName} WHERE {_username} = '{userName}';";
                string userType;
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    userType = reader.GetString(0);
                    reader.Close();
                }
                return userType;
            }
            finally
            {
                connection.Close();
            }
        }

        public override async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                foreach (string userName in idsOfRows)
                {
                    string sqlString = $"DELETE {_tableName} WHERE {_username} = '{userName}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            finally
            {
                connection.Close();
            }
        }

        public override async Task<List<string>> ReadByIdsAsync(List<string> idsOfRows)
        {
            List<string> result = new List<string>();

            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                foreach (string userName in idsOfRows)
                {
                    string sqlString = $"SELECT * FROM {_tableName} WHERE {_username} = '{userName}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    var reader = await command.ExecuteReaderAsync();

                    using (DataTable dataTable = new DataTable())
                    {
                        dataTable.Load(reader);
                        DataRow row = dataTable.Rows[0];
                        string stringResult = row.ToString();
                        result.Add(stringResult);
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public override async Task<bool> UpdateAsync(object record)
        {
            if (record.GetType() == typeof(UserRecord))
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                try
                {
                    connection.Open();
                    UserRecord userRecord = (UserRecord)record;
                    IDictionary<string, string> recordData = userRecord.GetData();

                    string sqlString = $"UPDATE {_tableName} SET ";

                    foreach (KeyValuePair<string, string> pair in recordData)
                    {
                        if (pair.Value != null && pair.Key != _username)
                        {
                            sqlString += $"{pair.Key} = '{pair.Value}',";
                        }

                    }
                    sqlString = sqlString.Remove(sqlString.Length - 1);
                    sqlString += $" WHERE {_username} = '{recordData[_username]}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();

                    return true;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                throw new ArgumentException("UserDAO.UpdateAsync argument must be of type UserRecord");
            }
        }

        /// <summary>
        /// Check if the username exists.
        /// </summary>
        /// <param name="userName"> username to be checked </param>
        /// <returns> true if username exists, otherwise false </returns>
        public async Task<bool> CheckUserExistenceAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                // Connect to the database.
                connection.Open();
                // Check if the username exists in the table.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_username} = '{userName}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    exist = reader.GetBoolean(0);
                }
            }
            finally
            {
                connection.Close();
            }

            return exist;
        }

        /// <summary>
        /// Check if the phone number exists.
        /// </summary>
        /// <param name="phoneNumber"> phone number to be checked </param>
        /// <returns> true if phone number exists, otherwise false </returns>
        public async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_phoneNumber} = '{phoneNumber}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    exist = reader.GetBoolean(0);
                }
            }
            finally
            {
                connection.Close();
            }

            return exist;
        }

        /// <summary>
        /// Check if the email exists.
        /// </summary>
        /// <param name="email"> email to be checked </param>
        /// <returns> true if email exists, otherwise false </returns>
        public async Task<bool> CheckEmailExistenceAsync(string email)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_email} = '{email}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    exist = reader.GetBoolean(0);
                }
            }
            finally
            {
                connection.Close();
            }

            return exist;
        }

        public async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool result;

            try
            {
                // Connect to the database.
                connection.Open();
                // Check if the username exists in the table.
                string sqlString = $"SELECT {_disabled} FROM {_tableName} WHERE {_username} = '{username}');";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    result = reader.GetString(0) == "1" ? true : false;
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public async Task<Tuple<string, string>> GetEmailCodeAndTimestamp(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT {_emailCode},{_emailCodeTimestamp}  FROM {_tableName} WHERE {_username} = '{userName}';";
                string emailCode = "";
                string emailCodeTimestamp = "";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    emailCode = reader.GetString(0);
                    emailCodeTimestamp = reader.GetString(1);
                    reader.Close();
                }

                return Tuple.Create(emailCode, emailCodeTimestamp);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<string> GetEmailCodeFailureCountAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                if (!(await CheckUserExistenceAsync(userName)))
                {
                    throw new Exception("Invalid user name or password");
                }

                string sqlString = $"SELECT {_emailCodeFailures}  FROM {_tableName} WHERE {_username} = '{userName}';";
                string emailCodeFailureCount = "";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    emailCodeFailureCount = reader.GetString(0);
                    reader.Close();
                }

                return emailCodeFailureCount;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<string> GetPhoneCodeFaiureCountAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                if (!(await CheckUserExistenceAsync(userName)))
                {
                    throw new Exception("Invalid user name or password");
                }

                string sqlString = $"SELECT {_emailCodeFailures}  FROM {_tableName} WHERE {_username} = '{userName}';";
                string phoneCodeFailureCount = "";

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    phoneCodeFailureCount = reader.GetString(0);
                    reader.Close();
                }

                return phoneCodeFailureCount;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
