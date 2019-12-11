using System;
using System.Collections.Generic;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.UnitTestServices
{
    public static class UserManagementServiceUT
    {
        private static readonly UnitTestUserDAO _userDAO;
        private static readonly UnitTestIPAddressDAO _ipDAO;

        static UserManagementServiceUT()
        {
            _userDAO = new UnitTestUserDAO();
            _ipDAO = new UnitTestIPAddressDAO();
        }

        /// <summary>
        /// Checks if a user exists in the data store.
        /// </summary>
        /// <param name="username">Username to check.</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public static bool CheckUserExistence(string username)
        {
            return _userDAO.CheckUserExistence(username);
        }

        /// <summary>
        /// Checks if a phone number exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check.</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public static bool CheckPhoneNumberExistence(string phoneNumber)
        {
            return _userDAO.CheckPhoneNumberExistence(phoneNumber);
        }

        /// <summary>
        /// Checks if an email exists in the data store.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public static bool CheckEmailExistence(string email)
        {
            return _userDAO.CheckEmailExistence(email);
        }

        public static bool CheckIPExistence(string ipAddress)
        {
            return _ipDAO.CheckIPExistence(ipAddress);
        }

        public static bool CreateIP(string ipAddress)
        {
            IPAddressRecord record = new IPAddressRecord(ipAddress, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong);
            return _ipDAO.Create(record);
        }

        public static IPAddressObject GetIPAddressInfo(string ipAddress)
        {
            return (IPAddressObject) _ipDAO.ReadById(ipAddress);
        }

        public static bool UnlockIP(string ipAddress)
        {
            IPAddressRecord record = new IPAddressRecord(ipAddress, timestampLocked: Constants.NoValueLong, registrationFailures: Constants.NoValueInt);
            return  _ipDAO.Update(record);
        }


        public static bool CheckIfIPLocked(string ipAddress)
        {
            IPAddressObject ip = GetIPAddressInfo(ipAddress);
            return (ip.TimestampLocked != Constants.NoValueLong);
        }

        /// <summary>
        /// Checks if a user is disabled in the data store.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Returns true if the user is disabled and false if not.</returns>
        public static bool CheckIfUserDisabled(string username)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);
            return (user.Disabled == Constants.DisabledStatus);
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
        public static bool CreateUser(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            long tempTimestamp = isTemp ? UtilityService.CurrentUnixTime() : Constants.NoValueLong;

            UserRecord record = new UserRecord(username, firstName, lastName, email, phoneNumber, password,
                                               disabled, userType, salt, tempTimestamp, Constants.NoValueString, Constants.NoValueLong,
                                               Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            return _userDAO.Create(record);
        }

        /// <summary>
        /// Deletes a user from the data store.
        /// </summary>
        /// <param name="username">Username to be deleted.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool DeleteUser(string username)
        {
            return _userDAO.DeleteByIds(new List<string>() { username });
        }

        /// <summary>
        /// Get the user object that represents all the information about a particular user.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static UserObject GetUserInfo(string username)
        {
            return (UserObject) _userDAO.ReadById(username);
        }

        /// <summary>
        /// Update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool MakeTempPerm(string username)
        {
            UserRecord record = new UserRecord(username, tempTimestamp: Constants.NoValueLong);

            return _userDAO.Update(record);
        }

        /// <summary>
        /// Store the email verification code that is sent to a user in the data store.
        /// </summary>
        /// <param name="username">Username that the email code is associated with.</param>
        /// <param name="emailCode">The email verification code that is sent to a user.</param>
        /// <param name="emailCodeTimestamp">The time stamp of when the code was sent in unix time.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static  bool StoreEmailCode(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp,
                                               emailCodeFailures: Constants.NoValueInt);

            return  _userDAO.Update(record);
        }

        /// <summary>
        /// Remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static  bool RemoveEmailCode(string username)
        {
            UserRecord record = new UserRecord(username, emailCode: Constants.NoValueString,
                                               emailCodeTimestamp: Constants.NoValueLong,
                                               emailCodeFailures: Constants.NoValueInt);

            return  _userDAO.Update(record);
        }

        /// <summary>
        /// Disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the user was originally enabled, false otherwise.</returns>
        public static  bool DisableUser(string username)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);

            if (user.Disabled == Constants.DisabledStatus)
            {
                return false;
            }

            UserRecord disabledUser = new UserRecord(username, disabled: Constants.DisabledStatus);
             _userDAO.Update(disabledUser);

            return true;
        }

        /// <summary>
        /// Enable a disabled user to login.
        /// </summary>
        /// <param name="username">Username of a user to enable.</param>
        /// <returns>Returns true if the user was originally disabled, false otherwise.</returns>
        public static  bool EnableUser(string username)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);

            if (user.Disabled == Constants.EnabledStatus)
            {
                return false;
            }

            // Enable the username.
            UserRecord disabledUser = new UserRecord(username, disabled: Constants.EnabledStatus);
             _userDAO.Update(disabledUser);

            return true;
        }

        /// <summary>
        /// Change the password digest associated with a user in the data store.
        /// </summary>
        /// <param name="username">Username of the user to update.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>Returns true if the operation is successful and false if it failed.</returns>
        public static  void ChangePassword(string username, string digest, string saltString)
        {
            UserRecord newPasswordUser = new UserRecord(username, password: digest, salt: saltString);
             _userDAO.Update(newPasswordUser);
        }

        /// <summary>
        /// Uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static  bool NotifySystemAdmin(string body, string sysAdminEmailAddress)
        {
            // Creates a thread for each day, which exceptions maxed out.
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

             client.Connect(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect);
             client.Authenticate($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}");
             client.Send(message);
             client.Disconnect(true);
            client.Dispose();

            return true;
        }

        public static  bool UpdatePhoneCodeFailures(string username, int numFailures)
        {
            UserRecord record = new UserRecord(username, phoneCodeFailures: numFailures);

             _userDAO.Update(record);

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
        public static  bool IncrementLoginFailures(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);
            UserRecord record;

            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = UtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = UtilityService.CurrentUnixTime();

            // Need to check if the last maxtime + lastTime is less than now.
            // if it is then reset the failure

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures
            if (lastLoginFailTimestamp + maxSeconds < currentUnix && lastLoginFailTimestamp != Constants.NoValueLong)
            {
                reset = true;
                record = new UserRecord(username, loginFailures: 0);
                 _userDAO.Update(record);
            }

            // Increment the user's login Failure count.
            int updatedLoginFailures = reset ? 1 : user.LogInFailures + 1;

            // Disable the user if they have reached the max number of tries.
            // Update the last login fail time.
            if (updatedLoginFailures >= maxNumberOfTries)
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, disabled: Constants.DisabledStatus, lastLoginFailTimestamp: currentUnix);
            }
            else
            {
                record = new UserRecord(username, loginFailures: updatedLoginFailures, lastLoginFailTimestamp: currentUnix);
            }

             _userDAO.Update(record);

            return false;
        }

        /// <summary>
        /// Increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static  bool IncrementEmailCodeFailures(string username)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
             _userDAO.Update(record);

            return true;
        }

        public static  bool IncrementPhoneCodeFailures(string username)
        {
            UserObject user = (UserObject) _userDAO.ReadById(username);

            // Get the current failure count.
            int currentFailures = user.PhoneCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, phoneCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
             _userDAO.Update(record);

            return true;
        }

        public static  bool IncrementRegistrationFailures(string ipAddress, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            IPAddressObject ip = (IPAddressObject) _ipDAO.ReadById(ipAddress);
            IPAddressRecord record;

            long lastRegFailTimestamp = ip.LastRegFailTimestamp;
            long maxSeconds = UtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = UtilityService.CurrentUnixTime();

            // Need to check if the last maxtime + lastTime is less than now.
            // if it is then reset the failure

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures
            if (lastRegFailTimestamp + maxSeconds < currentUnix && lastRegFailTimestamp != Constants.NoValueLong)
            {
                reset = true;
                record = new IPAddressRecord(ipAddress, registrationFailures: 0);
                 _ipDAO.Update(record);
            }

            // Increment the user's login Failure count.
            int updatedRegistrationFailures = reset ? 1 : ip.RegistrationFailures + 1;

            // Lock the ip if they have reached the max number of tries.
            // Update the last reg fail time.
            if (updatedRegistrationFailures >= maxNumberOfTries)
            {
                record = new IPAddressRecord(ipAddress, timestampLocked: currentUnix, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);
                 NotifySystemAdmin($"{ipAddress} was locked at {currentUnix}", Constants.SystemAdminEmailAddress);
            }
            else
            {
                record = new IPAddressRecord(ipAddress, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);
            }

             _ipDAO.Update(record);

            return false;
        }
    }
}
