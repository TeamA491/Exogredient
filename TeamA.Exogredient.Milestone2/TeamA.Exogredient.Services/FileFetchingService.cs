﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace TeamA.Exogredient.Services
{
    public static class FileFetchingService
    {
        public static bool FetchLogs(string sourceDirectory, string targetDirectory, int days)
        {
            // Check to make sure source Directory exists.
            if (!Directory.Exists(sourceDirectory))
            {
                return false;
            }

            // Create a folder that will get compressed and send later on. Delete already existing folder with same name. 
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            else
            {
                Directory.Delete(targetDirectory, true);
                Directory.CreateDirectory(targetDirectory);
            }

            // Gather file paths for logs in the source Directory
            string[] logFilePaths = Directory.GetFiles(sourceDirectory);

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
                if (fileInformation.CreationTime <= DateTime.Now.AddDays(-days))
                {
                    string fileName = fileInformation.Name;
                    targetFilePath = Path.Combine(targetDirectory, fileName);
                    File.Move(logFile, targetFilePath);
                }
            }

            // Check target directory to make sure files were moved properly
            string[] newLogFilePaths = Directory.GetFiles(targetDirectory);
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