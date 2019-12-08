using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserManagementServiceTests
    { 
        [DataTestMethod]
        [DataRow("Jason")]
        public async Task UserManagementService_CheckUserExistenceAsync_UserExistsSuccessAsync(string username)
        {
            // Arrange: Create user 
            bool createResult = await UserManagementService.CreateUserAsync(false, username, "Jason", "nguuuu", "emailf@gmail.com", "5629871234", "password", 0, 
                                                                                "Customer", "123123123");
            Assert.IsTrue(createResult);

            // Assert: Check that an existing user returns true.
            bool result = await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: delete the created user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("blahblah")]
        [DataRow("123123321")]
        public async Task UserManagementService_CheckUserExistenceAsync_UserDoesNotExistsFailureAsync(string username)
        {
            // Assert: Check that an non existing user returns false.
            bool result = await UserManagementService.CheckUserExistenceAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public async Task UserManagementService_CheckPhoneNumberExistenceAsync_PhoneNumberExistsSuccessAsync(bool isTemp, string username, string firstname, string lastname, string email,
                                                                                                             string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create users with the phonenumber inputted. 
            bool createResult = await UserManagementService.CreateUserAsync(isTemp, username, firstname, lastname, email, phoneNumber, password,
                                                                                isDisabled, userType, salt).ConfigureAwait(false);
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
        public async Task UserManagementService_CheckPhoneNumberExistenceAsync_PhoneNumberDoesNotExistsFailureAsync(string phoneNumber)
        {
            // Act: Check that a nonexistent phonenumber returns false. 
            bool result = await UserManagementService.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public async Task UserManagementService_CheckEmailExistenceAsync_EmailExistsSuccessAsync(bool isTemp, string username, string firstname, string lastname, string email,
                                                                                                             string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user. 
            bool createResult = await UserManagementService.CreateUserAsync(isTemp, username, firstname, lastname, email, phoneNumber, password,
                                                                    isDisabled, userType, salt).ConfigureAwait(false);
            Assert.IsTrue(createResult);


            // Act: check that an existing email returns true.
            bool result = await UserManagementService.CheckEmailExistenceAsync(email).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: Delete that user
            bool deleteResult = await UserManagementService.DeleteUserAsync(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("JasonDoesNotExists@gmail.com")]
        public async Task UserManagementService_CheckEmailExistenceAsync_EmailDoesNotExistsFailureAsync(string email)
        {
            // Act: check that a non existing email returns false.
            bool result = await UserManagementService.CheckEmailExistenceAsync(email).ConfigureAwait(false);
            Assert.IsFalse(result);
        }


        // TODO: insert actual diabled / enabled user into the database
        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "123123123")]
        public async Task UserManagementService_CheckIfUserDisabledAsync_UserIsDisabledSuccessAsync(bool isTemp, string username, string firstname, string lastname, string email,
                                                                                                             string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create a disabled user.
            bool createResult = await UserManagementService.CreateUserAsync(isTemp, username, firstname, lastname, email, phoneNumber, password,
                                                        isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Check that the disabled is disabled.
            bool result = await UserManagementService.CheckIfUserDisabledAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public async Task UserManagementService_CheckIfUserDisabledAsync_UserIsNotDisabledSuccessAsync(bool isTemp, string username, string firstname, string lastname, string email,
                                                                                                             string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user that is not diabled
            bool createResult = await UserManagementService.CreateUserAsync(isTemp, username, firstname, lastname, email, phoneNumber, password,
                                            isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Check that the non disabled returns false.
            bool result = await UserManagementService.CheckIfUserDisabledAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Cleanup: Delete that user.
            bool deleteResult = await UserManagementService.DeleteUserAsync(username);
            Assert.IsTrue(deleteResult);
        }


        // TODO: insert acutal ip results into the database
        [DataTestMethod]
        [DataRow("ipaddress", 3, 0, 0)]
        public async Task UserManagementService_CheckIPLockAsync_IpIsDisabledSuccessAsync(string ipAddress, int hours, int minutes, int seconds)
        {
            // Arrange: Insert Ip into ip table
            bool lockResult = await UserManagementService.LockIPAsync(ipAddress);
            Assert.IsTrue(lockResult);

            // Set how the Ip is locked
            TimeSpan maxLockTime = new TimeSpan(hours, minutes, seconds);

            // Act: Check that a locked IP returns true.
            bool result = await UserManagementService.CheckIPLockAsync(ipAddress, maxLockTime).ConfigureAwait(false);
            
            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("ipaddress", 3, 0, 0)]
        public async Task UserManagementService_CheckIPLockAsync_IpIsNotDisabledSuccessAsync(string username, int hours, int minutes, int seconds)
        {
            // Arrange:  Check if ip is in table 
            TimeSpan maxLockTime = new TimeSpan(hours, minutes, seconds);

            // Act: Check that a non existing IP returns false.
            bool result = await UserManagementService.CheckIPLockAsync(username, maxLockTime).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public async Task UserManagementService_CreateUserAsync_CreateNonExistentUserSuccessAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Act: Create the user 
            bool result = await UserManagementService.CreateUserAsync(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt).ConfigureAwait(false);

            // Assert: that user creation was successful 
            Assert.IsTrue(result);

            // Read that user. and assert that it has all the correct columns 
            bool readResult = await UserManagementService.

            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public async Task UserManagementService_DeleteUserAsync_DeleteUserSuccessAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                                string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange 
            // Create a user to be deleted
            await UserManagementService.CreateUserAsync(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt).ConfigureAwait(false);

            // Act 
            // Delete the user 
            bool result = await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);

            Assert.IsTrue(result);

            // Assert that the user is properly deleted from the table with CheckUserExistence


            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", "0", "Customer", "123123123")]
        public async Task UserManagementService_MakeTempPerm_ChangeTempToPermSuccessAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange 
            // Create a temporary user to be deleted
            await UserManagementService.CreateUserAsync(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt).ConfigureAwait(false);

            // Act 
            // Make the temporary user perm
            bool result = await UserManagementService.MakeTempPermAsync(username).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);

            // Assert that the data you read for that user does in fact have that field changed.

            // Cleanup 
            // Delete user
        }

        // TODO: update the email code timestamp
        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", "0", "Customer", "123123123", "1233", 123123123123)]
        public async Task UserManagementService_StoreEmailCode_StoreEmailCodeForUserSuccessAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange 
            // Create a temporary user to be deleted
            await UserManagementService.CreateUserAsync(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt).ConfigureAwait(false);

            // Act 
            bool result = await UserManagementService.StoreEmailCodeAsync(username, emailCode, emailCodeTimestamp).ConfigureAwait(false);

            Assert.IsTrue(result);

            // Read the email code and check if it did infact change.

            // Cleanup 
            // Delete user
        }

        // TODO: update the email code timestamp
        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123", "1233", "yyyy-mm-dd")]
        public async Task UserManagementService_RemoveEmailCode_StoreEmailCodeForUserSuccessAsync(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, string emailCodeTimestamp)
        {
            // Arrange 
            // Create a temporary user to be deleted
            await UserManagementService.CreateUserAsync(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt).ConfigureAwait(false);

            // Act 
            bool result = await UserManagementService.RemoveEmailCodeAsync(username).ConfigureAwait(false);

            Assert.IsTrue(result);
            
            // Read that user and check if infact that email code is removed.
            
            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_DisableUserNameAsync_DisableExistingUserSuccessAsync(string username)
        {
            // Arrange 
            // Create the user that will be disabled

            // Act 
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Read that user and check if it was disabled

            // Delete the created user 
        }


        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_DisableUserNameAsync_DisableNonExistingUserFailureAsync(string username)
        {
            // Act 
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_DisableUserNameAsync_DisableADisabledUserFailureAsync(string username)
        {
            // Arrange 
            // Create a user that is Disabled

            // Act 
            // Make the temporary user perm
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_EnableUserNameAsync_EnableADisabledUserSuccessAsync(string username)
        {
            // Arrange 
            // Create the user that will be disabled

   
            bool result = await UserManagementService.DisableUserAsync(username).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Act 
            bool enableResult = await UserManagementService.EnableUserAsync(username).ConfigureAwait(false);

            Assert.IsTrue(enableResult);
            // Cleanup 
            // Delete that created user
        }

        [DataTestMethod]
        [DataRow("username")]
        public async Task UserManagementService_EnableUserNameAsync_EnableAEnabledUserFailureAsync(string username)
        {
            // Arrange 
            // Create a user that is enabled

            // Act 
            // Make the temporary user perm
            bool result = await UserManagementService.EnableUserAsync(username).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        // For ChangePasswordAsync we check user existence and if user is daibled. 
        // is this the responsibility of the Manager?
        public async Task UserManagementService_ChangePasswordAsync_ChangePasswordOfExistingUserSuccessAsync(string username, string password)
        {
            // Arrange 
            // Create the user.

            // Act 
            // Change the user password 
            // TODO: finish this
            //await UserManagementService.ChangePasswordAsync(username, password).ConfigureAwait(false);

            // Assert 
            // Read the user and make sure his password now matches the change
        }

        public async Task UserManagementService_NotifySystemAdminAsync_SendEmailToSystemAdminSuccessAsync(string body)
        {

            // Act
            // Send message to system admin 
            bool result = await UserManagementService.NotifySystemAdminAsync(body).ConfigureAwait(false);

            // Assert
            // TODO: how do we ensure that the system admin got the email?
            Assert.IsTrue(result);
        }
    }
}
