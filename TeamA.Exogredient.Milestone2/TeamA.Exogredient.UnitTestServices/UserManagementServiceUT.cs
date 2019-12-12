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
    /// <summary>
    /// This object provides functions to manage users, anonymous or otherwise,
    /// in our system.
    /// </summary>
    public static class UserManagementServiceUT
    {
        private static readonly UnitTestUserDAO _userDAO;
        private static readonly UnitTestIPAddressDAO _ipDAO;

        /// <summary>
        /// Initiates the object and its dependencies.
        /// </summary>
        static UserManagementServiceUT()
        {
            _userDAO = new UnitTestUserDAO();
            _ipDAO = new UnitTestIPAddressDAO();
        }

        /// <summary>
        /// Checks if the <paramref name="username"/>exists in the data store.
        /// </summary>
        /// <param name="username">Username to check (string)</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public static bool CheckUserExistence(string username)
        {
            // Call the check method via the User DAO with the username.
            return _userDAO.CheckUserExistence(username);
        }

        /// <summary>
        /// Checks if the <paramref name="phoneNumber"/> exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check (string)</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public static bool CheckPhoneNumberExistence(string phoneNumber)
        {
            // Call the check method via the User DAO with the phone number.
            return _userDAO.CheckPhoneNumberExistence(phoneNumber);
        }

        /// <summary>
        /// Checks if the <paramref name="email"/> exists in the data store.
        /// </summary>
        /// <param name="email">Email to check (string)</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public static bool CheckEmailExistence(string email)
        {
            // Call the check method via the User DAO with the email.
            return _userDAO.CheckEmailExistence(email);
        }

        /// <summary>
        /// Checks if the <paramref name="ipAddress"/> exists in the data store.
        /// </summary>
        /// <param name="ipAddress">ip address to check (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public static bool CheckIPExistence(string ipAddress)
        {
            // Call the check method via the IP DAO with the ip address.
            return _ipDAO.CheckIPExistence(ipAddress);
        }

        /// <summary>
        /// Creates a record in the data store with the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">ip address to insert into the data store (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public static bool CreateIP(string ipAddress)
        {
            // Initialize the locked time and registration failures to initially have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong);

            // Call the create method via the IP DAO with the record.
            return _ipDAO.Create(record);
        }

        /// <summary>
        /// Gets the <paramref name="ipAddress"/> info and return as an object.
        /// </summary>
        /// <param name="ipAddress">The ip address of the record in the data store (string)</param>
        /// <returns>Task (IPAddressObject) the object representing the ip info</returns>
        public static IPAddressObject GetIPAddressInfo(string ipAddress)
        {
            // Cast the return result of reading by the ip address into the IP object.
            return (IPAddressObject)_ipDAO.ReadById(ipAddress);
        }

        /// <summary>
        /// Unlock the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to unlock (string)</param>
        /// <returns>Task (bool) wheter the function completed without exception</returns>
        public static bool UnlockIP(string ipAddress)
        {
            // Make the timestamp locked and registration failures have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, timestampLocked: Constants.NoValueLong, registrationFailures: Constants.NoValueInt);

            // Call the update funciton of the IP DAO with the record.
            return _ipDAO.Update(record);
        }

        /// <summary>
        /// Check if the <paramref name="ipAddress"/> is locked in the data store.
        /// </summary>
        /// <param name="ipAddress">The ip address to check (string)</param>
        /// <returns>Task (bool) whether the ip is locked</returns>
        public static bool CheckIfIPLocked(string ipAddress)
        {
            // Gets the info of the ipAddress.
            IPAddressObject ip = GetIPAddressInfo(ipAddress);

            // The ip is locked if the timetamp locked has a value.
            return (ip.TimestampLocked != Constants.NoValueLong);
        }

        /// <summary>
        /// Checks if the user with the <paramref name="username"/> is disabled in the data store.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Returns true if the user is disabled and false if not.</returns>
        public static bool CheckIfUserDisabled(string username)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);

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
            // If the user being created is temporary, update the timestamp to be the current unix time, otherwise
            // the timestamp has no value.
            long tempTimestamp = isTemp ? UtilityService.CurrentUnixTime() : Constants.NoValueLong;

            // Email code, email code timestamp, login failures, last login failure timestamp, email code failures,
            // and phone code failures initialized to have no value.
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
            // Create a list of the username to pass to the Delet By IDs function.
            return _userDAO.DeleteByIds(new List<string>() { username });
        }

        /// <summary>
        /// Get the user object that represents all the information about a particular user.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static UserObject GetUserInfo(string username)
        {
            return (UserObject)_userDAO.ReadById(username);
        }

        /// <summary>
        /// Update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool MakeTempPerm(string username)
        {
            // Make the temp timestamp have no value.
            UserRecord record = new UserRecord(username, tempTimestamp: Constants.NoValueLong);

            return _userDAO.Update(record);
        }

        /// <summary>
        /// Store the email verification code that is sent to a user in the data store. Reset their tries.
        /// </summary>
        /// <param name="username">Username that the email code is associated with.</param>
        /// <param name="emailCode">The email verification code that is sent to a user.</param>
        /// <param name="emailCodeTimestamp">The time stamp of when the code was sent in unix time.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool StoreEmailCode(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp,
                                               emailCodeFailures: Constants.NoValueInt);

            return _userDAO.Update(record);
        }

        /// <summary>
        /// Remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool RemoveEmailCode(string username)
        {
            // Everything related to the email code has no value.
            UserRecord record = new UserRecord(username, emailCode: Constants.NoValueString,
                                               emailCodeTimestamp: Constants.NoValueLong,
                                               emailCodeFailures: Constants.NoValueInt);

            return _userDAO.Update(record);
        }

        /// <summary>
        /// Disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the user was originally enabled, false otherwise.</returns>
        public static bool DisableUser(string username)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);

            // If already disabled, just return false.
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
        public static bool EnableUser(string username)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);

            // If already enabled, return false.
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
        public static bool ChangePassword(string username, string digest, string saltString)
        {
            UserRecord newPasswordUser = new UserRecord(username, password: digest, salt: saltString);
            return _userDAO.Update(newPasswordUser);
        }

        /// <summary>
        /// Uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool NotifySystemAdmin(string body, string sysAdminEmailAddress)
        {
            // Creates a thread for each day containing which exceptions maxed out.
            string title = DateTime.UtcNow.ToString(Constants.NotifySysAdminSubjectFormatString);

            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            // From the system email address to the sysAdminEmailAddress.
            message.From.Add(new MailboxAddress($"{Constants.SystemEmailAddress}"));
            message.To.Add(new MailboxAddress($"{sysAdminEmailAddress}"));

            message.Subject = title;

            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            // Create the SMTP client with the default certificate validation callback to prevent man in the middle attacks.
            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
            };

            // Connect over google SMTP, provide credentials, and send and disconnect the client.
            client.Connect(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect);
            client.Authenticate($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}");
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

            return true;
        }

        /// <summary>
        /// Update the phone code values for the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to update (string)</param>
        /// <param name="numFailures">The number of failures to update to (int)</param>
        /// <returns>Task (bool) whether the function executed without failure</returns>
        public static bool UpdatePhoneCodeFailures(string username, int numFailures)
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
        public static bool IncrementLoginFailures(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);
            UserRecord record;

            // Need to check if the maxtime + lastTime is less than now.
            // If it is then reset the failure
            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = UtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = UtilityService.CurrentUnixTime();

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures. Don't reset if
            // they have no last login fail timestamp.
            if (lastLoginFailTimestamp + maxSeconds < currentUnix && lastLoginFailTimestamp != Constants.NoValueLong)
            {
                reset = true;
                record = new UserRecord(username, loginFailures: 0);
                _userDAO.Update(record);
            }

            // Increment the user's login failure count.
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

            return true;
        }

        /// <summary>
        /// Increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static bool IncrementEmailCodeFailures(string username)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            _userDAO.Update(record);

            return true;
        }

        /// <summary>
        /// Increment the phone code failures of the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to increment the failures of (string)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public static bool IncrementPhoneCodeFailures(string username)
        {
            UserObject user = (UserObject)_userDAO.ReadById(username);

            // Get the current failure count.
            int currentFailures = user.PhoneCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, phoneCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            _userDAO.Update(record);

            return true;
        }

        /// <summary>
        /// Increment the registration failures of the anonymous user defined by the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to increment the registration failures of (string)</param>
        /// <param name="maxTimeBeforeFailureReset">The time before their failures reset (TimeSpan)</param>
        /// <param name="maxNumberOfTries">The max number of registration tries before they get locked (int)</param>
        /// <returns>Task (bool) whether the funciton executed without exception</returns>
        public static bool IncrementRegistrationFailures(string ipAddress, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            IPAddressObject ip = (IPAddressObject)_ipDAO.ReadById(ipAddress);
            IPAddressRecord record;

            // Need to check if the maxtime + lastTime is less than now.
            // If it is then reset the failure
            long lastRegFailTimestamp = ip.LastRegFailTimestamp;
            long maxSeconds = UtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = UtilityService.CurrentUnixTime();

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures. Don't reset
            // if they have no last registration fail timestamp.
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

                // Notify the system admin if an ip address was locked during registration.
                NotifySystemAdmin($"{ipAddress} was locked at {currentUnix}", Constants.SystemAdminEmailAddress);
            }
            else
            {
                record = new IPAddressRecord(ipAddress, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);
            }

            _ipDAO.Update(record);

            return true;
        }
    }
}
