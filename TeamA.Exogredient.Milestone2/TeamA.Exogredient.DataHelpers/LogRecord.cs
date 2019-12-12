using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents a Log record to be stored in both the data store
    /// and flat file. Logs keep track of operations performed by users and possible
    /// errors that occurred.
    /// </summary>
    public class LogRecord : INOSQLRecord
    {
        public string Timestamp { get; }

        public string Operation { get; }

        public string Identifier { get; }

        public string IPAddress { get; }

        public string ErrorType { get; }

        public List<string> Fields { get; }

        /// <summary>
        /// Constructs a LogRecord by initialzing its public fields. Will initialize the "Fields" member
        /// in the order of the parameters in this constructor.
        /// </summary>
        /// <param name="timestamp">The timstamp of the operation to be stored (string)</param>
        /// <param name="operation">The operation that was performed to be stored (string)</param>
        /// <param name="identifier">The identifier of the user to be stored (string)</param>
        /// <param name="ipAddress">The ip address of the user to be stored (string)</param>
        /// <param name="errorType">The error type that occured during the operation to be stored (string)</param>
        public LogRecord(string timestamp, string operation,
                         string identifier, string ipAddress, string errorType)
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

        public override bool Equals(object obj)
        {
            LogRecord logRecord;
            try
            {
                logRecord = (LogRecord)obj;
            }
            catch
            {
                throw new ArgumentException();
            }

            return (Timestamp.Equals(logRecord.Timestamp) &&
                   Operation.Equals(logRecord.Operation) &&
                   Identifier.Equals(logRecord.Identifier) &&
                   IPAddress.Equals(logRecord.IPAddress) &&
                   ErrorType.Equals(logRecord.ErrorType));
        }
    }
}
