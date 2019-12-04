using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class IPRecord
    {
        private static readonly IDictionary<string, string> _data = new Dictionary<string, string>();

        public IPRecord(string ip, string timestampLocked = null, string registrationFailures = null,
                        string lastRegFailTimestamp = null)
        {
            _data.Add(Constants.IPAddressDAOIPColumn, ip);
            _data.Add(Constants.IPAddressDAOtimestampLockedColumn, timestampLocked);
            _data.Add(Constants.IPAddressDAOregistrationFailuresColumn, registrationFailures);
            _data.Add(Constants.IPAddressDAOlastRegFailTimestampColumn, lastRegFailTimestamp);
        }

        public IDictionary<string, string> GetData()
        {
            return _data;
        }
    }
}
