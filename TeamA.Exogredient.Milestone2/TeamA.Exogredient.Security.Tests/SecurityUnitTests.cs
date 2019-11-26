using System;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Security.Tests
{
    [TestClass]
    public class SecurityUnitTests
    {
        [DataTestMethod]
        [DataRow(new byte[] { 104, 101, 108, 108, 111 }, "68656C6C6F")]
        public void BytesToHexString_GivenBytes_EqualsExpected(byte[] bytes, string expected)
        {
            //Arrange

            //Act
            string actual = SecurityService.BytesToHexString(bytes);

            //Assert
            Assert.IsTrue(actual.Equals(expected));
        }


        [DataTestMethod]
        [DataRow("password")]
        public void EncryptDecryptAES_GivenString_EqualsDecryptedOutput(string plainData)
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
        public void EncryptDecryptRSA_GivenBytes_EqualsDecryptedOutput(byte[] plainData)
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
        [DataRow("testing", "74657374696E67")]
        public void ToHexString_GivenString_EqualsExpected(string original, string expected)
        {
            //Arrange

            //Act
            string actual = SecurityService.ToHexString(original);

            //Assert
            Assert.IsTrue(expected.Equals(actual));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void HashPassword_HashSameStringTwoSeparateTimes_HashcodesMatch(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt(32);
            int iterations = 100;
            int hashBytesLength = 256;

            //Act
            string a = SecurityService.HashPassword(password, salt, iterations, hashBytesLength);
            string b = SecurityService.HashPassword(password, salt, iterations, hashBytesLength);

            //Assert
            Assert.IsTrue(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1", "string2")]
        public void HashPassword_HashTwoDifferentStrings_HashCodesNotMatch(string password1, string password2)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt(32);
            int iterations = 100;
            int hashBytesLength = 256;

            //Act
            string a = SecurityService.HashPassword(password1, salt, iterations, hashBytesLength);
            string b = SecurityService.HashPassword(password2, salt, iterations, hashBytesLength);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void HashPassword_HashSameStringTwoSeparateTimesWithDifferentSalt_HashCodesNotMatch(string password)
        {
            //Arrange
            byte[] salt1 = SecurityService.GenerateSalt(32);
            byte[] salt2 = SecurityService.GenerateSalt(32);
            int iterations = 100;
            int hashBytesLength = 256;

            //Act
            string a = SecurityService.HashPassword(password, salt1, iterations, hashBytesLength);
            string b = SecurityService.HashPassword(password, salt2, iterations, hashBytesLength);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void HashPassword_HashSameStringTwoSeparateTimesWithDifferentIterations_HashCodesNotMatch(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt(32);
            int iterations = 100;
            int hashBytesLength = 256;

            //Act
            string a = SecurityService.HashPassword(password, salt, iterations, hashBytesLength);
            string b = SecurityService.HashPassword(password, salt, iterations + 10, hashBytesLength);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("string1")]
        public void HashPassword_HashSameStringTwoSeparateTimesWithDifferentHashLength_HashCodesNotMatch(string password)
        {
            //Arrange
            byte[] salt = SecurityService.GenerateSalt(32);
            int iterations = 100;
            int hashBytesLength = 256;

            //Act
            string a = SecurityService.HashPassword(password, salt, iterations, hashBytesLength);
            string b = SecurityService.HashPassword(password, salt, iterations, hashBytesLength-128);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("A3D1FF2CB29F5FDC",new byte[] {163, 209, 255, 44, 178, 159, 95, 220})]
        public void HexStringToBytes_GivenString_MatchesExpected(string hexString, byte[] expected)
        {
            //Arrange

            //Act
            byte[] actual = SecurityService.HexStringToBytes(hexString);

            //Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}
