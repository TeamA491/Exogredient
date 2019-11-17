using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;

namespace TeamA.Exogredient.DAL
{
    public class TestDAO : MasterSQLDAO<int>
    {
        private string _tableName = "test_table";

        // Columns:
        private string _id = "idtest_table";
        private string _testColumn = "testColumn";

        public override void Create(Object record)
        {
            if (record.GetType() == typeof(TestRecord))
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);

                try
                {
                    connection.Open();

                    string sqlString = $"INSERT INTO {_tableName}({_testColumn}) VALUES ('{((TestRecord)record).TestColumn}');";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    // TODO: throw a proper execption e.g. DBException
                    Console.WriteLine(e.ToString());
                    throw e;
                }
            }
            else
            {
                throw new ArgumentException("Record must be of class TestRecord");
            }
        }

        public override void DeleteByIDs(List<int> idsOfRows)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                connection.Open();

                for (int i = 0; i < idsOfRows.Count; i++)
                {
                    string sqlString = $"DELETE FROM {_tableName} WHERE {_id} = {idsOfRows[i]};";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                // TODO: throw a proper execption e.g. DBException
                throw e;
            }
        }

        public override List<string> ReadByIDs(List<int> idsOfRows)
        {
            //List<TestRecord.TestRecord> result = new List<TestRecord.TestRecord>();
            List<string> result = new List<string>();

            MySqlConnection connection = new MySqlConnection(ConnectionString);

            try
            {
                connection.Open();

                for (int i = 0; i < idsOfRows.Count; i++)
                {
                    string sqlString = $"SELECT * FROM {_tableName} WHERE {_id} = {idsOfRows[i]};";
                    MySqlCommand command = new MySqlCommand(sqlString, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    string stringResult = "";

                    foreach (DataRow row in dataTable.Rows)
                    {
                        stringResult += row[_id].ToString() + "," + row[_testColumn];
                    }

                    result.Add(stringResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                // TODO: throw a proper execption e.g. DBException
                throw e;
            }

            return result;
        }

        public override void Update(int id, Object record)
        {
            if (record.GetType() == typeof(TestRecord))
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);

                try
                {
                    connection.Open();

                    TestRecord testRecord = (TestRecord)record;

                    bool updating = false;
                    bool needComma = false;
                    string sqlString = $"UPDATE {_tableName} SET ";

                    if (testRecord.Id != -1)
                    {
                        updating = true;
                        sqlString += $"{_id} = {testRecord.Id}";
                        needComma = true;
                    }
                    if (!testRecord.TestColumn.Equals(""))
                    {
                        updating = true;

                        if (needComma)
                        {
                            sqlString += ", ";
                        }

                        sqlString += $"{_testColumn} = '{testRecord.TestColumn}'";

                        needComma = true;
                    }

                    if (updating)
                    {
                        sqlString += $" WHERE {_id} = {id};";
                        MySqlCommand command = new MySqlCommand(sqlString, connection);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                    // TODO: throw a proper execption e.g. DBException
                    throw e;
                }
            }
            else
            {
                throw new ArgumentException("Record must be of class TestRecord");
            }
        }
    }
}