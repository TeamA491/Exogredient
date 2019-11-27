using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TeamA.Exogredient.Services
{
    public class AuthenticationService
    {
        UserDAO _userDao;
        SecurityService _securityService;

        public AuthenticationService()
        {
            _userDao = new UserDAO();
            _securityService = new SecurityService();
        }

        public async Task<bool> DisableUserNameAsync(string userName)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (await _userDao.IsUserNameDisabledAsync(userName))
                {
                    return false;
                    //throw new Exception("The username is already disabled!");
                }
                
                UserRecord disabledUser = new UserRecord(userName, disabled: "1");
                await _userDao.UpdateAsync(disabledUser);

                return true;
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }

        public async Task<bool> EnableUserNameAsync(string userName)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (! (await _userDao.IsUserNameDisabledAsync(userName)))
                {
                    return false;
                    //throw new Exception("The username is already enabled!");
                }
                UserRecord disabledUser = new UserRecord(userName, disabled: "0");
                await _userDao.UpdateAsync(disabledUser);

                return true;
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }


        public async Task<bool> AuthenticateAsync(string userName, byte[] encryptedPassword, byte[] aesKeyEncrypted, byte[] aesIV)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (await _userDao.IsUserNameDisabledAsync(userName))
                {
                    // HACK make custom exception
                    throw new Exception("This username is locked! To enable, contact the admin");
                }

                RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
                byte[] aesKey = _securityService.DecryptRSA(aesKeyEncrypted, privateKey);
                string hexPassword = _securityService.DecryptAES(encryptedPassword, aesKey, aesIV);
                
                Tuple<string, string> saltAndPassword = await _userDao.GetStoredPasswordAndSaltAsync(userName);

                string storedPassword = saltAndPassword.Item1;
                string saltString = saltAndPassword.Item2;

                byte[] saltBytes = _securityService.HexStringToBytes(saltString);
                string hashedPassword = _securityService.HashPassword(hexPassword, saltBytes, 100, 32);


                if (storedPassword.Equals(hashedPassword)) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }


        /*
        public SendPhoneVerification(string phoneNumber)
        {

        }

        public generateJWT()
        {

        }
        */

        public async Task<bool> SendSMSCodeAsync(string phoneNumber)
        {
            try
            {
                string accountSID = "AC94d03adc3d2da651c16c82932c29b047";
                string authorizationToken = "e061e796230f25411e208aa5f3257361";

                TwilioClient.Init(accountSID, authorizationToken);

                Random random = new Random();

                int code = random.Next(100000, 1000000);

                var message = await MessageResource.CreateAsync(
                    body: $"Your Exogredient verification code is: {code}",
                    from: new PhoneNumber("+18474533559"),
                    to: new PhoneNumber("+19499815506")
                );

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

        }
    }
}
