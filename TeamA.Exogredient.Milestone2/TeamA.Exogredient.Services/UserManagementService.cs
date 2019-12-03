using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TeamA.Exogredient.Services
{
    public static class UserManagementService
    {
        private static readonly string _sendingEmail = "exogredient.system@gmail.com";
        private static readonly string _sendingEmailPassword = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD", EnvironmentVariableTarget.User);
        private static readonly string _receivingEmail = "TEAMA.CS491@gmail.com";

        private static readonly UserDAO _userDao;

        static UserManagementService()
        {
            _userDao = new UserDAO();
        }


        // TODO finish making checkuserexistence class 
        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            return false;
        }

        // TODO finish making phonenumberexistence
        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return false;
        }

        // TODO finish making email exists
        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return false;
        }

        // TODO finish making user disabled
        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            return false;
        }
        
        public static async Task<bool> CheckIPLockAsync(string IPAddress)
        {
            return false;
        }

        public static async Task<bool> LockIPAsync(string IPAddress)
        {
            return false;
        }

        public static async Task<bool> CreateUserAsync(bool isTemp, string username, string firstName, string lastName,
                                                       string email, string phoneNumber, string password, string disabled,
                                                       string userType, string salt)
        {

            return false;
        }

        public static async Task<bool> DeleteUserAsync(string username)
        {
            return false;
        }

        public static async Task<bool> MakeTempPerm(string username)
        {
            return false;
        }

        /// <summary>
        /// Disable a username from logging in.
        /// </summary>
        /// <param name="userName"> username to disable </param>
        public static async Task<bool> DisableUserNameAsync(string userName)
        {
            try
            {
                // If the username doesn't exist, throw an exception.
                if (!(await _userDao.UserNameExistsAsync(userName)))
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

        /// <summary>
        /// Enable a username to log in.
        /// </summary>
        /// <param name="userName"> username to enable </param>
        public static async Task<bool> EnableUserNameAsync(string userName)
        {
            try
            {
                if (!(await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (!(await _userDao.IsUserNameDisabledAsync(userName)))
                {
                    return false;
                    //throw new Exception("The username is already enabled!");
                }
                // Enable the username.
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

        public static async Task ChangePasswordAsync(string userName, string password)
        {
            try
            {
                // Check if the username exists.
                if (!(await _userDao.UserNameExistsAsync(userName)))
                {
                    // TODO Create Custom Exception: For System
                    throw new Exception("The username doesn't exsit.");
                }
                // Check if the username is disabled.
                if (await _userDao.IsUserNameDisabledAsync(userName))
                {
                    // TODO Create Custom Exception: For User
                    throw new Exception("This username is locked! To enable, contact the admin");
                }
                byte[] saltBytes = SecurityService.GenerateSalt();
                string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);
                string saltString = StringUtilityService.BytesToHexString(saltBytes);
                UserRecord newPasswordUser = new UserRecord(userName, password: hashedPassword, salt: saltString);
                await _userDao.UpdateAsync(newPasswordUser);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Uses gmail smtp to send mail from a known email address to any other address.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message you want to send.</param>
        /// <returns>A bool representing whether the process succeeded.</returns>
        public static async Task<bool> NotifySystemAdminAsync(string body)
        {
            try
            {
                string title = DateTime.UtcNow.ToString("MM-dd-yyyy");

                var message = new MimeMessage();
                var bodyBuilder = new BodyBuilder();

                message.From.Add(new MailboxAddress($"{_sendingEmail}"));
                message.To.Add(new MailboxAddress($"{_receivingEmail}"));

                message.Subject = title;

                Random generator = new Random();
                string emailCode = generator.Next(100000, 1000000).ToString();

                bodyBuilder.HtmlBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                var client = new SmtpClient
                {
                    ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
                };

                await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync($"{_sendingEmail}", $"{_sendingEmailPassword}");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                client.Dispose();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Checks if a canonicalized email exists in the database.
        /// </summary>
        /// <param name="canonEmail">Email that already has been canonicalized.</param>
        /// <returns>Returns the value of bool to represent whether
        /// an canonicalized email is unique.</returns>
        public static async Task<bool> CheckEmailUniquenessAsync(string canonEmail)
        {
            //return await _userDAO.CheckEmailUniquenessAsync(canonEmail);
            return false;
        }

        /// <summary>
        /// Checks if a phone number already exists in the database.
        /// </summary>
        /// <param name="phoneNumber">The phone number we are checking.</param>
        /// <returns>Returns the value of bool to represent whether
        /// a phone number is unique.</returns>
        public static async Task<bool> CheckPhoneUniquenessAsync(string phoneNumber)
        {
            //return await _userDAO.CheckPhoneUniquenessAsync(phoneNumber);
            return false;
        }

        /// <summary>
        /// Checks if a username already exists in the database.
        /// </summary>
        /// <param name="username">The username we are checking.</param>
        /// <returns>Returns the value of bool to represent whether
        /// an username is unique.</returns>
        public static async Task<bool> CheckUsernameUniquenessAsync(string username)
        {
            //return await _userDAO.CheckUsernameUniquenessAsync(username);
            return false;
        }

        public static bool MakeTempUserPerm(string username)
        {
            //return async bool _UserDAO.MakeTempUserPerm(string username)
            return false;
        }


    }
}
