using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers.Upload
{
    public class AnalyzeResponse
    {
        public string Message { get; set; }
        public bool ExceptionOccurred { get; set; }
        public string[] Suggestions { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
    }
}
