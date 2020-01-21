using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using System.IO;
using System;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class CompressionServiceTests
    {
        [TestMethod]
        public void CompressionService_Compression_CompressValidSourceDirectory()
        {
            // arrange
            DateTime currentTime = DateTime.Now;
            string sourceDirectory = @"C:\_LogFiles\TF1";
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";

            // act
            bool result = CompressionService.Compress(sevenZipPath,sourceDirectory,targetDirectory);

            // assert 
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CompressionService_Compress_CompressInvalidSourceDirectory()
        {
            // arrange
            DateTime currentTime = DateTime.Now;
            string sourceDirectory = @"C:\_LogFiles\adfadfasdf";
            string targetDirectory = @"C:\_ArchiveFiles\" + currentTime.ToString("ddMMyy");
            string sevenZipPath = @"C:\Program Files\7-Zip\7z.exe";
            bool result;

            // act
            try
            {
                result = CompressionService.Compress(sevenZipPath, sourceDirectory, targetDirectory);
            }
            catch
            {
                result = false;
            }
                

            // assert 
            Assert.IsFalse(result);
        }


    }
}
