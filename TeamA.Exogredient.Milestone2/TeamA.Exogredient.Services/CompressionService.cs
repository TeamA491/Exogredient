using System.IO;
using System.Diagnostics;

namespace TeamA.Exogredient.Services
{
    public static class CompressionService
    {
        public static bool Compress(string _7zipPath, string _sourceDirectory, string _targetDirectory)
        {
            // Check to make sure source Directory exists
            if (!(Directory.Exists(_sourceDirectory) || Directory.Exists(_targetDirectory) || File.Exists(_7zipPath)))
            {
                return false;
            }

            // Set the name of the compressed target file 
            string targetFile = _targetDirectory + ".7z";

            // Set up arguments needed to start a 7zip process 
            ProcessStartInfo sevenZipProcessInfo = new ProcessStartInfo();
            sevenZipProcessInfo.FileName = _7zipPath;

            // Pass arguments for 7zip command. Archive source directory to targetfile and delete files that were compressed from source directory
            sevenZipProcessInfo.Arguments = "a -t7z " + targetFile + " " + _sourceDirectory + " -sdel";

            // Set window property of the process to Hidden so it runs in background. 
            sevenZipProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Start 7zip process with parameters defined in the ProcessStartInfo object.
            Process activeSevenZipProcess = Process.Start(sevenZipProcessInfo);
            activeSevenZipProcess.WaitForExit();

            // Check to see if archive was successfully created
            return File.Exists(targetFile);
        }
    }
}
