using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers.Upload
{
    public class SuccessResponse
    {
        public string Message { get; set; }
        public bool ExceptionOccurred { get; set; }
        public bool Success { get; set; }
    }
}
