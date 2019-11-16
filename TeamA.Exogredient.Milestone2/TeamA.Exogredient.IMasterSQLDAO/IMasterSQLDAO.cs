using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.IMasterSQLDAO
{
    public interface IMasterSQLDAO<T>
    {
        public static readonly string _connectionString = "server=localhost;user=root;database=exogredient;port=3306;password=password";

        // Create a record in the data store based on the model argument.
        public bool Create(Object record);

        // Deletes all rows based on each primary key id in the argument list.
        public bool DeleteByIDs(List<T> idsOfRows);

        // Reads all rows based on each primary key id in the argument list.
        public List<Object> ReadByIDs(List<T> idsOfRows);

        // Updates the row marked by the primary key id using the record object.
        // The "record" object defines default values for every column of the table,
        // the user passes named values to each column they wish to update.
        // Example: record = User(lastName : "Example")
        //          Update("jason1234", record)
        public bool Update(T id, Object record);
    }
}
