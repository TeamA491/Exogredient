using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public class LogRecord
    {
        public string Timestamp { get; }

        public string Operation { get; }

        public string Identifier { get; }

        public string IPAddress { get; }

        public string ErrorType { get; }

        public LogRecord(string timestamp, string operation,
                         string identifier, string ipAddress, string errorType = null)
        {
            Timestamp = timestamp;
            Operation = operation;
            Identifier = identifier;
            IPAddress = ipAddress;
            ErrorType = errorType;
        }
    }
}
