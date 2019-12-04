using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class FTPServiceTests
    {
        [TestMethod]
        public void FTPService_Send_SendValidCredentials()
        {
            //arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string ftpUrl = "ftp://*******/";
            string userName = "*****";
            string password = "*****";
            //act
            bool results = FTPService.Send(ftpUrl, "", targetDirectory, userName, password);
            //assert
            Assert.IsTrue(results);
        }

        [TestMethod]
        public void FTPService_Send_SendInvalidCredentials()
        {
            //arrange
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string ftpUrl = "ftp://******/";
            string userName = "******";
            // Attempting to transfer file with wrong password to ftp server
            string password = "********";
            //act

            bool results = FTPService.Send(ftpUrl, "", targetDirectory, userName, password);
            //assert
            Assert.IsFalse(results);
        }

        [TestMethod]
        public void FTPService_Send_SendWithoutTargetFile()
        {
            DateTime currentTime = DateTime.Now;
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");

            string ftpUrl = "ftp://********";
            string userName = "******";

            // Attempting to transfer file with wrong password to ftp server
            string password = "*******";
            string targetFilePath = targetDirectory + ".7z";

            // Delete the file we want to send to remote to test SendWithoutTargetFile.
            File.Delete(targetFilePath);

            //act
            bool results = FTPService.Send(ftpUrl, "", targetDirectory, userName, password);

            //assert
            Assert.IsFalse(results);
        }

    }
}
