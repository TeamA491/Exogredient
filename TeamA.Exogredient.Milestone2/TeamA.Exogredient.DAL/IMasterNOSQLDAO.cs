using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// Labels the common methods for normal NOSQL DAOs.
    /// CreateAsync with record and groupName parameters.
    /// </summary>
    /// <typeparam name="T">The type of the unique id</typeparam>
    public interface IMasterNOSQLDAO<T>
    {
        Task<bool> CreateAsync(INOSQLRecord record, string groupName);

        Task<bool> DeleteAsync(T uniqueId, string groupName);

        Task<string> FindIdFieldAsync(INOSQLRecord record, string groupName);
    }
}
