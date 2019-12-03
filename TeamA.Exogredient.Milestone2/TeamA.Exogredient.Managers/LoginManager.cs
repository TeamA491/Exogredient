using System;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class LoginManager
    {
        private const int MaxAttempts = 15;

        public static async Task<Result<bool>> InitLogin(string userName, byte[] encryptedPassword, byte[] encryptedAESKey, byte[] aesIV)
        {
            try
            {
                bool authenticationSuccess = await AuthenticationService.AuthenticateAsync(userName, encryptedPassword, encryptedAESKey, aesIV);
                if (authenticationSuccess)
                {
                    Result<bool> result = new Result<bool>("Logged in successfully.");
                    result.Data = authenticationSuccess;
                    return result;
                }
                else
                {
                    //TODO add failure increment function here
                    Result<bool> result = new Result<bool>("Invalid username or password.");
                    result.Data = authenticationSuccess;
                    return result;
                }
            }
            catch(InvalidOperationException e)
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
