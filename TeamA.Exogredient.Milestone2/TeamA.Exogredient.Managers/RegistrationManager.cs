using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public static class RegistrationManager
    {
        // Time out after X seconds will be conducted in Controllers with Task.Wait

        // Re-trying after exceptions occur will be conducted in Controllers, who will check if an exception occurred and how
        // many exceptions have currently occured after a manager has returned.

        // Encrypted password, encrypted AES key, and AES IV are all in hex string format.
        public static async Task<Result<bool>> RegisterAsync(bool scopeAnswer, string firstName, string lastName,
                                                             string email, string username, string phoneNumber,
                                                             string ipAddress, string encryptedPassword,
                                                             string encryptedAESKey, string aesIV, int currentNumExceptions) // Connection string, 
        {
            try
            {
                bool registrationSuccess = false;

                // If the ip address is not in our system. Insert into datastore
                if (!await UserManagementService.CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
                {
                    await UserManagementService.CreateIPAsync(ipAddress).ConfigureAwait(false);
                }

                // Grab the user ip object.
                IPAddressObject ip = await UserManagementService.GetIPAddressInfoAsync(ipAddress).ConfigureAwait(false);

                // Set fields for repeated fails to lock them out. 
                long timeLocked = ip.TimestampLocked;
                long maxSeconds = UtilityService.TimespanToSeconds(Constants.MaxIPLockTime);
                long currentUnix = UtilityService.CurrentUnixTime();

                // If the time has passed their max time before unlock, unlock them
                if (timeLocked + maxSeconds < currentUnix && timeLocked != Constants.NoValueLong)
                {
                    await UserManagementService.UnlockIPAsync(ipAddress).ConfigureAwait(false);
                }

                if (await UserManagementService.CheckIfIPLockedAsync(ipAddress).ConfigureAwait(false))
                {
                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.IPLockedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.IPLockedUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // If user is not within our scope incremenent and log the failure to register. 
                if (!scopeAnswer)
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidScopeLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidScopeUserMassage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the length of their first name.
                if (!UtilityService.CheckLength(firstName, Constants.MaximumFirstNameCharacters,
                                                Constants.MinimumFirstNameCharacters))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidFirstNameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidFirstNameLengthUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their first name.
                if (!UtilityService.CheckCharacters(firstName, Constants.CharSetsData[Constants.FirstNameCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidFirstNameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidFirstNameCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the length of their last name.
                if (!UtilityService.CheckLength(lastName, Constants.MaximumLastNameCharacters,
                                                Constants.MinimumLastNameCharacters))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidLastNameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLastNameLengthUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their last name.
                if (!UtilityService.CheckCharacters(lastName, Constants.CharSetsData[Constants.LastNameCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidLastNameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLastNameCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the length of their email.
                if (!UtilityService.CheckLength(email, Constants.MaximumEmailCharacters,
                                                Constants.MinimumEmailCharacters))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailLengthUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their email.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.EmailCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the format of their email.
                if (!UtilityService.CheckEmailFormatValidity(email))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidEmailFormatMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidEmailFormatMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Email must be unique after canonicalization.
                string canonicalizedEmail = UtilityService.CanonicalizeEmail(email);

                if (await UserManagementService.CheckEmailExistenceAsync(canonicalizedEmail).ConfigureAwait(false))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.EmailExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the length of their username.
                if (!UtilityService.CheckLength(username, Constants.MaximumUsernameCharacters,
                                                Constants.MinimumUsernameCharacters))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidUsernameLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidUsernameLengthUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their username.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.UsernameCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidUsernameCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidUsernameCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UsernameExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the length of their phone number.
                if (!UtilityService.CheckLength(phoneNumber, Constants.PhoneNumberCharacterLength))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPhoneNumberLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPhoneNumberLengthUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their phone number.
                if (!UtilityService.CheckCharacters(phoneNumber, Constants.CharSetsData[Constants.PhoneNumberCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPhoneNumberCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPhoneNumberCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PhoneNumberExistsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Password decryption.
                byte[] encryptedPasswordBytes = UtilityService.HexStringToBytes(encryptedPassword);
                byte[] encryptedAESKeyBytes = UtilityService.HexStringToBytes(encryptedAESKey);
                byte[] AESIVBytes = UtilityService.HexStringToBytes(aesIV);
                // Get RSA key information.
                byte[] publicKeyBytes = UtilityService.HexStringToBytes(Constants.PublicKey);
                byte[] privateKeyBytes = UtilityService.HexStringToBytes(Constants.PrivateKey);

                byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                // Get the plain text password from the encrypted one.
                string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);
                byte[] passwordBytes = UtilityService.HexStringToBytes(hexPassword);
                string plaintextPassword = UtilityService.BytesToUTF8String(passwordBytes);

                // Check the length of their password.
                if (!UtilityService.CheckLength(plaintextPassword, Constants.MaximumPasswordCharacters,
                                                Constants.MinimumPasswordCharacters))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       registrationSuccess, false, currentNumExceptions);
                }

                // Check the character requirements of their password.
                if (!UtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordCharactersLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidPasswordCharactersUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Check if password for context specific words.
                if (UtilityService.ContainsContextSpecificWords(plaintextPassword))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordContextSpecificMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordContextSpecificMessage,
                                                       registrationSuccess, false, currentNumExceptions);
                }

                // Check if password contains sequences or repetitions.
                if (UtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordSequencesOrRepetitionsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordSequencesOrRepetitionsUserMessage,
                                                       registrationSuccess, false, currentNumExceptions);
                }

                // Check if password contains dictionary words.
                if (await UtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordWordsLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordWordsUserMessage,
                                                       registrationSuccess, false, currentNumExceptions);
                }

                // Check if password is a previously corrupted password.
                if (await UtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false))
                {
                    await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.PasswordCorruptedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PasswordCorruptedUserMessage, registrationSuccess, false, currentNumExceptions);
                }

                // Successful registration!
                registrationSuccess = true;

                // Hash password with salt
                byte[] saltBytes = SecurityService.GenerateSalt();
                string saltHex = UtilityService.BytesToHexString(saltBytes);

                string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                // Create user record object to represent a user.

                // Email code, email code timestamp, login failures, last login failure timestamp, email code failures, and phone code failures initialized to have no value.
                UserRecord user = new UserRecord(username, firstName + " " + lastName, canonicalizedEmail, phoneNumber, digest, Constants.EnabledStatus, Constants.CustomerUserType,
                                                    saltHex, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);
                
                // Create that user.
                await UserManagementService.CreateUserAsync(true, user, "system", "localhost").ConfigureAwait(false);

                await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.RegistrationSuccessUserMessage, registrationSuccess, false, currentNumExceptions);
            }
            catch (Exception e)
            {
                await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await UserManagementService.NotifySystemAdminAsync($"{Constants.RegistrationOperation} failed a maximum number of times for {ipAddress}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
