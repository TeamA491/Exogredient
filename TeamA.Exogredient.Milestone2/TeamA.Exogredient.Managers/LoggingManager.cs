using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class LoggingManager
    {
        public void Log(string operation, DateTime timestamp, string userType, string username,
                        string IPAddress, string errorType)
        {
            FlatFileLoggingService ffLogger = new FlatFileLoggingService();

            Console.WriteLine("Here in the manager.");
        }
    }
}
