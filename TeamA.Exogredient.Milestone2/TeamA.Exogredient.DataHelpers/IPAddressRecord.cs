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
        private bool _masked = false;

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

        /// <summary>
        /// Sets the status of this object to "Masked".
        /// </summary>
        public void SetToMasked()
        {
            _masked = true;
        }

        /// <summary>
        /// Evaluates whether or not the object is masked.
        /// </summary>
        /// <returns>(bool) whether the object is masked.</returns>
        public bool IsMasked()
        {
            return _masked;
        }

        /// <summary>
        /// Gets the types of the parameters to this object's constructor.
        /// </summary>
        /// <returns>(Type[]) the array of types of the constructor's parameters</returns>
        public Type[] GetParameterTypes()
        {
            Type[] result = new Type[4]
            {
                typeof(string), typeof(long), typeof(int),
                typeof(long)
            };

            return result;
        }

        /// <summary>
        /// Gets the masking information of this object (a list of tuples: objects and whether they are/should be masked).
        /// </summary>
        /// <returns>(List<Tuple<object, bool>>) the masking information of this object.</object></returns>
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
    }
}
