using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace TeamA.Exogredient.Services
{
    public static class FileFetchingService
    {
        public static bool FetchLogs(string _sourceDirectory, string _targetDirectory, int _days)
        {
            // Check to make sure source Directory exists.
            if (!Directory.Exists(_sourceDirectory))
            {
                return false;
            }

            // Create a folder that will get compressed and send later on. Delete already existing folder with same name. 
            if (!Directory.Exists(_targetDirectory))
            {
                Directory.CreateDirectory(_targetDirectory);
            }
            else
            {
                Directory.Delete(_targetDirectory, true);
                Directory.CreateDirectory(_targetDirectory);
            }

            // Gather file paths for logs in the source Directory
            string[] logFilePaths = Directory.GetFiles(_sourceDirectory);

            // Return false if no log files found
            if (logFilePaths.Length == 0)
            {
                return false;
            }

            // Identify files older than _days amount of days and move them to the target directory. 
            string targetFilePath;
            foreach (string logFile in logFilePaths)
            {
                FileInfo fileInformation = new FileInfo(logFile);
                if (fileInformation.CreationTime <= DateTime.Now.AddDays(-_days))
                {
                    string fileName = fileInformation.Name;
                    targetFilePath = Path.Combine(_targetDirectory, fileName);
                    File.Move(logFile, targetFilePath);
                }
            }

            // Check target directory to make sure files were moved properly
            string[] newLogFilePaths = Directory.GetFiles(_targetDirectory);
            if (newLogFilePaths.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
