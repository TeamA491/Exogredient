using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterSQLDAO<T>
    {
        protected static readonly string ConnectionString = Constants.SQLConnection;

        // Create a record in the data store based on the model argument.
        public abstract Task<bool> CreateAsync(object record);

        // Deletes all rows based on each primary key id in the argument list.
        public abstract Task<bool> DeleteByIdsAsync(List<T> idsOfRows);

        // Reads all rows based on each primary key id in the argument list.
        public abstract Task<List<string>> ReadByIdsAsync(List<T> idsOfRows);

        // Updates the row marked by the primary key id using the record object.
        // The "record" object defines default values for every column of the table,
        // the user passes named values to each column they wish to update.
        // Example: record = User(lastName : "Example")
        //          Update("jason1234", record)
        public abstract Task<bool> UpdateAsync(object record);
    }
}
