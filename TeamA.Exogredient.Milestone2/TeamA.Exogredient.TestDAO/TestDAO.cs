using System;
using System.Collections.Generic;
using TeamA.Exogredient.MasterSQLDAO;
using MySql.Data;
using MySql.Data.MySqlClient;
//using TeamA.Exogredient.TestRecord;

namespace TeamA.Exogredient.TestDAO
{

    public class TestDAO : MasterSQLDAO<int>
    {
        private string _tableName = "test_table";
        private string _testColumn = "testColumn";

        public override bool Create(Object record)
        {
            if (record.GetType() == typeof(TestRecord.TestRecord))
            {
                MySqlConnection conn = new MySqlConnection(ConnectionString);

                try
                {
                    conn.Open();

                    string sql = $"INSERT INTO {_tableName}({_testColumn}) VALUES ('{((TestRecord.TestRecord)record).TestColumn}')";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
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

            return false;
        }

        public override bool DeleteByIDs(List<int> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public override List<object> ReadByIDs(List<int> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public override bool Update(int id, object record)
        {
            throw new NotImplementedException();
        }
    }
}
