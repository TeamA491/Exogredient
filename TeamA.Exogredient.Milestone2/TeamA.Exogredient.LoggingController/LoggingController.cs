namespace LoggingController
{
    using System;
    using TeamA.Exogredient.LoggingManager;

    class LoggingController
    {
        static void Main(string[] args)
        {
            LoggingManager manager = new LoggingManager();

            manager.Log(null, new DateTime(), null, null, null, null);
        }
    }
}