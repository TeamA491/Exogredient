namespace TeamA.Exogredient.LoggingManager
{
    using System;
    using TeamA.Exogredient.FlatFileLoggingService;
    using TeamA.Exogredient.DataStoreLoggingService;

    public class LoggingManager
    {
        public LoggingManager() { }

        public void Log(string operation, DateTime timestamp, string userType, string username,
                        string IPAddress, string errorType)
        {
            FlatFileLoggingService ffLogger = new FlatFileLoggingService();

            System.Console.WriteLine("Here in the manager.");
        }
    }
}
