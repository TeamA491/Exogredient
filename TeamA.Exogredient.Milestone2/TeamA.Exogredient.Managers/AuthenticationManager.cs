using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class AuthenticationManager
    {
        IDictionary<string, int> _failureCounter;
        AuthenticationService _authenticationService;
        int _maxAttempts = 3;

        public AuthenticationManager()
        {
            _authenticationService = new AuthenticationService();
            _failureCounter = new Dictionary<string, int>();
        }

        public bool InitAuthentication(string userName, byte[] encryptedPassword, byte[]encryptedAESKey, byte[] aesIV)
        {
            try
            {
                if (!_failureCounter.ContainsKey(userName))
                {
                    if (_authenticationService.Authenticate(userName, encryptedPassword, encryptedAESKey, aesIV) == false)
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
                    if (_authenticationService.Authenticate(userName, encryptedPassword, encryptedAESKey, aesIV) == false)
                    {
                        _failureCounter[userName] += 1;
                        if (_failureCounter[userName] == _maxAttempts)
                        {
                            _authenticationService.DisableUserName(userName);
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
