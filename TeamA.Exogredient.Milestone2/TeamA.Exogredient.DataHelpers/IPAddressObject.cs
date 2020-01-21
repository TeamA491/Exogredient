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

        /// <summary>
        /// Sets the status of this object to "UnMasked".
        /// </summary>
        public void SetToUnMasked()
        {
            _unmasked = true;
        }

        /// <summary>
        /// Evaluates whether or not the object is unmasked.
        /// </summary>
        /// <returns>(bool) whether the object is unmasked.</returns>
        public bool IsUnMasked()
        {
            return _unmasked;
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
                new Tuple<object, bool>(IP, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOIPColumn]),
                new Tuple<object, bool>(TimestampLocked, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOtimestampLockedColumn]),
                new Tuple<object, bool>(RegistrationFailures, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOregistrationFailuresColumn]),
                new Tuple<object, bool>(LastRegFailTimestamp, Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOlastRegFailTimestampColumn])
            };

            return result;
        }
    }
}
