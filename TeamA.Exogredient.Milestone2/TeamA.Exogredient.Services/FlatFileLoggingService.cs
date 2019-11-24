using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class FlatFileLoggingService
    {
        private readonly string _logFolder = @"C:\Logs";
        private readonly string _fileType = ".CSV";

        public async Task<bool> LogToFlatFileAsync(string timestamp, string operation, string identifier,
                                                   string ipAddress, string errorType)
        {
            try
            {
                Directory.CreateDirectory(_logFolder);

                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException("Timestamp Format Incorrect");
                }

                string fileName = splitResult[2] + _fileType;

                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                string path = _logFolder + @"\" + fileName;

                string result = "";

                // Preparing string log to protect against csv injection.
                for (int i = 0; i < logRecord.Fields.Count; i++)
                {
                    string field = logRecord.Fields[i];

                    if (field.StartsWith("=") || field.StartsWith("@") || field.StartsWith("+") || field.StartsWith("-"))
                    {
                        result += (@"\t" + field + ",");
                    }
                    else
                    {
                        result += (field + ",");
                    }
                }

                // Get rid of last comma.
                result = result.Substring(0, result.Length - 1);

                // TODO: See what write field does and emulate
                using (StreamWriter writer = File.AppendText(path))
                {
                    await writer.WriteLineAsync(result);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFromFlatFileAsync(string timestamp, string operation, string identifier,
                                                        string ipAddress, string errorType)
        {
            try
            {
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException("Timestamp Format Incorrect");
                }

                string fileName = splitResult[2] + _fileType;

                string path = _logFolder + @"\" + fileName;

                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                string tempFile = Path.GetTempFileName();

                // NOTE: For very large files
                // HACK: better way, start at end
                using (StreamReader reader = new StreamReader(path))
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    string lineInput = "";
                    string lineToDelete = "";

                    for (int i = 0; i < logRecord.Fields.Count; i++)
                    {
                        string field = logRecord.Fields[i];

                        if (field.StartsWith("=") || field.StartsWith("@") || field.StartsWith("+") || field.StartsWith("-"))
                        {
                            lineToDelete += $@"\t{field},";
                        }
                        else
                        {
                            lineToDelete += $"{field},";
                        }
                    }

                    // Get rid of last comma.
                    lineToDelete = lineToDelete.Substring(0, lineToDelete.Length - 1);

                    while ((lineInput = await reader.ReadLineAsync()) != null)
                    {
                        if (lineInput != lineToDelete)
                        {
                            await writer.WriteLineAsync(lineInput);
                        }
                    }
                }

                File.Delete(path);
                File.Move(tempFile, path);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
