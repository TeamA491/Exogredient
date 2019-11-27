using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TeamA.Exogredient.DAL
{
    public class UserDAO : MasterSQLDAO<string>
    {
        //Table name
        private const string _tableName = "User";

        //Column names
        private const string _firstName = "first_name";         //VARCHAR(200)
        private const string _lastName = "last_name";           //VARCHAR(200)
        private const string _email = "email";                  //VARCHAR(200)
        private const string _userName = "username";            //VARCHAR(200)
        private const string _phoneNumber = "phone_number";     //VARCHAR(12)
        private const string _password = "password";            //VARCHAR(2000)
        private const string _disabled = "disabled";            //VARCHAR(5)
        private const string _userType = "user_type";           //VARCHAR(11)
        private const string _salt = "salt";                    //VARCHAR(200)

        //check if the username is disabled
        public async Task<bool> IsUserNameDisabledAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool isDisabled;
            try
            {
                connection.Open();
                string sqlString = $"SELECT {_disabled} FROM {_tableName} WHERE {_userName} = '{userName}'";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    isDisabled = reader.GetBoolean(0);
                    return isDisabled;
                }
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

        //Check if the username exists
        public async Task<bool> UserNameExistsAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                connection.Open();
                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_userName} = '{userName}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    exist = reader.GetBoolean(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            finally
            {
                connection.Close();
            }

            return exist;
        }

        //Get the password of the username
        public async Task<Tuple<string, string>> GetStoredPasswordAndSaltAsync(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();

                if (!(await UserNameExistsAsync(userName)))
                {
                    throw new Exception("Invalid user name or password");
                }

                string sqlString = $"SELECT {_password},{_salt}  FROM {_tableName} WHERE {_userName} = '{userName}';";
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
            catch (Exception e)
            {
                throw e;
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
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return false;
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
                    string sqlString = $"DELETE {_tableName} WHERE {_userName} = '{userName}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
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
                    string sqlString = $"SELECT * FROM {_tableName} WHERE {_userName} = '{userName}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    var reader = await command.ExecuteReaderAsync();

                    using (DataTable dataTable = new DataTable())
                    {
                        dataTable.Load(reader);
                        DataRow row = dataTable.Rows[0];
                        string stringResult = row.ToString();
                        result.Add(stringResult);
                    }
                    //stringResult += row[_id].ToString() + "," + row[_testColumn];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
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
                        if (pair.Value != null && pair.Key != _userName)
                        {
                            sqlString += $"{pair.Key} = '{pair.Value}',";
                        }

                    }
                    sqlString = sqlString.Remove(sqlString.Length - 1);
                    sqlString += $" WHERE {_userName} = '{recordData[_userName]}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return false;
            }
        }

        public bool CheckEmailUniquenessAsync(string phonenumber)
        {

            return false;
        }

        public bool CheckUsernameUniquenessAsync(string username)
        {
            return false;
        }

        public bool GenerateTempUser(string username)
        {
            return false;
        }

        public bool DeleteTempUser(string username)
        {
            return false;
        }

        public bool MakeTempUserPerm(string username)
        {
            return false;
        }


    }
}
