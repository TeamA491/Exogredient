namespace LoggingController
{
    using System;
    using TeamA.Exogredient.LoggingManager;
    class Program
    {
        static void Main(string[] args)
        {
            LoggingManager test = new LoggingManager();

            test.Log(null, new DateTime(), null, null, null, null);
        }
    }
}