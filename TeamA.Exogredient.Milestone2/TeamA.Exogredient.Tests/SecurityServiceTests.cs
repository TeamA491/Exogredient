using System;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Security.Tests
{
    [TestClass]
    public class SecurityServiceTests
    { 
        [DataTestMethod]
        [DataRow(new byte[] { 104, 101, 108, 108, 111 }, "68656C6C6F")]
        public void SecurityService_BytesToHexString_GenerateCorrectHexString(byte[] bytes, string expected)
        {
            //Arrange

            //Act
            string actual = StringUtilityService.BytesToHexString(bytes);

            //Assert
            Assert.IsTrue(actual.Equals(expected));
        }


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
        [DataRow("testing", "74657374696E67")]
        public void SecurityService_ToHexString_GenerateCorrectHexString(string original, string expected)
        {
            //Arrange

            //Act
            string actual = StringUtilityService.ToHexString(original);

            //Assert
            Assert.IsTrue(expected.Equals(actual));
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
            string a = SecurityService.HashWithKDF(hexPassword, salt, hashBytesLength);
            string b = SecurityService.HashWithKDF(hexPassword, salt, hashBytesLength-16);

            //Assert
            Assert.IsFalse(a.Equals(b));
        }

        [DataTestMethod]
        [DataRow("A3D1FF2CB29F5FDC",new byte[] {163, 209, 255, 44, 178, 159, 95, 220})]
        public void SecurityService_HexStringToBytes_GenerateCorrectByteArray(string hexString, byte[] expected)
        {
            //Arrange

            //Act
            byte[] actual = StringUtilityService.HexStringToBytes(hexString);

            //Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}
