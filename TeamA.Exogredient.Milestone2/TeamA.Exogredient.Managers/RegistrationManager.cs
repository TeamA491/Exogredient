using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class RegistrationManager
    {

        private readonly UserManagementService _userManagementService;
        private readonly LoggingManager _loggingManager;

        public RegistrationManager(UserManagementService userManagementService,
                                  LoggingManager loggingManager)
        {
            _userManagementService = userManagementService;
            _loggingManager = loggingManager;
        }

        public async Task<bool> TesterAsync(bool isTemp, UserRecord record, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            return await _userManagementService.CreateUserAsync(isTemp, record, adminName, adminIp).ConfigureAwait(false);
        }


        // Time out after X seconds will be conducted in Controllers with Task.Wait

        // Re-trying after exceptions occur will be conducted in Controllers, who will check if an exception occurred and how
        // many exceptions have currently occured after a manager has returned.

        // Encrypted password, encrypted AES key, and AES IV are all in hex string format.
        public async Task<Result<bool>> RegisterAsync(string firstName, string lastName,
                                                      string email, string username, string phoneNumber,
                                                      string ipAddress, string hashedPassword, string salt,
                                                      string proxyPassword, int currentNumExceptions)
        {
            try
            {
                bool registrationSuccess = false;

                // If the ip address is not in our system. Insert into datastore
                if (!await _userManagementService.CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
                {
                    await _userManagementService.CreateIPAsync(ipAddress).ConfigureAwait(false);
                }

                // Grab the user ip object.
                IPAddressObject ip = await _userManagementService.GetIPAddressInfoAsync(ipAddress).ConfigureAwait(false);

                // Set fields for repeated fails to lock them out. 
                long timeLocked = ip.TimestampLocked;
                long maxSeconds = TimeUtilityService.TimespanToSeconds(Constants.MaxIPLockTime);
                long currentUnix = TimeUtilityService.CurrentUnixTime();

                // If the time has passed their max time before unlock, unlock them
                if (timeLocked + maxSeconds < currentUnix && timeLocked != Constants.NoValueLong)
                {
                    await _userManagementService.UnlockIPAsync(ipAddress).ConfigureAwait(false);
                }

                if (await _userManagementService.CheckIfIPLockedAsync(ipAddress).ConfigureAwait(false))
                {
                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.IPLockedLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.IPLockedUserMessage, registrationSuccess, false);
                }

                // Check the length of their first name.
                if (!StringUtilityService.CheckLength(firstName, Constants.MaximumFirstNameCharacters,
                                                Constants.MinimumFirstNameCharacters))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidFirstNameLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidFirstNameLengthUserMessage, registrationSuccess, false);
                }

                // Check the character requirements of their first name.
                if (!StringUtilityService.CheckCharacters(firstName, Constants.CharSetsData[Constants.FirstNameCharacterType]))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidFirstNameCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidFirstNameCharactersUserMessage, registrationSuccess, false);
                }

                // Check the length of their last name.
                if (!StringUtilityService.CheckLength(lastName, Constants.MaximumLastNameCharacters,
                                                Constants.MinimumLastNameCharacters))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidLastNameLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidLastNameLengthUserMessage, registrationSuccess, false);
                }

                // Check the character requirements of their last name.
                if (!StringUtilityService.CheckCharacters(lastName, Constants.CharSetsData[Constants.LastNameCharacterType]))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidLastNameCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidLastNameCharactersUserMessage, registrationSuccess, false);
                }

                // Check the length of their email.
                if (!StringUtilityService.CheckLength(email, Constants.MaximumEmailCharacters,
                                                Constants.MinimumEmailCharacters))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidEmailLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidEmailLengthUserMessage, registrationSuccess, false);
                }

                // Check the character requirements of their email.
                if (!StringUtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.EmailCharacterType]))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidEmailCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidEmailCharactersUserMessage, registrationSuccess, false);
                }

                // Check the format of their email.
                if (!StringUtilityService.CheckEmailFormatValidity(email))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidEmailFormatMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidEmailFormatMessage, registrationSuccess, false);
                }

                // Email must be unique after canonicalization.
                string canonicalizedEmail = StringUtilityService.CanonicalizeEmail(email);

                if (await _userManagementService.CheckEmailExistenceAsync(canonicalizedEmail).ConfigureAwait(false))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.EmailExistsLogMessage).ConfigureAwait(false);

                    Console.WriteLine("Email");
                    return SystemUtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false);
                }

                // Check the length of their username.
                if (!StringUtilityService.CheckLength(username, Constants.MaximumUsernameCharacters,
                                                Constants.MinimumUsernameCharacters))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidUsernameLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidUsernameLengthUserMessage, registrationSuccess, false);
                }

                // Check the character requirements of their username.
                if (!StringUtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.UsernameCharacterType]))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidUsernameCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidUsernameCharactersUserMessage, registrationSuccess, false);
                }

                // Check username uniqueness.
                if (await _userManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.UsernameExistsLogMessage).ConfigureAwait(false);

                    Console.WriteLine("username");
                    return SystemUtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false);
                }

                // Check the length of their phone number.
                if (!StringUtilityService.CheckLength(phoneNumber, Constants.PhoneNumberCharacterLength))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidPhoneNumberLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPhoneNumberLengthUserMessage, registrationSuccess, false);
                }

                // Check the character requirements of their phone number.
                if (!StringUtilityService.CheckCharacters(phoneNumber, Constants.CharSetsData[Constants.PhoneNumberCharacterType]))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidPhoneNumberCharactersLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPhoneNumberCharactersUserMessage, registrationSuccess, false);
                }

                // Check phone number uniqueness.
                if (await _userManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.PhoneNumberExistsLogMessage).ConfigureAwait(false);

                    Console.WriteLine("phone");
                    return SystemUtilityService.CreateResult(Constants.UniqueIdExistsRegistrationUserMessage, registrationSuccess, false);
                }

                // Check the length of their password.
                if (!StringUtilityService.CheckLength(proxyPassword, Constants.MaximumPasswordCharacters,
                                                Constants.MinimumPasswordCharacters))
                {
                    await _userManagementService.IncrementRegistrationFailuresAsync(ipAddress,
                                                                                   Constants.RegistrationTriesResetTime,
                                                                                   Constants.MaxRegistrationAttempts).ConfigureAwait(false);

                    _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                 Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                 Constants.InvalidPasswordLengthLogMessage).ConfigureAwait(false);

                    return SystemUtilityService.CreateResult(Constants.InvalidPasswordLengthUserMessage,
                                                       registrationSuccess, false);
                }

                // Successful registration!
                registrationSuccess = true;

                // Create user record object to represent a user.

                // Email code, email code timestamp, login failures, last login failure timestamp, email code failures, and phone code failures initialized to have no value.
                UserRecord user = new UserRecord(username, firstName + " " + lastName, canonicalizedEmail, phoneNumber, hashedPassword, Constants.EnabledStatus, Constants.CustomerUserType,
                                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);
                
                // Create that user.
                await _userManagementService.CreateUserAsync(true, user).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.RegistrationSuccessUserMessage, registrationSuccess, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.RegistrationOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.RegistrationOperation} failed a maximum number of times for {ipAddress}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true);
            }
        }
    }
}
