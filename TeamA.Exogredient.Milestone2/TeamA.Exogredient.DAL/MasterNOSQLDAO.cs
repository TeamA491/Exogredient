using System;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAO<T>
    {
        protected static readonly string ConnectionString = "mysqlx://root:****@localhost:3306";

        public abstract void Create(string json);

        public abstract void Read(T uniqueID);

        public abstract void Delete(T uniqueID);
    }
}
