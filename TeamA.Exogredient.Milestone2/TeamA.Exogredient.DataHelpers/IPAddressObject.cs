using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class IPAddressObject : IDataObject
    {
        public string IP { get; }
        public long TimestampLocked { get; }
        public int RegistrationFailures { get; }
        public long LastRegFailTimestamp { get; }

        public IPAddressObject(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            IP = ip;
            TimestampLocked = timestampLocked;
            RegistrationFailures = registrationFailures;
            LastRegFailTimestamp = lastRegFailTimestamp;
        }
    }
}
