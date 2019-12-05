﻿using System;
using System.IO;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Managers
{
    public class LogInManager
    {
        public static async Task<Result<bool>> LogInAsync(string userName, byte[] encryptedPassword,
                                                          byte[] encryptedAESKey, byte[] aesIV)
        {
            try
            {
                bool authenticationSuccess = await AuthenticationService.AuthenticateAsync(userName, encryptedPassword, encryptedAESKey, aesIV);
                if (authenticationSuccess)
                {
                    Result<bool> result = new Result<bool>("Logged in successfully.");
                    result.Data = authenticationSuccess;
                    string token = await AuthorizationService.CreateTokenAsync(userName);
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    path = path + $"{path.Substring(0, 1)}" + "token.txt";
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(token);
                    }
                    return result;
                }
                else
                {
                    //TODO add failure increment function here
                    Result<bool> result = new Result<bool>("Invalid username and/or password.");
                    result.Data = authenticationSuccess;
                    return result;
                }
            }
            catch (InvalidOperationException e)
            {
                Result<bool> result = new Result<bool>(e.Message);
                result.Data = false;
                return result;
            }
            catch
            {
                Result<bool> result = new Result<bool>("Error: unsuccessful operation");
                result.Data = false;
                return result;
            }
        }
    }
}
