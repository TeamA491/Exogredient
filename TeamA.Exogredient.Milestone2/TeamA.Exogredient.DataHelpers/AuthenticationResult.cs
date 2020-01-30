using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class AuthenticationResult
    {
        public bool IsSuccessful { get; }
        public bool UserExists { get; }

        public AuthenticationResult(bool isSuccessful, bool userExists)
        {
            this.IsSuccessful = isSuccessful;
            this.UserExists = userExists;
        }

        public bool GetIsSuccessful()
        {
            return IsSuccessful;
        }
        public bool GetUserExists()
        {
            return UserExists;
        }
    }
}
