using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class AuthorizationManager
    {
        readonly AuthorizationService _authorizationService;
        readonly SessionService _sessionService;

        public AuthorizationManager(AuthorizationService authorizationService, SessionService sessionService)
        {
            _authorizationService = authorizationService;
            _sessionService = sessionService;
        }

        public bool AuthorizeUser(string operation, ref string jwtToken)
        {
            try
            {
                // Decrypt it here to make sure the token wasn't tampered with
                // or it isn't valid
                Dictionary<string, string> payload = _authorizationService.DecryptJWT(jwtToken);

                if (_sessionService.TokenIsExpired(jwtToken))
                    return false;

                // Only refresh the token if it isn't expired
                jwtToken = _sessionService.RefreshJWT(jwtToken);

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
