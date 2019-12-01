using System.IO;
using System.Diagnostics;

namespace TeamA.Exogredient.Services
{
    public static class CompressionService
    {
        public static bool Compress(string sevenZipPath, string sourceDirectory, string targetDirectory)
        {
            // Check to make sure source Directory exists
            if (!(Directory.Exists(sourceDirectory) || Directory.Exists(targetDirectory) || File.Exists(sevenZipPath)))
            {
                return false;
            }

            // Set the name of the compressed target file 
            string targetFile = targetDirectory + ".7z";

            // Set up arguments needed to start a 7zip process 
            ProcessStartInfo sevenZipProcessInfo = new ProcessStartInfo();
            sevenZipProcessInfo.FileName = sevenZipPath;

            // Pass arguments for 7zip command. Archive source directory to targetfile and delete files that were compressed from source directory
            sevenZipProcessInfo.Arguments = "a -t7z " + targetFile + " " + sourceDirectory + " -sdel";

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
