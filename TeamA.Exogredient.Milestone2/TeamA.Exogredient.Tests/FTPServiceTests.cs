using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using System.Net;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class FTPServiceTests
    {

        [TestMethod]
        public async Task FTPService_Send_SendValidCredentialsAsync()
        {
            // Arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\TF1\" + currentTime.ToString("ddMMyy");
            
            // Create a base archive directory with another directory inside of it(named after date). 
            if (!Directory.Exists(@"C:\_ArchiveFiles\TF1\"))
            {
                Directory.CreateDirectory(@"C:\_ArchiveFiles\TF1\");
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
            }
            // List the path to archive file. 
            string targetFile = targetDirectory + Constants.SevenZipFileExtension;
            if (!File.Exists(targetFile))
            {
                File.Create(targetFile);
            }
            // Set up FTP credentials
            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            string password = Constants.FTPpassword;
            
            // Act
            bool results = await FTPService.SendAsync(ftpUrl, "", targetDirectory, userName, password);
            
            // Assert
            Assert.IsTrue(results);


            // Cleanup 
            //Directory.Delete(@"C:\_ArchiveFiles\TF1", true);

        }

        [TestMethod]
        public async Task FTPService_Send_SendInvalidCredentialsAsync()
        {
            // Arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\TF2\" + currentTime.ToString("ddMMyy");
            // Create a test folder with another directory inside named after the date. 
            if (!Directory.Exists(@"C:\_ArchiveFiles\TF2\"))
            {
                Directory.CreateDirectory(@"C:\_ArchiveFiles\TF2\");
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
            }
            // Create variable for archive file inside directories. 
            string targetFile = targetDirectory + Constants.SevenZipFileExtension;
            if (!File.Exists(targetFile))
            {
                File.Create(targetFile);
            }
            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            bool results;
            // Attempting to transfer file with wrong password to ftp server
            string password = "********";

            // Act
            try
            {
                results = await FTPService.SendAsync(ftpUrl, "", targetDirectory, userName, password);
            }
            catch
            {
                results = false;
            }
            // Assert
            Assert.IsFalse(results);
            
            // Cleanup
            //Directory.Delete(@"C:\_ArchiveFiles\TF2\", true);
        }

        [TestMethod]
        public async Task FTPService_Send_SendWithoutTargetFileAsync()
        {
 
            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            string password = Constants.FTPpassword;
            bool results;

            // Delete the file we want to send to remote to test SendWithoutTargetFile.

            // Act
            try
            {
                results = await FTPService.SendAsync(ftpUrl, "", "", userName, password);
            }
            catch
            {
                results = false;
            }

            // Assert
            Assert.IsFalse(results);
            
            // Cleanup
            //Directory.Delete(@"C:\_ArchiveFiles\TF3\", true);
        }

    }
}
