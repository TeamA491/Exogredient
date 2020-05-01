using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class VerifyUploadResult
    {
        public string Message { get; }
        public bool VerificationStatus { get; }

        public VerifyUploadResult(string message, bool verificationStatus)
        {
            Message = message;
            VerificationStatus = verificationStatus;
        }
    }
}
