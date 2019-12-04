using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    public class LogRecord : INOSQLRecord
    {
        public string Timestamp { get; }

        public string Operation { get; }

        // Restrict users from creating account with name of machine.
        public string Identifier { get; }

        public string IPAddress { get; }

        public string ErrorType { get; }

        // All fields in the order you want them to be in, both in the database and flat file
        public List<string> Fields { get; }

        public LogRecord(string timestamp, string operation,
                         string identifier, string ipAddress, string errorType)
        {
            Fields = new List<string>();

            Operation = operation;
            Fields.Add(operation);

            Timestamp = timestamp;
            Fields.Add(timestamp);

            Identifier = identifier;
            Fields.Add(identifier);

            IPAddress = ipAddress;
            Fields.Add(ipAddress);

            ErrorType = errorType;
            Fields.Add(errorType);
        }
    }
}
