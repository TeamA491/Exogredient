using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class SecurityServiceTests
    { 
        [DataTestMethod]
        [DataRow("password")]
        public void SecurityService_EncryptAESDecryptAES_RevertBackToOriginalData(string plainData)
        {
            //Arrange
            byte[] key = SecurityService.GenerateAESKey();
            byte[] IV = SecurityService.GenerateAESIV();

            //Act
            byte[] encryptedData = SecurityService.EncryptAES(plainData, key, IV);
            string decryptedData = SecurityService.DecryptAES(encryptedData, key, IV);

            //Assert
            Assert.IsTrue(plainData.Equals(decryptedData));
        }

        [DataTestMethod]
        [DataRow(new byte[] {90,127,65,9,255,0,1,23,44,77,200,163})]
        public void SecurityService_EncryptRSADecryptRSA_RevertBackToOriginalData(byte[] plainData)
        {
            //Arrange
            byte[] publicKey = SecurityService.GetRSAPublicKey();
            byte[] privateKey = SecurityService.GetRSAPrivateKey();

            //Act
            byte[] encryptedData = SecurityService.EncryptRSA(plainData, publicKey);
            byte[] decryptedData = SecurityService.DecryptRSA(encryptedData, privateKey);

            //Assert
            Assert.IsTrue(plainData.SequenceEqual(decryptedData));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_GenerateSameHashWithSameInputs(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt();
            string hexPassword = StringUtilityService.ToHexString(password);

            //Act
            string a = SecurityService.HashWithKDF(hexPassword, salt);
            string b = SecurityService.HashWithKDF(hexPassword, salt);
            Console.WriteLine(a);
            Console.WriteLine(b);

            //Assert
            Assert.IsTrue(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1", "string2")]
        public void SecurityService_HashWithKDF_GenerateDifferentHashesWithDifferentPasswords(string password1, string password2)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt();
            string hexPassword1 = StringUtilityService.ToHexString(password1);
            string hexPassword2 = StringUtilityService.ToHexString(password2);

            //Act
            string a = SecurityService.HashWithKDF(hexPassword1, salt);
            string b = SecurityService.HashWithKDF(hexPassword2, salt);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_GenerateDifferentHashesWithDifferentSalts(string password)
        {
            //Arrange
            byte[] salt1 = SecurityService.GenerateSalt();
            byte[] salt2 = SecurityService.GenerateSalt();
            string hexPassword = StringUtilityService.ToHexString(password);

            //Act
            string a = SecurityService.HashWithKDF(hexPassword, salt1);
            string b = SecurityService.HashWithKDF(hexPassword, salt2);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_GenerateDifferentHashesWithDifferentHashLengths(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt();
            int hashBytesLength = 32;
            string hexPassword = StringUtilityService.ToHexString(password);

            //Act
            string a = SecurityService.HashWithKDF(hexPassword, salt, hashLength:hashBytesLength);
            string b = SecurityService.HashWithKDF(hexPassword, salt, hashLength:hashBytesLength-16);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_GenerateDifferentHashesWithDifferentIterations(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt();
            int iterations = 10000;
            string hexPassword = StringUtilityService.ToHexString(password);

            //Act
            string a = SecurityService.HashWithKDF(hexPassword, salt);
            string b = SecurityService.HashWithKDF(hexPassword, salt, iterations:iterations-100);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("test", "A94A8FE5CCB19BA61C4C0873D391E987982FBBD3")]
        public void SecurityService_HashWithSHA1_AcutalHashMatchesExpectedHash(string data, string expected)
        {
            //Arrange

            //Act
            string actual = SecurityService.HashWithSHA1(data);

            //Assert
            Assert.IsTrue(expected.Equals(actual));

        }

        [DataTestMethod]
        [DataRow("test", "9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08")]
        public void SecurityService_HashWithHMACSHA256_ActualHashMatchesExpectedHash(string data, string expected)
        {
            //Arrange

            //Act
            string actual = SecurityService.HashWithHMACSHA256(data);

            //Assert
            Assert.IsTrue(actual.Equals(expected));
        }

        [DataTestMethod]
        [DataRow("test")]
        public void SecurityService_HashWithHMACSHA256_SameStringGeneratesSameHash(string data)
        {
            //Arrange

            //Act
            string string1 = SecurityService.HashWithHMACSHA256(data);
            string string2 = SecurityService.HashWithHMACSHA256(data);

            //Assert
            Assert.IsTrue(string1.Equals(string2));
        }

        [DataTestMethod]
        [DataRow("test1", "test2")]
        public void SecurityService_HashWithHMACSHA256_DifferentStringsGenerateDifferentHashes(string data1, string data2)
        {
            //Arrange

            //Act
            string string1 = SecurityService.HashWithHMACSHA256(data1);
            string string2 = SecurityService.HashWithHMACSHA256(data2);

            //Assert
            Assert.IsFalse(string1.Equals(string2));
        }
    }
}
