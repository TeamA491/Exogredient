namespace TeamA.Exogredient.DataStoreLoggingService
{
    using System;
    using TeamA.Exogredient.DataStoreLoggingDAO;

    public class DataStoreLoggingService
    {
        public DataStoreLoggingService() { }

        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            return false;
        }
    }
}
