using System;

namespace TeamA.Exogredient.ErrorHandling
{
    public class Result<T>
    {
        public bool IsSuccessfull { get; }

        public string ErrorMessage { get; }

        public T Data { get; set; }

        public Result(bool isSuccess, string message)
        {
            IsSuccessfull = isSuccess;
            ErrorMessage = message;
        }
    }
}
