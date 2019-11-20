using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public class FlatFileLoggingDAO : IMasterFlatFileDAO
    {
        private readonly string _logFolder = @"C:\Logs";

        public FlatFileLoggingDAO()
        {
            try
            {
                Directory.CreateDirectory(_logFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void Create(object record, string folderName, string fileName)
        {
            if (record.GetType() == typeof(LogRecord))
            {
                LogRecord logRecord = (LogRecord)record;

                string directory = _logFolder + $"/{folderName}";

                Directory.CreateDirectory(directory);

                string path = directory + "/" + fileName;

                // TODO: See what write field does and emulate
                using (StreamWriter writer = File.AppendText(path))
                {
                    string result = "";

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

                    writer.WriteLine(result);
                }
            }
            else
            {
                throw new ArgumentException("Record must be of type LogRecord");
            }
        }

        public void Delete(object record, string folderName, string fileName)
        {
            if (record.GetType() == typeof(LogRecord))
            {
                LogRecord logRecord = (LogRecord)record;

                string tempFile = Path.GetTempFileName();

                string directory = _logFolder + $"/{folderName}";
                string path = directory + "/" + fileName;

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

                    while ((lineInput = reader.ReadLine()) != null)
                    {
                        if (lineInput != lineToDelete)
                        {
                            writer.WriteLine(lineInput);
                        }
                    }
                }

                File.Delete(path);
                File.Move(tempFile, path);
            }
            else
            {
                throw new ArgumentException("Record must be of type LogRecord");
            }
        }
    }
}
