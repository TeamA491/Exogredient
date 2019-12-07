using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;

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
            return await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false);
        }

        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
        }

        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return await _userDAO.CheckEmailExistenceAsync(email).ConfigureAwait(false);
        }

        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            return (user.Disabled == 1);
        }
        
        public static async Task<bool> CheckIPLockAsync(string ipAddress, TimeSpan maxLockTime)
        {
            IPAddressObject ip = (IPAddressObject)await _ipDAO.ReadByIdAsync(ipAddress).ConfigureAwait(false);

            if (!await _ipDAO.CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
            {
                return false;
            }
            else
            {
                long timestamp = ip.TimestampLocked;

                long maxLockSeconds = UtilityService.TimespanToSeconds(maxLockTime);
                long currentUnix = UtilityService.CurrentUnixTime();

                return (timestamp + maxLockSeconds < currentUnix ? true : false);
            }
        }

        public static async Task<bool> LockIPAsync(string ipAddress)
        {
            // CHANGE: should we check existence here?
            if (!await _ipDAO.CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
            {
                return false;
            }
            IPAddressRecord record = new IPAddressRecord(ipAddress, UtilityService.CurrentUnixTime());

            return await _ipDAO.CreateAsync(record).ConfigureAwait(false);
        }

        public static async Task<bool> CreateUserAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // CHANGE: should we check user existence here 
            if(!await CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }

            long tempTimestamp = isTemp ? UtilityService.CurrentUnixTime() : 0;

            UserRecord record = new UserRecord(username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt, tempTimestamp, "", 0, 0, 0, 0, 0);

            return await _userDAO.CreateAsync(record).ConfigureAwait(false);
        }

        public static async Task<bool> DeleteUserAsync(string username)
        {
            return await _userDAO.DeleteByIdsAsync(new List<string>() { username }).ConfigureAwait(false);
        }

        public static async Task<UserObject> GetUserInfoAsync(string username)
        {
            return (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);
        }

        public static async Task<bool> MakeTempPermAsync(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: 0);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
        }

        public static async Task<bool> StoreEmailCodeAsync(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp, emailCodeFailures: 0);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
        }

        public static async Task<bool> RemoveEmailCodeAsync(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: "", emailCodeTimestamp: 0, emailCodeFailures: 0);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Disable a user from logging in.
        /// </summary>
        /// <param name="username"> username to disable </param>
        public static async Task<bool> DisableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // If the username doesn't exist, throw an exception.
            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }
            if (user.Disabled == 1)
            {
                //throw new Exception("The username is already disabled!");
                return false;
            }

            UserRecord disabledUser = new UserRecord(username, disabled: 1);
            await _userDAO.UpdateAsync(disabledUser).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Enable a user to log in.
        /// </summary>
        /// <param name="username"> username to enable </param>
        public static async Task<bool> EnableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }
            if (user.Disabled == 0)
            {
                //throw new Exception("The username is already enabled!");
                return false;
            }
            // Enable the username.
            UserRecord disabledUser = new UserRecord(username, disabled: 0);
            await _userDAO.UpdateAsync(disabledUser).ConfigureAwait(false);

            return true;
        }

        public static async Task ChangePasswordAsync(string username, string digest, string saltString)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // Check if the username exists.
            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
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

            UserRecord newPasswordUser = new UserRecord(username, password: digest, salt: saltString);
            await _userDAO.UpdateAsync(newPasswordUser).ConfigureAwait(false);
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

            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
            await client.AuthenticateAsync($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}").ConfigureAwait(false);
            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();

            return true;
        }

        public static async Task<bool> UpdatePhoneCodeFailuresAsync(string username, int numFailures)
        {
            UserRecord record = new UserRecord(username, phoneCodeFailures: numFailures);

            await _userDAO.UpdateAsync(record).ConfigureAwait(false);

            return true;
        }


        public static async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            UserRecord record;

            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = UtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = UtilityService.CurrentUnixTime();

            // Need to check if the last maxtime + lastTime is less than now.
            // if it is then reset the failure

            bool reset = false;

            // If the last Login Time has passed the maxTimeBeforeFailureReset then reset their loginFailure number.
            if (lastLoginFailTimestamp + maxSeconds < currentUnix)
            {
                reset = true;
                record = new UserRecord(username, loginFailures: 0);
                await _userDAO.UpdateAsync(record).ConfigureAwait(false);
            }

            // Increment the user's login Failure count.
            int updatedLoginFailures = reset ? 1 : user.LogInFailures + 1;
            
            // Disable the user if they have reached the max number of tries. 
            // Update the last login fail time.
            if (updatedLoginFailures >= maxNumberOfTries)
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, disabled: 1, lastLoginFailTimestamp: UtilityService.CurrentUnixTime());
            }
            else
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, lastLoginFailTimestamp: UtilityService.CurrentUnixTime());
            }

            await _userDAO.UpdateAsync(record).ConfigureAwait(false);
            
            return false;
        }

        public static async Task<bool> IncrementEmailCodeFailuresAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(record).ConfigureAwait(false);

            return true;
        }

        public static async Task<bool> IncrementPhoneCodeFailuresAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.PhoneCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, phoneCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(record).ConfigureAwait(false);

            return true;
        }
    }
}
