using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public interface IMasterSQLCRD<T>
    {
        Task<bool> CreateAsync(ISQLRecord record);

        Task<IDataObject> ReadByIdAsync(T id);

        Task<bool> DeleteByIdsAsync(List<string> idsOfRows);
    }
}
