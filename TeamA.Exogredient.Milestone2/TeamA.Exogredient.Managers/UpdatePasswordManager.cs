using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class UpdatePasswordManager
    {

        private readonly LoggingManager _loggingManager;
        private readonly UserManagementService _userManagementService;
        private readonly VerificationService _verificationService;
        private readonly SessionService _sessionService;
        private readonly AuthorizationService _authorizationService;

        public UpdatePasswordManager(LoggingManager loggingManager, UserManagementService userManagementService,
            VerificationService verificationService, SessionService sessionService, AuthorizationService authorizationService)
        {
            _loggingManager = loggingManager;
            _userManagementService = userManagementService;
            _verificationService = verificationService;
            _sessionService = sessionService;
            _authorizationService = authorizationService;
        }

        public async Task<bool> SendResetPasswordLinkAsync(string username, string ipAddress)
        {
            try
            {
                if (!await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    return false;
                }

                var userInfo = await _userManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                var email = userInfo.Email;

                await _verificationService.SendResetPasswordLinkAsync(username, email).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.UpdatePasswordOperation, username, ipAddress).ConfigureAwait(false);
                return true;
            }
            catch(Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.UpdatePasswordOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                throw e;
            }
            
        }

        public async Task<Result<bool>> UpdatePasswordAsync(string username, string ipAddress,
                                                            string hashedPassword, string proxyPassword,
                                                            string salt, int currentNumExceptions)
        {
            try
            {
                bool updateSuccess = false;

                //// Password decryption.
                //byte[] encryptedPasswordBytes = StringUtilityService.HexStringToBytes(encryptedPassword);
                //byte[] encryptedAESKeyBytes = StringUtilityService.HexStringToBytes(encryptedAESKey);
                //byte[] AESIVBytes = StringUtilityService.HexStringToBytes(aesIV);
                //byte[] publicKeyBytes = StringUtilityService.HexStringToBytes(Constants.PublicKey);
                //byte[] privateKeyBytes = StringUtilityService.HexStringToBytes(Constants.PrivateKey);

                //byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                //string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);
                //byte[] passwordBytes = StringUtilityService.HexStringToBytes(hexPassword);
                //string plaintextPassword = StringUtilityService.BytesToUTF8String(passwordBytes);

                // Check the length of their password.
                if (!StringUtilityService.CheckLength(proxyPassword, Constants.MaximumPasswordCharacters,
                                                Constants.MinimumPasswordCharacters))
                {
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.UpdatePasswordOperation, username, ipAddress,
                                                 Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       updateSuccess, false);
                }

                // Check the character requirements of their password.
                //if (!StringUtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                //{
                //    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                //                                  Constants.UpdatePasswordOperation, username, ipAddress,
                //                                  Constants.InvalidPasswordCharactersLogMessage).ConfigureAwait(false);

                //    return SystemUtilityService.CreateResult(Constants.InvalidPasswordCharactersUserMessage, updateSuccess, false);
                //}

                // Check if password for context specific words.
                //if (StringUtilityService.ContainsContextSpecificWords(plaintextPassword))
                //{
                //    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                //                                  Constants.UpdatePasswordOperation, username, ipAddress,
                //                                  Constants.PasswordContextSpecificMessage).ConfigureAwait(false);

                //    return SystemUtilityService.CreateResult(Constants.PasswordContextSpecificMessage,
                //                                       updateSuccess, false);
                //}

                // Check if password contains sequences or repetitions.
                //if (StringUtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                //{
                //    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                //                                  Constants.UpdatePasswordOperation, username, ipAddress,
                //                                  Constants.PasswordSequencesOrRepetitionsLogMessage).ConfigureAwait(false);

                //    return SystemUtilityService.CreateResult(Constants.PasswordSequencesOrRepetitionsUserMessage,
                //                                       updateSuccess, false);
                //}

                // Check if password contains dictionary words.
                //if (await StringUtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false))
                //{
                //    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                //                                  Constants.UpdatePasswordOperation, username, ipAddress,
                //                                  Constants.PasswordWordsLogMessage).ConfigureAwait(false);

                //    return SystemUtilityService.CreateResult(Constants.PasswordWordsUserMessage,
                //                                       updateSuccess, false);
                //}

                // Check if password is a previously corrupted password.
                //if (await SystemUtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false))
                //{
                //    await _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                //                                  Constants.UpdatePasswordOperation, username, ipAddress,
                //                                  Constants.PasswordCorruptedLogMessage).ConfigureAwait(false);

                //    return SystemUtilityService.CreateResult(Constants.PasswordCorruptedUserMessage, updateSuccess, false);
                //}

                // Successful update!
                updateSuccess = true;

                // Hash password with salt
                //byte[] saltBytes = SecurityService.GenerateSalt();
                //string saltHex = StringUtilityService.BytesToHexString(saltBytes);

                //string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                await _userManagementService.ChangePasswordAsync(username, hashedPassword, salt).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.UpdatePasswordOperation, username, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.UpdatePasswordSuccessUserMessage, updateSuccess, false);
            }
            catch (Exception e)
            {
                Console.WriteLine("UpdatePassword:" + e.Message);
                Console.WriteLine("UpdatePassword:" + e.StackTrace);
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.UpdatePasswordOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.UpdatePasswordOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true);
            }
        }

        public Result<bool> ValidateToken(string token, string username)
        {
            try
            {
                if (_sessionService.TokenIsExpired(token))
                {
                    return SystemUtilityService.CreateResult(Constants.ResetLinkExpired, false, false);
                }

                var tokenUsername = _authorizationService.DecryptJWT(token)[Constants.IdKey];

                if (username != tokenUsername)
                {
                    return SystemUtilityService.CreateResult(Constants.UsernameNotMatchResetLink, false, false);
                }

                return SystemUtilityService.CreateResult(null, true, false);
            }
            catch(Exception e)
            {
                Console.WriteLine("ValidateToken:" + e.Message);
                Console.WriteLine("ValidateToken:" + e.StackTrace);
                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true);
            }
            
        }
    }
}
