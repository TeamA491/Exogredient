using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public class LogRecord
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
                         string identifier, string ipAddress, string errorType = "null")
        {
            Fields = new List<string>();

            Timestamp = timestamp;
            Fields.Add(timestamp);

            Operation = operation;
            Fields.Add(operation);

            Identifier = identifier;
            Fields.Add(identifier);

            IPAddress = ipAddress;
            Fields.Add(ipAddress);

            ErrorType = errorType;
            Fields.Add(errorType);
        }
    }
}
