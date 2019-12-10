namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents an item stored in the IP address table.
    /// </summary>
    public class IPAddressObject : IDataObject
    {
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
    }
}
