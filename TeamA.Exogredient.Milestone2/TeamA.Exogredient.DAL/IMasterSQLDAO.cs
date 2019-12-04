using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public interface IMasterSQLDAO<T>
    {
        // Create a record in the data store based on the model argument.
        Task<bool> CreateAsync(ISQLRecord record);

        // Deletes all rows based on each primary key id in the argument list.
        Task<bool> DeleteByIdsAsync(List<string> idsOfRows);

        // Reads all rows based on each primary key id in the argument list.
        Task<IDataObject> ReadByIdAsync(T id);

        // Updates the row marked by the primary key id using the record object.
        // The "record" object defines default values for every column of the table,
        // the user passes named values to each column they wish to update.
        // Example: record = User(lastName : "Example")
        //          Update("jason1234", record)
        Task<bool> UpdateAsync(ISQLRecord record);
    }
}
