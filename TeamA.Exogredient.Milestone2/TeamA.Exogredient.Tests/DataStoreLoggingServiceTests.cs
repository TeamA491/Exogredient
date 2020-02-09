using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class DataStoreLoggingServiceTests
    {
        private static readonly LogDAO _logDAO = new LogDAO(Constants.SQLConnection);
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.SQLConnection);
        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private readonly DataStoreLoggingService _dsLog = new DataStoreLoggingService(_logDAO, _maskingService);

        [DataTestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "127.0.0.1", "errortype")]
        public async Task DataStoreLoggingService_LogToDataStoreAsync_SuccessfullLog(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // ACT: 
            bool result = await _dsLog.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(result);

            // Cleanup: Delete the created log.
            bool deleteResult = await _dsLog.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "127.0.0.1", "errortype")]
        public async Task DataStoreLoggingService_LogToDataStoreAsync_SuccessCreateDuplicates(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // Arrange: Create the initial log. 
            bool createResult = await _dsLog.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(createResult);

            // Act: Create the duplicate log.
            bool duplicateResult = await _dsLog.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(duplicateResult);

            // Cleanup: Delete the created logs.
            bool firstDelete = await _dsLog.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(firstDelete);

            bool secondDelete = await _dsLog.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(secondDelete);
        }

        [TestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "127.0.0.1", "errortype")]

        public async Task DataStoreLoggingService_DeleteLogFromDataStoreAsync_SuccessfullDeleteNonExistent(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // Act: Deleting non existent log return false.
            bool deleteResult = await _dsLog.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsFalse(deleteResult);
        }

    }
}
