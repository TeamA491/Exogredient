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
        // Encrypted password, encrypted AES key, and aesIV are all in hex string format.
        public static async Task<Result<bool>> LogInAsync(string username, string ipAddress,
                                                          string encryptedPassword, string encryptedAESKey,
                                                          string aesIV)
        {
            try
            {
                bool authenticationSuccess = false;

                if (!await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Log In", username, ipAddress,
                                                  "Username does not exist").ConfigureAwait(false);

                    return UtilityService.CreateResult("Username or password was invalid.", authenticationSuccess);
                }

                UserObject user = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
                
                if (user.Disabled == 1)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Log In", username, ipAddress,
                                                  "User disabled").ConfigureAwait(false);

                    return UtilityService.CreateResult("Your account is disabled, please contact the system administrator.", authenticationSuccess);
                }

                byte[] encryptedPasswordBytes = UtilityService.HexStringToBytes(encryptedPassword);
                byte[] encryptedAESKeyBytes = UtilityService.HexStringToBytes(encryptedAESKey);
                byte[] AESIVBytes = UtilityService.HexStringToBytes(aesIV);
                byte[] publicKeyBytes = UtilityService.HexStringToBytes(Constants.PublicKey);
                byte[] privateKeyBytes = UtilityService.HexStringToBytes(Constants.PrivateKey);

                byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);

                byte[] userSaltBytes = UtilityService.HexStringToBytes(user.Salt);

                string hashedPassword = SecurityService.HashWithKDF(hexPassword, userSaltBytes);

                if (user.Password == hashedPassword)
                {
                    authenticationSuccess = true;

                    string token = await AuthorizationService.CreateTokenAsync(username).ConfigureAwait(false);
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    path = path + $"{path.Substring(0, 1)}" + "token.txt";

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Log In", username, ipAddress).ConfigureAwait(false);

                    return UtilityService.CreateResult("Logged in successfully.", authenticationSuccess);
                }
                else
                {
                    await UserManagementService.IncrementLoginFailuresAsync(username,
                                                                            Constants.LogInTriesResetTime,
                                                                            Constants.MaxLogInAttempts).ConfigureAwait(false);

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                                  "Log In", username, ipAddress,
                                                  "Invalid password entered").ConfigureAwait(false);

                    return UtilityService.CreateResult("Username or password was invalid.", authenticationSuccess);
                }
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"),
                                              "Log In", username, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult("A system error occurred. Please try again later." +
                                                   "A team of highly trained monkeys is working on the situation.", false);
            }
        }
    }
}
