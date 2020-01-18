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
    /// <summary>
    /// This object provides functions to manage users, anonymous or otherwise,
    /// in our system.
    /// </summary>
    public static class UserManagementService
    {
        private static readonly UserDAO _userDAO;
        private static readonly IPAddressDAO _ipDAO;

        /// <summary>
        /// Initiates the object and its dependencies.
        /// </summary>
        static UserManagementService()
        {
            _userDAO = new UserDAO();
            _ipDAO = new IPAddressDAO();
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="username"/>exists in the data store.
        /// </summary>
        /// <param name="username">Username to check (string)</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public static async Task<bool> CheckUserExistenceAsync(string username)
        {
            // Asynchronously call the check method via the User DAO with the username.
            return await _userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="phoneNumber"/> exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check (string)</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public static async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            // Asynchronously call the check method via the User DAO with the phone number.
            return await _userDAO.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="email"/> exists in the data store.
        /// </summary>
        /// <param name="email">Email to check (string)</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public static async Task<bool> CheckEmailExistenceAsync(string email)
        {
            // Asynchronously call the check method via the User DAO with the email.
            return await _userDAO.CheckEmailExistenceAsync(email).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="ipAddress"/> exists in the data store.
        /// </summary>
        /// <param name="ipAddress">ip address to check (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public static async Task<bool> CheckIPExistenceAsync(string ipAddress)
        {
            // Asynchronously call the check method via the IP DAO with the ip address.
            return await _ipDAO.CheckIPExistenceAsync(ipAddress).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a record in the data store with the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">ip address to insert into the data store (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public static async Task<bool> CreateIPAsync(string ipAddress)
        {
            // Initialize the locked time and registration failures to initially have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong);

            MaskingService maskingService = new MaskingService(new MapDAO());

            IPAddressRecord resultRecord = (IPAddressRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            // Asynchronously call the create method via the IP DAO with the record.
            return await _ipDAO.CreateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously gets the <paramref name="ipAddress"/> info and return as an object.
        /// </summary>
        /// <param name="ipAddress">The ip address of the record in the data store (string)</param>
        /// <returns>Task (IPAddressObject) the object representing the ip info</returns>
        public static async Task<IPAddressObject> GetIPAddressInfoAsync(string ipAddress)
        {
            // Cast the return result of asynchronously reading by the ip address into the IP object.
            IPAddressObject ip = (IPAddressObject)await _ipDAO.ReadByIdAsync(ipAddress).ConfigureAwait(false);

            MaskingService maskingService = new MaskingService(new MapDAO());

            return (IPAddressObject)await maskingService.UnMaskAsync(ip).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously unlock the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to unlock (string)</param>
        /// <returns>Task (bool) wheter the function completed without exception</returns>
        public static async Task<bool> UnlockIPAsync(string ipAddress)
        {
            // Make the timestamp locked and registration failures have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, timestampLocked: Constants.NoValueLong, registrationFailures: Constants.NoValueInt);

            MaskingService maskingService = new MaskingService(new MapDAO());

            IPAddressRecord resultRecord = (IPAddressRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            // Asynchronously call the update funciton of the IP DAO with the record.
            return await _ipDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously check if the <paramref name="ipAddress"/> is locked in the data store.
        /// </summary>
        /// <param name="ipAddress">The ip address to check (string)</param>
        /// <returns>Task (bool) whether the ip is locked</returns>
        public static async Task<bool> CheckIfIPLockedAsync(string ipAddress)
        {
            // Asynchronously gets the info of the ipAddress.
            IPAddressObject ip = await GetIPAddressInfoAsync(ipAddress).ConfigureAwait(false);

            // The ip is locked if the timetamp locked has a value.
            return (ip.TimestampLocked != Constants.NoValueLong);
        }

        /// <summary>
        /// Asynchronously checks if the user with the <paramref name="username"/> is disabled in the data store.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Returns true if the user is disabled and false if not.</returns>
        public static async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            return (user.Disabled == Constants.DisabledStatus);
        }

        /// <summary>
        /// Asynchronously create a user in the data store.
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
        public static async Task<bool> CreateUserAsync(bool isTemp, UserRecord record, string adminName, string adminIp)
        {
            // Check that the User of function is an admin.
            UserObject admin = (UserObject)await GetUserInfoAsync(adminName);
            if(admin.UserType != Constants.AdminUserType)
            {
                throw new ArgumentException(Constants.MustBeAdmin);
            }

            // Check for user existence.
            bool result = await CheckUserExistenceAsync((string)record.GetData()["username"]);
            if (!result)
            {
                throw new ArgumentException(Constants.UsernameDNE);
            }

            // If the user being created is temporary, update the timestamp to be the current unix time, otherwise
            // the timestamp has no value.
            long tempTimestamp = isTemp ? UtilityService.CurrentUnixTime() : Constants.NoValueLong;

            record.GetData()["temp_timestamp"] = tempTimestamp;

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            // Log the action.
            await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.SingleUserCreateOperation, adminName, adminIp);


            return await _userDAO.CreateAsync(resultRecord).ConfigureAwait(false);


        }

        public static async Task<bool> CreateUsersAsync(IEnumerable<UserRecord> records, string adminName, string adminIp)
        {
            // Check that the User of function is an admin.
            UserObject admin = (UserObject)await GetUserInfoAsync(adminName);
            if (admin.UserType != Constants.AdminUserType)
            {
                throw new ArgumentException(Constants.MustBeAdmin);
            }

            // Disable this function if project is not in development.
            if (Constants.ProjectStatus != Constants.StatusDev)
            {
                throw new Exception(Constants.NotInDevelopment);
            }

            // Check for user existence for every record
            bool result;
            foreach(UserRecord user in records)
            {
                result = await CheckUserExistenceAsync((string)user.GetData()["username"]);
                if (!result)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
            }

            // Mask personal information about the user before inserting into data store.
            MaskingService maskingService = new MaskingService(new MapDAO());
            foreach (UserRecord user in records)
            {
                    UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(user).ConfigureAwait(false);
                    await _userDAO.CreateAsync(resultRecord).ConfigureAwait(false);
            }

            // Log the bulk create operation.
            await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.BulkUserCreateOperation, adminName, adminIp);

            return true;
        }

        /// <summary>
        /// Makes changes to a single user record.
        /// </summary>
        /// <param name="userRecord">User that needs to be updated.</param>
        /// <returns> Returns true if operation was successful and false otherwise.</returns>
        public static async Task<bool> UpdateUserAsync(ISQLRecord userRecord, string adminName, string adminIp)
        {
            // Check that the User of function is an admin.
            UserObject admin = (UserObject)await GetUserInfoAsync(adminName);
            if (admin.UserType != Constants.AdminUserType)
            {
                throw new ArgumentException(Constants.MustBeAdmin);
            }
            await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.UpdateSingleUserOperation, adminName, adminIp);

            return await _userDAO.UpdateAsync(userRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes changes to multiple user records. 
        /// </summary>
        /// <param name="userRecords"> A collection of records that need to be updated.</param>
        /// <returns> Returns true if successful and false otherwise.</returns>
        public static async Task<bool> BulkUpdateUsersAsync(IEnumerable<ISQLRecord> userRecords, string adminName, string adminIp)
        {
            // Check that the User of function is an admin.
            UserObject admin = (UserObject)await GetUserInfoAsync(adminName);
            if (admin.UserType != Constants.AdminUserType)
            {
                throw new ArgumentException(Constants.MustBeAdmin);
            }

            foreach (ISQLRecord record in userRecords)
            {
                await _userDAO.UpdateAsync(record).ConfigureAwait(false);
            }
            await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.BulkUserUpdateOperation, adminName, adminIp);

            return true;
        }

        /// <summary>
        /// Asynchronously deletes a user from the data store.
        /// </summary>
        /// <param name="username">Username to be deleted.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> DeleteUserAsync(string username)
        {
            // Create a list of the username to pass to the Delet By IDs function.
            return await _userDAO.DeleteByIdsAsync(new List<string>() { username }).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously get the user object that represents all the information about a particular user.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<UserObject> GetUserInfoAsync(string username)
        {
            UserObject rawUser = (UserObject)await _userDAO.ReadByIdAsync(username).ConfigureAwait(false);

            MaskingService maskingService = new MaskingService(new MapDAO());

            return (UserObject)await maskingService.UnMaskAsync(rawUser).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> MakeTempPermAsync(string username)
        {
            // Make the temp timestamp have no value.
            UserRecord record = new UserRecord(username, tempTimestamp: Constants.NoValueLong);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            return await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously store the email verification code that is sent to a user in the data store. Reset their tries.
        /// </summary>
        /// <param name="username">Username that the email code is associated with.</param>
        /// <param name="emailCode">The email verification code that is sent to a user.</param>
        /// <param name="emailCodeTimestamp">The time stamp of when the code was sent in unix time.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> StoreEmailCodeAsync(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp,
                                               emailCodeFailures: Constants.NoValueInt);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            return await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> RemoveEmailCodeAsync(string username)
        {
            // Everything related to the email code has no value.
            UserRecord record = new UserRecord(username, emailCode: Constants.NoValueString,
                                               emailCodeTimestamp: Constants.NoValueLong,
                                               emailCodeFailures: Constants.NoValueInt);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            return await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the user was originally enabled, false otherwise.</returns>
        public static async Task<bool> DisableUserAsync(string username)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // If already disabled, just return false.
            if (user.Disabled == Constants.DisabledStatus)
            {
                return false;
            }

            UserRecord record = new UserRecord(username, disabled: Constants.DisabledStatus);

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously enable a disabled user to login.
        /// </summary>
        /// <param name="username">Username of a user to enable.</param>
        /// <returns>Returns true if the user was originally disabled, false otherwise.</returns>
        public static async Task<bool> EnableUserAsync(string username)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // If already enabled, return false.
            if (user.Disabled == Constants.EnabledStatus)
            {
                return false;
            }

            // Enable the username.
            UserRecord record = new UserRecord(username, disabled: Constants.EnabledStatus);

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously change the password digest associated with a user in the data store.
        /// </summary>
        /// <param name="username">Username of the user to update.</param>
        /// <param name="digest">The digest of password.</param>
        /// <param name="saltString">Salt used to produce the digest.</param>
        /// <returns>Returns true if the operation is successful and false if it failed.</returns>
        public static async Task<bool> ChangePasswordAsync(string username, string digest, string saltString)
        {
            UserRecord record = new UserRecord(username, password: digest, salt: saltString);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            return await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="body">The message to send.</param>
        /// <param name="sysAdminEmailAddress">The email of system admin</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> NotifySystemAdminAsync(string body, string sysAdminEmailAddress)
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

            // Connect over google SMTP, provide credentials, and asynchronously send and disconnect the client.
            await client.ConnectAsync(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
            await client.AuthenticateAsync($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}").ConfigureAwait(false);
            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();

            return true;
        }

        /// <summary>
        /// Asynchronously update the phone code values for the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to update (string)</param>
        /// <param name="numFailures">The number of failures to update to (int)</param>
        /// <returns>Task (bool) whether the function executed without failure</returns>
        public static async Task<bool> UpdatePhoneCodeFailuresAsync(string username, int numFailures)
        {
            UserRecord record = new UserRecord(username, phoneCodeFailures: numFailures);

            MaskingService maskingService = new MaskingService(new MapDAO());

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously increment the login failures of a user and disables that user if he has
        /// reached the <paramref name="maxNumberOfTries"/> parameter and his last login failure time
        /// hs not exceeded the <paramref name="maxTimeBeforeFailureReset"/> parameter.
        /// </summary>
        /// <param name="username">Username of the user to increment</param>
        /// <param name="maxTimeBeforeFailureReset">TimeSpan object to represent how long the before the login failure resets.</param>
        /// <param name="maxNumberOfTries">The max number of tries a user can login before he is disabled.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            UserRecord record;
            UserRecord resultRecord;

            // Need to check if the maxtime + lastTime is less than now.
            // if it is then reset the failure
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

                resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

                await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
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

            resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> IncrementEmailCodeFailuresAsync(string username)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously increment the phone code failures of the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to increment the failures of (string)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public static async Task<bool> IncrementPhoneCodeFailuresAsync(string username)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.PhoneCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, phoneCodeFailures: currentFailures + 1);

            UserRecord resultRecord = (UserRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            // Increment the failure count for that user.
            await _userDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Increment the registration failures of the anonymous user defined by the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to increment the registration failures of (string)</param>
        /// <param name="maxTimeBeforeFailureReset">The time before their failures reset (TimeSpan)</param>
        /// <param name="maxNumberOfTries">The max number of registration tries before they get locked (int)</param>
        /// <returns>Task (bool) whether the funciton executed without exception</returns>
        public static async Task<bool> IncrementRegistrationFailuresAsync(string ipAddress, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            MaskingService maskingService = new MaskingService(new MapDAO());

            IPAddressObject ip = await GetIPAddressInfoAsync(ipAddress).ConfigureAwait(false);

            IPAddressRecord record;
            IPAddressRecord resultRecord;

            // Need to check if the maxtime + lastTime is less than now.
            // if it is then reset the failure
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

                resultRecord = (IPAddressRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

                await _ipDAO.UpdateAsync(resultRecord).ConfigureAwait(false);
            }

            // Increment the user's login Failure count.
            int updatedRegistrationFailures = reset ? 1 : ip.RegistrationFailures + 1;

            // Lock the ip if they have reached the max number of tries.
            // Update the last reg fail time.
            if (updatedRegistrationFailures >= maxNumberOfTries)
            {
                record = new IPAddressRecord(ipAddress, timestampLocked: currentUnix, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);

                // Asynchronously notify the system admin if an ip address was locked during registration.
                await NotifySystemAdminAsync($"{ipAddress} was locked at {currentUnix}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
            }
            else
            {
                record = new IPAddressRecord(ipAddress, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);
            }

            resultRecord = (IPAddressRecord)await maskingService.MaskAsync(record).ConfigureAwait(false);

            await _ipDAO.UpdateAsync(resultRecord).ConfigureAwait(false);

            return true;
        }
    }
}
