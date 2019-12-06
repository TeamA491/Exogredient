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
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UsernameDNELogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenticationSuccess);
                }

                UserObject user = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
                
                if (user.Disabled == 1)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.UserDisableLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.UserDisableUserMessage, authenticationSuccess);
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
                    path = path + $"{path.Substring(0, 1)}" + Constants.TokenFile;

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, username, ipAddress).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.LogInSuccessUserMessage, authenticationSuccess);
                }
                else
                {
                    await UserManagementService.IncrementLoginFailuresAsync(username,
                                                                            Constants.LogInTriesResetTime,
                                                                            Constants.MaxLogInAttempts).ConfigureAwait(false);

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.InvalidPasswordLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.InvalidLogInUserMessage, authenticationSuccess);
                }
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.LogInOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
