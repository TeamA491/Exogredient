using System;
using TeamA.Exogredient.LoggingManager;

namespace LoggingController
{
    class LoggingControl
    {
        static void Main(string[] args)
        {
            LoggingManager manager = new LoggingManager();

            manager.Log(null, new DateTime(), null, null, null, null);
        }
    }
}