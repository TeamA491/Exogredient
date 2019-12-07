using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class FileFetchingServiceTests
    {
        [TestMethod]
      public void FileFetchingService_FetchLogs_FetchExistingFilesToNewDirectory()
        {
            //arrage
            DateTime currentTime = DateTime.Now;
            string sourceDirectory = @"C:\_LogFiles\TF3";
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            int days = 0;

            // If source directory doesn't exist create it.
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }

            // If sourceDirectory doesn't contain any logs create them.
            if (Directory.GetFiles(sourceDirectory).Length == 0)
            {
                using (File.Create(sourceDirectory + @"\log1.csv")) { } ;
            }

            // Delete targetdirectory to test FetchExistingFilesToNewDirectory
            if (Directory.Exists(targetDirectory))
            {
                Directory.Delete(targetDirectory, true);
            }

            //act
            bool result = FileFetchingService.FetchLogs(sourceDirectory,targetDirectory,days);
            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FileFetchingService_FetchLogs_NoLogFilesinSourceDirectory()
        {
            DateTime currentTime = DateTime.Now;
            string sourceDirectory = @"C:\ajdshfkjahds";
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");

            // If source Directory exists delete it and recreate so it has no logs files.
            if (Directory.Exists(sourceDirectory))
            {
                Directory.Delete(sourceDirectory, true);
                Directory.CreateDirectory(sourceDirectory);
            }
            // If source Directory doesn't exist create it.
            else
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            //act
            bool result = FileFetchingService.FetchLogs(sourceDirectory,targetDirectory,30);
            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FileFetchingService_FetchLogs_FetchToExistingDirectory()
        {
            DateTime currentTime = DateTime.Now;
            string sourceDirectory = @"C:\_LogFiles\TF1";
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");

            // If source directory doesn't exist create it.
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }

            // If sourceDirectory doesn't contain any logs create them.
            if (Directory.GetFiles(sourceDirectory).Length == 0)
            {
                using (File.Create(sourceDirectory + @"\log1.csv")) { }
            }

            // Create target directory if it doesn't exists to test FetchToExistingDirectory.
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            //act
            bool result = FileFetchingService.FetchLogs(sourceDirectory,targetDirectory,0);

            //assert 
            Assert.IsTrue(result);
        }
    }
}
