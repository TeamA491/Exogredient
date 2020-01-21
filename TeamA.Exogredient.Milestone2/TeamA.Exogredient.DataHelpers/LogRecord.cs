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
    public class LogRecord : INOSQLRecord, IMaskableRecord
    {
        private bool _masked = false;

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

        /// <summary>
        /// Sets the status of this object to "Masked".
        /// </summary>
        public void SetToMasked()
        {
            _masked = true;
        }

        /// <summary>
        /// Evaluates whether or not the object is masked.
        /// </summary>
        /// <returns>(bool) whether the object is masked.</returns>
        public bool IsMasked()
        {
            return _masked;
        }

        /// <summary>
        /// Evaluates whether an object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to compare this one to.</param>
        /// <returns>(bool) whether the two objects are equal</returns>
        public bool IsEqual(object obj)
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


        /// <summary>
        /// Gets the types of the parameters to this object's constructor.
        /// </summary>
        /// <returns>(Type[]) the array of types of the constructor's parameters</returns>
        public Type[] GetParameterTypes()
        {
            Type[] result = new Type[5]
            {
                typeof(string), typeof(string), typeof(string),
                typeof(string), typeof(string)
            };

            return result;
        }

        /// <summary>
        /// Gets the masking information of this object (a list of tuples: objects and whether they are/should be masked).
        /// </summary>
        /// <returns>(List<Tuple<object, bool>>) the masking information of this object.</object></returns>
        public List<Tuple<object, bool>> GetMaskInformation()
        {
            List<Tuple<object, bool>> result = new List<Tuple<object, bool>>
            {
                new Tuple<object, bool>(Timestamp, Constants.LogsCollectionIsColumnMasked[Constants.LogsTimestampField]),
                new Tuple<object, bool>(Operation, Constants.LogsCollectionIsColumnMasked[Constants.LogsOperationField]),
                new Tuple<object, bool>(Identifier, Constants.LogsCollectionIsColumnMasked[Constants.LogsIdentifierField]),
                new Tuple<object, bool>(IPAddress, Constants.LogsCollectionIsColumnMasked[Constants.LogsIPAddressField]),
                new Tuple<object, bool>(ErrorType, Constants.LogsCollectionIsColumnMasked[Constants.LogsErrorTypeField])
            };

            return result;
        }
    }
}
