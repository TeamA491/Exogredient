using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAOReadOnly
    {
        // HACK: Change this to your specific password
        protected static readonly string ConnectionString = Environment.GetEnvironmentVariable("NOSQL_CONNECTION", EnvironmentVariableTarget.User);

        public abstract Task<List<string>> ReadAsync();
    }
}
