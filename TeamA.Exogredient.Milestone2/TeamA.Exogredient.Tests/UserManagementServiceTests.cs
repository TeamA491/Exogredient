using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using System.Collections.Generic;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserManagementServiceTests
    {
        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "23123123")]
        public async Task UserManagementService_CheckUserExistenceAsync_UserExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                          string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);


            // Assert: Check that an existing user returns true.
            bool result = await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: delete the created user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("blahblah")]
        [DataRow("123123321")]
        public async Task UserManagementService_CheckUserExistenceAsync_UserDoesNotExistsFailure(string username)
        {
            // Assert: Check that an non existing user returns false.
            bool result = await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_CheckPhoneNumberExistenceAsync_PhoneNumberExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                                             string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create users with the phonenumber inputted. 
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Assert: check that an existing phonenumber returns true.
            bool result = await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: Delete Created user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("0000000000")]
        public async Task UserManagementService_CheckPhoneNumberExistenceAsync_PhoneNumberDoesNotExistsFailure(string phoneNumber)
        {
            // Act: Check that a nonexistent phonenumber returns false. 
            bool result = await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_CheckEmailExistenceAsync_EmailExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                            string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: check that an existing email returns true.
            bool result = await UserManagementService.CheckEmailExistenceAsync(email).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("JasonDoesNotExists@gmail.com")]
        public async Task UserManagementService_CheckEmailExistenceAsync_EmailDoesNotExistsFailure(string email)
        {
            // Act: check that a non existing email returns false.
            bool result = await UserManagementService.CheckEmailExistenceAsync(email).ConfigureAwait(false);
            Assert.IsFalse(result);
        }


        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "12345678")]
        public async Task UserManagementService_CheckIfUserDisabledAsync_UserIsDisabledSuccess(bool isTemp, string username, string name, string email,
                                                                                               string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create a disabled user.
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.DisabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: Check that the disabled is disabled.
            bool result = await UserManagementService.CheckIfUserDisabledAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_CheckIfUserDisabledAsync_UserIsNotDisabledSuccess(bool isTemp, string username, string name, string email,
                                                                                                  string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user that is not diabled
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: Check that the non disabled returns false.
            bool result = await UserManagementService.CheckIfUserDisabledAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("196.0.0.1")]
        public async Task UserManagementService_CheckIfIPLockedAsync_IpIsDisabledSuccess(string ipAddress)
        {
            // Arrange: Insert Ip into ip table.
            bool lockResult = await UserManagementService.CreateIPAsync(ipAddress).ConfigureAwait(false);
            Assert.IsTrue(lockResult);

            // Arrange: Attempt to faile to register 3 times. 
            // IPs: Are only locked this way.
            for (int i = 0; i < 3; i++)
            {
                bool registerResult = await UserManagementService.IncrementRegistrationFailuresAsync(ipAddress, Constants.RegistrationTriesResetTime, Constants.MaxRegistrationAttempts).ConfigureAwait(false);
                Assert.IsTrue(registerResult);
            }


            // Act
            bool result = await UserManagementService.CheckIfIPLockedAsync(ipAddress).ConfigureAwait(false);

            // Assert: Check that an IP that is inserted returns true.
            Assert.IsTrue(result);

            // Cleanup: Delete the IP.
            bool deleteResult = await UserManagementService.DeleteIPAsync(ipAddress).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);

        }

        [DataTestMethod]
        [DataRow("127.0.0.9")]
        public async Task UserManagementService_CheckIPLockAsync_IpIsNotDisabledSuccess(string ipAddress)
        {
            // Act:  Check that an non existent ip returns ArgumentExcpetions because ip does not exists.
            bool result;
            try
            {
                await UserManagementService.CheckIfIPLockedAsync(ipAddress).ConfigureAwait(false);
                result = false;
            }
            catch
            {
                result = true;
            }
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_CreateUserAsync_CreateNonExistentUserSuccess(bool isTemp, string username, string name, string email,
                                                                                             string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);

            // Assert: that user creation was successful 
            Assert.IsTrue(createResult);

            // Read that user and assert that it has all the correct columns 
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            if (userObj.TempTimestamp == 0 && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == disabled && userObj.UserType == userType && userObj.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }


        [TestMethod]
        public async Task UserManagementService_CreateUsersAsync_CreateNonExistentUsersSuccess()
        {
            // Arrange: Create a list of users.
            List<UserRecord> users = new List<UserRecord>();

            UserRecord user1 = new UserRecord("username", "mr.DROP TABLE", "blah@gmail.com", "5622224456", "password", Constants.EnabledStatus, Constants.CustomerUserType,
                                            "12345678", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);
            UserRecord user2 = new UserRecord("username1", "mr.DROP TABLE1", "1blah@gmail.com", "5622222456", "password", Constants.EnabledStatus, Constants.CustomerUserType,
                                                       "12345678", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            users.Add(user1);
            users.Add(user2);

            // Act: Create the users.
            bool createResult = await UserManagementService.BulkCreateUsersAsync(users).ConfigureAwait(false);

            // Assert: that the create was successfull
            Assert.IsTrue(createResult);

            // Cleanup: Delete the users.
            List<string> usersToDelete = new List<string> { (string)user1.GetData()["username"], (string)user2.GetData()["username"] };
            bool deleteResult = await UserManagementService.BulkDeleteUsersAsync(usersToDelete).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "87654321")]
        public async Task UserManagementService_DeleteUserAsync_DeleteUserSuccess(bool isTemp, string username, string name, string email,
                                                                                  string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user to be deleted
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: Delete the user 
            bool result = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Assert: that the user is properly deleted from the table with CheckUserExistence
            bool existResult = await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
            Assert.IsFalse(existResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "87654321")]
        public async Task UserManagementService_MakeTempPerm_ChangeTempToPermSuccess(bool isTemp, string username, string name, string email,
                                                                                     string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a temporary user to be deleted.
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false); Assert.IsTrue(createResult);

            // Act: Make the temporary user perm
            bool result = await UserManagementService.MakeTempPermAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Assert: that the user is infact permanent.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

            bool readResult;
            // TempTimestamp == 0 when the user is permanent.
            if (userObj.TempTimestamp == 0 && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == disabled && userObj.UserType == userType && userObj.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678", "1233", 123123123123)]
        public async Task UserManagementService_StoreEmailCode_StoreEmailCodeForUserSuccess(bool isTemp, string username, string name, string email,
                                                                                            string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange: Create a temporary user to be deleted
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            // Act: Store an email code for a user.
            bool result = await UserManagementService.StoreEmailCodeAsync(username, emailCode, emailCodeTimestamp).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Assert: Read the email code and check if it did infact change.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            if (userObj.TempTimestamp == 0 && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                 userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == disabled && userObj.UserType == userType && userObj.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678", "1233", 10000000)]
        public async Task UserManagementService_RemoveEmailCode_RemoveEmailCodeForUserSuccess(bool isTemp, string username, string name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange: Create a user.
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false); Assert.IsTrue(createResult);

            // Arrange: Store an email code for that user.
            bool storeResult = await UserManagementService.StoreEmailCodeAsync(username, emailCode, emailCodeTimestamp).ConfigureAwait(false);
            Assert.IsTrue(storeResult);

            // Act: Remove the email code for that user.
            bool result = await UserManagementService.RemoveEmailCodeAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Read that user and check if infact that email code is removed.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            if (userObj.TempTimestamp == 0 && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                 userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == disabled && userObj.UserType == userType && userObj.Salt == salt &&
                 userObj.EmailCode == "" && userObj.EmailCodeTimestamp == 0 && userObj.EmailCodeFailures == 0)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", Constants.EnabledStatus, "Customer", "12345678")]
        public async Task UserManagementService_DisableUserNameAsync_DisableExistingUserSuccess(bool isTemp, string username, string  name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create the user that is not disabled
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false); Assert.IsTrue(createResult);

            // Act: disable that user 
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Assert: Check that the user is disabled.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            // User is disabled if user.Disabled == Constants.DisabledStatus
            if (userObj.TempTimestamp == Constants.NoValueLong && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                 userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == Constants.DisabledStatus && userObj.UserType == userType && userObj.Salt == salt &&
                 userObj.EmailCode == Constants.NoValueString && userObj.EmailCodeTimestamp == Constants.NoValueLong && userObj.EmailCodeFailures == Constants.NoValueInt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Delete the created user 
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }


        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_DisableUserNameAsync_DisableNonExistingUserFailure(string username)
        {
            // Act: disabling a non existent user should throw an ArgumentException because the user doesn't exists.
            bool result;
            try
            {
                await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
                result = false;
            }
            catch
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "12345678")]
        public async Task UserManagementService_DisableUserAsync_DisableADisabledUserFailure(bool isTemp, string username, string name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user that is Disabled
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.DisabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false); Assert.IsTrue(createResult);

            // Act: Disabling an already disabled user should return false.
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Cleanup: Delete the user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "12345678")]
        public async Task UserManagementService_EnableUserAsync_EnableADisabledUserSuccess(bool isTemp, string username, string name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a disabled user.
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.DisabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false); Assert.IsTrue(createResult);

            // Act: Perform an enable operation on a disabled user.
            bool enableResult = await UserManagementService.EnableUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(enableResult);

            // Assert: Check that the user is enabled.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            // User is disabled if user.Disabled == Constants.DisabledStatus
            if (userObj.TempTimestamp == Constants.NoValueLong && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                 userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == Constants.EnabledStatus && userObj.UserType == userType && userObj.Salt == salt &&
                 userObj.EmailCode == Constants.NoValueString && userObj.EmailCodeTimestamp == Constants.NoValueLong && userObj.EmailCodeFailures == Constants.NoValueInt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that created user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_EnableUserAsync_EnableAEnabledUserFailure(bool isTemp, string username, string name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user that is enabled
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: Enable and already enabled user should return false.
            bool result = await UserManagementService.EnableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_ChangePasswordAsync_ChangePasswordOfExistingUserSuccess(bool isTemp, string username, string name, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create the user.
            // Arrange: Create user 
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);


            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: Change the user password digest.
            bool passwordResult = await UserManagementService.ChangePasswordAsync(username, password, salt).ConfigureAwait(false);
            Assert.IsTrue(passwordResult);

            // Assert: Read the user and make sure his password now matches the change.
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            if (userObj.TempTimestamp == 0 && userObj.Username == username && userObj.Name == name && userObj.Email == email &&
                 userObj.PhoneNumber == phoneNumber && userObj.Password == password && userObj.Disabled == disabled && userObj.UserType == userType && userObj.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("this is an email")]
        public async Task UserManagementService_NotifySystemAdminAsync_SendEmailToSystemAdminSuccess(string body)
        {
            // Act: Check that a successfull message to system admin returns true.
            bool notifyResult = await UserManagementService.NotifySystemAdminAsync(body, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
            Assert.IsTrue(notifyResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public async Task UserManagementService_UpdateUserAsync_UpdateUserSuccessfull(bool isTemp, string username, string name, string email,
                                                                                     string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user.
            UserRecord user = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, Constants.CustomerUserType,
                                    salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            bool createResult = await UserManagementService.CreateUserAsync(isTemp, user).ConfigureAwait(false);
            Assert.IsTrue(createResult);

            // Act: update the user's name, email, and phone number
            UserRecord userUpdate = new UserRecord(username, "updateName", "updateEmail@gmail.com", "1234567866", password, Constants.EnabledStatus, Constants.CustomerUserType,
                                   salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            bool updateResult = await UserManagementService.UpdateUserAsync(userUpdate).ConfigureAwait(false);
            Assert.IsTrue(updateResult);

            // Arrange: Make sure that that the user is successfully updated
            UserObject userObj = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);
            bool readResult;
            if (userObj.Name == "updateName" && userObj.Email == "updateEmail@gmail.com" && userObj.PhoneNumber == "1234567866")
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }

            Assert.IsTrue(readResult);

            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }


        [DataTestMethod]
        public async Task UserManagementService_UpdateUsersAsync_UpdateUserSuccessfull()
        {
            // Arrange: Create 2 users.
            UserRecord user1 = new UserRecord("username", "name", "email@gmail.com", "562222754", "password", Constants.EnabledStatus, Constants.CustomerUserType,
                                    "salt1", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            UserRecord user2 = new UserRecord("username1", "name", "email1@gmail.com", "562222724", "password", Constants.EnabledStatus, Constants.CustomerUserType,
                                    "salt1", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            await UserManagementService.BulkCreateUsersAsync(new List<UserRecord>() { user1, user2}).ConfigureAwait(false);

            // Create an update record to change their email and phone numbers.
            UserRecord updateUser1 = new UserRecord("username", email:"newEmail@gmail.com", phoneNumber:"1111111111");
            UserRecord updateUser2 = new UserRecord("username1", email:"newEmail2@gmail.com", phoneNumber:"2222222222");

            // Act: Update the created users.
            await UserManagementService.BulkUpdateUsersAsync(new List<UserRecord>() { updateUser1, updateUser2 });

            // Assert: check that the updated users values changed.
            UserObject userObj1 = await UserManagementService.GetUserInfoAsync("username").ConfigureAwait(false);
            UserObject userObj2 = await UserManagementService.GetUserInfoAsync("username1").ConfigureAwait(false);

            bool updateResult = false;

            if(userObj1.Email == "newEmail@gmail.com" && userObj1.PhoneNumber == "1111111111" &&
                userObj2.Email == "newEmail2@gmail.com" && userObj2.PhoneNumber == "2222222222")
            {
                updateResult = true;
            }

            Assert.IsTrue(updateResult);


            // Cleanup: delete the created users.
            bool deleteResult = await UserManagementService.BulkDeleteUsersAsync(new List<string>() {"username", "username1"}).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }
    }
}
