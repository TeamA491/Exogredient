namespace TeamA.Exogredient.FlatFileLoggingService
{
    using TeamA.Exogredient.FlatFileLoggingDAO;
    using System;

    public class FlatFileLoggingService
    {
        public FlatFileLoggingService() { }

        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            FlatFileLoggingDAO test = new FlatFileLoggingDAO();
            return false;
        }
    }
}
