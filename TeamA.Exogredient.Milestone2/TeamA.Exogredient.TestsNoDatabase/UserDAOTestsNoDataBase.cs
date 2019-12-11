using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserDAOTestsNoDataBase
    {
        private bool DataEquals(UserRecord userRecord, UserObject userObject)
        {
            IDictionary<string,object> recordData = userRecord.GetData();

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
        public void UserDAO_Create_SuccessfulCreation
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user record.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);

            //Act

            // Create the user.
            userDAO.Create(userRecord);
            // Read the created user's data.
            UserObject userObject = (UserObject) userDAO.ReadById(username);
            // If the created user has correct data, set the result to true.
            bool correctDataResult = DataEquals(userRecord, userObject);

            //Assert

            // The created user should have the correct data.
            Assert.IsTrue(correctDataResult);

        }

        // Creating a duplicated user throws an exception and fails. 
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_Create_UnsuccessfulCreation
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
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
                userDAO.Create(userRecord);
                // Create a duplicated user.
                userDAO.Create(userRecord);
            }
            catch (Exception)
            {
                // Catch exception and set the true.
                result = true;
            }

            //Assert

            // Result should be true.
            Assert.IsTrue(result);
        }

        // The specified user gets deleted succssfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public void UserDAO_DeleteByIds_SuccessfulDeletion
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange
            UnitTestUserDAO userDAO = new UnitTestUserDAO();

            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Delete the user.
            userDAO.DeleteByIds(new List<string> { username });
            // Check if the user exists and set the result accordingly. 
            bool result = userDAO.CheckUserExistence(username);

            //Assert

            // The result should be false.
            Assert.IsFalse(result);
        }

        // Given a non-existing user, deletion fails.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public void UserDAO_DeleteByIds_UnsuccessfulDeletion
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Delete the user. 
                userDAO.DeleteByIds(new List<string> { "nonExistingUser" });
            }
            catch (ArgumentException)
            {
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // The specified user's data gets read successfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_ReadById_SuccessfulRead
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);
            
            //Act

            // Read the user's data.
            UserObject userObject = (UserObject)userDAO.ReadById(username);
            // Check if it's correct and set the result accordingly.
            bool correctDataResult = DataEquals(userRecord, userObject);

            //Assert

            // Result should be true.
            Assert.IsTrue(correctDataResult);
        }

        // Given a non-existing user, read fails.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public void UserDAO_ReadById_UnsuccessfulRead
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);
            bool result = false;

            //Act

            try
            {
                // Read the user's data.
                UserObject userObject = (UserObject) userDAO.ReadById("nonExistingUser");
            }
            catch (ArgumentException)
            {
                // Catch exception and set the result true.
                result = true;
            }

            //Assert

            // Result should be true.
            Assert.IsTrue(result);
        }

        // Update the user successfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_Update_SuccessfulUpdate
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            // Same username with new data.
            string newString = "new";
            int newNumber = 1;
            UserRecord updatedUserRecord = new UserRecord(username, newString, newString, newString,
                           newString, newString, newNumber, newString, newString,
                          newNumber, newString, newNumber, newNumber,
                           newNumber, newNumber, newNumber);

            //Act

            // Update the user.
            userDAO.Update(updatedUserRecord);
            // Read the updated user's data.
            UserObject userObject = (UserObject)userDAO.ReadById(username);
            // Check if the data are updatd correctly, and set the result accordingly.
            bool correctDataResult = DataEquals(updatedUserRecord, userObject);

            //Assert

            // The result should be true.
            Assert.IsTrue(correctDataResult);
        }

        // Update the user unsuccessfully.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public void UserDAO_Update_UnsuccessfulUpdate
            (string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

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
                userDAO.Update(updatedUserRecord);
            }
            catch (ArgumentException)
            {
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // Given an exsiting user, return true.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckUserExistence_UserExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Check if the user exsits, and set the result accordingly.
            bool userExistence = userDAO.CheckUserExistence(username);

            //Assert

            // The result should be true.
            Assert.IsTrue(userExistence);
        }


        // Given a non-exsiting user, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckUserExistence_UserNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Check if the user exists, and set the result accordingly.
            bool userExistence = userDAO.CheckUserExistence("nonExistingUser");

            //Assert

            // The result should be false.
            Assert.IsFalse(userExistence);
        }

        // Given an existing phone number, return true. 
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckPhoneNumberExistence_PhoneNumberExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange
            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Check if the phone number exists, and set the result accordingly.
            bool phoneNumberExistence = userDAO.CheckPhoneNumberExistence(phoneNumber);

            //Assert

            // The result should be true.
            Assert.IsTrue(phoneNumberExistence);
        }


        // Given a non-existing phone number, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckPhoneNumberExistence_PhoneNumberNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            //Check if the phone number exists, and set the result accordingly.
            bool phoneNumberExistence = userDAO.CheckPhoneNumberExistence("0000000000");

            //Assert

            // The result should be false.
            Assert.IsFalse(phoneNumberExistence);
        }

        // Given an existing email, return true.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckEmailExistence_EmailExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Check if the email exists, and set the result accordingly.
            bool emailExistence = userDAO.CheckEmailExistence(email);

            //Assert

            // The result should be true.
            Assert.IsTrue(emailExistence);
        }

        // Given a non-existing email, return false.
        [TestMethod]
        [DataRow("UserDAOTest", "firstname", "lastname", "email", "1234567890", "password", 0, "usertype", "salt", 0, "ecode", 0, 0, 0, 0, 0)]
        public  void UserDAO_CheckEmailExistence_EmailNonExists(string username, string firstName, string lastName,
             string email, string phoneNumber, string password, int disabled, string userType, string salt,
             long tempTimestamp, string emailCode, long emailCodeTimestamp, int loginFailures,
             long lastLoginFailTimestamp, int emailCodeFailures, int phoneCodeFailures)
        {
            //Arrange

            UnitTestUserDAO userDAO = new UnitTestUserDAO();
            // Create a user for the test.
            UserRecord userRecord = new UserRecord(username, firstName, lastName, email,
                           phoneNumber, password, disabled, userType, salt,
                          tempTimestamp, emailCode, emailCodeTimestamp, loginFailures,
                           lastLoginFailTimestamp, emailCodeFailures, phoneCodeFailures);
            userDAO.Create(userRecord);

            //Act

            // Check if the email exists, and set the result accordingly.
            bool emailExistence = userDAO.CheckEmailExistence("nonExistingEmail");

            //Assert

            // The result should be false.
            Assert.IsFalse(emailExistence);
        }


    }
}
