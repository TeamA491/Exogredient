namespace TeamA.Exogredient.FlatFileLoggingService
{
    using System;
    using TeamA.Exogredient.FlatFileLoggingDAO;

    public class FlatFileLoggingService
    {
        public FlatFileLoggingService() { }

        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            FlatFileLoggingDAO ffDAO = new FlatFileLoggingDAO();
            return false;
        }
    }
}
