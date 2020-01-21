using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents a record that is meant to be stored in the Map table.
    /// </summary>
    public class MapRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// Constructs a MapRecord, the hash is the minimum field required as it serves
        /// as identification.
        /// </summary>
        /// <param name="hash">The hash to be stored in the table (string)</param>
        /// <param name="actual">The actual value of the hash to be stored in the table (string)</param>
        /// <param name="occurrences">The number of occurrences of the hash in other tables to be stored in the table (string)</param>
        public MapRecord(string hash, string actual = null, int occurrences = -1)
        {
            _data.Add(Constants.MapDAOHashColumn, hash);
            _data.Add(Constants.MapDAOActualColumn, actual);
            _data.Add(Constants.MapDAOoccurrencesColumn, occurrences);
        }

        /// <summary>
        /// Gets the internal data of this object.
        /// </summary>
        /// <returns>IDictionary of (string, object)</returns>
        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
