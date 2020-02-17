using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class AuthenticationDTO
    {
        string username;
        string password;

        public AuthenticationDTO(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AuthenticationDTO input = (AuthenticationDTO)obj;

            if (input.username == username && input.password == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
