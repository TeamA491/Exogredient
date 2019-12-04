using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class IPAddressRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public string IP { get; }
        public long TimestampLocked { get; }
        public int RegistrationFailures { get; }
        public long LastRegFailTimestamp { get; }

        public IPAddressRecord(string ip, long timestampLocked = -1, int registrationFailures = -1,
                        long lastRegFailTimestamp = -1)
        {
            _data.Add(Constants.IPAddressDAOIPColumn, ip);
            IP = ip;

            _data.Add(Constants.IPAddressDAOtimestampLockedColumn, timestampLocked);
            TimestampLocked = timestampLocked;

            _data.Add(Constants.IPAddressDAOregistrationFailuresColumn, registrationFailures);
            RegistrationFailures = registrationFailures;

            _data.Add(Constants.IPAddressDAOlastRegFailTimestampColumn, lastRegFailTimestamp);
            LastRegFailTimestamp = lastRegFailTimestamp;
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
