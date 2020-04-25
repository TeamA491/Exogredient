using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// Labels the common methods for normal SQL DAOs.
    /// CreateAsync which takes an ISQLRecord and inserts it into the data store.
    /// DeleteByIdsAsync which takes a list of ids and deletes them from the data store.
    /// ReadByIdAsync which takes an id and returns the information as an IDataObject.
    /// UpdateAsync which takes an ISQLRecord and updates the fields that need to be updated in the data store.
    /// </summary>
    /// <typeparam name="T">The type of the id in the data store.</typeparam>
    public interface IMasterSQLDAO<T>
    {
        Task<bool> CreateAsync(ISQLRecord record);

        Task<bool> DeleteByIdsAsync(List<T> idsOfRows);

        Task<IDataObject> ReadByIdAsync(T id);

        Task<bool> UpdateAsync(ISQLRecord record);
    }
}
