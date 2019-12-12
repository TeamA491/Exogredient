using System.IO;
using System.Diagnostics;
using TeamA.Exogredient.AppConstants;
using System;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// The Compression service is used to compress 
    /// all files in a directory. 
    /// </summary>
    public static class CompressionService
    {
        /// <summary>
        /// The compress function uses 7zip.exe to compress a directory
        /// and store the archive in a new location.
        /// </summary>
        /// <param name="sevenZipPath"> The file path to 7z.exe</param>
        /// <param name="sourceDirectory"> The directory that is meant to be archived</param>
        /// <param name="targetDirectory"> The directory where archive file should be stored.</param>
        /// <returns></returns>
        public static bool Compress(string sevenZipPath, string sourceDirectory, string targetDirectory)
        {
            // Check to make sure source Directory exists
            if (!(Directory.Exists(sourceDirectory) && Directory.Exists(targetDirectory) && File.Exists(sevenZipPath)))
            {
                throw new ArgumentException("Invalid source Directory or 7zip file path.");
            }

            // Set the name of the compressed target file 
            string targetFile = targetDirectory + Constants.SevenZipFileExtension;

            // Set up arguments needed to start a 7zip process 
            ProcessStartInfo sevenZipProcessInfo = new ProcessStartInfo
            {
                FileName = Constants.SevenZipPath,

                // Pass arguments for 7zip command. Archive source directory to targetfile and delete files that were compressed from source directory
                Arguments = Constants.ArchivePrefixArgument + targetFile + " " + sourceDirectory + Constants.ArchivePostfixArgument,

                // Set window property of the process to Hidden so it runs in background. 
                WindowStyle = ProcessWindowStyle.Hidden
            };

            // Start 7zip process with parameters defined in the ProcessStartInfo object.
            Process activeSevenZipProcess = Process.Start(sevenZipProcessInfo);
            activeSevenZipProcess.WaitForExit();

            // Check to see if archive was successfully created
            if (!File.Exists(targetFile))
            {
                throw new Exception("Archive failed to create.");
            }
            else
            {
                return true;
            }
        }
    }
}
