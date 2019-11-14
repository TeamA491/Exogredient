using System;
using TeamA.Exogredient.LoggingManager;

namespace LoggingController
{
    class LoggingControl
    {
        static void Main(string[] args)
        {
            LoggingManagerObject manager = new LoggingManagerObject();

            manager.Log(null, new DateTime(), null, null, null, null);
        }
    }
}