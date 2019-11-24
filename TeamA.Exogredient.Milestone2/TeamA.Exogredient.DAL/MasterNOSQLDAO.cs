using System;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAO<T>
    {
        // HACK: Change this to your specific password
        protected static readonly string ConnectionString = "mysqlx://root:****@localhost:33060";

        protected static readonly string Schema = "exogredient_logs";

        public abstract Task<bool> CreateAsync(object record, string yyyymmdd);

        public abstract Task<bool> DeleteAsync(T uniqueId, string yyyymmdd);

        public abstract Task<string> FindIdFieldAsync(object record, string yyyymmdd);
    }
}
