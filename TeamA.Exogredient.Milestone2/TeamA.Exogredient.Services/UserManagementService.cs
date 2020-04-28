using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;


namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// This object provides functions to manage users, anonymous or otherwise,
    /// in our system.
    /// </summary>
    public class UserManagementService
    {
        private readonly UserDAO _userDAO;
        private readonly AnonymousUserDAO _anonymousUserDAO;
        private readonly MaskingService _maskingService;
        private readonly DataStoreLoggingService _dsLoggingService;
        private readonly FlatFileLoggingService _ffLoggingService;
        /// <summary>
        /// Initiates the object and its dependencies.
        /// </summary>
        public UserManagementService(UserDAO userDAO, AnonymousUserDAO anonymousUserDAO,
                                     DataStoreLoggingService dsLoggingService, FlatFileLoggingService ffLoggingService,
                                     MaskingService maskingService)
        {
            _userDAO = userDAO;
            _anonymousUserDAO = anonymousUserDAO;
            _dsLoggingService = dsLoggingService;
            _ffLoggingService = ffLoggingService;
            _maskingService = maskingService;

        }

        /// <summary>
        /// Asynchronously logs to the data store and flat file.
        /// </summary>
        /// <param name="timestamp">The timestamp of the operation.</param>
        /// <param name="operation">The type of the operation.</param>
        /// <param name="identifier">The identifier of the operation's performer.</param>
        /// <param name="ipAddress">The ip address of the operation's performer.</param>
        /// <param name="errorType">The type of error that occurred during the operation (default = no error)</param>
        /// <returns>Task (bool) whether the logging operation was successful.</returns>
        private async Task<bool> LogAsync(string timestamp, string operation, string identifier,
                                                string ipAddress, string errorType = Constants.NoError)
        {
            // Attempt logging to both the data store and the flat file and track the results.
            bool ffLoggingResult = await _ffLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
            bool dsLoggingResult = await _dsLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

            int count = 0;

            // Retry whichever one failed, a maximum number of times.
            while (!(ffLoggingResult && dsLoggingResult) && count < Constants.LoggingRetriesAmount)
            {
                if (!ffLoggingResult)
                {
                    ffLoggingResult = await _ffLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
                }
                if (!dsLoggingResult)
                {
                    dsLoggingResult = await _dsLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);
                }
                count++;
            }

            // If both succeeded we are finished.
            if (ffLoggingResult && dsLoggingResult)
            {
                return true;
            }
            else
            {
                // Otherwise, if both failed notify the system admin.
                if (!ffLoggingResult && !dsLoggingResult)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"Data Store and Flat File Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }
                else
                {
                    // Otherwise rollback whichever one succeeded and notify the system admin.

                    bool rollbackSuccess = false;

                    if (ffLoggingResult)
                    {
                        rollbackSuccess = await _ffLoggingService.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);
                    }
                    if (dsLoggingResult)
                    {
                        rollbackSuccess = await _dsLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);
                    }

                    await SystemUtilityService.NotifySystemAdminAsync($"{(ffLoggingResult ? "Flat File" : "Data Store")} Logging failure for the following information:\n\n\t{timestamp}, {operation}, {identifier}, {ipAddress}, {errorType}\n\nRollback status: {(rollbackSuccess ? "successful" : "failed")}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return false;
            }
        }

        /// <summary>
        /// Asynchronously creates a record in the data store with the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">ip address to insert into the data store (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> CreateIPAsync(string ipAddress)
        {
            // Initialize the locked time and registration failures to initially have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong);

            IPAddressRecord resultRecord = await _maskingService.MaskAsync(record, true).ConfigureAwait(false) as IPAddressRecord;

            // Asynchronously call the create method via the IP DAO with the record.
            return await _anonymousUserDAO.CreateAsync(resultRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an entry in the IP Address data store using the <paramref name="ipRecord"/>.
        /// </summary>
        /// <param name="ipRecord">The unmasked record conveying the data to update.</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> UpdateIPAsync(IPAddressRecord ipRecord)
        {
            // Record must be unmasked.
            if (ipRecord.IsMasked())
            {
                throw new ArgumentException(Constants.UpdateIPRecordMasked);
            }

            // If the column is masked, mask the ip.
            string id = ipRecord.GetData()[Constants.AnonymousUserDAOIPColumn] as string;

            if (Constants.AnonymousUserDAOIsColumnMasked[Constants.AnonymousUserDAOIPColumn])
            {
                id = _maskingService.MaskString(id);
            }

            IPAddressRecord maskedRecord = await _maskingService.MaskAsync(ipRecord, false).ConfigureAwait(false) as IPAddressRecord;

            // Get the masked object from the ipDAO and decrement its mapping before updating.
            IPAddressObject maskedObj = await _anonymousUserDAO.ReadByIdAsync(id).ConfigureAwait(false) as IPAddressObject;

            await _maskingService.DecrementMappingForUpdateAsync(maskedRecord, maskedObj).ConfigureAwait(false);

            return await _anonymousUserDAO.UpdateAsync(maskedRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the entry in the data store pointed to by the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to delete from the data store.</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> DeleteIPAsync(string ipAddress)
        {
            // Check for ip existence.
            if (!await CheckIPExistenceAsync(ipAddress).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.DeleteIPDNE);
            }

            // If the column is masked, mask the ip.
            string id = ipAddress;

            if (Constants.AnonymousUserDAOIsColumnMasked[Constants.AnonymousUserDAOIPColumn])
            {
                id = _maskingService.MaskString(ipAddress);
            }

            // Get the masked object from the ipDAO and decrement its mapping before deleteing.
            IPAddressObject maskedObj = await _anonymousUserDAO.ReadByIdAsync(id).ConfigureAwait(false) as IPAddressObject;

            await _maskingService.DecrementMappingForDeleteAsync(maskedObj).ConfigureAwait(false);

            await _anonymousUserDAO.DeleteByIdsAsync(new List<string>() { id }).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously create a user in the data store.
        /// </summary>
        /// <param name="isTemp">Used to specify whether the user is temporary.</param>
        /// <param name="record">The unmasked record conveying the data to create</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> CreateUserAsync(bool isTemp, UserRecord record, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            // Check that the user of function is an admin and throw an exception if they are not.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            // Record must be unmasked.
            if (record.IsMasked())
            {
                throw new ArgumentException(Constants.CreateUserRecordMasked);
            }

            // Check for user existence and throw an exception if the user already exists.
            if (await CheckUserExistenceAsync(record.GetData()[Constants.UserDAOusernameColumn] as string).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.UsernameExistsLogMessage);
            }

            // If the user being created is temporary, update the timestamp to be the current unix time, otherwise
            // the timestamp has no value.
            long tempTimestamp = isTemp ? TimeUtilityService.CurrentUnixTime() : Constants.NoValueLong;
            record.GetData()[Constants.UserDAOtempTimestampColumn] = tempTimestamp;

            // Mask all the sensitive information.
            UserRecord resultRecord = await _maskingService.MaskAsync(record, true).ConfigureAwait(false) as UserRecord;

            // Insert the masked user into the datastore.
            await _userDAO.CreateAsync(resultRecord).ConfigureAwait(false);

            // Log the action.
            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.SingleUserCreateOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously creates all users represented by the <paramref name="records"/>.
        /// </summary>
        /// <param name="records">The unmasked records conveying the data to create</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> BulkCreateUsersAsync(IEnumerable<UserRecord> records, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            // Check that the user of function is an admin and throw an exception if they are not.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            // Disable this function if project is not in development.
            if (Constants.ProjectStatus != Constants.StatusDev)
            {
                throw new Exception(Constants.NotInDevelopment);
            }

            // Check for user existence and throw an exception if the user already exists.
            bool result;
            foreach (UserRecord user in records)
            {
                // Record must be unmasked.
                if (user.IsMasked())
                {
                    throw new ArgumentException(Constants.CreateUsersRecordMasked);
                }

                result = await CheckUserExistenceAsync(user.GetData()[Constants.UserDAOusernameColumn] as string).ConfigureAwait(false);
                if (result)
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
            }

            // Mask personal information about the users before inserting into datastore.
            foreach (UserRecord user in records)
            {
                UserRecord resultRecord = await _maskingService.MaskAsync(user, true).ConfigureAwait(false) as UserRecord;
                await _userDAO.CreateAsync(resultRecord).ConfigureAwait(false);
            }

            // Log the bulk create operation.
            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.BulkUserCreateOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Makes changes to a single user.
        /// </summary>
        /// <param name="userRecord">The unmasked record conveying the data to update</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> UpdateUserAsync(UserRecord userRecord, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            // Check that the User of function is an admin.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            // Record must be unmasked.
            if (userRecord.IsMasked())
            {
                throw new ArgumentException(Constants.UpdateUserRecordMasked);
            }

            // If the column is masked, mask the username.
            string id = userRecord.GetData()[Constants.UserDAOusernameColumn] as string;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                id = _maskingService.MaskString(id);
            }

            UserRecord maskedRecord = await _maskingService.MaskAsync(userRecord, false).ConfigureAwait(false) as UserRecord;

            // Get the masked object from the userDAO and decrement its mapping before updating.
            UserObject maskedObj = await _userDAO.ReadByIdAsync(id).ConfigureAwait(false) as UserObject;

            await _maskingService.DecrementMappingForUpdateAsync(maskedRecord, maskedObj).ConfigureAwait(false);

            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.UpdateSingleUserOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return await _userDAO.UpdateAsync(maskedRecord).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes changes to multiple users. 
        /// </summary>
        /// <param name="userRecords"> A collection of records that need to be updated.</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> BulkUpdateUsersAsync(IEnumerable<UserRecord> userRecords, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            // Check that the User of function is an admin.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            foreach (UserRecord record in userRecords)
            {
                // Record must be unmasked.
                if (record.IsMasked())
                {
                    throw new ArgumentException(Constants.BulkUpdateUsersRecordMasked);
                }

                // If the column is masked, mask the username.
                string id = record.GetData()[Constants.UserDAOusernameColumn] as string;

                if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
                {
                    id = _maskingService.MaskString(id);
                }

                UserRecord maskedRecord = await _maskingService.MaskAsync(record, false).ConfigureAwait(false) as UserRecord;

                // Get the masked object from the userDAO and decrement its mapping before updating.
                UserObject maskedObj = await _userDAO.ReadByIdAsync(id).ConfigureAwait(false) as UserObject;

                await _maskingService.DecrementMappingForUpdateAsync(maskedRecord, maskedObj).ConfigureAwait(false);

                await _userDAO.UpdateAsync(maskedRecord).ConfigureAwait(false);
            }

            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.BulkUserUpdateOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously deletes a user from the data store.
        /// </summary>
        /// <param name="username">Username to be deleted.</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> DeleteUserAsync(string username, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {

            // Check that the User of function is an admin.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            // Check for user existence.
            if (!await CheckUserExistenceAsync(username).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.UsernameDNE);
            }

            // If the column is masked, mask the username.
            string id = username;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                id = _maskingService.MaskString(username);
            }

            // Get the masked object from the userDAO and decrement its mapping before deleteing.
            UserObject maskedObj = await _userDAO.ReadByIdAsync(id).ConfigureAwait(false) as UserObject;

            await _maskingService.DecrementMappingForDeleteAsync(maskedObj).ConfigureAwait(false);

            await _userDAO.DeleteByIdsAsync(new List<string>() { id }).ConfigureAwait(false);

            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.SingleUserDeleteOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously deletes multiple users from the data store.
        /// </summary>
        /// <param name="usernames">List of usernames to be deleted.</param>
        /// <param name="adminName">The username of the admin performing this operation (default = system)</param>
        /// <param name="adminIp">The ip address of the admin performing this operation (default = localhost)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> BulkDeleteUsersAsync(List<string> usernames, string adminName = Constants.SystemIdentifier, string adminIp = Constants.LocalHost)
        {
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            // Check for user existence for every username.
            foreach (string user in usernames)
            {
                if (!await CheckUserExistenceAsync(user).ConfigureAwait(false))
                {
                    throw new ArgumentException(Constants.UsernameDNE);
                }
            }

            List<string> ids = new List<string>();

            foreach (string user in usernames)
            {
                // If the column is masked, mask the username.
                string id = user;

                if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
                {
                    id = _maskingService.MaskString(user);
                }

                ids.Add(id);

                // Get the masked object from the userDAO and decrement its mapping before deleteing.
                UserObject maskedObj = await _userDAO.ReadByIdAsync(id).ConfigureAwait(false) as UserObject;

                await _maskingService.DecrementMappingForDeleteAsync(maskedObj).ConfigureAwait(false);
            }

            await _userDAO.DeleteByIdsAsync(ids).ConfigureAwait(false);

            // Log the bulk create operation.
            await LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.BulkUserDeleteOperation,
                                          adminName, adminIp).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="username"/>exists in the data store.
        /// </summary>
        /// <param name="username">Username to check (string)</param>
        /// <returns>Returns true if the user exists and false if not.</returns>
        public async Task<bool> CheckUserExistenceAsync(string username)
        {
            string value = username;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                value = _maskingService.MaskString(username);
            }

            // Asynchronously call the check method via the User DAO with the username.
            return await _userDAO.CheckUserExistenceAsync(value).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="phoneNumber"/> exists in the data store.
        /// </summary>
        /// <param name="phoneNumber">Phone number to check (string)</param>
        /// <returns>Returns true if the phone number exists and false if not.</returns>
        public async Task<bool> CheckPhoneNumberExistenceAsync(string phoneNumber)
        {
            string value = phoneNumber;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOphoneNumberColumn])
            {
                value = _maskingService.MaskString(phoneNumber);
            }

            // Asynchronously call the check method via the User DAO with the phone number.
            return await _userDAO.CheckPhoneNumberExistenceAsync(value).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="email"/> exists in the data store.
        /// </summary>
        /// <param name="email">Email to check (string)</param>
        /// <returns>Returns true if the email exists and false if not.</returns>
        public async Task<bool> CheckEmailExistenceAsync(string email)
        {
            string value = email;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOemailColumn])
            {
                value = _maskingService.MaskString(email);
            }

            // Asynchronously call the check method via the User DAO with the email.
            return await _userDAO.CheckEmailExistenceAsync(value).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously checks if the <paramref name="ipAddress"/> exists in the data store.
        /// </summary>
        /// <param name="ipAddress">ip address to check (string)</param>
        /// <returns>Task (bool) whether the function completed without exception</returns>
        public async Task<bool> CheckIPExistenceAsync(string ipAddress)
        {
            string value = ipAddress;

            if (Constants.AnonymousUserDAOIsColumnMasked[Constants.AnonymousUserDAOIPColumn])
            {
                value = _maskingService.MaskString(ipAddress);
            }

            // Asynchronously call the check method via the IP DAO with the ip address.
            return await _anonymousUserDAO.CheckIPExistenceAsync(value).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously check if the <paramref name="ipAddress"/> is locked in the data store.
        /// </summary>
        /// <param name="ipAddress">The ip address to check (string)</param>
        /// <returns>Task (bool) whether the ip is locked</returns>
        public async Task<bool> CheckIfIPLockedAsync(string ipAddress)
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
        public async Task<bool> CheckIfUserDisabledAsync(string username)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            return (user.Disabled == Constants.DisabledStatus);
        }

        /// <summary>
        /// Asynchronously gets the <paramref name="ipAddress"/> info and return as an object.
        /// </summary>
        /// <param name="ipAddress">The ip address of the record in the data store (string)</param>
        /// <returns>Task (IPAddressObject) the object representing the ip info</returns>
        public async Task<IPAddressObject> GetIPAddressInfoAsync(string ipAddress)
        {
            string id = ipAddress;

            if (Constants.AnonymousUserDAOIsColumnMasked[Constants.AnonymousUserDAOIPColumn])
            {
                id = _maskingService.MaskString(ipAddress);
            }

            // Cast the return result of asynchronously reading by the ip address into the IP object.
            IPAddressObject ip = await _anonymousUserDAO.ReadByIdAsync(id).ConfigureAwait(false) as IPAddressObject;

            return await _maskingService.UnMaskAsync(ip).ConfigureAwait(false) as IPAddressObject;
        }

        /// <summary>
        /// Asynchronously get the user object that represents all the information about a particular user.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public async Task<UserObject> GetUserInfoAsync(string username)
        {
            string id = username;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                id = _maskingService.MaskString(username);
            }

            UserObject rawUser = await _userDAO.ReadByIdAsync(id).ConfigureAwait(false) as UserObject;

            return await _maskingService.UnMaskAsync(rawUser).ConfigureAwait(false) as UserObject;
        }

        /// <summary>
        /// Asynchronously unlock the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to unlock (string)</param>
        /// <returns>Task (bool) wheter the function completed without exception</returns>
        public async Task<bool> UnlockIPAsync(string ipAddress)
        {
            // Make the timestamp locked and registration failures have no value.
            IPAddressRecord record = new IPAddressRecord(ipAddress, timestampLocked: Constants.NoValueLong, registrationFailures: Constants.NoValueInt);

            // Asynchronously call the update funciton of the IP DAO with the record.
            return await UpdateIPAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously update a user in the data store to make him permanent.
        /// </summary>
        /// <param name="username">Username of the user to make permanent.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public async Task<bool> MakeTempPermAsync(string username)
        {
            // Make the temp timestamp have no value.
            UserRecord record = new UserRecord(username, tempTimestamp: Constants.NoValueLong);

            return await UpdateUserAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously store the email verification code that is sent to a user in the data store. Reset their tries.
        /// </summary>
        /// <param name="username">Username that the email code is associated with.</param>
        /// <param name="emailCode">The email verification code that is sent to a user.</param>
        /// <param name="emailCodeTimestamp">The time stamp of when the code was sent in unix time.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public async Task<bool> StoreEmailCodeAsync(string username, string emailCode, long emailCodeTimestamp)
        {
            UserRecord record = new UserRecord(username, emailCode: emailCode, emailCodeTimestamp: emailCodeTimestamp,
                                               emailCodeFailures: Constants.NoValueInt);

            return await UpdateUserAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously remove the email verification code for a specified user in the data store.
        /// </summary>
        /// <param name="username">Username that the email verification code is removed from.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public async Task<bool> RemoveEmailCodeAsync(string username)
        {
            // Everything related to the email code has no value.
            UserRecord record = new UserRecord(username, emailCode: Constants.NoValueString,
                                               emailCodeTimestamp: Constants.NoValueLong,
                                               emailCodeFailures: Constants.NoValueInt);

            return await UpdateUserAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously disable a user from login.
        /// </summary>
        /// <param name="username">Username of the user to disable.</param>
        /// <returns>Returns true if the user was originally enabled, false otherwise.</returns>
        public async Task<bool> DisableUserAsync(string username, string adminName = Constants.SystemIdentifier)
        {
            // Check that the user of function is an admin and throw an exception if they are not.
            if (!adminName.Equals(Constants.SystemIdentifier))
            {
                UserObject admin = await GetUserInfoAsync(adminName).ConfigureAwait(false);

                if (admin.UserType != Constants.AdminUserType)
                {
                    throw new ArgumentException(Constants.MustBeAdmin);
                }
            }

            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // If already disabled, just return false.
            if (user.Disabled == Constants.DisabledStatus)
            {
                return false;
            }

            UserRecord record = new UserRecord(username, disabled: Constants.DisabledStatus);

            await UpdateUserAsync(record).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously enable a disabled user to login.
        /// </summary>
        /// <param name="username">Username of a user to enable.</param>
        /// <returns>Returns true if the user was originally disabled, false otherwise.</returns>
        public async Task<bool> EnableUserAsync(string username)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // If already enabled, return false.
            if (user.Disabled == Constants.EnabledStatus)
            {
                return false;
            }

            // Enable the username.
            UserRecord record = new UserRecord(username, disabled: Constants.EnabledStatus);

            await UpdateUserAsync(record).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously change the password digest associated with a user in the data store.
        /// </summary>
        /// <param name="username">Username of the user to update.</param>
        /// <param name="digest">The digest of password.</param>
        /// <param name="saltString">Salt used to produce the digest.</param>
        /// <returns>Returns true if the operation is successful and false if it failed.</returns>
        public async Task<bool> ChangePasswordAsync(string username, string digest, string saltString)
        {
            UserRecord record = new UserRecord(username, password: digest, salt: saltString);

            return await UpdateUserAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously update the phone code values for the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to update (string)</param>
        /// <param name="numFailures">The number of failures to update to (int)</param>
        /// <returns>Task (bool) whether the function executed without failure</returns>
        public async Task<bool> UpdatePhoneCodeFailuresAsync(string username, int numFailures)
        {
            UserRecord record = new UserRecord(username, phoneCodeFailures: numFailures);

            await UpdateUserAsync(record).ConfigureAwait(false);

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
        public async Task<bool> IncrementLoginFailuresAsync(string username, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            UserRecord record;

            // Need to check if the maxtime + lastTime is less than now.
            // if it is then reset the failure
            long lastLoginFailTimestamp = user.LastLoginFailTimestamp;
            long maxSeconds = TimeUtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = TimeUtilityService.CurrentUnixTime();

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures. Don't reset if
            // they have no last login fail timestamp.
            if (lastLoginFailTimestamp + maxSeconds < currentUnix && lastLoginFailTimestamp != Constants.NoValueLong)
            {
                reset = true;
                record = new UserRecord(username, loginFailures: 0);

                await UpdateUserAsync(record).ConfigureAwait(false);
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

            await UpdateUserAsync(record).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Asynchronously increment the email code failure of a user.
        /// </summary>
        /// <param name="username">Username of the user to increment the email code failure.</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public async Task<bool> IncrementEmailCodeFailuresAsync(string username)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.EmailCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, emailCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            return await UpdateUserAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously increment the phone code failures of the user defined by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to increment the failures of (string)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public async Task<bool> IncrementPhoneCodeFailuresAsync(string username)
        {
            UserObject user = await GetUserInfoAsync(username).ConfigureAwait(false);

            // Get the current failure count.
            int currentFailures = user.PhoneCodeFailures;

            // Create user record to insert into update.
            UserRecord record = new UserRecord(username, phoneCodeFailures: currentFailures + 1);

            // Increment the failure count for that user.
            await UpdateUserAsync(record).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Increment the registration failures of the anonymous user defined by the <paramref name="ipAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address to increment the registration failures of (string)</param>
        /// <param name="maxTimeBeforeFailureReset">The time before their failures reset (TimeSpan)</param>
        /// <param name="maxNumberOfTries">The max number of registration tries before they get locked (int)</param>
        /// <returns>Task (bool) whether the funciton executed without exception</returns>
        public async Task<bool> IncrementRegistrationFailuresAsync(string ipAddress, TimeSpan maxTimeBeforeFailureReset, int maxNumberOfTries)
        {
            IPAddressObject ip = await GetIPAddressInfoAsync(ipAddress).ConfigureAwait(false);

            IPAddressRecord record;

            // Need to check if the maxtime + lastTime is less than now.
            // if it is then reset the failure
            long lastRegFailTimestamp = ip.LastRegFailTimestamp;
            long maxSeconds = TimeUtilityService.TimespanToSeconds(maxTimeBeforeFailureReset);
            long currentUnix = TimeUtilityService.CurrentUnixTime();

            bool reset = false;

            // If the time has passed their max time before reset, reset their failures. Don't reset
            // if they have no last registration fail timestamp.
            if (lastRegFailTimestamp + maxSeconds < currentUnix && lastRegFailTimestamp != Constants.NoValueLong)
            {
                reset = true;
                record = new IPAddressRecord(ipAddress, registrationFailures: 0);

                await UpdateIPAsync(record).ConfigureAwait(false);
            }

            // Increment the user's login Failure count.
            int updatedRegistrationFailures = reset ? 1 : ip.RegistrationFailures + 1;

            // Lock the ip if they have reached the max number of tries.
            // Update the last reg fail time.
            if (updatedRegistrationFailures >= maxNumberOfTries)
            {
                record = new IPAddressRecord(ipAddress, timestampLocked: currentUnix, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);

                // Asynchronously notify the system admin if an ip address was locked during registration.
                await SystemUtilityService.NotifySystemAdminAsync($"{ipAddress} was locked at {currentUnix}", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
            }
            else
            {
                record = new IPAddressRecord(ipAddress, registrationFailures: updatedRegistrationFailures, lastRegFailTimestamp: currentUnix);
            }

            return await UpdateIPAsync(record).ConfigureAwait(false);
        }

        public async Task<String> GetUserType(String username)
        {
            return await _userDAO.ReadUserType(username).ConfigureAwait(false);
        }
    }
}
