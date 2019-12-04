using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterNOSQLDAO<T>
    {
        public abstract Task<bool> CreateAsync(object record, string yyyymmdd);

        public abstract Task<bool> DeleteAsync(T uniqueId, string yyyymmdd);

        public abstract Task<string> FindIdFieldAsync(object record, string yyyymmdd);
    }
}
