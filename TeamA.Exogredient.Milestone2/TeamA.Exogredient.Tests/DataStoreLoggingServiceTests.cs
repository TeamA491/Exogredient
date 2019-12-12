using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class DataStoreLoggingServiceTests
    {
        [DataTestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "ipaddress", "errortype")]
        public async Task DataStoreLoggingService_LogToDataStoreAsync_SuccessfullLog(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // ACT: 
            bool result = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(result);

            // Cleanup: Delete the created log.
            bool deleteResult = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(deleteResult);
        }

        [DataTestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "ipaddress", "errortype")]
        public async Task DataStoreLoggingService_LogToDataStoreAsync_SuccessCreateDuplicates(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // Arrange: Create the initial log. 
            bool createResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(createResult);

            // Act: Create the duplicate log.
            bool duplicateResult = await DataStoreLoggingService.LogToDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(duplicateResult);

            // Cleanup: Delete the created logs.
            bool firstDelete = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(firstDelete);

            bool secondDelete = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(secondDelete);
        }

        [TestMethod]
        [DataRow("01:01:11:11 UTC 20191201", "operation", "identifier", "ipaddress", "errortype")]

        public async Task DataStoreLoggingService_DeleteLogFromDataStoreAsync_SuccessfullDeleteNonExistent(string timestamp, string operation, string identifier, string ipAddress, string errorType)
        {
            // Act: Deleting non existent log should throw and error.
            bool deleteResult = await DataStoreLoggingService.DeleteLogFromDataStoreAsync(timestamp, operation, identifier, ipAddress, errorType);
            Assert.IsTrue(deleteResult);
        }

    }
}
