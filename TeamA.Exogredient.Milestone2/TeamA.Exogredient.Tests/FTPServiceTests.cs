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
        public async Task FTPService_Send_SendValidCredentials()
        {
            // Arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            string password = Constants.FTPpassword;
            
            // Act
            bool results = await FTPService.SendAsync(ftpUrl, "", targetDirectory, userName, password).ConfigureAwait(false);
            
            // Assert
            Assert.IsTrue(results);
        }

        [TestMethod]
        public async Task FTPService_Send_SendInvalidCredentials()
        {
            // Arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            bool results;
            // Attempting to transfer file with wrong password to ftp server
            string password = "********";

            // Act
            try
            {
                results = await FTPService.SendAsync(ftpUrl, "", targetDirectory, userName, password).ConfigureAwait(false);
            }
            catch(WebException e)
            {
                results = false;
            }
            // Assert
            Assert.IsFalse(results);
        }

        [TestMethod]
        public async Task FTPService_Send_SendWithoutTargetFile()
        {
            // Arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");

            string ftpUrl = Constants.FTPUrl;
            string userName = Constants.FTPUsername;
            bool results;
            
            // Attempting to transfer file with wrong password to ftp server
            string password = Constants.FTPpassword;
            string targetFilePath = targetDirectory + Constants.SevenZipFileExtension;

            // Delete the file we want to send to remote to test SendWithoutTargetFile.
            File.Delete(targetFilePath);

            // Act
            try
            {
                results = await FTPService.SendAsync(ftpUrl, "", targetDirectory, userName, password).ConfigureAwait(false);
            }
            catch(ArgumentException e)
            {
                results = false;
            }

            // Assert
            Assert.IsFalse(results);
        }

    }
}
