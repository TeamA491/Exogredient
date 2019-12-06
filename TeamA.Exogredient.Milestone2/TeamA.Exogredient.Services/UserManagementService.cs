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

        /// <summary>
        /// Checks if a user exists in the data store.
        /// </summary>
        /// <param name="username">Username to check.</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            return await _userDAO.CheckUserExistenceAsync(username);
        }

        /// <summary>
        /// Checks if a phone number exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check.</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber);
        }

        /// <summary>
        /// Checks if an email exists in the data store.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return await _userDAO.CheckEmailExistenceAsync(email);
        }

        /// <summary>
        /// Checks if a user is disabled in the data store.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Returns true if the user is disabled and false if not.</returns>
        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            return (user.Disabled == 1);
        }
        
        /// <summary>
        /// Checks if an IP address in the data store is still locked given the max lock time.
        /// </summary>
        /// <param name="ipAddress">IP address to check</param>
        /// <param name="maxLockTime">Length that the ip address is locked.</param>
        /// <returns>Returns true if the IP address is locked and false if not.</returns>
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

        /// <summary>
        /// Lock and IP address.
        /// </summary>
        /// <param name="ipAddress">IP address to be locked</param>
        /// <returns>Returns true if the operation succeeded and false if it failed.</returns>
        public static async Task<bool> LockIPAsync(string ipAddress)
        {
            // CHANGE: should we check existence here?
            if (!(await _ipDAO.CheckIPExistenceAsync(ipAddress)))
            {
                return false;
            }
            IPAddressRecord record = new IPAddressRecord(ipAddress, StringUtilityService.CurrentUnixTime());

            return await _ipDAO.CreateAsync(record);
        }

        /// <summary>
        /// Create a user in the data store.
        /// </summary>
        /// <param name="isTemp">Used to specify whether the user is temporary.</param>
        /// <param name="username">Used to specify the user's username.</param>
        /// <param name="firstName">Used to specify the user's first name.</param>
        /// <param name="lastName">Used to specify the user's last name.</param>
        /// <param name="email">Used to specify the user's email.</param>
        /// <param name="phoneNumber">Used to specify the user's phone number.</param>
        /// <param name="password">Used to specify the user's password digest.</param>
        /// <param name="disabled">Used to specify whether the user is disabled.</param>
        /// <param name="userType">Used to specify the user's type.</param>
        /// <param name="salt">Used to specify the salt associated with the user's password digest.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> CreateUserAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // CHANGE: should we check user existence here 
            if(!await CheckUserExistenceAsync(username))
            {
                return false;
            }

            long tempTimestamp = isTemp ? StringUtilityService.CurrentUnixTime() : 0;

            UserRecord record = new UserRecord(username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt, tempTimestamp, "", 0, 0, 0, 0, 0);

            return await _userDAO.CreateAsync(record);
        }

        /// <summary>
        /// Deletes a user from the data store.
        /// </summary>
        /// <param name="username">Username to be deleted.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> DeleteUserAsync(string username)
        {
            return await _userDAO.DeleteByIdsAsync(new List<string>() { username });
        }

        /// <summary>
        /// Get the user object that represents all the information about a particular user. 
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<UserObject> GetUserInfoAsync(string username)
        {
            return (UserObject) await _userDAO.ReadByIdAsync(username);
        }

        /// <summary>
        /// Update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> MakeTempPermAsync(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        /// <summary>
        /// Store the email verification code that is sent to a user in the data store.
        /// </summary>
        /// <param name="username">Username that the email code is associated with.</param>
        /// <param name="emailCode">The email verification code that is sent to a user.</param>
        /// <param name="emailCodeTimestamp">The time stamp of when the code was sent in unix time.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> StoreEmailCodeAsync(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp);

            return await _userDAO.UpdateAsync(record);
        }

        /// <summary>
        /// Remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> RemoveEmailCodeAsync(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: "", emailCodeTimestamp: 0);

            return await _userDAO.UpdateAsync(record);
        }

        /// <summary>
        /// Disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
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
                //throw new Exception("The username is already disabled!");
                return false;
            }

            UserRecord disabledUser = new UserRecord(username, disabled: 1);
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        /// <summary>
        /// Enable a disabled user to login.
        /// </summary>
        /// <param name="username">Username of a user to enable.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> EnableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);

            if (!(await _userDAO.CheckUserExistenceAsync(username)))
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
            await _userDAO.UpdateAsync(disabledUser);

            return true;
        }

        /// <summary>
        /// Change the password digest associated with a user in the data store.
        /// </summary>
        /// <param name="username">Username of the user to update.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
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
        /// Uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
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

        /// <summary>
        /// Increment the login failures of a user and disables that user if he has 
        /// reached the <paramref name="maxNumberOfTries"/> parameter and his last login failure time 
        /// hs not exceeded the <paramref name="maxTimeBeforeFailureReset"/> parameter.
        /// </summary>
        /// <param name="username">Username of the user to increment</param>
        /// <param name="maxTimeBeforeFailureReset">TimeSpan object to represent how long the before the login failure resets.</param>
        /// <param name="maxNumberOfTries">The max number of tries a user can login before he is disabled.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username);
            UserRecord record;

            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = StringUtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = StringUtilityService.CurrentUnixTime();

            // Need to check if the last maxtime + lastTime is less than now.
            // if it is then reset the failure

            bool reset = false;

            // If the last Login Time has passed the maxTimeBeforeFailureReset then reset their loginFailure number.
            if (lastLoginFailTimestamp + maxSeconds < currentUnix)
            {
                reset = true;
                record = new UserRecord(username, loginFailures: 0);
                await _userDAO.UpdateAsync(record);
            }

            // Increment the user's login Failure count.
            int updatedLoginFailures = reset ? 1 : user.LogInFailures + 1;
            
            // Disable the user if they have reached the max number of tries. 
            // Update the last login fail time.
            if (updatedLoginFailures >= maxNumberOfTries)
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, disabled: 1, lastLoginFailTimestamp: StringUtilityService.CurrentUnixTime());
            }
            else
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, lastLoginFailTimestamp: StringUtilityService.CurrentUnixTime());
            }

            await _userDAO.UpdateAsync(record);
            
            return false;
        }

        /// <summary>
        /// Increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
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
    }
}
