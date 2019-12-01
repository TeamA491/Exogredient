using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public static class AuthenticationManager
    {
        private static readonly IDictionary<string, int> _failureCounter;

        private static readonly int _maxAttempts = 3;

        static AuthenticationManager()
        {
            _failureCounter = new Dictionary<string, int>();
        }

        public static async Task<bool> InitAuthentication(string userName, byte[] encryptedPassword, byte[]encryptedAESKey, byte[] aesIV)
        {
            try
            {
                if (!_failureCounter.ContainsKey(userName))
                {
                    if (await AuthenticationService.AuthenticateAsync(userName, encryptedPassword, encryptedAESKey, aesIV) == false)
                    {
                        _failureCounter.Add(userName, 1);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (_failureCounter[userName] < _maxAttempts)
                {
                    if (await AuthenticationService.AuthenticateAsync(userName, encryptedPassword, encryptedAESKey, aesIV) == false)
                    {
                        _failureCounter[userName] += 1;
                        if (_failureCounter[userName] == _maxAttempts)
                        {
                            await UserManagementService.DisableUserNameAsync(userName);
                        }
                        return false;
                    }
                    else
                    {
                        _failureCounter.Remove(userName);
                        return true;
                    }
                }
                else
                {
                    throw new Exception("This username is locked! To enable, contact an admin");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
