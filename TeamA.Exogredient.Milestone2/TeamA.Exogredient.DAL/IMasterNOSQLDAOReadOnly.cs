using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamA.Exogredient.DAL
{
    public interface IMasterNOSQLDAOReadOnly
    {
        Task<List<string>> ReadAsync();
    }
}
