using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers.Upload
{
    public class CreateResponse
    {
        public string Message { get; set; }
        public bool ExceptionOccurred { get; set; }
        public bool Success { get; set; }
    }
}
