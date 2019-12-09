namespace TeamA.Exogredient.DataHelpers
{
    public class Result<T>
    {
        public string Message { get; }

        public T Data { get; set; }

        public bool ExceptionOccurred { get; set; }

        public int NumExceptions { get; set; }

        public Result(string message)
        {
            Message = message;
        }
    }
}
