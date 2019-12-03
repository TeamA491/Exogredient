using System;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAO<T>
    {
        protected static readonly string ConnectionString = Environment.GetEnvironmentVariable("NOSQL_CONNECTION", EnvironmentVariableTarget.User);

        public abstract Task<bool> CreateAsync(object record, string yyyymmdd);

        public abstract Task<bool> DeleteAsync(T uniqueId, string yyyymmdd);

        public abstract Task<string> FindIdFieldAsync(object record, string yyyymmdd);
    }
}
