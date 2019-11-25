using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TeamA.Exogredient.DAL;

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

        public void DisableUserName(string userName)
        {
            try
            {
                if (!_userDao.UserNameExists(userName))
                {
                    return;
                }
                if (_userDao.IsUserNameDisabled(userName))
                {
                    throw new Exception("The username is already disabled!");
                }
                UserRecord disabledUser = new UserRecord(userName, disabled: "1");
                _userDao.Update(disabledUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void EnableUserName(string userName)
        {
            try
            {
                if (!_userDao.UserNameExists(userName))
                {
                    return;
                }
                if (!_userDao.IsUserNameDisabled(userName))
                {
                    throw new Exception("The username is already enabled!");
                }
                UserRecord disabledUser = new UserRecord(userName, disabled: "0");
                _userDao.Update(disabledUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public bool Authenticate(string userName, byte[] encryptedPassword, byte[] aesKeyEncrypted, byte[] aesIV)
        {
            try
            {
                if (!_userDao.UserNameExists(userName))
                {
                    return false;
                }
                if (_userDao.IsUserNameDisabled(userName))
                {
                    // HACK make custom exception
                    throw new Exception("This username is locked! To enable, contact the admin");
                }
                RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
                byte[] aesKey = _securityService.DecryptRSA(aesKeyEncrypted, privateKey);
                string hexPassword = _securityService.DecryptAES(encryptedPassword, aesKey, aesIV);
                string storedPassword;
                string saltString;
                _userDao.GetStoredPasswordAndSalt(userName, out storedPassword, out saltString);
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
                throw e;
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
    }
}
