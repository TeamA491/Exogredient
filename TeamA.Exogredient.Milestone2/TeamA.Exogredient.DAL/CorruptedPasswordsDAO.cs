using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class CorruptedPasswordsDAO : MasterNOSQLDAOReadOnly
    {
        private const string _schema = Constants.CorruptedPassSchemaName;
        private const string _collectionName = Constants.CorruptedPassCollectionName;
        private const string _passwordField = Constants.CorruptedPassPasswordField;

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
                string temp = (string)result.Current[_passwordField];

                resultList.Add(temp);

            }

            session.Close();

            return resultList;
        }
    }
}
