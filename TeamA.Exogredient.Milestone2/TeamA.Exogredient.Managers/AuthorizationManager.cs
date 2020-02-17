using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class AuthorizationManager
    {

        readonly AuthorizationService _authorizationService;

        public AuthorizationManager(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


        public bool AuthorizeUser(string operation, string jwsToken)
        {
            try
            {
                // Decrypt it here to make sure the token wasn't tampered with
                // or it isn't valid
                Dictionary<string, string> payload = _authorizationService.DecryptJWS(jwsToken);

                if (_authorizationService.TokenIsExpired(jwsToken))
                    return false;

                // Only refresh the token if it isn't expired
                jwsToken = _authorizationService.RefreshJWS(jwsToken);

                // Make sure the user type is an int
                int userType;
                bool isInt = int.TryParse(payload[Constants.UserTypeKey], out userType);
                if (!isInt)
                    return false;

                // Check if this user has permission to access the resource
                return _authorizationService.UserHasPermissionForOperation(userType, operation);
            }
            catch
            {
                return false;
            }
        }
    }
}
