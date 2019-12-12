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
    public class LogDAOTests
    {
        readonly LogDAO logDAO = new LogDAO();
        
        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public async Task LogDAO_CreateAsync_SuccessfulCreation(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: 
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);

            // Act: 
            bool result = await logDAO.CreateAsync(record, date).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Cleanup: find the Id of the log we created and delete it.
            string id = await logDAO.FindIdFieldAsync(record, date).ConfigureAwait(false);
            bool deleteResult = await logDAO.DeleteAsync(id, date).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("Timestamp", "Operation", "Identifier", "IPAddress", "errorType", "20190101")]
        public async Task LogDAO_CreateAsync_SuccessfulDuplicateCreation(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {
            // Arrange: 
            LogRecord record = new LogRecord(timestamp, operation, identifier, ipAddress, errorType);

            // Act: 
            bool resultCreate = await logDAO.CreateAsync(record, date).ConfigureAwait(false);
            Assert.IsTrue(resultCreate);

            bool resultDuplicate = await logDAO.CreateAsync(record, date).ConfigureAwait(false);
            Assert.IsTrue(resultDuplicate);

            // Cleanup: find the Id of the log we created and delete it.
            string id = await logDAO.FindIdFieldAsync(record, date).ConfigureAwait(false);

            bool deleteResult = await logDAO.DeleteAsync(id, date).ConfigureAwait(false);
            Assert.IsTrue(deleteResult);

            string idDuplicate = await logDAO.FindIdFieldAsync(record, date).ConfigureAwait(false);

            bool deleteResultDuplicate = await logDAO.DeleteAsync(idDuplicate, date).ConfigureAwait(false);
            Assert.IsTrue(deleteResultDuplicate);
        }

        [DataTestMethod]
        [DataRow("Doesn't Exist")]
        public async Task LogDAO_FindIdFieldAsync_SuccessFindNonExistentId(string timestamp, string operation, string identifier, string ipAddress, string errorType, string date)
        {

        }



    }
}
