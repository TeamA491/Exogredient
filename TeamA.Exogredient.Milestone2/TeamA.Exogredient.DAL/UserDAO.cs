using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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

        //connection string(for testing)
        new string ConnectionString = "server=localhost;user=root;database=exogredient;port=3306;password=1234567890";

        //check if the username is disabled
        public bool IsUserNameDisabled(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool isDisabled;
            try
            {
                connection.Open();
                string sqlString = $"SELECT {_disabled} FROM {_tableName} WHERE {_userName} = '{userName}'";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();
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
        public bool UserNameExists(string userName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                connection.Open();
                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_userName} = '{userName}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();
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
        public void GetStoredPasswordAndSalt(string userName, out string storedPassword, out string salt)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                if (!UserNameExists(userName))
                {
                    throw new Exception("Invalid user name or password");
                }
                string sqlString = $"SELECT {_password},{_salt}  FROM {_tableName} WHERE {_userName} = '{userName}';";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    storedPassword = reader.GetString(0);
                    salt = reader.GetString(1);
                    reader.Close();
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

        public override void Create(object record)
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
                    command.ExecuteNonQuery();
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
            else
            {
                throw new ArgumentException("Record must be of class UserRecord");
            }
        }

        public override void DeleteByIDs(List<string> idsOfRows)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                foreach (string userName in idsOfRows)
                {
                    string sqlString = $"DELETE {_tableName} WHERE {_userName} = '{userName}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    command.ExecuteNonQuery();
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
        }

        public override List<string> ReadByIDs(List<string> idsOfRows)
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
                    MySqlDataReader reader = command.ExecuteReader();

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

        public override void Update(object record)
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
                    command.ExecuteNonQuery();
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
            }
            else
            {
                throw new ArgumentException("Record must be of class TestRecord");
            }
        }
    }
}
