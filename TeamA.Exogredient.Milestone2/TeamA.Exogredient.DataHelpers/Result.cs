namespace TeamA.Exogredient.DataHelpers
{
    public class Result<T>
    {
        public string ErrorMessage { get; }

        public T Data { get; set; }

        public Result(string message)
        {
            ErrorMessage = message;
        }
    }
}
