using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class IPAddressDAO : MasterSQLDAO<string>
    {
        // Table name.
        private const string _tableName = Constants.IPAddressDAOtableName;

        // Column names.
        private const string _ip = Constants.IPAddressDAOIPColumn;
        private const string _timestampLocked = Constants.IPAddressDAOtimestampLockedColumn;
        private const string _registrationFailures = Constants.IPAddressDAOregistrationFailuresColumn;
        private const string _lastRegFailTimestamp = Constants.IPAddressDAOlastRegFailTimestampColumn;

        public override async Task<bool> CreateAsync(object record)
        {
            if (record.GetType() == typeof(IPRecord))
            {
                IPRecord ipRecord = (IPRecord)record;
                IDictionary<string, object> recordData = ipRecord.GetData();

                MySqlConnection connection = new MySqlConnection(ConnectionString);
                try
                {
                    connection.Open();
                    string sqlString = $"INSERT INTO {_tableName} (";

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Value is string)
                        {
                            if (pair.Value == null)
                            {
                                throw new NoNullAllowedException("All columns in IPRecord must be not null.");
                            }
                        }
                        if (pair.Value is short || pair.Value is int || pair.Value is long)
                        {
                            if ((long)pair.Value == -1)
                            {
                                throw new NoNullAllowedException("All columns in IPRecord must be not null.");
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
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();

                    return true;
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
                throw new ArgumentException("LockedIPDAO.CreateAsync argument must be of type IPRecord");
            }
        }

        public override async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                foreach (string ipAddress in idsOfRows)
                {
                    string sqlString = $"DELETE {_tableName} WHERE {_ip} = '{ipAddress}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
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

        public override async Task<List<string>> ReadByIdsAsync(List<string> idsOfRows)
        {
            List<string> result = new List<string>();

            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();
                foreach (string ipAddress in idsOfRows)
                {
                    string sqlString = $"SELECT * FROM {_tableName} WHERE {_ip} = '{ipAddress}';";
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
            catch (Exception e)
            {
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
            if (record.GetType() == typeof(IPRecord))
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                try
                {
                    connection.Open();
                    IPRecord ipRecord = (IPRecord)record;
                    IDictionary<string, object> recordData = ipRecord.GetData();

                    string sqlString = $"UPDATE {_tableName} SET ";

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != _ip)
                        {
                            if (pair.Value is short || pair.Value is int || pair.Value is long)
                            {
                                if ((long)pair.Value != -1)
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
                    sqlString += $" WHERE {_ip} = '{recordData[_ip]}';";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    await command.ExecuteNonQueryAsync();

                    return true;
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
                throw new ArgumentException("LockedIPDAO.UpdateAsync argument must be of type IPRecord");
            }
        }

        /// <summary>
        /// Check if the ip exists.
        /// </summary>
        /// <param name="ipAddress"> ip address to be checked </param>
        /// <returns> true if ip address exists, otherwise false </returns>
        public async Task<bool> CheckIPExistenceAsync(string ipAddress)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            bool exist;
            try
            {
                // Connect to the database.
                connection.Open();
                // Check if the username exists in the table.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {_tableName} WHERE {_ip} = '{ipAddress}');";
                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    exist = reader.GetBoolean(0);
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

            return exist;
        }

        public async Task<long> GetTimestamp(string ipAddress)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                // Connect to the database.
                connection.Open();

                string sqlString = $"SELECT {_timestampLocked}  FROM {_tableName} WHERE {_ip} = '{ipAddress}';";
                long result = 0;

                using (MySqlCommand command = new MySqlCommand(sqlString, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    result = (long)reader.GetValue(0);
                    reader.Close();
                }

                return result;
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
