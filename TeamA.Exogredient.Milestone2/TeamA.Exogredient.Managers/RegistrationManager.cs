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

                // Check existence of username.
                if (await UserManagementService.CheckUserExistenceAsync(username))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Username taken");

                    return UtilityService.CreateResult("Username or password was invalid.", registrationSuccess);
                }

                // Validate there answer to the scope question.
                if (!scopeAnswer)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Username not in scope");

                    return UtilityService.CreateResult("User was not located in California.", registrationSuccess);
                }

                // Check the length of their first name.
                if (!UtilityService.CheckLength(firstName, Constants.MaximumFirstNameCharacters,
                                                Constants.MinimumFirstNameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "First Name length invalid");

                    return UtilityService.CreateResult($"First name length invalid ({Constants.MaximumFirstNameCharacters}" +
                                                       " max)", registrationSuccess);
                }

                // Check the character requirements of their first name.
                if (!UtilityService.CheckCharacters(firstName, Constants.CharSetsData[Constants.FirstNameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "First Name characters invalid");

                    return UtilityService.CreateResult($"First name characters invalid", registrationSuccess);
                }

                // Check the length of their last name.
                if (!UtilityService.CheckLength(lastName, Constants.MaximumLastNameCharacters,
                                                Constants.MinimumLastNameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Last Name length invalid");

                    return UtilityService.CreateResult($"Last name length invalid ({Constants.MaximumLastNameCharacters}" +
                                                       " max).", registrationSuccess);
                }

                // Check the character requirements of their last name.
                if (!UtilityService.CheckCharacters(lastName, Constants.CharSetsData[Constants.LastNameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Last Name characters invalid");

                    return UtilityService.CreateResult($"Last name characters invalid.", registrationSuccess);
                }

                // Check the length of their email.
                if (!UtilityService.CheckLength(email, Constants.MaximumEmailCharacters,
                                                Constants.MinimumEmailCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Email length invalid");

                    return UtilityService.CreateResult($"Email length invalid ({Constants.MaximumEmailCharacters}" +
                                                       " max).", registrationSuccess);
                }

                // Check the character requirements of their email.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.EmailCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Email characters invalid");

                    return UtilityService.CreateResult($"Email characters invalid.", registrationSuccess);
                }

                // Check the format of their email.
                if (!UtilityService.CheckEmailFormatValidity(email))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Email format invalid");

                    return UtilityService.CreateResult($"Email format invalid.", registrationSuccess);
                }

                // Email must be unique after canonicalization.
                string canonicalizedEmail = UtilityService.CanonicalizeEmail(email);

                if (await UserManagementService.CheckEmailExistenceAsync(canonicalizedEmail))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Email not uniqe");

                    return UtilityService.CreateResult($"Your email, username, or phone number was invalid... " +
                                                       "please try again.", registrationSuccess);
                }

                // Check the length of their username.
                if (!UtilityService.CheckLength(username, Constants.MaximumUsernameCharacters,
                                                Constants.MinimumUsernameCharacters))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Username length invalid");

                    return UtilityService.CreateResult($"Username length invalid ({Constants.MaximumUsernameCharacters}" +
                                                       " max).", registrationSuccess);
                }

                // Check the character requirements of their username.
                if (!UtilityService.CheckCharacters(email, Constants.CharSetsData[Constants.UsernameCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Username characters invalid");

                    return UtilityService.CreateResult($"Username characters invalid.", registrationSuccess);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckUserExistenceAsync(username))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Username exists");

                    return UtilityService.CreateResult($"Your email, username, or phone number was invalid... " +
                                                       "please try again.", registrationSuccess);
                }

                // Check the length of their phone number.
                if (!UtilityService.CheckLength(phoneNumber, Constants.PhoneNumberCharacterLength))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Phone number length invalid");

                    return UtilityService.CreateResult($"Phone number length invalid ({Constants.PhoneNumberCharacterLength}" +
                                                       " max).", registrationSuccess);
                }

                // Check the character requirements of their phone number.
                if (!UtilityService.CheckCharacters(phoneNumber, Constants.CharSetsData[Constants.PhoneNumberCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Phone number characters invalid");

                    return UtilityService.CreateResult($"Phone number characters invalid.", registrationSuccess);
                }

                // Check username uniqueness.
                if (await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Phone number exists");

                    return UtilityService.CreateResult($"Your email, username, or phone number was invalid... " +
                                                       "please try again.", registrationSuccess);
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
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password length invalid");

                    return UtilityService.CreateResult($"Password length invalid ({Constants.MaximumUsernameCharacters}" +
                                                       $" max, {Constants.MinimumPasswordCharacters} min).",
                                                       registrationSuccess);
                }

                // Check the character requirements of their password.
                if (!UtilityService.CheckCharacters(plaintextPassword, Constants.CharSetsData[Constants.PasswordCharacterType]))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password characters invalid");

                    return UtilityService.CreateResult($"Password characters invalid.", registrationSuccess);
                }

                // Check if password for context specific words.
                if (UtilityService.ContainsContextSpecificWords(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password contains context specific words");

                    return UtilityService.CreateResult($"Password contains context specific words.",
                                                       registrationSuccess);
                }

                // Check if password contains sequences or repetitions.
                if (UtilityService.ContainsRepetitionOrSequence(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password contains sequences or repetitions");

                    return UtilityService.CreateResult($"Password contains sequences (111) or repetitions (123).",
                                                       registrationSuccess);
                }

                // Check if password contains dictionary words.
                if (await UtilityService.ContainsDictionaryWordsAsync(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password contains words");

                    return UtilityService.CreateResult($"Password contains words from the English dicitonary.",
                                                       registrationSuccess);
                }

                // Check if password is a previously corrupted password.
                if (await UtilityService.IsCorruptedPasswordAsync(plaintextPassword))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Registration", "", ipAddress, "Password corrupted");

                    return UtilityService.CreateResult($"Your password has been corrupted.", registrationSuccess);
                }

                // Successful registration!
                registrationSuccess = true;

                // Hash password with salt
                byte[] saltBytes = SecurityService.GenerateSalt();
                string saltHex = UtilityService.BytesToHexString(saltBytes);

                string digest = SecurityService.HashWithKDF(hexPassword, saltBytes);

                await AuthenticationService.SendEmailVerificationAsync(username, canonicalizedEmail);
                await UserManagementService.CreateUserAsync(true, username, firstName, lastName, canonicalizedEmail,
                                                            phoneNumber, digest, 0, "Customer", saltHex);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                              "Registration", "", ipAddress);

                return UtilityService.CreateResult($"Registration Successful!", registrationSuccess);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                              "Registration", username, ipAddress, e.Message);

                return UtilityService.CreateResult("A system error occurred. Please try again later." +
                                                   " A team of highly trained monkeys is working on the situation.", false);
            }
        }
    }
}
