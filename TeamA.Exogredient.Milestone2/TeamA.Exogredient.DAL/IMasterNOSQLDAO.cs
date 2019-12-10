using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public interface IMasterNOSQLDAO<T>
    {
        Task<bool> CreateAsync(INOSQLRecord record, string yyyymmdd);

        Task<bool> DeleteAsync(T uniqueId, string yyyymmdd);

        Task<string> FindIdFieldAsync(INOSQLRecord record, string yyyymmdd);
    }
}
