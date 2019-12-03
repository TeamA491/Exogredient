using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAOReadOnly
    {
        protected static readonly string ConnectionString = Constants.NOSQLConnection;

        public abstract Task<List<string>> ReadAsync();
    }
}
