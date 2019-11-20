using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using CsvHelper;

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
                using (CsvWriter csv = new CsvWriter(writer))
                {
                    for (int i = 0; i < logRecord.Fields.Count; i++)
                    {
                        csv.WriteField(logRecord.Fields[i]);
                    }
                    csv.NextRecord();
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
                using (StreamReader reader = new StreamReader(path))
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        //if (line != "removeme")
                        //{
                        //    writer.WriteLine(line);
                        //}
                    }
                }

                File.Delete("file.txt");
                File.Move(tempFile, "file.txt");
            }
            else
            {
                throw new ArgumentException("Record must be of type LogRecord");
            }
        }
    }
}
