using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public abstract class MasterSQLDAO<T>
    {
        // HACK: Change this to your specific password
        protected static readonly string ConnectionString = "server=localhost;user=root;database=exogredient;port=3306;password=****";

        // Create a record in the data store based on the model argument.
        public abstract void Create(object record);

        // Deletes all rows based on each primary key id in the argument list.
        public abstract void DeleteByIDs(List<T> idsOfRows);

        // Reads all rows based on each primary key id in the argument list.
        public abstract List<string> ReadByIDs(List<T> idsOfRows);

        // Updates the row marked by the primary key id using the record object.
        // The "record" object defines default values for every column of the table,
        // the user passes named values to each column they wish to update.
        // Example: record = User(lastName : "Example")
        //          Update("jason1234", record)
        public abstract void Update(T id, object record);
    }
}
