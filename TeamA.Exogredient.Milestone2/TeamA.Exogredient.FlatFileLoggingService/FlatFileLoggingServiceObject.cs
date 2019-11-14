using System;
using TeamA.Exogredient.FlatFileLoggingDAO;

namespace TeamA.Exogredient.FlatFileLoggingService
{
    public class FlatFileLoggingServiceObject
    {
        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            FFLoggingDAObject ffDAO = new FFLoggingDAObject();
            return false;
        }
    }
}
