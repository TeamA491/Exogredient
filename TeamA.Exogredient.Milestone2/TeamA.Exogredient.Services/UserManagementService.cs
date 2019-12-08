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

        /// <summary>
        /// Checks if a user exists in the data store.
        /// </summary>
        /// <param name="username">Username to check.</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            return await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a phone number exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check.</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if an email exists in the data store.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            return await _userDAO.CheckEmailExistenceAsync(email).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a user is disabled in the data store.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Returns true if the user is disabled and false if not.</returns>
        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            return (user.Disabled == Constants.DisabledStatus);
        }

        /// <summary>
        /// Checks if an IP address in the data store is still locked given the max lock time.
        /// </summary>
        /// <param name="ipAddress">IP address to check</param>
        /// <param name="maxLockTime">Length that the ip address is locked.</param>
        /// <returns>Returns true if the IP address is locked and false if not.</returns>
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

        /// <summary>
        /// Lock and IP address.
        /// </summary>
        /// <param name="ipAddress">IP address to be locked</param>
        /// <returns>Returns true if the operation succeeded and false if it failed.</returns>
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
            if(!await CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }

            long tempTimestamp = isTemp ? UtilityService.CurrentUnixTime() : Constants.NoValueLong;

            UserRecord record = new UserRecord(username, firstName, lastName, email, phoneNumber, password,
                                               disabled, userType, salt, tempTimestamp, Constants.NoValueString, Constants.NoValueLong,
                                               Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            return await _userDAO.CreateAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a user from the data store.
        /// </summary>
        /// <param name="username">Username to be deleted.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> DeleteUserAsync(string username)
        {
            return await _userDAO.DeleteByIdsAsync(new List<string>() { username }).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the user object that represents all the information about a particular user.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<UserObject> GetUserInfoAsync(string username)
        {
            return (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> MakeTempPermAsync(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: Constants.NoValueLong);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
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
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp,
                                               emailCodeFailures: Constants.NoValueInt);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> RemoveEmailCodeAsync(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: Constants.NoValueString,
                                               emailCodeTimestamp: Constants.NoValueLong,
                                               emailCodeFailures: Constants.NoValueInt);

            return await _userDAO.UpdateAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> DisableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // If the username doesn't exist, throw an exception.
            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }
            if (user.Disabled == Constants.DisabledStatus)
            {
                //throw new Exception("The username is already disabled!");
                return false;
            }

            UserRecord disabledUser = new UserRecord(username, disabled: Constants.DisabledStatus);
            await _userDAO.UpdateAsync(disabledUser).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Enable a disabled user to login.
        /// </summary>
        /// <param name="username">Username of a user to enable.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> EnableUserAsync(string username)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                return false;
            }
            if (user.Disabled == Constants.EnabledStatus)
            {
                //throw new Exception("The username is already enabled!");
                return false;
            }
            // Enable the username.
            UserRecord disabledUser = new UserRecord(username, disabled: Constants.EnabledStatus);
            await _userDAO.UpdateAsync(disabledUser).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Change the password digest associated with a user in the data store.
        /// </summary>
        /// <param name="username">Username of the user to update.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task ChangePasswordAsync(string username, string digest, string saltString)
        {
            UserObject user = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            // Check if the username exists.
            if (!await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                // TODO Create Custom Exception: For System
                throw new Exception(Constants.UsernameDNE);
            }
            // Check if the username is disabled.
            if (user.Disabled == Constants.DisabledStatus)
            {
                // TODO Create Custom Exception: For User
                throw new Exception(Constants.UserLocked);
            }

            UserRecord newPasswordUser = new UserRecord(username, password: digest, salt: saltString);
            await _userDAO.UpdateAsync(newPasswordUser).ConfigureAwait(false);
        }

        /// <summary>
        /// Uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> NotifySystemAdminAsync(string body, string sysAdminEmailAddress)
        {
            string title = DateTime.UtcNow.ToString(Constants.NotifySysAdminSubjectFormatString);

            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            message.From.Add(new MailboxAddress($"{Constants.SystemEmailAddress}"));
            message.To.Add(new MailboxAddress($"{sysAdminEmailAddress}"));

            message.Subject = title;

            Random generator = new Random();

            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
            };

            await client.ConnectAsync(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
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

        /// <summary>
        /// Increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
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
