using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class FlatFileLoggingServiceTests
    {
        // Flat file log directory
        private readonly string _logDirectory = Constants.LogFolder;

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
            bool result = await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

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
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

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
                File.Delete(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}");
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

                bool result = File.Exists(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}");

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
            LogRecord rec = new LogRecord(timestamp.Split(' ')[0] + " " + timestamp.Split(' ')[1], operation, identifier, ipAddress, errorType);

            MaskingService ms = new MaskingService(new MapDAO());

            LogRecord logRecord = (LogRecord)await ms.MaskAsync(rec, false).ConfigureAwait(false);
            try
            {
                File.Delete(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}");
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

                bool result = false;

                string lineInput = "";

                using (StreamReader reader = new StreamReader(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}"))
                {
                    // Construct the line to delete
                    string lineToFind = "";

                    for (int i = 0; i < logRecord.Fields.Count; i++)
                    {
                        string field = logRecord.Fields[i];

                        string startsWith = field.Substring(0, 1);

                        // If the field starts with a csv vulnerability, re-add the padding to the beginning.
                        if (Constants.CsvVulnerabilities.Contains(startsWith))
                        {
                            lineToFind += $"{Constants.CsvProtection}{field},";
                        }
                        else
                        {
                            lineToFind += $"{field},";
                        }
                    }

                    // Get rid of last comma.
                    lineToFind = lineToFind.Substring(0, lineToFind.Length - 1);

                    while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (lineInput.Equals(lineToFind))
                        {
                            result = true;
                        }
                    }
                }

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
        public async Task FlatFileLoggingService_DeleteFromFlatFileAsync_DeleteSuccessful(string timestamp, string operation, string identifier,
                                                                                          string ipAddress, string errorType)
        {
            LogRecord rec = new LogRecord(timestamp.Split(' ')[0] + " " + timestamp.Split(' ')[1], operation, identifier, ipAddress, errorType);

            MaskingService ms = new MaskingService(new MapDAO());

            LogRecord logRecord = (LogRecord)await ms.MaskAsync(rec, false).ConfigureAwait(false);

            try
            {
                File.Delete(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}");
            }
            catch
            { }

            try
            {
                await FlatFileLoggingService.LogToFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

                Assert.IsTrue(File.Exists(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}"));

                bool result = false;

                string lineInput = "";

                using (StreamReader reader = new StreamReader(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}"))
                {
                    // Construct the line to delete
                    string lineToFind = "";

                    for (int i = 0; i < logRecord.Fields.Count; i++)
                    {
                        string field = logRecord.Fields[i];

                        string startsWith = field.Substring(0, 1);

                        // If the field starts with a csv vulnerability, re-add the padding to the beginning.
                        if (Constants.CsvVulnerabilities.Contains(startsWith))
                        {
                            lineToFind += $"{Constants.CsvProtection}{field},";
                        }
                        else
                        {
                            lineToFind += $"{field},";
                        }
                    }

                    // Get rid of last comma.
                    lineToFind = lineToFind.Substring(0, lineToFind.Length - 1);

                    while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (lineInput.Equals(lineToFind))
                        {
                            result = true;
                        }
                    }
                }

                Assert.IsTrue(result);

                result = false;

                await FlatFileLoggingService.DeleteFromFlatFileAsync(timestamp, operation, identifier, ipAddress, errorType, Constants.LogFolder, Constants.LogFileType).ConfigureAwait(false);

                using (StreamReader reader = new StreamReader(_logDirectory + $@"\{timestamp.Split(' ')[2]}{Constants.LogFileType}"))
                {
                    // Construct the line to delete
                    string lineToFind = "";

                    for (int i = 0; i < logRecord.Fields.Count; i++)
                    {
                        string field = logRecord.Fields[i];

                        string startsWith = field.Substring(0, 1);

                        // If the field starts with a csv vulnerability, re-add the padding to the beginning.
                        if (Constants.CsvVulnerabilities.Contains(startsWith))
                        {
                            lineToFind += $"{Constants.CsvProtection}{field},";
                        }
                        else
                        {
                            lineToFind += $"{field},";
                        }
                    }

                    // Get rid of last comma.
                    lineToFind = lineToFind.Substring(0, lineToFind.Length - 1);

                    while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        if (lineInput.Equals(lineToFind))
                        {
                            result = true;
                        }
                    }
                }

                Assert.IsFalse(result);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
