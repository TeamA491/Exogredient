using System;
using System.Collections.Generic;
using TeamA.Exogredient.IMasterSQLDAO;


namespace TeamA.Exogredient.TestDAO
{

    public class TestDAO : IMasterSQLDAO<string>
    {
        public bool Create(object record)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByIDs(List<string> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public List<object> ReadByIDs(List<string> idsOfRows)
        {
            throw new NotImplementedException();
        }

        public bool Update(string id, object record)
        {
            throw new NotImplementedException();
        }
    }
}
