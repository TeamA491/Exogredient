using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class IPRecord : IRecord
    {
        private static readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public IPRecord(string ip, long timestampLocked = -1, int registrationFailures = -1,
                        long lastRegFailTimestamp = -1)
        {
            _data.Add(Constants.IPAddressDAOIPColumn, ip);
            _data.Add(Constants.IPAddressDAOtimestampLockedColumn, timestampLocked);
            _data.Add(Constants.IPAddressDAOregistrationFailuresColumn, registrationFailures);
            _data.Add(Constants.IPAddressDAOlastRegFailTimestampColumn, lastRegFailTimestamp);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}
