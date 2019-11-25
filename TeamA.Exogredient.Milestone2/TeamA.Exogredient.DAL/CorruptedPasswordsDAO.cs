using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    public class CorruptedPasswordsDAO : MasterNOSQLDAOReadOnly
    {
        private readonly string _id = "_id";

        private readonly string _collectionName = "passwords";

        private readonly string _schema = "corrupted_passwords";

        public async override Task<List<string>> ReadAsync()
        {
            Session session = MySQLX.GetSession(ConnectionString);

            Schema schema = session.GetSchema(_schema);

            var collection = schema.GetCollection(_collectionName);

            DocResult result = await collection.Find().ExecuteAsync();

            List<string> resultList = new List<string>();

            while (result.Next())
            {
                // TODO: flesh out columns. make columns into fields.
                string temp = (string)result.Current["password"];
                string[] splitResult = temp.Split(':');

                resultList.Add(splitResult[0]);

            }

            session.Close();

            return resultList;
        }
    }
}
