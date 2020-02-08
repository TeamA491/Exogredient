using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class UpdatePasswordManager
    {

        private readonly LoggingService _loggingService;
        private readonly UserManagementService _userManagementService;

        public UpdatePasswordManager(LoggingService loggingService, UserManagementService userManagementService)
        {
            _loggingService = loggingService;
            _userManagementService = userManagementService;
        }

        public async Task<Result<bool>> UpdatePasswordAsync(string username, string ipAddress,
                                                                   string encryptedPassword, string encryptedAESKey,
                                                                   string aesIV,
                                                                   int currentNumExceptions)
        {
            try
            {
                bool updateSuccess = false;

                // Password decryption.
                byte[] encryptedPasswordBytes = StringUtilityService.HexStringToBytes(encryptedPassword);
                byte[] encryptedAESKeyBytes = StringUtilityService.HexStringToBytes(encryptedAESKey);
                byte[] AESIVBytes = StringUtilityService.HexStringToBytes(aesIV);
                byte[] publicKeyBytes = StringUtilityService.HexStringToBytes(Constants.PublicKey);
                byte[] privateKeyBytes = StringUtilityService.HexStringToBytes(Constants.PrivateKey);

                byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);
                byte[] passwordBytes = StringUtilityService.HexStringToBytes(hexPassword);
                string plaintextPassword = StringUtilityService.BytesToUTF8String(passwordBytes);

                // Check the length of their password.
                if (!StringUtilityService.CheckLength(plaintextPassword, Constants.MaximumPasswordCharacters,
                                                Constants.MinimumPasswordCharacters))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       updateSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their password.
                if (!StringUtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.InvalidPasswordCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPasswordCharactersUserMessage, updateSuccess, false, currentNumExceptions);
                }

                // Check if password for context specific words.
                if (StringUtilityService.ContainsContextSpecificWords(plaintextPassword))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordContextSpecificMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.PasswordContextSpecificMessage,
                                                       updateSuccess, false, currentNumExceptions);
                }

                // Check if password contains sequences or repetitions.
                if (StringUtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordSequencesOrRepetitionsLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.PasswordSequencesOrRepetitionsUserMessage,
                                                       updateSuccess, false, currentNumExceptions);
                }

                // Check if password contains dictionary words.
                if (await StringUtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordWordsLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.PasswordWordsUserMessage,
                                                       updateSuccess, false, currentNumExceptions);
                }

                // Check if password is a previously corrupted password.
                if (await SystemUtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordCorruptedLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.PasswordCorruptedUserMessage, updateSuccess, false, currentNumExceptions);
                }

                // Successful update!
                updateSuccess = true;

                // Hash password with salt
                byte[] saltBytes = SecurityService.GenerateSalt();
                string saltHex = StringUtilityService.BytesToHexString(saltBytes);

                string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                await _userManagementService.ChangePasswordAsync(username, digest, saltHex).ConfigureAwait(false);

                await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.UpdatePasswordOperation, username, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.UpdatePasswordSuccessUserMessage, updateSuccess, false, currentNumExceptions);
            }
            catch (Exception e)
            {
                await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.UpdatePasswordOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.UpdatePasswordOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
