namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents data to be returned from a manager to a controller.
    /// Message is the message to be conveyed to the UI.
    /// Data is the data that the controller will process (e.g. bool of success or fail).
    /// ExceptionOccurred is a boolean that conveys whether a system exception occurred during operation.
    /// NumExceptions is the current number of exceptions that have occurred during this call and re-call sequence.
    /// </summary>
    /// <typeparam name="T">The type of data to be stored in the Data field.</typeparam>
    public class Result<T>
    {
        public string Message { get; }

        public T Data { get; set; }

        public bool ExceptionOccurred { get; set; }


        /// <summary>
        /// Constructs the Result object with a message.
        /// </summary>
        /// <param name="message">The message to be stored in this result. Eventually conveyed to the UI. (string)</param>
        public Result(string message)
        {
            Message = message;
        }
    }
}
