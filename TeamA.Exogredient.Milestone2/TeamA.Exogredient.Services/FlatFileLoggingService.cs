using System;
using System.IO;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// This service stores or deletes a log from a flat file.
    /// </summary>
    public class FlatFileLoggingService
    {
        readonly MaskingService _maskingService;

        public FlatFileLoggingService(MaskingService maskingService)
        {
            _maskingService = maskingService;
        }
        /// <summary>
        /// Asynchronously create the log in the flate file.
        /// </summary>
        /// <param name="timestamp">timestamp of the log (string)</param>
        /// <param name="operation">operation of the log (string)</param>
        /// <param name="identifier">identifier of the performer of the operation (string)</param>
        /// <param name="ipAddress">ip address of the performer of the operation (string)</param>
        /// <param name="errorType">the error type that occurred during the operation (string)</param>
        /// <param name="logFolder">the folder to store the file in (string)</param>
        /// <param name="logFileType">the type of file to create (string)</param>
        /// <returns>Task (bool) whether the log creation was successful</returns>
        public async Task<bool> LogToFlatFileAsync(string timestamp, string operation, string identifier,
                                                          string ipAddress, string errorType, string logFolder,
                                                          string logFileType)
        {
            try
            {
                // Create the directory if it doesn't exist.
                Directory.CreateDirectory(logFolder);

                // Currently the timestamp is expected to be in the following format: "HH:mm:ss:ff UTC yyyyMMdd".
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                string fileName = splitResult[2] + logFileType;

                // Create the log record to be stored, mostly just the parameters to this function apart from the timestamp.
                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);
                LogRecord resultRecord = await _maskingService.MaskAsync(logRecord, false).ConfigureAwait(false) as LogRecord;

                // Construct the path to the file.
                string path = Constants.LogFolder + @"\" + fileName;

                // Construct the line to insert.
                string result = "";

                for (int i = 0; i < resultRecord.Fields.Count; i++)
                {
                    string field = resultRecord.Fields[i];

                    // Find the 1 character string that the field starts with.
                    string startsWith = field.Substring(0, 1);

                    // If the field starts with a csv vulnerabilitie, pad it with protection (i.e. excel functions).
                    if (Constants.CsvVulnerabilities.Contains(startsWith))
                    {
                        result += ($"{Constants.CsvProtection}" + field + ",");
                    }
                    else
                    {
                        result += (field + ",");
                    }
                }

                // Get rid of last comma.
                result = result.Substring(0, result.Length - 1);

                // AppendText creates the file if it doesn't exist, then opens it for appending.
                using (StreamWriter writer = File.AppendText(path))
                {
                    // Asynchronously write the line to the file.
                    await writer.WriteLineAsync(result).ConfigureAwait(false);
                }

                return true;
            }
            catch
            {
                // Any exception results in a failed creation.
                return false;
            }
        }

        /// <summary>
        /// Asynchronously delete the log from the flat file.
        /// </summary>
        /// <param name="timestamp">the timestamp that was logged (string)</param>
        /// <param name="operation">the operation that was logged (string)</param>
        /// <param name="identifier">the identifier that was logged (string)</param>
        /// <param name="ipAddress">the ip address that was logged (string)</param>
        /// <param name="errorType">the error type that was logged</param>
        /// <param name="logFolder">the folder where the file is located (string)</param>
        /// <param name="logFileType">the type of file that was created (string)</param>
        /// <returns>Task (bool) whether the log deletion was successful</returns>
        public async Task<bool> DeleteFromFlatFileAsync(string timestamp, string operation, string identifier,
                                                               string ipAddress, string errorType, string logFolder,
                                                               string logFileType)
        {
            try
            {
                // Currently the timestamp is expected to be in the following format: "HH:mm:ss:ff UTC yyyyMMdd".
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                string fileName = splitResult[2] + logFileType;

                // Construct the path to the file.
                string path = logFolder + @"\" + fileName;

                // Create the log record to be found, the parameters to this function should make it unique among the data store,
                // i.e no unique operation should have the same timestamp.
                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);
                LogRecord resultRecord = await _maskingService.MaskAsync(logRecord, false).ConfigureAwait(false) as LogRecord;


                // Temporary file.
                string tempFile = Path.GetTempFileName();

                // NOTE: For very large files, create a temporary file with the lines you want to keep and
                //       replace the file you read from.
                // HACK: better way, start at end
                using (StreamReader reader = new StreamReader(path))
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    // Construct the line to delete
                    string lineToDelete = "";

                    for (int i = 0; i < resultRecord.Fields.Count; i++)
                    {
                        string field = resultRecord.Fields[i];

                        string startsWith = field.Substring(0, 1);

                        // If the field starts with a csv vulnerability, re-add the padding to the beginning.
                        if (Constants.CsvVulnerabilities.Contains(startsWith))
                        {
                            lineToDelete += $"{Constants.CsvProtection}{field},";
                        }
                        else
                        {
                            lineToDelete += $"{field},";
                        }
                    }

                    // Get rid of last comma.
                    lineToDelete = lineToDelete.Substring(0, lineToDelete.Length - 1);

                    string lineInput = "";

                    // Asynchronously read every line in the original file.
                    while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        // Write every line to the temporary file except the line to delete.
                        if (!lineInput.Equals(lineToDelete))
                        {
                            // Asynchronously write the line to the temp file.
                            await writer.WriteLineAsync(lineInput).ConfigureAwait(false);
                        }
                    }
                }

                // Delete the file read from.
                File.Delete(path);

                // Move the temporary file to the correct location.
                File.Move(tempFile, path);

                return true;
            }
            catch
            {
                // Any exception results in a failed deletion.
                return false;
            }
        }
    }
}
