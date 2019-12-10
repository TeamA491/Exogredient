using System;
using System.IO;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.ArchivingConsoleApp
{
    public class ArchivingScript
    {
        public static void Main(string[] args)
        {
            //make sure only days, source Directory, and targetDirectory are entered into program
            if (args.Length != 3)
            {
                System.Console.WriteLine("Need to enter days, source Directory, and target Directory for archiving");
            }

            DateTime currentTime = DateTime.Now;
            int days = Convert.ToInt32(args[0]);
            string sourceDir = "";
            if (Directory.Exists(args[1]))
            {
                sourceDir = args[1];
            }

            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";

            if (FileFetchingService.FetchLogs(sourceDir, targetDirectory, days))
            {
                string targetFile = targetDirectory + ".7z";
                CompressionService.Compress(sevenZipPath, sourceDir, targetDirectory);
            }
        }
    }
}
