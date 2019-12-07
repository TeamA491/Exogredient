using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class UpdatePasswordManager
    {
        public static async Task<Result<bool>> UpdatePasswordAsync(string username, string ipAddress,
                                                                   string encryptedPassword, string encryptedAESKey,
                                                                   string aesIV)
        {
            try
            {
                bool updateSuccess = false;

                // Password decryption.
                byte[] encryptedPasswordBytes = UtilityService.HexStringToBytes(encryptedPassword);
                byte[] encryptedAESKeyBytes = UtilityService.HexStringToBytes(encryptedAESKey);
                byte[] AESIVBytes = UtilityService.HexStringToBytes(aesIV);
                byte[] publicKeyBytes = UtilityService.HexStringToBytes(Constants.PublicKey);
                byte[] privateKeyBytes = UtilityService.HexStringToBytes(Constants.PrivateKey);

                byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);
                byte[] passwordBytes = UtilityService.HexStringToBytes(hexPassword);
                string plaintextPassword = UtilityService.BytesToUTF8String(passwordBytes);

                // Check the length of their password.
                if (!UtilityService.CheckLength(plaintextPassword, Constants.MaximumPasswordCharacters,
                                                Constants.MinimumPasswordCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       updateSuccess);
                }

                // Check the character requirements of their password.
                if (!UtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.InvalidPasswordCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordCharactersUserMessage, updateSuccess);
                }

                // Check if password for context specific words.
                if (UtilityService.ContainsContextSpecificWords(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordContextSpecificMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordContextSpecificMessage,
                                                       updateSuccess);
                }

                // Check if password contains sequences or repetitions.
                if (UtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordSequencesOrRepetitionsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordSequencesOrRepetitionsUserMessage,
                                                       updateSuccess);
                }

                // Check if password contains dictionary words.
                if (await UtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordWordsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordWordsUserMessage,
                                                       updateSuccess);
                }

                // Check if password is a previously corrupted password.
                if (await UtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.UpdatePasswordOperation, username, ipAddress,
                                                  Constants.PasswordCorruptedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordCorruptedUserMessage, updateSuccess);
                }

                // Successful update!
                updateSuccess = true;

                // Hash password with salt
                byte[] saltBytes = SecurityService.GenerateSalt();
                string saltHex = UtilityService.BytesToHexString(saltBytes);

                string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                await UserManagementService.ChangePasswordAsync(username, digest, saltHex);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.UpdatePasswordOperation, username, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.UpdatePasswordSuccessUserMessage, updateSuccess);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.UpdatePasswordOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
