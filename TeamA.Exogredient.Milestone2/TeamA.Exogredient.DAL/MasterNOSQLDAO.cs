using System;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAO<T>
    {
        // HACK: Change this to your specific password
        protected static readonly string ConnectionString = "mysqlx://root:****@localhost:33060";

        protected static readonly string Schema = "exogredient_logs";

        public abstract void Create(object record, string collectionname);

        public abstract void Delete(T uniqueId, string collectionName);

        public abstract string FindIdField(object record, string collectionName);
    }
}
