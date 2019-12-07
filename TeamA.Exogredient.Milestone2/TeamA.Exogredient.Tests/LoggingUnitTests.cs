using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class LoggingUnitTests
    {
        // Flat file log directory
        private readonly string _logDirectory = @"C:\Logs";

        [TestInitialize]
        public void init()
        {
        }

        [DataTestMethod]
        [DataRow("this is an invalid timestamp", "1", "1", "1", "1")]
        [DataRow("invalid", "2", "2", "2", "2")]
        public async Task FlatFileLoggingService_LogToFlatFileAsync_InvalidTimestampRejected(string timestamp, string operation, string identifier,
                                                                                             string ipAddress, string errorType)
        {
            bool result = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

            Assert.IsFalse(result);
        }

        // IMPORTANT: make sure you are not in the directory in windows explorer etc.
        //            or have any of its children open when running this test.
        [DataTestMethod]
        [DataRow("20:33:08:59 UTC 20191125", "1", "1", "1", "1")]
        [DataRow("20:33:08:59 UTC 20191125", "2", "2", "2", "2")]
        public async Task FlatFileLoggingService_LogToFlatFileAsync_DirectoryCreated(string timestamp, string operation, string identifier,
                                                                                     string ipAddress, string errorType)
        {
            try
            {
                Directory.Delete(_logDirectory, true);
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

                bool result = Directory.Exists(_logDirectory);

                Assert.IsTrue(result);
            }
            catch
            {
                Assert.Fail();
            }
        }

        // IMPORTANT: make sure you don't have 20191125.CSV open when running this test.
        [DataTestMethod]
        [DataRow("20:33:08:59 UTC 20191125", "1", "1", "1", "1")]
        [DataRow("20:33:08:59 UTC 20191125", "2", "2", "2", "2")]
        public async Task FlatFileLoggingService_LogToFlatFileAsync_FileCreated(string timestamp, string operation, string identifier,
                                                                                string ipAddress, string errorType)
        {
            try
            {
                File.Delete(_logDirectory + @"\20191125.CSV");
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

                bool result = File.Exists(_logDirectory + @"\20191125.CSV");

                Assert.IsTrue(result);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [DataTestMethod]
        [DataRow("20:33:08:59 UTC 20191125", "=1", "1", "1", "1")]
        [DataRow("20:33:08:59 UTC 20191125", "@1", "1", "1", "1")]
        [DataRow("20:33:08:59 UTC 20191125", "+1", "1", "1", "1")]
        [DataRow("20:33:08:59 UTC 20191125", "-1", "1", "1", "1")]
        public async Task FlatFileLoggingService_LogToFlatFileAsync_CsvProtection(string timestamp, string operation, string identifier,
                                                                                  string ipAddress, string errorType)
        {
            try
            {
                File.Delete(_logDirectory + @"\20191125.CSV");
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType).ConfigureAwait(false);

                bool result = false;
                bool lineRead = false;

                string lineInput = "";

                using (StreamReader reader = new StreamReader(_logDirectory + @"\20191125.CSV"))
                {
                    while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        
                    }
                }

                Assert.IsTrue(result);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
