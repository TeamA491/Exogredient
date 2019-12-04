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
    public class IPAddressDAO : IMasterSQLDAO<string>
    {
        public async Task<bool> CreateAsync(ISQLRecord record)
        {
            if (record.GetType() == typeof(IPAddressRecord))
            {
                IPAddressRecord ipRecord = (IPAddressRecord)record;
                IDictionary<string, object> recordData = ipRecord.GetData();

                MySqlConnection connection = new MySqlConnection(Constants.SQLConnection);
                try
                {
                    connection.Open();
                    string sqlString = $"INSERT INTO {Constants.IPAddressDAOtableName} (";

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Value is string)
                        {
                            if (pair.Value == null)
                            {
                                throw new NoNullAllowedException("All columns in IPRecord must be not null.");
                            }
                        }
                        if (pair.Value is int || pair.Value is long)
                        {
                            if (pair.Value.Equals(-1))
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

        public async Task<bool> DeleteByIdsAsync(List<string> idsOfRows)
        {
            MySqlConnection connection = new MySqlConnection(Constants.SQLConnection);

            try
            {
                connection.Open();
                foreach (string ipAddress in idsOfRows)
                {
                    string sqlString = $"DELETE {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = '{ipAddress}';";
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

        public async Task<IDataObject> ReadByIdAsync(string id)
        {
            IPAddressObject result;

            MySqlConnection connection = new MySqlConnection(Constants.SQLConnection);

            try
            {
                connection.Open();

                string sqlString = $"SELECT * FROM {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = '{id}';";
                MySqlCommand command = new MySqlCommand(sqlString, connection);
                var reader = await command.ExecuteReaderAsync();

                using (DataTable dataTable = new DataTable())
                {
                    dataTable.Load(reader);
                    DataRow row = dataTable.Rows[0];
                    string stringResult = row.ToString();
                    result = new IPAddressObject((string)row[Constants.IPAddressDAOIPColumn], (long)row[Constants.IPAddressDAOtimestampLockedColumn],
                                                 (int)row[Constants.IPAddressDAOregistrationFailuresColumn], (long)row[Constants.IPAddressDAOlastRegFailTimestampColumn]);
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

        public async Task<bool> UpdateAsync(ISQLRecord record)
        {
            if (record.GetType() == typeof(IPAddressRecord))
            {
                MySqlConnection connection = new MySqlConnection(Constants.SQLConnection);
                try
                {
                    connection.Open();
                    IPAddressRecord ipRecord = (IPAddressRecord)record;
                    IDictionary<string, object> recordData = ipRecord.GetData();

                    string sqlString = $"UPDATE {Constants.IPAddressDAOtableName} SET ";

                    foreach (KeyValuePair<string, object> pair in recordData)
                    {
                        if (pair.Key != Constants.IPAddressDAOIPColumn)
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
                    sqlString += $" WHERE {Constants.IPAddressDAOIPColumn} = '{recordData[Constants.IPAddressDAOIPColumn]}';";
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
            MySqlConnection connection = new MySqlConnection(Constants.SQLConnection);
            bool exist;
            try
            {
                // Connect to the database.
                connection.Open();
                // Check if the username exists in the table.
                string sqlString = $"SELECT EXISTS (SELECT * FROM {Constants.IPAddressDAOtableName} WHERE {Constants.IPAddressDAOIPColumn} = '{ipAddress}');";
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
    }
}
