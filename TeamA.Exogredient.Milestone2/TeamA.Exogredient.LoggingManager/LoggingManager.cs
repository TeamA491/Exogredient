using System;
using TeamA.Exogredient.FlatFileLoggingService;
using TeamA.Exogredient.DataStoreLoggingService;

namespace TeamA.Exogredient.LoggingManager
{
    public class LoggingManager
    {
        public void Log(string operation, DateTime timestamp, string userType, string username,
                        string IPAddress, string errorType)
        {
            FlatFileLoggingService.FlatFileLoggingService ffLogger = new FlatFileLoggingService.FlatFileLoggingService();

            System.Console.WriteLine("Here in the manager.");
        }
    }
}
