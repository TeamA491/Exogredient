using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public static class AuthorizationManager
    {
        public static bool AuthorizeUser(string operation, string jwsToken)
        {
            try
            {
                // Decrypt it here to make sure the token wasn't tampered with
                // or it isn't valid
                Dictionary<string, string> payload = AuthorizationService.DecryptJWS(jwsToken);

                if (AuthorizationService.TokenIsExpired(jwsToken))
                    return false;

                // Only refresh the token if it isn't expired
                jwsToken = AuthorizationService.RefreshJWS(jwsToken);

                // Make sure the user type is an int
                int userType;
                bool isInt = int.TryParse(payload[Constants.UserTypeKey], out userType);
                if (!isInt)
                    return false;

                // Check if this user has permission to access the resource
                return AuthorizationService.UserHasPermissionForOperation(userType, operation);
            }
            catch
            {
                return false;
            }
        }
    }
}
