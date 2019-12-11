using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// Labels the common methods for read only NOSQL DAOs.
    /// ReadAsync, returning a list of strings from the data store.
    /// </summary>
    public interface IMasterNOSQLDAOReadOnly
    {
        Task<List<string>> ReadAsync();
    }
}
