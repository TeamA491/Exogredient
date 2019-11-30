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

        public AuthenticationService()
        {
            _userDao = new UserDAO();
        }

        /// <summary>
        /// Disable a username from logging in. 
        /// </summary>
        /// <param name="userName"> username to disable </param>
        public void DisableUserName(string userName)
        {
            try
            {
                // If the username doesn't exist, throw an exception. 
                if (!_userDao.UserNameExists(userName))
                {
                    // TODO Create Custom Exception: For system
                    throw new Exception("Since the username doesn't exist, it can't be disabled.");
                }
                // If the username is already disabled, throw an exception. 
                if (_userDao.IsUserNameDisabled(userName))
                {
                    // TODO Create Custom Exception: For system
                    throw new Exception("The username is already disabled!");
                }
                // Disable the username.
                UserRecord disabledUser = new UserRecord(userName, disabled: "1");
                _userDao.Update(disabledUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Enable a username to log in.
        /// </summary>
        /// <param name="userName"> username to enable </param>
        public void EnableUserName(string userName)
        {
            try
            {
                // If the username doesnt't exist, throw an exception.
                if (!_userDao.UserNameExists(userName))
                {
                    // TODO Create Custom Exception: For system
                    throw new Exception("Since the username doesn't exist, it can't be enabled.");
                }
                // If the username is already enabled, throw an exception.
                if (!_userDao.IsUserNameDisabled(userName))
                {
                    // TODO Create Custom Exception: For system
                    throw new Exception("The username is already enabled!");
                }
                // Enable the username.
                UserRecord disabledUser = new UserRecord(userName, disabled: "0");
                _userDao.Update(disabledUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Create a token for a logged-in user.
        /// </summary>
        /// <param name="userName"> logged-in username </param>
        /// <returns> string of token that represents the user type and unique ID of the username </returns>
        private string CreateToken(string userName)
        {
            // Get the user type of the username.
            string userType = _userDao.GetUserType(userName);

            // Craete a dictionary that represents the user type and unique ID.
            Dictionary<string, string> userInfo = new Dictionary<string, string>()
            {
                {"userType", userType},
                {"id", userName }
            };

            // Generate a token with the dictionary.
            return AuthorizationService.GenerateJWS(userInfo);
        }

        /// <summary>
        /// Check if the username and the password are correct.
        /// </summary>
        /// <param name="userName"> the username used for login</param>
        /// <param name="encryptedPassword"> the password used for login encrypted </param>
        /// <param name="aesKeyEncrypted"> AES key used for encrypting the password </param>
        /// <param name="aesIV"> AES Initialization Vector used for encryptinh the password </param>
        /// <returns> true if the username and password are correct, false otherwise </returns>
        public bool Authenticate(string userName, byte[] encryptedPassword, byte[] aesKeyEncrypted, byte[] aesIV)
        {
            try
            {
                // Check if the username exists.
                if (!_userDao.UserNameExists(userName))
                {
                    return false;
                }
                // Check if the username is disabled.
                if (_userDao.IsUserNameDisabled(userName))
                {
                    // TODO Create Custom Exception: For User
                    throw new Exception("This username is locked! To enable, contact the admin");
                }

                byte[] privateKey = SecurityService.GetRSAPrivateKey();
                byte[] aesKey = SecurityService.DecryptRSA(aesKeyEncrypted,privateKey);
                // Decrypt the encrypted password.
                string hexPassword = SecurityService.DecryptAES(encryptedPassword, aesKey, aesIV);
                string storedPassword;
                string saltString;
                // Get the password and the salt stored corresponding to the username.
                _userDao.GetStoredPasswordAndSalt(userName, out storedPassword, out saltString);
                // Convert the salt to byte array.
                byte[] saltBytes = SecurityService.HexStringToBytes(saltString);
                //Number of iterations for has && length of the hash in bytes.
                // Hash the decrypted password with the byte array of salt.
                string hashedPassword = SecurityService.HashWithKDF(hexPassword, saltBytes);

                //Check if the stored password matches the hashed password
                if (storedPassword.Equals(hashedPassword)) 
                {
                    // TODO Uncomment when GenerateJWS is implemented
                    //string token = CreateToken(userName);
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

        public void ChangePassword(string userName, string password)
        {
            try
            {
                // Check if the username exists.
                if (!_userDao.UserNameExists(userName))
                {
                    // TODO Create Custom Exception: For System
                    throw new Exception("The username doesn't exsit.");
                }
                // Check if the username is disabled.
                if (_userDao.IsUserNameDisabled(userName))
                {
                    // TODO Create Custom Exception: For User
                    throw new Exception("This username is locked! To enable, contact the admin");
                }
                byte[] saltBytes = SecurityService.GenerateSalt();
                string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);
                string saltString = SecurityService.BytesToHexString(saltBytes);
                UserRecord newPasswordUser = new UserRecord(userName, password:hashedPassword, salt:saltString);
                _userDao.Update(newPasswordUser);
            }
            catch(Exception e)
            {
                throw e;
            }

        }
        

        /*
        public SendPhoneVerification(string phoneNumber)
        {

        }
        */
    }
}
