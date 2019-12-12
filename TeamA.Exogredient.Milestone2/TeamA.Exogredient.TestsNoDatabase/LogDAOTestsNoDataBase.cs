using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
namespace TeamA.Exogredient.TestsNoDatabase
{
    [TestClass]
    public class LogDAOTestsNoDataBase
    {
        readonly UnitTestLogDAO logDAO = new UnitTestLogDAO();

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public void LogDAO_Create_SuccessfulCreation(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: 
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);

            // Act: 
            bool result = logDAO.Create(record, date);
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public void LogDAO_Create_SuccessfulDuplicateCreation(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: 
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);

            // Act: 
            bool resultCreate = logDAO.Create(record, date);
            Assert.IsTrue(resultCreate);

            bool resultDuplicate = logDAO.Create(record, date);
            Assert.IsTrue(resultDuplicate);

        }

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public void LogDAO_Delete_SuccessfulDeletion(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: Create a log and find its ID.
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);
            bool resultCreate = logDAO.Create(record, date);
            Assert.IsTrue(resultCreate);

            string id = logDAO.FindIdField(record, date);

            // Act: Delete the log.
            bool deleteResult = logDAO.Delete(id, date);
            Assert.IsTrue(deleteResult);
           
        }

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public void LogDAO_Delete_UnsuccessfulDeletion(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: Create a log.
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);
            bool resultCreate = logDAO.Create(record, date);
            Assert.IsTrue(resultCreate);
            string invalidID = "invalid";
            bool result = false;

            // Act: Delete an ID that does not exist.
            try
            {
                bool deleteResult = logDAO.Delete(invalidID, date);
            }
            catch(ArgumentException)
            {
                result = true;
            }
            
            Assert.IsTrue(result);

        }

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public void LogDAO_FindIdField_SuccessFindNonExistentId(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: Create an empty group by creating one log and deleting it.

            bool result;
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);
            bool resultCreate = logDAO.Create(record, date);
            Assert.IsTrue(resultCreate);

            string id = logDAO.FindIdField(record, date);
            bool deleteResult = logDAO.Delete(id, date);
            Assert.IsTrue(deleteResult);

            // Act: finding a field that doesn't exists throws an argument exception.
            try
            {
                logDAO.FindIdField(record, date);
                result = false;
            }
            catch (ArgumentException)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

    }
}
