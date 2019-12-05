using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class CorruptedPasswordsDAO : IMasterNOSQLDAOReadOnly
    {
        public async Task<List<string>> ReadAsync()
        {
            using (Session session = MySQLX.GetSession(Constants.NOSQLConnection))
            {

                Schema schema = session.GetSchema(Constants.CorruptedPassSchemaName);

                var collection = schema.GetCollection(Constants.CorruptedPassCollectionName);

                DocResult result = await collection.Find().ExecuteAsync();

                List<string> resultList = new List<string>();

                while (result.Next())
                {
                    // TODO: flesh out columns. make columns into fields.
                    string temp = (string)result.Current[Constants.CorruptedPassPasswordField];

                    resultList.Add(temp);

                }

                return resultList;
            }
        }
    }
}
