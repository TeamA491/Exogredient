using System;

namespace TeamA.Exogredient.DataHelpers
{
    public class LogResult
    {
        public string Timestamp { get; set; }
        public string Operation { get; set; }
        public string Identifier { get; set; }
        public string IpAddress { get; set; }
        public string ErrorType { get; set; }

        public LogResult(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            Timestamp = timestamp;
            Operation = operation;
            Identifier = identifier;
            IpAddress = ipAddress;
            ErrorType = errorType;
        }
    }
}
