using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class UserManagementService
    {
        private static readonly string _systemEmailAddress = Constants.SystemEmailAddress;
        private static readonly string _systemEmailPassword = Constants.SystemEmailPassword;
        private static readonly string _systemAdminEmailAddress = Constants.SystemAdminEmailAddress;

        private static readonly UserDAO _userDAO;
        private static readonly IPAddressDAO _lockedIPDAO;

        static UserManagementService()
        {
            _userDAO = new UserDAO();
            _lockedIPDAO = new IPAddressDAO();
        }


        // TODO finish making checkuserexistence class 
        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            return await _userDAO.CheckUserExistenceAsync(username);
        }

        // TODO finish making phonenumberexistence
        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber);
        }

        // TODO finish making email exists
        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return await _userDAO.CheckEmailExistenceAsync(email);
        }

        // TODO finish making user disabled
        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            return await _userDAO.CheckIfUserDisabledAsync(username);
        }
        
        public static async Task<bool> CheckIPLockAsync(string ipAddress, TimeSpan maxLockTime)
        {
            if (! (await _lockedIPDAO.CheckIPExistenceAsync(ipAddress)))
            {
                return false;
            }
            else
            {
                long timestamp = await _lockedIPDAO.GetTimestamp(ipAddress);

                long maxLockSeconds = StringUtilityService.TimespanToSeconds(maxLockTime);
                long currentUnix = StringUtilityService.CurrentUnixTime();

                return (timestamp + maxLockSeconds < currentUnix ? true : false);
            }
        }

        public static async Task<bool> LockIPAsync(string ipAddress)
        {
            IPRecord record = new IPRecord(ipAddress, StringUtilityService.CurrentUnixTime());

            return await _lockedIPDAO.CreateAsync(record);
        }

        public static async Task<bool> CreateUserAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, short disabled, string userType, string salt)
        {
            long tempTimestamp = isTemp ? StringUtilityService.CurrentUnixTime() : 0;

            UserRecord record = new UserRecord(username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt, tempTimestamp, "", 0, 0, 0, 0, 0);

            return await _userDAO.CreateAsync(record);
        }

        public static async Task<bool> DeleteUserAsync(string username)
        {
            return await _userDAO.DeleteByIdsAsync(new List<string>() { username });
        }

        public static async Task<bool> MakeTempPerm(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        public static async Task<bool> StoreEmailCode(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp);

            return await _userDAO.UpdateAsync(record);
        }

        public static async Task<bool> RemoveEmailCode(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: "", emailCodeTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        /// <summary>
        /// Disable a username from logging in.
        /// </summary>
        /// <param name="userName"> username to disable </param>
        public static async Task<bool> DisableUserNameAsync(string userName)
        {
            // If the username doesn't exist, throw an exception.
            if (!(await _userDAO.CheckUserExistenceAsync(userName)))
            {
                return false;
            }
            if (await _userDAO.CheckIfUserDisabledAsync(userName))
            {
                return false;
                //throw new Exception("The username is already disabled!");
            }

            UserRecord disabledUser = new UserRecord(userName, disabled: 1);
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        /// <summary>
        /// Enable a username to log in.
        /// </summary>
        /// <param name="userName"> username to enable </param>
        public static async Task<bool> EnableUserNameAsync(string userName)
        {
            if (!(await _userDAO.CheckUserExistenceAsync(userName)))
            {
                return false;
            }
            if (!(await _userDAO.CheckIfUserDisabledAsync(userName)))
            {
                return false;
                //throw new Exception("The username is already enabled!");
            }
            // Enable the username.
            UserRecord disabledUser = new UserRecord(userName, disabled: 0);
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        public static async Task ChangePasswordAsync(string userName, string password)
        {
            // Check if the username exists.
            if (!(await _userDAO.CheckUserExistenceAsync(userName)))
            {
                // TODO Create Custom Exception: For System
                throw new Exception("The username doesn't exsit.");
            }
            // Check if the username is disabled.
            if (await _userDAO.CheckIfUserDisabledAsync(userName))
            {
                // TODO Create Custom Exception: For User
                throw new Exception("This username is locked! To enable, contact the admin");
            }
            byte[] saltBytes = SecurityService.GenerateSalt();
            string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);
            string saltString = StringUtilityService.BytesToHexString(saltBytes);
            UserRecord newPasswordUser = new UserRecord(userName, password: hashedPassword, salt: saltString);
            await _userDAO.UpdateAsync(newPasswordUser);
        }

        /// <summary>
        /// Uses gmail smtp to send mail from a known email address to any other address.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message you want to send.</param>
        /// <returns>A bool representing whether the process succeeded.</returns>
        public static async Task<bool> NotifySystemAdminAsync(string body)
        {
            string title = DateTime.UtcNow.ToString("MM-dd-yyyy");

            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            message.From.Add(new MailboxAddress($"{_systemEmailAddress}"));
            message.To.Add(new MailboxAddress($"{_systemAdminEmailAddress}"));

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
            await client.AuthenticateAsync($"{_systemEmailAddress}", $"{_systemEmailPassword}");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();

            return true;
        }



        public static async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserRecord record;
            record = new UserRecord(username, lastLoginFailTimestamp: StringUtilityService.CurrentUnixTime());

            long lastLoginFailTimestamp = await _userDAO.GetLastLoginFailTimestampAsync(username);
            long maxSeconds = StringUtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = StringUtilityService.CurrentUnixTime();

            if (lastLoginFailTimestamp + maxSeconds < currentUnix)
            {
                record = new UserRecord(username, loginFailures: 0);
                await _userDAO.UpdateAsync(record);
            }
            else
            {
                int updatedLoginFailures = (await _userDAO.GetLoginFailuresAsync(username)) + 1;
                if (updatedLoginFailures > maxNumberOfTries)
                {
                    
                }
            }

            // Need to check if the last maxtime + lastTime is less than now.
                // if it is then reset the failure
            return false;
        }

        public static async Task<bool> IncrementEmailCodeFailuresAsync(string username)
        {
            // Get the current failure count.
            int currentFailures = await _userDAO.GetEmailCodeFailureCountAsync(username);

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(record);

            return true;
        }

        public static async Task<int> GetEmailCodeFailureCountAsync(string username)
        {
            // This returns string right now. update to int when we update database
            return await _userDAO.GetEmailCodeFailureCountAsync(username);
        }

        public static async Task<int> GetPhoneCodeFaiureCountAsync(string username)
        {
            // This returns string right now. update to int when we update database
            return await _userDAO.GetPhoneCodeFaiureCountAsync(username);
        }


    }
}
