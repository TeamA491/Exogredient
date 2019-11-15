using System;
using TeamA.Exogredient.FlatFileLoggingDAO;

namespace TeamA.Exogredient.FlatFileLoggingService
{
    public class FlatFileLoggingService
    {
        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            FFLoggingDAO ffDAO = new FFLoggingDAO();
            return false;
        }
    }
}
