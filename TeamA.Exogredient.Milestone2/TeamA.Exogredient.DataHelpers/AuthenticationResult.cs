using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class AuthenticationResult
    {
        public bool IsSuccessful { get; }
        public bool UserExists { get; }
        public string Token { get; }
        public string UserType { get; }

        public AuthenticationResult(bool isSuccessful, bool userExists, string token = null, string userType = null)
        {
            IsSuccessful = isSuccessful;
            UserExists = userExists;
            Token = token;
            UserType = userType;
        }
    }
}
