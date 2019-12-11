using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.UnitTestServices;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserManagementServiceTests
    { 
        [DataTestMethod]
        [DataRow("Jason")]
        public void UserManagementService_CheckUserExistence_UserExistsSuccess(string username)
        {
            // Arrange
            // Create user 

            bool result = UserManagementService.CheckUserExistence(username);
            Assert.IsTrue(result);

            // Cleanup: delete user
        }

        [DataTestMethod]
        [DataRow("blahblah")]
        [DataRow("123123321")]
        public void UserManagementService_CheckUserExistence_UserDoesNotExistsFailure(string username)
        {
            // check if user exists
            // delete it if it does 
            bool result = UserManagementService.CheckUserExistence(username);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("5622537232")]
        public void UserManagementService_CheckPhoneNumberExistence_PhoneNumberExistsSuccess(string phoneNumber)
        {
            // Arrange
            // Create users with phonenumber above
            bool result = UserManagementService.CheckPhoneNumberExistence(phoneNumber);
            Assert.IsTrue(result);

            // Cleanup 
            // Delete Created user
        }

        [DataTestMethod]
        [DataRow("0000000000")]
        public void UserManagementService_CheckPhoneNumberExistence_PhoneNumberDoesNotExistsFailure(string phoneNumber)
        {
            // Arrange
            // Check if phonenumber exists. if it does delete that user

            bool result = UserManagementService.CheckPhoneNumberExistence(phoneNumber);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("jasonExists@gmail.com")]
        public void UserManagementService_CheckEmailExistence_EmailExistsSuccess(string email)
        {
            // Arrange
            // Create user. 

            bool result = UserManagementService.CheckEmailExistence(email);
            Assert.IsTrue(result);

            // Cleanup 
            // Delete that user
        }

        [DataTestMethod]
        [DataRow("JasonDoesNotExists@gmail.com")]
        public void UserManagementService_CheckEmailExistence_EmailDoesNotExistsFailure(string email)
        {
            // Arrange
            // check if that email exist
            // if it does delete that user

            bool result = UserManagementService.CheckEmailExistence(email);
            Assert.IsFalse(result);
        }


        // TODO: insert actual diabled / enabled user into the database
        [DataTestMethod]
        [DataRow("BadUser")]
        public void UserManagementService_CheckIfUserDisabled_UserIsDisabledSuccess(string username)
        {
            // Arrange 
            // Create user
            // Disable user

            bool result = UserManagementService.CheckIfUserDisabled(username);
            Assert.IsTrue(result);

            // Cleanup 
            // delete that user
        }

        [DataTestMethod]
        [DataRow("GoodUser")]
        public void UserManagementService_CheckIfUserDisabled_UserIsNotDisabledSuccess(string username)
        {
            // Arrange 
            // Create user that is not diabled
            

            bool result = UserManagementService.CheckIfUserDisabled(username);
            Assert.IsFalse(result);

            // Cleanup
            // Delete user
        }


        // TODO: insert acutal ip results into the database
        [DataTestMethod]
        [DataRow("ipaddress")]
        public void UserManagementService_CheckIPLock_IpIsDisabledSuccess(string ipAddress, int hours, int minutes, int seconds)
        {
            // Arrange 
            // Insert Ip into ip table

            // Act
            bool result = UserManagementService.CheckIfIPLocked(ipAddress);
            
            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("ipaddress", 3, 0, 0)]
        public void UserManagementService_CheckIPLock_IpIsNotDisabledSuccess(string ipAddress, int hours, int minutes, int seconds)
        {
            // Arrange
            // Check if ip is in table 
            // if it is selete it from it 

            TimeSpan maxLockTime = new TimeSpan(hours, minutes, seconds);
            bool result = UserManagementService.CheckIfIPLocked(ipAddress);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public void UserManagementService_CreateUser_CreateNonExistentUserSuccess(bool isTemp, string username, string firstName, string lastName, string email,
                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            
            // Act 
            // Create the user 
            bool result = UserManagementService.CreateUser(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt);

            // Assert that user creation was successful 
            Assert.IsTrue(result);

            // Read that user. and assert that it has all the correct columns 


            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public void UserManagementService_DeleteUser_DeleteUserSuccess(bool isTemp, string username, string firstName, string lastName, string email,
                                                string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange 
            // Create a user to be deleted
            UserManagementService.CreateUser(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt);

            // Act 
            // Delete the user 
            bool result = UserManagementService.DeleteUser(username);

            Assert.IsTrue(result);

            // Assert that the user is properly deleted from the table with CheckUserExistence


            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123")]
        public void UserManagementService_MakeTempPerm_ChangeTempToPermSuccess(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange 
            // Create a temporary user to be deleted
            UserManagementService.CreateUser(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt);

            // Act 
            // Make the temporary user perm
            bool result = UserManagementService.MakeTempPerm(username);

            // Assert
            Assert.IsTrue(result);

            // Assert that the data you read for that user does in fact have that field changed.

            // Cleanup 
            // Delete user
        }

        // TODO: update the email code timestamp
        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", "0", "Customer", "123123123", "1233", 123123123123)]
        public void UserManagementService_StoreEmailCode_StoreEmailCodeForUserSuccess(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange 
            // Create a temporary user to be deleted
            UserManagementService.CreateUser(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt);

            // Act 
            bool result = UserManagementService.StoreEmailCode(username, emailCode, emailCodeTimestamp);

            Assert.IsTrue(result);

            // Read the email code and check if it did infact change.

            // Cleanup 
            // Delete user
        }

        // TODO: update the email code timestamp
        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP", "TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "123123123", "1233", "yyyy-mm-dd")]
        public void UserManagementService_RemoveEmailCode_StoreEmailCodeForUserSuccess(bool isTemp, string username, string firstName, string lastName, string email,
                                         string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, string emailCodeTimestamp)
        {
            // Arrange 
            // Create a temporary user to be deleted
            UserManagementService.CreateUser(isTemp, username, firstName, lastName, email, phoneNumber, password, disabled, userType, salt);

            // Act 
            bool result = UserManagementService.RemoveEmailCode(username);

            Assert.IsTrue(result);
            
            // Read that user and check if infact that email code is removed.
            
            // Cleanup 
            // Delete user
        }

        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementService_DisableUserName_DisableExistingUserSuccess(string username)
        {
            // Arrange 
            // Create the user that will be disabled

            // Act 
            bool result = UserManagementService.DisableUser(username);
            Assert.IsFalse(result);

            // Read that user and check if it was disabled

            // Delete the created user 
        }


        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementService_DisableUserName_DisableNonExistingUserFailure(string username)
        {
            // Act 
            bool result = UserManagementService.DisableUser(username);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementService_DisableUserName_DisableADisabledUserFailure(string username)
        {
            // Arrange 
            // Create a user that is Disabled

            // Act 
            // Make the temporary user perm
            bool result = UserManagementService.DisableUser(username);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementService_EnableUserName_EnableADisabledUserSuccess(string username)
        {
            // Arrange 
            // Create the user that will be disabled

   
            bool result = UserManagementService.DisableUser(username);
            Assert.IsTrue(result);

            // Act 
            bool enableResult = UserManagementService.EnableUser(username);

            Assert.IsTrue(enableResult);
            // Cleanup 
            // Delete that created user
        }

        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementService_EnableUserName_EnableAEnabledUserFailure(string username)
        {
            // Arrange 
            // Create a user that is enabled

            // Act 
            // Make the temporary user perm
            bool result = UserManagementService.EnableUser(username);
            Assert.IsFalse(result);
        }

        // For ChangePassword we check user existence and if user is daibled. 
        // is this the responsibility of the Manager?
        public void UserManagementService_ChangePassword_ChangePasswordOfExistingUserSuccess(string username, string password)
        {
            // Arrange 
            // Create the user.

            // Act 
            // Change the user password 
            // TODO: finish this
            //UserManagementService.ChangePassword(username, password);

            // Assert 
            // Read the user and make sure his password now matches the change
        }

        public void UserManagementService_NotifySystemAdmin_SendEmailToSystemAdminSuccess(string body)
        {

            // Act
            // Send message to system admin 
            bool result = UserManagementService.NotifySystemAdmin(body, Constants.SystemAdminEmailAddress);

            // Assert
            // TODO: how do we ensure that the system admin got the email?
            Assert.IsTrue(result);
        }
    }
}
