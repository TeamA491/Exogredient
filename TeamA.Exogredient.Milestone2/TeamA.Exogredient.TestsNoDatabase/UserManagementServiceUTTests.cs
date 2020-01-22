using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.UnitTestServices;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.TestsNoDatabase
{
    [TestClass]
    public class UserManagementServiceUTTests
    {
        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "23123123")]
        public void UserManagementServiceUT_CheckUserExistence_UserExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                 string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user 
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, isDisabled, userType, salt);
            Assert.IsTrue(createResult);


            // Assert: Check that an existing user returns true.
            bool result = UserManagementServiceUT.CheckUserExistence(username);
            Assert.IsTrue(result);

            // Cleanup: delete the created user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("blahblah")]
        [DataRow("123123321")]
        public void UserManagementServiceUT_CheckUserExistence_UserDoesNotExistsFailure(string username)
        {
            // Assert: Check that an non existing user returns false.
            bool result = UserManagementServiceUT.CheckUserExistence(username);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public void UserManagementServiceUT_CheckPhoneNumberExistence_PhoneNumberExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                               string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create users with the phonenumber inputted. 
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Assert: check that an existing phonenumber returns true.
            bool result = UserManagementServiceUT.CheckPhoneNumberExistence(phoneNumber);
            Assert.IsTrue(result);

            // Cleanup: Delete Created user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("0000000000")]
        public void UserManagementServiceUT_CheckPhoneNumberExistence_PhoneNumberDoesNotExistsFailure(string phoneNumber)
        {
            // Act: Check that a nonexistent phonenumber returns false. 
            bool result = UserManagementServiceUT.CheckPhoneNumberExistence(phoneNumber);
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public void UserManagementServiceUT_CheckEmailExistence_EmailExistsSuccess(bool isTemp, string username, string name, string email,
                                                                                   string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user. 
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: check that an existing email returns true.
            bool result = UserManagementServiceUT.CheckEmailExistence(email);
            Assert.IsTrue(result);

            // Cleanup: Delete that user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("JasonDoesNotExists@gmail.com")]
        public void UserManagementServiceUT_CheckEmailExistence_EmailDoesNotExistsFailure(string email)
        {
            // Act: check that a non existing email returns false.
            bool result = UserManagementServiceUT.CheckEmailExistence(email);
            Assert.IsFalse(result);
        }


        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "12345678")]
        public void UserManagementServiceUT_CheckIfUserDisabled_UserIsDisabledSuccess(bool isTemp, string username, string name, string email,
                                                                                      string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create a disabled user.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Check that the disabled is disabled.
            bool result = UserManagementServiceUT.CheckIfUserDisabled(username);
            Assert.IsTrue(result);

            // Cleanup: Delete that user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public void UserManagementServiceUT_CheckIfUserDisabled_UserIsNotDisabledSuccess(bool isTemp, string username, string name, string email,
                                                                                         string phoneNumber, string password, int isDisabled, string userType, string salt)
        {
            // Arrange: Create user that is not diabled
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, isDisabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Check that the non disabled returns false.
            bool result = UserManagementServiceUT.CheckIfUserDisabled(username);
            Assert.IsFalse(result);

            // Cleanup: Delete that user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("127.0.0.1")]
        public void UserManagementServiceUT_CheckIfIPLocked_IpIsDisabledSuccess(string ipAddress)
        {
            // Arrange: Insert Ip into ip table.
            bool lockResult = UserManagementServiceUT.CreateIP(ipAddress);
            Assert.IsTrue(lockResult);

            // Arrange: Attempt to faile to register 3 times. 
            // IPs: Are only locked this way.
            for (int i = 0; i < 3; i++)
            {
                bool registerResult = UserManagementServiceUT.IncrementRegistrationFailures(ipAddress, Constants.RegistrationTriesResetTime, Constants.MaxRegistrationAttempts);
                Assert.IsTrue(registerResult);
            }


            // Act
            bool result = UserManagementServiceUT.CheckIfIPLocked(ipAddress);

            // Assert: Check that an IP that is inserted returns true.
            Assert.IsTrue(result);

        }

        [DataTestMethod]
        [DataRow("127.0.0.9")]
        public void UserManagementServiceUT_CheckIPLock_IpIsNotDisabledSuccess(string ipAddress)
        {
            // Act:  Check that an non existent ip returns ArgumentExcpetions because ip does not exists.
            bool result;
            try
            {
                UserManagementServiceUT.CheckIfIPLocked(ipAddress);
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
        public void UserManagementServiceUT_CreateUser_CreateNonExistentUserSuccess(bool isTemp, string username, string name, string email,
                                                                                    string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Act: Create the user.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);

            // Assert: that user creation was successful 
            Assert.IsTrue(createResult);

            // Read that user and assert that it has all the correct columns 
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            if (user.TempTimestamp == 0 && user.Username == username && user.Name == name && user.Email == email &&
                user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == disabled && user.UserType == userType && user.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(true, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "87654321")]
        public void UserManagementServiceUT_DeleteUser_DeleteUserSuccess(bool isTemp, string username, string name, string email,
                                                                         string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user to be deleted
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Delete the user 
            bool result = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(result);

            // Assert: that the user is properly deleted from the table with CheckUserExistence
            bool existResult = UserManagementServiceUT.CheckUserExistence(username);
            Assert.IsFalse(existResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "87654321")]
        public void UserManagementServiceUT_MakeTempPerm_ChangeTempToPermSuccess(bool isTemp, string username, string name, string email,
                                                                                 string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a temporary user to be deleted.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Make the temporary user perm
            bool result = UserManagementServiceUT.MakeTempPerm(username);
            Assert.IsTrue(result);

            // Assert: that the user is infact permanent.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);

            bool readResult;
            // TempTimestamp == 0 when the user is permanent.
            if (user.TempTimestamp == 0 && user.Username == username && user.Name == name && user.Email == email &&
                user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == disabled && user.UserType == userType && user.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678", "1233", 123123123123)]
        public void UserManagementServiceUT_StoreEmailCode_StoreEmailCodeForUserSuccess(bool isTemp, string username, string name, string email,
                                                                                        string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange: Create a temporary user to be deleted
            UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);

            // Act: Store an email code for a user.
            bool result = UserManagementServiceUT.StoreEmailCode(username, emailCode, emailCodeTimestamp);
            Assert.IsTrue(result);

            // Assert: Read the email code and check if it did infact change.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            if (user.TempTimestamp == 0 && user.Username == username && user.Name == name && user.Email == email &&
                 user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == disabled && user.UserType == userType && user.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678", "1233", 10000000)]
        public void UserManagementServiceUT_RemoveEmailCode_RemoveEmailCodeForUserSuccess(bool isTemp, string username, string name, string email,
                                                                                          string phoneNumber, string password, int disabled, string userType, string salt, string emailCode, long emailCodeTimestamp)
        {
            // Arrange: Create a user.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Arrange: Store an email code for that user.
            bool storeResult = UserManagementServiceUT.StoreEmailCode(username, emailCode, emailCodeTimestamp);
            Assert.IsTrue(storeResult);

            // Act: Remove the email code for that user.
            bool result = UserManagementServiceUT.RemoveEmailCode(username);
            Assert.IsTrue(result);

            // Read that user and check if infact that email code is removed.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            if (user.TempTimestamp == 0 && user.Username == username && user.Name == name && user.Email == email &&
                 user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == disabled && user.UserType == userType && user.Salt == salt &&
                 user.EmailCode == "" && user.EmailCodeTimestamp == 0 && user.EmailCodeFailures == 0)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", Constants.EnabledStatus, "Customer", "12345678")]
        public void UserManagementServiceUT_DisableUserName_DisableExistingUserSuccess(bool isTemp, string username, string name, string email,
                                                                                       string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create the user that is not disabled
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: disable that user 
            bool result = UserManagementServiceUT.DisableUser(username);
            Assert.IsTrue(result);

            // Assert: Check that the user is disabled.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            // User is disabled if user.Disabled == Constants.DisabledStatus
            if (user.TempTimestamp == Constants.NoValueLong && user.Username == username && user.Name == name && user.Email == email &&
                 user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == Constants.DisabledStatus && user.UserType == userType && user.Salt == salt &&
                 user.EmailCode == Constants.NoValueString && user.EmailCodeTimestamp == Constants.NoValueLong && user.EmailCodeFailures == Constants.NoValueInt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Delete the created user 
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }


        [DataTestMethod]
        [DataRow("username")]
        public void UserManagementServiceUT_DisableUserName_DisableNonExistingUserFailure(string username)
        {
            // Act: disabling a non existent user should throw an ArgumentException because the user doesn't exists.
            bool result;
            try
            {
                UserManagementServiceUT.DisableUser(username);
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
        public void UserManagementServiceUT_DisableUser_DisableADisabledUserFailure(bool isTemp, string username, string name, string email,
                                                                                    string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user that is Disabled
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Disabling an already disabled user should return false.
            bool result = UserManagementServiceUT.DisableUser(username);
            Assert.IsFalse(result);

            // Cleanup: Delete the user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 1, "Customer", "12345678")]
        public void UserManagementServiceUT_EnableUser_EnableADisabledUserSuccess(bool isTemp, string username, string name, string email,
                                                                                  string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a disabled user.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Perform an enable operation on a disabled user.
            bool enableResult = UserManagementServiceUT.EnableUser(username);
            Assert.IsTrue(enableResult);

            // Assert: Check that the user is enabled.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            // User is disabled if user.Disabled == Constants.DisabledStatus
            if (user.TempTimestamp == Constants.NoValueLong && user.Username == username && user.Name == name && user.Email == email &&
                 user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == Constants.EnabledStatus && user.UserType == userType && user.Salt == salt &&
                 user.EmailCode == Constants.NoValueString && user.EmailCodeTimestamp == Constants.NoValueLong && user.EmailCodeFailures == Constants.NoValueInt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that created user
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public void UserManagementServiceUT_EnableUser_EnableAEnabledUserFailure(bool isTemp, string username, string name, string email,
                                                                                 string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create a user that is enabled
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Enable and already enabled user should return false.
            bool result = UserManagementServiceUT.EnableUser(username);
            Assert.IsFalse(result);

            // Cleanup: Delete that user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", 0, "Customer", "12345678")]
        public void UserManagementServiceUT_ChangePassword_ChangePasswordOfExistingUserSuccess(bool isTemp, string username, string name, string email,
                                                                                               string phoneNumber, string password, int disabled, string userType, string salt)
        {
            // Arrange: Create the user.
            bool createResult = UserManagementServiceUT.CreateUser(isTemp, username, name, email, phoneNumber, password, disabled, userType, salt);
            Assert.IsTrue(createResult);

            // Act: Change the user password digest.
            bool passwordResult = UserManagementServiceUT.ChangePassword(username, password, salt);
            Assert.IsTrue(passwordResult);

            // Assert: Read the user and make sure his password now matches the change.
            UserObject user = UserManagementServiceUT.GetUserInfo(username);
            bool readResult;
            if (user.TempTimestamp == 0 && user.Username == username && user.Name == name && user.Email == email &&
                 user.PhoneNumber == phoneNumber && user.Password == password && user.Disabled == disabled && user.UserType == userType && user.Salt == salt)
            {
                readResult = true;
            }
            else
            {
                readResult = false;
            }
            Assert.IsTrue(readResult);

            // Cleanup: Delete that user.
            bool deleteResult = UserManagementServiceUT.DeleteUser(username);
            Assert.IsTrue(deleteResult);
        }

        public void UserManagementServiceUT_NotifySystemAdmin_SendEmailToSystemAdminSuccess(string body)
        {
            // Act: Check that a successfull message to system admin returns true.
            bool notifyResult = UserManagementServiceUT.NotifySystemAdmin(body, Constants.SystemAdminEmailAddress);
            Assert.IsTrue(notifyResult);
        }
    }
}
