using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents an item stored in the IP address table.
    /// </summary>
    public class IPAddressObject : IDataObject, IUnMaskableObject
    {
        private bool _unmasked = false;

        public string IP { get; }
        public long TimestampLocked { get; }
        public int RegistrationFailures { get; }
        public long LastRegFailTimestamp { get; }

        /// <summary>
        /// Constructs a IPAddressObject by initializing its public fields.
        /// </summary>
        /// <param name="ip">The ip address stored in the table (string)</param>
        /// <param name="timestampLocked">The timestamp stored in the table (long)</param>
        /// <param name="registrationFailures">The registration failures stored in the table (int)</param>
        /// <param name="lastRegFailTimestamp">The last registration failure timestamp stored in the table (long)</param>
        public IPAddressObject(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            IP = ip;
            TimestampLocked = timestampLocked;
            RegistrationFailures = registrationFailures;
            LastRegFailTimestamp = lastRegFailTimestamp;
        }

        public void SetToUnMasked()
        {
            _unmasked = true;
        }

        public bool IsUnMasked()
        {
            return _unmasked;
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

        public List<Tuple<object, bool>> GetMaskInformation()
        {
            List<Tuple<object, bool>> result = new List<Tuple<object, bool>>
            {
                new Tuple<object, bool>(IP, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOIPColumn]),
                new Tuple<object, bool>(TimestampLocked, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOtimestampLockedColumn]),
                new Tuple<object, bool>(RegistrationFailures, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOregistrationFailuresColumn]),
                new Tuple<object, bool>(LastRegFailTimestamp, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOlastRegFailTimestampColumn])
            };

            return result;
        }
    }
}
