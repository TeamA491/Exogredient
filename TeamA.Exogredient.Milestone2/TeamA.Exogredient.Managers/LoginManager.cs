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
        public static async Task<Result<bool>> LogInAsync(string username, string encryptedPassword,
                                                          string encryptedAESKey, string aesIV, string ipAddress)
        {
            try
            {
                Result<bool> result;
                bool authenticationSuccess = false;

                if (! (await UserManagementService.CheckUserExistenceAsync(username)))
                {
                    result = new Result<bool>("Username or password was invalid.")
                    {
                        Data = authenticationSuccess
                    };

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"), "Log In", username, ipAddress, "Username does not exist");

                    return result;
                }

                UserObject user = await UserManagementService.GetUserInfoAsync(username);
                
                if (user.Disabled == 1)
                {
                    result = new Result<bool>("Your account is disabled, please contact the system administrator.")
                    {
                        Data = authenticationSuccess
                    };
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"), "Log In", username, ipAddress, "User disabled");


                    return result;
                }

                byte[] encryptedPasswordBytes = StringUtilityService.HexStringToBytes(encryptedPassword);
                byte[] encryptedAESKeyBytes = StringUtilityService.HexStringToBytes(encryptedAESKey);
                byte[] AESIVBytes = StringUtilityService.HexStringToBytes(aesIV);
                byte[] publicKeyBytes = StringUtilityService.HexStringToBytes(Constants.PublicKey);
                byte[] privateKeyBytes = StringUtilityService.HexStringToBytes(Constants.PrivateKey);

                byte[] decryptedAESKeyBytes = SecurityService.DecryptRSA(encryptedAESKeyBytes, privateKeyBytes);

                string hexPassword = SecurityService.DecryptAES(encryptedPasswordBytes, decryptedAESKeyBytes, AESIVBytes);

                byte[] userSaltBytes = StringUtilityService.HexStringToBytes(user.Salt);

                string hashedPassword = SecurityService.HashWithKDF(hexPassword, userSaltBytes);

                if (user.Password == hashedPassword)
                {
                    authenticationSuccess = true;

                    result = new Result<bool>("Logged in successfully.")
                    {
                        Data = authenticationSuccess
                    };

                    string token = await AuthorizationService.CreateTokenAsync(username);
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    path = path + $"{path.Substring(0, 1)}" + "token.txt";

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"), "Log In", username, ipAddress);

                    return result;
                }
                else
                {
                    await UserManagementService.IncrementLoginFailuresAsync(username,
                                                                            Constants.LogInTriesResetTime,
                                                                            Constants.MaxLogInAttempts);

                    result = new Result<bool>("Username or password was invalid.")
                    {
                        Data = authenticationSuccess
                    };

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"), "Log In", username, ipAddress, "Invalid password entered");

                    return result;
                }
            }
            catch (Exception e)
            {
                Result<bool> result = new Result<bool>("A system error occurred. Please try again later." + 
                                                       "A team of highly trained monkeys is working on the situation.")
                {
                    Data = false
                };

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString("HH: mm:ss: ff UTC yyyyMMdd"), "Log In", username, ipAddress, e.Message);

                return result;
            }
        }
    }
}
