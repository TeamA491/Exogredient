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
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public static class UserManagementService
    {
        private static readonly UserDAO _userDAO;
        private static readonly IPAddressDAO _ipDAO;

        static UserManagementService()
        {
            _userDAO = new UserDAO();
            _ipDAO = new IPAddressDAO();
        }

        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            return await _userDAO.CheckUserExistenceAsync(username);
        }

        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber);
        }

        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return await _userDAO.CheckEmailExistenceAsync(email);
        }

        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            return (user.Disabled == 1);
        }
        
        public static async Task<bool> CheckIPLockAsync(string ipAddress, TimeSpan maxLockTime)
        {
            IPAddressObject ip = (IPAddressObject)await _ipDAO.ReadByIdAsync(ipAddress);

            if (! (await _ipDAO.CheckIPExistenceAsync(ipAddress)))
            {
                return false;
            }
            else
            {
                long timestamp = ip.TimestampLocked;

                long maxLockSeconds = StringUtilityService.TimespanToSeconds(maxLockTime);
                long currentUnix = StringUtilityService.CurrentUnixTime();

                return (timestamp + maxLockSeconds < currentUnix ? true : false);
            }
        }

        public static async Task<bool> LockIPAsync(string ipAddress)
        {
            IPAddressRecord record = new IPAddressRecord(ipAddress, StringUtilityService.CurrentUnixTime());

            return await _ipDAO.CreateAsync(record);
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

        public static async Task<bool> MakeTempPermAsync(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        public static async Task<bool> StoreEmailCodeAsync(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp);

            return await _userDAO.UpdateAsync(record);
        }

        public static async Task<bool> RemoveEmailCodeAsync(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: "", emailCodeTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        /// <summary>
        /// Disable a user from logging in.
        /// </summary>
        /// <param name="username"> username to disable </param>
        public static async Task<bool> DisableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            // If the username doesn't exist, throw an exception.
            if (!(await _userDAO.CheckUserExistenceAsync(username)))
            {
                return false;
            }
            if (user.Disabled == 1)
            {
                return false;
                //throw new Exception("The username is already disabled!");
            }

            UserRecord disabledUser = new UserRecord(username, disabled: 1);
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        /// <summary>
        /// Enable a user to log in.
        /// </summary>
        /// <param name="username"> username to enable </param>
        public static async Task<bool> EnableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            if (!(await _userDAO.CheckUserExistenceAsync(username)))
            {
                return false;
            }
            if (user.Disabled == 0)
            {
                return false;
                //throw new Exception("The username is already enabled!");
            }
            // Enable the username.
            UserRecord disabledUser = new UserRecord(username, disabled: 0);
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        public static async Task ChangePasswordAsync(string username, string password)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            // Check if the username exists.
            if (!(await _userDAO.CheckUserExistenceAsync(username)))
            {
                // TODO Create Custom Exception: For System
                throw new Exception("The username doesn't exsit.");
            }
            // Check if the username is disabled.
            if (user.Disabled == 1)
            {
                // TODO Create Custom Exception: For User
                throw new Exception("This username is locked! To enable, contact the admin");
            }
            byte[] saltBytes = SecurityService.GenerateSalt();
            string hashedPassword = SecurityService.HashWithKDF(password, saltBytes);
            string saltString = StringUtilityService.BytesToHexString(saltBytes);
            UserRecord newPasswordUser = new UserRecord(username, password: hashedPassword, salt: saltString);
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

            message.From.Add(new MailboxAddress($"{Constants.SystemEmailAddress}"));
            message.To.Add(new MailboxAddress($"{Constants.SystemAdminEmailAddress}"));

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
            await client.AuthenticateAsync($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();

            return true;
        }



        public static async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            UserRecord record;

            record = new UserRecord(username, lastLoginFailTimestamp: StringUtilityService.CurrentUnixTime());

            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = StringUtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = StringUtilityService.CurrentUnixTime();

            // Need to check if the last maxtime + lastTime is less than now.
            // if it is then reset the failure
            if (lastLoginFailTimestamp + maxSeconds < currentUnix)
            {
                record = new UserRecord(username, loginFailures: 0);
                await _userDAO.UpdateAsync(record);
            }

            user = (UserObject)await _userDAO.ReadByIdAsync(username);

            int updatedLoginFailures = user.LogInFailures + 1;
            if (updatedLoginFailures >= maxNumberOfTries)
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, disabled: 1);
            }
            else
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures);
            }

            await _userDAO.UpdateAsync(record);
            
            return false;
        }

        public static async Task<bool> IncrementEmailCodeFailuresAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(record);

            return true;
        }

        public static async Task<int> GetEmailCodeFailureCountAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            // This returns string right now. update to int when we update database
            return user.EmailCodeFailures;
        }

        public static async Task<int> GetPhoneCodeFailureCountAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            // This returns string right now. update to int when we update database
            return user.PhoneCodeFailures;
        }


    }
}
