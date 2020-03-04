using System;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public class SessionService
    {
        private readonly UserDAO _userDAO;
        private readonly AuthorizationService _authorizationService;

        public SessionService(UserDAO userDAO, AuthorizationService authorizationService)
        {
            _userDAO = userDAO;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Create a token for a logged-in user.
        /// </summary>
        /// <param name="username"> logged-in username </param>
        /// <returns> string of token that represents the user type and unique ID of the username </returns>
        public async Task<string> CreateTokenAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // Get the user type of the username.
            string userType = user.UserType;

            // Craete a dictionary that represents the user type and unique ID.
            Dictionary<string, string> userInfo = new Dictionary<string, string>
            {
                {Constants.UserTypeKey, userType},
                {Constants.IdKey, username }
            };

            return _authorizationService.GenerateJWT(userInfo);
        }

        /// <summary>
        /// Refreshes a token to be active for 20 more minutes.
        /// </summary>
        /// <param name="jwt">The token that needs to be refreshed.</param>
        /// <returns>A new token that has been refreshed and active for 20 more minutes.</returns>
        public string RefreshJWT(string jwt, int minutes = Constants.TOKEN_EXPIRATION_MIN)
        {
            Dictionary<string, string> payload = _authorizationService.DecryptJWT(jwt);

            // Refresh the token for an additional 20 minutes
            payload[Constants.EXPIRATION_FIELD] = TimeUtilityService.GetEpochFromNow(minutes).ToString();

            return _authorizationService.GenerateJWT(payload);
        }

        /// <summary>
        /// Checks whether the current token is expired or not.
        /// </summary>
        /// <param name="jwt">The token to check.</param>
        /// <returns>Whether the current token is past it's 20 minute lifetime.</returns>
        public bool TokenIsExpired(string jwt)
        {
            Dictionary<string, string> payload = _authorizationService.DecryptJWT(jwt);

            // Check if the expiration key exists first
            if (!payload.ContainsKey(Constants.EXPIRATION_FIELD))
                throw new ArgumentException("Expiration time is not specified!");

            long expTime;
            bool isNumeric = long.TryParse(payload[Constants.EXPIRATION_FIELD], out expTime);

            // Make sure we are dealing with a number first
            if (!isNumeric)
                throw new ArgumentException("Expiration time is not a number!");

            return TimeUtilityService.CurrentUnixTime() > expTime;
        }
    }
}
