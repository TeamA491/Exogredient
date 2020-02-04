using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public static class SQLAuthenticationManager
    {
        public static async Task<AuthenticationResult> Authenticate(string username, string password)
        {
            UserDAO userDAO = new UserDAO();
            UserObject userObject = await userDAO.ReadByIdAsync(username).ConfigureAwait(false) as UserObject;

            AuthenticationDTO credentials = new AuthenticationDTO(username, password);
            AuthenticationDTO existing = new AuthenticationDTO(username, userObject.Password);

            bool result = true;//AuthenticationService.Authenticate(existing, credentials);

            return new AuthenticationResult(result, true);
        }
    }
}
