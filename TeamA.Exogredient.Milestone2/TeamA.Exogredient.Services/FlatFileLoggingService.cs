using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class FlatFileLoggingService
    {
        public bool Execute(string operation, DateTime timestamp, string userType, string username,
                            string IPAddress, string errorType)
        {
            FlatFileLoggingDAO ffDAO = new FlatFileLoggingDAO();
            return false;
        }
    }
}
