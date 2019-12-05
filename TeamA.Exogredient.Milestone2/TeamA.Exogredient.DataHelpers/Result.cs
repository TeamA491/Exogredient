using System;
using System.Collections.Generic;
using System.Text;

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
