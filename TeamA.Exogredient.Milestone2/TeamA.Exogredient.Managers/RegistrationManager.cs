using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public static class RegistrationManager
    {
        // Encrypted password, encrypted AES key, and AES IV are all in hex string format.
        public static async Task<Result<bool>> RegisterAsync(bool scopeAnswer, string firstName, string lastName,
                                                             string email, string username, string phoneNumber,
                                                             string ipAddress, string encryptedPassword,
                                                             string encryptedAESKey, string aesIV)
        {
            try
            {
                bool registrationSuccess = false;

                // Validate there answer to the scope question.
                if (!scopeAnswer)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidScopeLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidScopeUserMassage, registrationSuccess);
                }

                // Check the length of their first name.
                if (!UtilityService.CheckLength(firstName, Constants.MaximumFirstNameCharacters,
                                                Constants.MinimumFirstNameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidFirstNameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidFirstNameLengthUserMessage, registrationSuccess);
                }

                // Check the character requirements of their first name.
                if (!UtilityService.CheckCharacters(firstName, Constants.CharSetsData[Constants.FirstNameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidFirstNameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidFirstNameCharactersUserMessage, registrationSuccess);
                }

                // Check the length of their last name.
                if (!UtilityService.CheckLength(lastName, Constants.MaximumLastNameCharacters,
                                                Constants.MinimumLastNameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidLastNameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLastNameLengthUserMessage, registrationSuccess);
                }

                // Check the character requirements of their last name.
                if (!UtilityService.CheckCharacters(lastName, Constants.CharSetsData[Constants.LastNameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidLastNameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLastNameCharactersUserMessage, registrationSuccess);
                }

                // Check the length of their email.
                if (!UtilityService.CheckLength(email, Constants.MaximumEmailCharacters,
                                                Constants.MinimumEmailCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailLengthUserMessage, registrationSuccess);
                }

                // Check the character requirements of their email.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.EmailCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailCharactersUserMessage, registrationSuccess);
                }

                // Check the format of their email.
                if (!UtilityService.CheckEmailFormatValidity(email))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailFormatMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailFormatMessage, registrationSuccess);
                }

                // Email must be unique after canonicalization.
                string canonicalizedEmail = UtilityService.CanonicalizeEmail(email);

                if (await UserManagementService.CheckEmailExistenceAsync(canonicalizedEmail).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.EmailExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess);
                }

                // Check the length of their username.
                if (!UtilityService.CheckLength(username, Constants.MaximumUsernameCharacters,
                                                Constants.MinimumUsernameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidUsernameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidUsernameLengthUserMessage, registrationSuccess);
                }

                // Check the character requirements of their username.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.UsernameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidUsernameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidUsernameCharactersUserMessage, registrationSuccess);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UsernameExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess);
                }

                // Check the length of their phone number.
                if (!UtilityService.CheckLength(phoneNumber, Constants.PhoneNumberCharacterLength))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPhoneNumberLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPhoneNumberLengthUserMessage, registrationSuccess);
                }

                // Check the character requirements of their phone number.
                if (!UtilityService.CheckCharacters(phoneNumber, Constants.CharSetsData[Constants.PhoneNumberCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPhoneNumberCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPhoneNumberCharactersUserMessage, registrationSuccess);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PhoneNumberExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess);
                }

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
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       registrationSuccess);
                }

                // Check the character requirements of their password.
                if (!UtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordCharactersUserMessage, registrationSuccess);
                }

                // Check if password for context specific words.
                if (UtilityService.ContainsContextSpecificWords(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordContextSpecificMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordContextSpecificMessage,
                                                       registrationSuccess);
                }

                // Check if password contains sequences or repetitions.
                if (UtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordSequencesOrRepetitionsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordSequencesOrRepetitionsUserMessage,
                                                       registrationSuccess);
                }

                // Check if password contains dictionary words.
                if (await UtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordWordsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordWordsUserMessage,
                                                       registrationSuccess);
                }

                // Check if password is a previously corrupted password.
                if (await UtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordCorruptedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordCorruptedUserMessage, registrationSuccess);
                }

                // Successful registration!
                registrationSuccess = true;

                // Hash password with salt
                byte[] saltBytes = SecurityService.GenerateSalt();
                string saltHex = UtilityService.BytesToHexString(saltBytes);

                string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                await AuthenticationService.SendEmailVerificationAsync(username, canonicalizedEmail).ConfigureAwait(false);
                await UserManagementService.CreateUserAsync(true, username, firstName, lastName, canonicalizedEmail,
                                                            phoneNumber, digest, Constants.EnabledStatus, Constants.CustomerUserType,
                                                            saltHex).ConfigureAwait(false);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.RegistrationSuccessUserMessage, registrationSuccess);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
