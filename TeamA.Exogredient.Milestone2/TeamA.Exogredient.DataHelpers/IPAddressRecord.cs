using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents a record that is meant to be stored in the IP address table.
    /// </summary>
    public class IPAddressRecord : ISQLRecord, IMaskableRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// Constructs an IPAddressRecord, the ip address is the minimum field required as it serves
        /// as identification.
        /// </summary>
        /// <param name="ip">The ip address to be stored in the table (string)</param>
        /// <param name="timestampLocked">The timestamp the ip was locked to be stored in the table,
        /// if left default it will not be changed during an update (long)</param>
        /// <param name="registrationFailures">The number of registration failures to be stored in the table,
        /// if left default it will not be changed during an update (int)</param>
        /// <param name="lastRegFailTimestamp">The timestamp of the last registration failure to be stored in the table,
        /// if left default it will not be changed during an update (long)</param>
        public IPAddressRecord(string ip, long timestampLocked = -1, int registrationFailures = -1,
                               long lastRegFailTimestamp = -1)
        {
            _data.Add(Constants.IPAddressDAOIPColumn, ip);
            _data.Add(Constants.IPAddressDAOtimestampLockedColumn, timestampLocked);
            _data.Add(Constants.IPAddressDAOregistrationFailuresColumn, registrationFailures);
            _data.Add(Constants.IPAddressDAOlastRegFailTimestampColumn, lastRegFailTimestamp);
        }

        /// <summary>
        /// Gets the internal data of this object.
        /// </summary>
        /// <returns>IDictionary of (string, object)</returns>
        public IDictionary<string, object> GetData()
        {
            return _data;
        }

        public List<Tuple<object, bool>> GetMaskInformation()
        {
            List<Tuple<object, bool>> result = new List<Tuple<object, bool>>
            {
                new Tuple<object, bool>(_data[Constants.IPAddressDAOIPColumn], Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOIPColumn]),
                new Tuple<object, bool>(_data[Constants.IPAddressDAOtimestampLockedColumn], Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOtimestampLockedColumn]),
                new Tuple<object, bool>(_data[Constants.IPAddressDAOregistrationFailuresColumn], Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOregistrationFailuresColumn]),
                new Tuple<object, bool>(_data[Constants.IPAddressDAOlastRegFailTimestampColumn], Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOlastRegFailTimestampColumn])
            };

            return result;
        }

        public Type[] GetParameterTypes()
        {
            Type[] result = new Type[4]
            {
                typeof(string), typeof(long), typeof(int),
                typeof(long)
            };

            return result;
        }
    }
}
