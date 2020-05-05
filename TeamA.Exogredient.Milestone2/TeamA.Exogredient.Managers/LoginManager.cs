using System;
using System.IO;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class LogInManager
    {

        private readonly UserManagementService _userManagementService;
        private readonly LoggingManager _loggingManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly SessionService _sessionService;

        public LogInManager(UserManagementService userManagementService,
                            LoggingManager loggingManager,
                            IAuthenticationService authenService,
                            SessionService sessionService)
        {
            _userManagementService = userManagementService;
            _loggingManager = loggingManager;
            _authenticationService = authenService;
            _sessionService = sessionService;
        }


        // Encrypted password, encrypted AES key, and aesIV are all in hex string format.
        public async Task<Result<AuthenticationResult>> LogInAsync(string username, string ipAddress,
                                                          string password, int currentNumExceptions)
        {
            bool authenticationSuccessful = false;
            bool userExist = false;
            try
            {
                // If the username doesn't exist.
                if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {

                    // Log the action.
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.UsernameDNELogMessage).ConfigureAwait(false);

                    // Return the result of the login failure.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccessful, userExist);
                    return SystemUtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenResult, false);
                }

                userExist = true;

                // Get the information of the usernmae.
                UserObject user = await _userManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                // If the username is disabled.
                if (user.Disabled == 1)
                {
                    // Log the action.
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.UserDisableLogMessage).ConfigureAwait(false);

                    // Return the result of the disabled username's login try.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccessful, userExist);
                    return SystemUtilityService.CreateResult(Constants.UserDisableUserMessage, authenResult, false);
                }
               
                AuthenticationDTO existing = new AuthenticationDTO(username, user.Password);
                AuthenticationDTO credentials = new AuthenticationDTO(username, password);

                // If the username's stored password matches the hashed password.
                if (_authenticationService.Authenticate(existing,credentials))
                {
                    authenticationSuccessful = true;

                    // Create a token for the username.
                    string token = await _sessionService.CreateTokenAsync(username).ConfigureAwait(false);

                    // Get user type.
                    string userType = await _userManagementService.GetUserTypeAsync(username).ConfigureAwait(false);
                    Console.WriteLine(userType);

                    // Get the path to store the token.
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    path = path + $"{path.Substring(0, 1)}" + Constants.TokenFile;

                    // Save the token the the path.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }

                    // Log the action.
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, username, ipAddress).ConfigureAwait(false);

                    // Return the result of the successful login.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccessful, userExist, token, userType);
                    return SystemUtilityService.CreateResult(Constants.LogInSuccessUserMessage, authenResult, false);
                }
                // If the password doesn't match.
                else
                {
                    // Increment the number of login failure.
                    await _userManagementService.IncrementLoginFailuresAsync(username,
                                                                            Constants.LogInTriesResetTime,
                                                                            Constants.MaxLogInAttempts).ConfigureAwait(false);
                    // Log the action.
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidPasswordLogMessage).ConfigureAwait(false);

                    // Return the result of the unsuccessful login.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccessful, userExist);
                    return SystemUtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenResult, false);
                }
            }
            // Catch exceptions.
            catch (Exception e)
            {
                // Log the exception.
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                // If the current number of consecutive exceptions has reached the maximum number of retries.
                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    // Notify the system admin.
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.LogInOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                // Return the result of the exception occured.
                AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccessful, userExist);
                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, authenResult, true);
            }
        }

        public async Task<string> GetSaltAsync(string username, string ipAddress)
        {
            if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                             Constants.UsernameDNELogMessage).ConfigureAwait(false);

                return null;
            }

            return await _userManagementService.GetSaltAsync(username).ConfigureAwait(false);
        }
    }
}
