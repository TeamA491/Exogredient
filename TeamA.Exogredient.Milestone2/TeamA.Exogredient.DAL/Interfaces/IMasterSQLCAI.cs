using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL.Interfaces
{
    // Master SQL create Auto Increment ID
    public interface IMasterSQLCAI<T>
    {
        Task<T> CreateAsync(ISQLRecord record);
    }
}
