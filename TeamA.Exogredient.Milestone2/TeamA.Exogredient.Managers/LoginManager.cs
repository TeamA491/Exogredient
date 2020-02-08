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
        private readonly LoggingService _loggingService;
        private readonly AuthorizationService _authorizationService;
        private readonly IAuthenticationService _authenticationService;

        public LogInManager(UserManagementService userManagementService,
                            LoggingService loggingService,
                            AuthorizationService authorizationService,
                            IAuthenticationService authenService)
        {
            _userManagementService = userManagementService;
            _loggingService = loggingService;
            _authorizationService = authorizationService;
            _authenticationService = authenService;
        }
        

        // Encrypted password, encrypted AES key, and aesIV are all in hex string format.
        public async Task<Result<AuthenticationResult>> LogInAsync(string username, string ipAddress,
                                                          string password, int currentNumExceptions)
        {
            bool authenticationSuccess = false;
            bool userExist = false;
            try
            {
                // If the username doesn't exist.
                if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {

                    // Log the action.
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UsernameDNELogMessage).ConfigureAwait(false);

                    // Return the result of the login failure.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccess, userExist);
                    return SystemUtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenResult, false, currentNumExceptions);
                }

                userExist = true;

                // Get the information of the usernmae.
                UserObject user = await _userManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                // If the username is disabled.
                if (user.Disabled == 1)
                {
                    // Log the action.
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UserDisableLogMessage).ConfigureAwait(false);

                    // Return the result of the disabled username's login try.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccess, userExist);
                    return SystemUtilityService.CreateResult(Constants.UserDisableUserMessage, authenResult, false, currentNumExceptions);
                }

                // Convert the encrypted password hex string to byte array.
                //byte[] encryptedPasswordBytes = UtilityService.HexStringToBytes(encryptedPassword);
                // Convert the encrypted AES key hex string to byte array.
                //byte[] encryptedAESKeyBytes = UtilityService.HexStringToBytes(encryptedAESKey);
                // Convert the AES IV hex string to byte array.
                //byte[] AESIVBytes = UtilityService.HexStringToBytes(aesIV);
                // Convert the username's salt hex string to byte array.
                //byte[] userSaltBytes = UtilityService.HexStringToBytes(user.Salt);
                // Convert the RSA public key hex string to byte array.
                //byte[] publicKeyBytes = UtilityService.HexStringToBytes(Constants.PublicKey);
                // Convert the RSA private key hex string to byte array.
                //byte[] privateKeyBytes = UtilityService.HexStringToBytes(Constants.PrivateKey);


                // Decrypt the encrypted AES key byte array with the RSA private key byte array.
                //byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);
                // Decrypt the encrypted Password byte array with the AES Key byte array & AES IV byte array.
                //string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);
                // Hash the password hex string with the username's salt.
                //string hashedPassword = SecurityService.HashWithKDF(hexPassword, userSaltBytes);

                // If the username's stored password matches the hashed password.
                AuthenticationDTO existing = new AuthenticationDTO(username, user.Password);
                AuthenticationDTO credentials = new AuthenticationDTO(username, password);
                if (_authenticationService.Authenticate(existing,credentials))
                {
                    authenticationSuccess = true;

                    // Create a token for the username.
                    string token = await _authorizationService.CreateTokenAsync(username).ConfigureAwait(false);
                    // Get the path to store the token.
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    path = path + $"{path.Substring(0, 1)}" + Constants.TokenFile;

                    // Save the token the the path.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }

                    // Log the action.
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, username, ipAddress).ConfigureAwait(false);

                    // Return the result of the successful login.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccess, userExist);
                    return SystemUtilityService.CreateResult(Constants.LogInSuccessUserMessage, authenResult, false, currentNumExceptions);
                }
                // If the password doesn't match.
                else
                {
                    // Increment the number of login failure.
                    await _userManagementService.IncrementLoginFailuresAsync(username,
                                                                            Constants.LogInTriesResetTime,
                                                                            Constants.MaxLogInAttempts).ConfigureAwait(false);
                    // Log the action.
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordLogMessage).ConfigureAwait(false);

                    // Return the result of the unsuccessful login.
                    AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccess, userExist);
                    return SystemUtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenResult, false, currentNumExceptions);
                }
            }
            // Catch exceptions.
            catch (Exception e)
            {
                // Log the exception.
                await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                // If the current number of consecutive exceptions has reached the maximum number of retries.
                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    // Notify the system admin.
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.LogInOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                // Return the result of the exception occured.
                AuthenticationResult authenResult = new AuthenticationResult(authenticationSuccess, userExist); 
                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, authenResult, true, currentNumExceptions + 1);
            }
        }
    }
}
