using System;
using TeamA.Exogredient.Managers;

namespace TeamA.Exogredient.LoggingController
{
    class LoggingController
    {
        static void Main(string[] args)
        {
            LoggingManager manager = new LoggingManager();

            manager.Log(null, new DateTime(), null, null, null, null);

        }
    }
}
