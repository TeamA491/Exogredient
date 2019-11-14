using System;
using TeamA.Exogredient.FlatFileLoggingService;
using TeamA.Exogredient.DataStoreLoggingService;

namespace TeamA.Exogredient.LoggingManager
{
    public class LoggingManagerObject
    {
        public void Log(string operation, DateTime timestamp, string userType, string username,
                        string IPAddress, string errorType)
        {
            FlatFileLoggingServiceObject ffLogger = new FlatFileLoggingServiceObject();

            System.Console.WriteLine("Here in the manager.");
        }
    }
}
