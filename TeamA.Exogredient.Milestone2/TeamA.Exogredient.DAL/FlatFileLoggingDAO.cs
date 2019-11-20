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
            }
            else
            {
                throw new ArgumentException("Record must be of type LogRecord");
            }
        }
    }
}
