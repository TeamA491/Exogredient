using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserDAOTests
    {
        UserDAO userDAO = new UserDAO();

        private bool DataEquals(UserRecord userRecord, UserObject userObject)
        {
            IDictionary<string, object> recordData = userRecord.GetData();

            if (recordData[Constants.UserDAOusernameColumn].Equals(userObject.Username) &&
                recordData[Constants.UserDAOfirstNameColumn].Equals(userObject.FirstName) &&
                recordData[Constants.UserDAOlastNameColumn].Equals(userObject.LastName) &&
                recordData[Constants.UserDAOemailColumn].Equals(userObject.Email) &&
                recordData[Constants.UserDAOphoneNumberColumn].Equals(userObject.PhoneNumber) &&
                recordData[Constants.UserDAOpasswordColumn].Equals(userObject.Password) &&
                recordData[Constants.UserDAOdisabledColumn].Equals(userObject.Disabled) &&
                recordData[Constants.UserDAOuserTypeColumn].Equals(userObject.UserType) &&
                recordData[Constants.UserDAOsaltColumn].Equals(userObject.Salt) &&
                recordData[Constants.UserDAOtempTimestampColumn].Equals(userObject.TempTimestamp) &&
                recordData[Constants.UserDAOemailCodeColumn].Equals(userObject.EmailCode) &&
                recordData[Constants.UserDAOemailCodeTimestampColumn].Equals(userObject.EmailCodeTimestamp) &&
                recordData[Constants.UserDAOloginFailuresColumn].Equals(userObject.LogInFailures) &&
                recordData[Constants.UserDAOlastLoginFailTimestampColumn].Equals(userObject.LastLoginFailTimestamp) &&
                recordData[Constants.UserDAOemailCodeFailuresColumn].Equals(userObject.EmailCodeFailures) &&
                recordData[Constants.UserDAOphoneCodeFailuresColumn].Equals(userObject.PhoneCodeFailures))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Given the user information, successfully create the user.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CreateAsync_SuccessfulCreation
            (string username, string firstName, string lastName,
             string email,string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user record.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp,loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);

            //Act

            // Create the user.
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);
            // Read the created user's data.
            UserObject userObject = (UserObject)await userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            // If the created user has correct data, set the result to true.
            bool correctDataResult = DataEquals(userRecord, userObject);

            //Assert

            // The created user should have the correct data.
            Assert.IsTrue(correctDataResult);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string>{ username }).ConfigureAwait(false);
        }

        // Creating a duplicated user throws an exception and fails.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CreateAsync_UnsuccessfulCreation
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user record.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {   // Create the user.
                await userDAO.CreateAsync(userRecord).ConfigureAwait(false);
                // Create a duplicated user.
                await userDAO.CreateAsync(userRecord).ConfigureAwait(false);
            }
            catch(Exception)
            {
                // Catch exception and set the result true.
                result = true;
            }

            //Assert

            // Result should be true.
            Assert.IsTrue(result);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // The specified user gets deleted succssfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_DeleteByIdsAsync_SuccessfulDeletion
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string>{ username }).ConfigureAwait(false);
            // Check if the user exists and set the result accordingly.
            bool result = await userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false);

            //Assert

            // The result should be false.
            Assert.IsFalse(result);
        }

        // Given a non-existing user, deletion fails.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_DeleteByIdsAsync_UnsuccessfulDeletion
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Delete the user.
                await userDAO.DeleteByIdsAsync(new List<string> { "nonExistingUser" }).ConfigureAwait(false);
            }
            catch (ArgumentException)
            {
                // Catch exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);

            //Clean up
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // The specified user's data gets read successfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_ReadByIdAsync_SuccessfulRead
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);


            //Act

            // Read the user's data.
            UserObject userObject = (UserObject)await userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            // Check if it's correct and set the result accordingly.
            bool result = DataEquals(userRecord, userObject);

            //Assert

            // Result should be true.
            Assert.IsTrue(result);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given a non-existing user, read fails.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_ReadByIdAsync_UnsuccessfulRead
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);
            bool result = false;

            //Act

            try
            {
                // Read the user's data.
                UserObject userObject = (UserObject)await userDAO.ReadByIdAsync("nonExistingUser").ConfigureAwait(false);
            }
            catch(ArgumentException)
            {
                // Catch exception and set the result true.
                result = true;
            }

            //Assert

            // Result should be true.
            Assert.IsTrue(result);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Update the user successfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_UpdateAsync_SuccessfulUpdate
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            // Same username with new data.
            string newString = "new";
            int newNumber = 1;
            UserRecord updatedUserRecord = new UserRecord(username, newString, newString, newString,
                           newString, newString, newNumber, newString, newString,
                          newNumber, newString, newNumber, newNumber,
                           newNumber, newNumber, newNumber);

            //Act

            // Update the user.
            await userDAO.UpdateAsync(updatedUserRecord).ConfigureAwait(false);
            // Read the updated user's data.
            UserObject userObject= (UserObject)await userDAO.ReadByIdAsync(username).ConfigureAwait(false);
            // Check if the data are updatd correctly, and set the result accordingly.
            bool result = DataEquals(updatedUserRecord, userObject);

            //Assert

            // The result should be true.
            Assert.IsTrue(result);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Update the user unsuccessfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_UpdateAsync_UnsuccessfulUpdate
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            // Different username with new data.
            string newString = "new";
            int newNumber = 1;
            UserRecord updatedUserRecord = new UserRecord("wrongUsername", newString, newString, newString,
                           newString, newString, newNumber, newString, newString,
                          newNumber, newString, newNumber, newNumber,
                           newNumber, newNumber, newNumber);

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Update the user.
                await userDAO.UpdateAsync(updatedUserRecord).ConfigureAwait(false);
            }
            catch(ArgumentException)
            {
                // Catch exception and set the result true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given an exsiting user, return true.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckUserExistenceAsync_UserExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Check if the user exsits, and set the result accordingly.
            bool userExistence = await userDAO.CheckUserExistenceAsync(username).ConfigureAwait(false);

            //Assert

            // The result should be true.
            Assert.IsTrue(userExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given a non-exsiting user, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckUserExistenceAsync_UserNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Check if the user exists, and set the result accordingly.
            bool userExistence = await userDAO.CheckUserExistenceAsync("nonExistingUser").ConfigureAwait(false);

            //Assert

            // The result should be false.
            Assert.IsFalse(userExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given an existing phone number, return true.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckPhoneNumberExistenceAsync_PhoneNumberExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Check if the phone number exists, and set the result accordingly.
            bool phoneNumberExistence = await userDAO.CheckPhoneNumberExistenceAsync(phoneNumber).ConfigureAwait(false);

            //Assert

            // The result should be true.
            Assert.IsTrue(phoneNumberExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }


        // Given a non-existing phone number, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckPhoneNumberExistenceAsync_PhoneNumberNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            //Check if the phone number exists, and set the result accordingly.
            bool phoneNumberExistence = await userDAO.CheckPhoneNumberExistenceAsync("0000000000").ConfigureAwait(false);

            //Assert

            // The result should be false.
            Assert.IsFalse(phoneNumberExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given an existing email, return true.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckEmailExistenceAsync_EmailExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Check if the email exists, and set the result accordingly.
            bool emailExistence = await userDAO.CheckEmailExistenceAsync(email).ConfigureAwait(false);

            //Assert

            // The result should be true.
            Assert.IsTrue(emailExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }

        // Given a non-existing email, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public async Task UserDAO_CheckEmailExistenceAsync_EmailNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            await userDAO.CreateAsync(userRecord).ConfigureAwait(false);

            //Act

            // Check if the email exists, and set the result accordingly.
            bool emailExistence = await userDAO.CheckEmailExistenceAsync("nonExistingEmail").ConfigureAwait(false);

            //Assert

            // The result should be false.
            Assert.IsFalse(emailExistence);

            //CleanUp

            // Delete the user.
            await userDAO.DeleteByIdsAsync(new List<string> { username }).ConfigureAwait(false);
        }


    }
}
