using System;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class SecurityServiceTests
    {
        private static byte[] _originalPlainData;
        private static byte[] _alteredPlainData;
        private static byte[] _setOnePublicKey;
        private static byte[] _setOnePrivateKey;
        private static byte[] _setOneEncryption;
        private static byte[] _setTwoPublicKey;
        private static byte[] _setTwoPrivateKey;
        private static byte[] _setTwoEncryption;


        public SecurityServiceTests()
        {
            //TEST DATA
            _originalPlainData = new byte[] { 90, 127, 65, 9, 255, 0, 1, 23, 44, 77, 200, 163 };

            _alteredPlainData = new byte[] { 1, 23, 44, 77, 200, 163, 90, 127, 65, 9, 255, 0 };

            _setOnePublicKey = new byte[]
            { 6, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 49, 0, 4, 0, 0, 1, 0, 1, 0, 145, 157, 38, 138, 61, 86, 76, 58,
                73, 38, 93, 201, 112, 232, 133, 173, 39, 107, 119, 140, 86, 188, 197, 226, 5, 118, 71, 5, 151, 166,
                104, 148, 175, 70, 119, 199, 143, 56, 65, 84, 29, 27, 59, 178, 228, 41, 53, 202, 83, 232, 99, 149, 11,
                82, 29, 245, 178, 166, 189, 106, 42, 224, 201, 42, 117, 62, 220, 8, 186, 48, 82, 45, 160, 185, 202,
                213, 199, 80, 50, 135, 26, 77, 144, 95, 200, 234, 75, 216, 138, 132, 28, 20, 199, 176, 122, 250, 214,
                121, 102, 76, 233, 37, 8, 149, 181, 26, 185, 207, 245, 24, 148, 49, 128, 47, 100, 195, 139, 169, 108,
                13, 88, 213, 119, 85, 177, 92, 130, 179 };

            _setOnePrivateKey = new byte[]
            { 7, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 50, 0, 4, 0, 0, 1, 0, 1, 0, 145, 157, 38, 138, 61, 86, 76, 58, 73, 38,
                93, 201, 112, 232, 133, 173, 39, 107, 119, 140, 86, 188, 197, 226, 5, 118, 71, 5, 151, 166, 104, 148, 175,
                70, 119, 199, 143, 56, 65, 84, 29, 27, 59, 178, 228, 41, 53, 202, 83, 232, 99, 149, 11, 82, 29, 245, 178,
                166, 189, 106, 42, 224, 201, 42, 117, 62, 220, 8, 186, 48, 82, 45, 160, 185, 202, 213, 199, 80, 50, 135, 26,
                77, 144, 95, 200, 234, 75, 216, 138, 132, 28, 20, 199, 176, 122, 250, 214, 121, 102, 76, 233, 37, 8, 149,
                181, 26, 185, 207, 245, 24, 148, 49, 128, 47, 100, 195, 139, 169, 108, 13, 88, 213, 119, 85, 177, 92, 130,
                179, 135, 199, 253, 254, 94, 9, 238, 38, 144, 0, 147, 97, 230, 181, 152, 229, 108, 87, 134, 119, 217, 108,
                254, 134, 1, 250, 228, 32, 204, 114, 206, 167, 171, 204, 193, 8, 165, 63, 115, 23, 243, 199, 169, 157, 216,
                20, 97, 145, 59, 119, 105, 142, 77, 30, 247, 120, 203, 63, 9, 177, 209, 0, 242, 219, 39, 8, 247, 192, 84, 4,
                236, 201, 115, 108, 35, 112, 40, 129, 47, 92, 241, 156, 130, 125, 93, 119, 103, 225, 64, 86, 213, 127, 65,
                117, 204, 91, 43, 65, 156, 3, 193, 42, 71, 85, 23, 135, 73, 13, 226, 9, 174, 73, 88, 206, 139, 251, 48, 51,
                121, 3, 116, 159, 51, 159, 170, 117, 239, 208, 55, 228, 213, 58, 182, 214, 140, 6, 39, 12, 74, 78, 104, 131,
                153, 184, 222, 44, 6, 235, 99, 10, 160, 227, 216, 118, 179, 78, 219, 204, 184, 4, 156, 98, 140, 15, 95, 91,
                154, 142, 45, 238, 239, 125, 76, 86, 189, 153, 181, 87, 193, 151, 179, 58, 130, 102, 249, 162, 199, 185, 198,
                27, 68, 9, 111, 37, 5, 109, 24, 167, 145, 90, 88, 11, 64, 98, 71, 26, 245, 254, 73, 140, 176, 182, 247, 87,
                80, 119, 94, 145, 39, 98, 251, 54, 220, 118, 174, 231, 132, 155, 238, 228, 1, 71, 253, 96, 94, 13, 195, 86,
                104, 32, 164, 20, 53, 165, 205, 177, 114, 56, 202, 154, 102, 62, 124, 33, 11, 81, 76, 195, 221, 90, 185, 7,
                56, 226, 190, 225, 84, 19, 175, 145, 189, 41, 218, 188, 219, 247, 11, 203, 59, 73, 188, 125, 229, 76, 198,
                12, 69, 101, 131, 60, 244, 80, 130, 226, 110, 54, 54, 7, 79, 48, 30, 72, 191, 132, 157, 147, 28, 85, 194,
                227, 1, 63, 169, 154, 143, 244, 157, 124, 232, 5, 169, 127, 145, 90, 113, 187, 108, 24, 201, 56, 150, 57,
                68, 123, 124, 53, 17, 247, 212, 95, 9, 227, 10, 197, 195, 77, 67, 68, 125, 137, 112, 181, 144, 13, 251, 222,
                165, 140, 89, 239, 109, 19, 181, 4, 29, 188, 246, 129, 212, 60, 233, 99, 3, 133, 180, 234, 231, 129, 115,
                199, 20, 206, 232, 192, 235, 33, 138, 232, 248, 194, 165, 96, 128, 191, 10, 129, 128, 254, 53, 51, 88, 219,
                130, 54, 206, 50, 134, 235, 205, 205, 23, 236, 85, 75, 182, 124, 68, 111, 242, 19, 139, 148, 219, 4, 229,
                122, 163, 172, 18, 16, 129, 190, 146, 198, 18, 26, 27, 139, 119, 116, 8, 254, 93, 201, 208, 124, 133, 7 };

            _setOneEncryption = new byte[]
            { 11, 40, 85, 212, 64, 54, 108, 122, 105, 126, 17, 7, 147, 137, 79, 147, 173, 2, 227, 241, 115, 142, 58, 117,
                162, 166, 63, 66, 15, 106, 158, 253, 106, 15, 170, 206, 177, 94, 96, 222, 186, 129, 127, 70, 154, 25, 149,
                217, 167, 68, 86, 3, 151, 232, 54, 35, 32, 209, 19, 186, 155, 94, 180, 45, 36, 205, 206, 118, 77, 105, 240,
                65, 118, 237, 162, 143, 130, 90, 107, 22, 179, 124, 83, 113, 225, 173, 204, 86, 210, 212, 190, 199, 198, 106,
                63, 156, 145, 138, 220, 17, 179, 244, 35, 30, 14, 198, 36, 77, 20, 0, 97, 24, 215, 98, 192, 217, 95, 100, 41,
                150, 187, 128, 82, 229, 24, 0, 241, 181 };


            _setTwoPublicKey = new byte[]
            { 6, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 49, 0, 4, 0, 0, 1, 0, 1, 0, 93, 2, 100, 213, 89, 184, 103, 19, 208,
                69, 130, 160, 222, 82, 214, 137, 75, 213, 150, 89, 23, 148, 49, 73, 249, 137, 24, 126, 200, 182, 24, 8,
                192, 71, 81, 162, 23, 31, 219, 164, 169, 78, 112, 204, 234, 230, 133, 13, 172, 151, 90, 202, 211, 23, 17,
                119, 80, 248, 187, 246, 166, 55, 210, 48, 52, 194, 233, 50, 37, 51, 216, 240, 81, 226, 253, 190, 6, 2, 238,
                155, 141, 107, 63, 138, 11, 194, 15, 249, 178, 16, 12, 61, 239, 95, 67, 208, 76, 77, 222, 117, 24, 171, 121,
                224, 170, 138, 113, 20, 26, 24, 46, 232, 233, 183, 169, 244, 222, 140, 253, 77, 143, 234, 157, 31, 118, 207,
                7, 200 };

            _setTwoPrivateKey = new byte[]
            { 7, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 50, 0, 4, 0, 0, 1, 0, 1, 0, 93, 2, 100, 213, 89, 184, 103, 19, 208, 69,
                130, 160, 222, 82, 214, 137, 75, 213, 150, 89, 23, 148, 49, 73, 249, 137, 24, 126, 200, 182, 24, 8, 192, 71,
                81, 162, 23, 31, 219, 164, 169, 78, 112, 204, 234, 230, 133, 13, 172, 151, 90, 202, 211, 23, 17, 119, 80, 248,
                187, 246, 166, 55, 210, 48, 52, 194, 233, 50, 37, 51, 216, 240, 81, 226, 253, 190, 6, 2, 238, 155, 141, 107,
                63, 138, 11, 194, 15, 249, 178, 16, 12, 61, 239, 95, 67, 208, 76, 77, 222, 117, 24, 171, 121, 224, 170, 138,
                113, 20, 26, 24, 46, 232, 233, 183, 169, 244, 222, 140, 253, 77, 143, 234, 157, 31, 118, 207, 7, 200, 195,
                151, 177, 84, 87, 156, 138, 127, 237, 196, 99, 225, 132, 54, 235, 24, 172, 115, 69, 118, 166, 151, 232, 185,
                112, 66, 145, 74, 244, 239, 238, 199, 232, 214, 248, 100, 169, 119, 167, 252, 247, 212, 94, 171, 113, 235,
                230, 106, 169, 207, 133, 76, 214, 109, 9, 35, 118, 52, 103, 206, 46, 215, 163, 251, 95, 123, 6, 23, 124, 217,
                216, 109, 111, 214, 27, 153, 40, 29, 212, 184, 48, 67, 235, 86, 5, 98, 95, 222, 162, 18, 65, 202, 111, 191,
                208, 14, 71, 25, 37, 137, 36, 37, 112, 101, 12, 155, 98, 10, 124, 217, 97, 96, 102, 105, 114, 142, 55, 187,
                215, 102, 200, 159, 81, 89, 200, 13, 127, 203, 191, 104, 179, 251, 111, 56, 143, 127, 223, 98, 213, 50, 233,
                94, 173, 28, 116, 187, 64, 139, 104, 250, 145, 10, 251, 212, 215, 14, 155, 160, 177, 10, 215, 30, 29, 129,
                86, 60, 57, 44, 179, 252, 74, 38, 9, 88, 38, 46, 226, 55, 137, 203, 151, 238, 40, 252, 1, 158, 7, 103, 240,
                3, 27, 8, 239, 215, 15, 79, 195, 228, 123, 84, 167, 60, 142, 141, 84, 160, 2, 208, 152, 95, 200, 127, 145,
                254, 76, 83, 116, 184, 219, 165, 60, 233, 16, 159, 98, 54, 198, 234, 55, 197, 91, 220, 25, 137, 78, 151, 142,
                152, 49, 113, 184, 4, 177, 115, 49, 152, 88, 184, 105, 123, 177, 233, 211, 195, 196, 63, 55, 65, 80, 255, 2,
                17, 23, 210, 26, 161, 118, 179, 233, 120, 114, 125, 106, 58, 43, 131, 208, 118, 183, 65, 201, 212, 68, 173,
                120, 244, 197, 228, 6, 236, 182, 20, 122, 7, 210, 87, 18, 72, 162, 129, 143, 139, 235, 249, 67, 252, 226, 69,
                62, 21, 20, 190, 71, 16, 81, 181, 140, 143, 101, 216, 133, 178, 102, 156, 221, 167, 188, 164, 255, 87, 13, 46,
                208, 44, 164, 203, 95, 213, 104, 14, 172, 75, 157, 173, 97, 32, 117, 175, 159, 20, 176, 155, 194, 37, 85, 97,
                31, 25, 50, 229, 116, 133, 17, 218, 74, 185, 81, 227, 12, 166, 185, 183, 202, 136, 163, 44, 67, 168, 246, 127,
                131, 216, 120, 28, 225, 200, 194, 225, 173, 180, 220, 140, 145, 143, 223, 25, 104, 76, 134, 22, 27, 149, 106,
                92, 254, 158, 204, 34, 208, 225, 3, 16, 49, 104, 233, 143, 223, 210, 190, 177, 2, 186, 211, 152, 90, 161, 60,
                201, 59, 79, 48, 182, 238, 81, 244, 141, 24, 33, 197, 63, 14, 48, 197, 246, 134, 35, 83, 97 };

            _setTwoEncryption = new byte[]
            { 187, 118, 79, 235, 73, 244, 244, 239, 97, 163, 30, 43, 58, 99, 205, 165, 52, 164, 53, 154, 212, 16, 183, 15,
                40, 112, 247, 71, 28, 51, 209, 205, 215, 194, 221, 142, 161, 29, 208, 36, 53, 83, 229, 81, 232, 140, 75, 72,
                167, 61, 29, 137, 101, 49, 143, 121, 231, 143, 34, 21, 204, 207, 30, 165, 54, 68, 119, 220, 225, 51, 238, 28,
                150, 13, 163, 5, 131, 213, 58, 147, 167, 135, 224, 42, 3, 166, 42, 194, 187, 10, 75, 96, 62, 124, 230, 33,
                240, 192, 188, 250, 139, 161, 26, 152, 64, 30, 74, 78, 217, 179, 169, 107, 129, 4, 101, 33, 82, 122, 47, 139,
                221, 64, 173, 217, 34, 14, 233, 75 };

        }

        // With the given plaindata, key, and IV, AES encryption generates the expected data.
        [DataTestMethod]
        [DataRow(
            "password",
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 },
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 })]
        public void SecurityService_EncryptAES_CorrectEncryption(string plainData, byte[] key, byte[] IV, byte[] expected)
        {
            //Arrange

            //Act

            // Encrypt the plain data.
            byte[] actual = SecurityService.EncryptAES(plainData, key, IV);

            //Assert

            // The encrypted data should match the expected data.
            Assert.IsTrue(actual.SequenceEqual(expected));
        }

        // AES encryption should generate different encrypted data for every different plaindata.
        [DataTestMethod]
        [DataRow(
            "differentPassword",
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 },
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 })]
        public void SecurityService_EncryptAES_DifferentPlainDataGeneratesDifferentEncryption(string plainData, byte[] key, byte[] IV, byte[] notExpected)
        {
            //Arrange

            //Act

            // Encrypt the plain data.
            byte[] actual = SecurityService.EncryptAES(plainData, key, IV);

            //Assert

            // The encrypted data should not match the expected data.
            Assert.IsFalse(actual.SequenceEqual(notExpected));
        }

        // AES encryption should generate different encrypted data for every different key.
        [DataTestMethod]
        [DataRow(
            "password",
            new byte[]{ 239, 103, 9, 21, 200, 33, 61, 191, 47, 40, 129, 107, 170, 58, 148,
                205, 17, 228, 223, 6, 225, 79, 126, 238, 88, 25, 188, 101, 171, 138, 205, 47 },
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 },
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 })]
        public void SecurityService_EncryptAES_DifferentKeyGeneratesDifferentEncryption(string plainData, byte[] key, byte[] IV, byte[] notExpected)
        {
            //Arrange

            //Act

            // Encrypt the plain data.
            byte[] actual = SecurityService.EncryptAES(plainData, key, IV);

            //Assert

            // The encrypted data should not match the expected data.
            Assert.IsFalse(actual.SequenceEqual(notExpected));
        }

        // AES encryption should generate different encrypted data for every different IV.
        [DataTestMethod]
        [DataRow(
            "password",
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 202, 121, 87, 20, 65, 206, 214, 185, 226, 177, 46, 139, 100, 136, 35, 144 },
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 })]
        public void SecurityService_EncryptAES_DifferentIVGeneratesDifferentEncryption(string plainData, byte[] key, byte[] IV, byte[] notExpected)
        {
            //Arrange

            //Act

            // Encrypt the plain data.
            byte[] actual = SecurityService.EncryptAES(plainData, key, IV);

            //Assert

            // The encrypted data should not match the expected data.
            Assert.IsFalse(actual.SequenceEqual(notExpected));
        }

        // Given encrypted data with correct key and IV, AES decryption should generate a correct plaindata
        [DataTestMethod]
        [DataRow(
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 },
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 },
            "password")]
        public void SecurityService_DecryptAES_CorrectDecryption(byte[] encryptedData, byte[] key, byte[] IV, string expected)
        {
            //Arrange

            //Act

            // Decrypt the encrypted data.
            string actual = SecurityService.DecryptAES(encryptedData, key, IV);

            //Assert

            // The decrypted data should match the expected data.
            Assert.IsTrue(actual.Equals(expected));
        }

        // AES decryption should fail if the given encrypted data was not encrypted by the given key.
        [DataTestMethod]
        [DataRow(
            new byte[] { 92, 171, 229, 37, 94, 183, 196, 218, 80, 202, 173, 102, 177, 201, 255, 141 },
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 })]
        public void SecurityService_DecryptAES_DifferentEncryptedDataFails(byte[] encryptedData, byte[] key, byte[] IV)
        {
            //Arrange

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Decrypt the encrypted data.
                string actual = SecurityService.DecryptAES(encryptedData, key, IV);
            }
            catch (CryptographicException)
            {
                // Catch cryptographic exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // AES decryption should fail if the given key was not used to encrypt the encrypted data.
        [DataTestMethod]
        [DataRow(
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 },
            new byte[]{ 239, 103, 9, 21, 200, 33, 61, 191, 47, 40, 129, 107, 170, 58, 148, 205,
                17, 228, 223, 6, 225, 79, 126, 238, 88, 25, 188, 101, 171, 138, 205, 47 },
            new byte[] { 187, 88, 193, 218, 105, 87, 248, 115, 96, 55, 105, 16, 184, 81, 116, 70 })]
        public void SecurityService_DecryptAES_DifferentKeyFails(byte[] encryptedData, byte[] key, byte[] IV)
        {
            //Arrange

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Decrypt the encrypted data.
                string actual = SecurityService.DecryptAES(encryptedData, key, IV);
            }
            catch (CryptographicException)
            {
                // Catch cryptographic exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // AES decryption should fail if the given IV was not used to encrypt the encrypted data.
        [DataTestMethod]
        [DataRow(
            new byte[] { 80, 202, 173, 102, 177, 201, 255, 141, 92, 171, 229, 37, 94, 183, 196, 218 },
            new byte[]{141,67,25,148,62,32,159,28,4,216,162,18,58,33,173,
            122,40,30,74,59,161,105,223,21,65,227,169,169,36,245,52,225},
            new byte[] { 202, 121, 87, 20, 65, 206, 214, 185, 226, 177, 46, 139, 100, 136, 35, 144 })]
        public void SecurityService_DecryptAES_DifferentIVFails(byte[] encryptedData, byte[] key, byte[] IV)
        {
            //Arrange

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Decrypt the encrypted data.
                string actual = SecurityService.DecryptAES(encryptedData, key, IV);
            }
            catch (CryptographicException)
            {
                // Catch cryptographic exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // Given the encrypted data and private key used for the encryption,
        // RSA decryption should generate the correct plaindata using the correct public key.
        [DataTestMethod]
        public void SecurityService_DecryptRSA_CorrectDecryption()
        {
            //Arrange

            //Act

            // Decrypt the encrypted data.
            byte[] decryptedData = SecurityService.DecryptRSA(_setOneEncryption, _setOnePrivateKey);

            //Assert

            // The decrypted data should match the original plain data.
            Assert.IsTrue(_originalPlainData.SequenceEqual(decryptedData));
        }

        // RSA decryption should fail if the encrypted data was not encrypted with the correct public key.
        [DataTestMethod]
        public void SecurityService_DecryptRSA_DifferentEncryptedDataFails()
        {
            //Arrange

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Decrypt the encrypted data.
                byte[] decryptedData = SecurityService.DecryptRSA(_setTwoEncryption, _setOnePrivateKey);
            }
            catch (CryptographicException)
            {
                // Catch cryptographic exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // RSA decryption should fail if the private key used for decryption is not correct.
        [DataTestMethod]
        public void SecurityService_DecryptRSA_DifferentPrivateKeyFails()
        {
            //Arrange

            // Set the result to false by default.
            bool result = false;

            //Act
            try
            {
                // Decrypt the encrypted data.
                byte[] decryptedData = SecurityService.DecryptRSA(_setOneEncryption, _setTwoPrivateKey);
            }
            catch (CryptographicException)
            {
                // Catch cryptographic exception and set the result to true.
                result = true;
            }

            //Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        // Given plain data and salt, KDF hash should generate an expected hashcode.
        [DataTestMethod]
        [DataRow("string1", new byte[] { 181, 34, 206, 117, 63, 242, 81, 99 },
            "94B60BB77542F645A01FC705B952F324642DCD4A15BEF982B775A146B0E97587")]
        public void SecurityService_HashWithKDF_CorrectHash(string password, byte[] salt, string expected)
        {
            //Arrange

            // Convert the password to hex string.
            string hexPassword = UtilityService.ToHexString(password);

            //Act

            // Hash the hex string with the salt.
            string actual = SecurityService.HashWithKDF(hexPassword, salt);

            //Assert

            // Hashed password should match the expected hash code. 
            Assert.IsTrue(actual.Equals(expected));
        }

        // KDF hash should generate the same hashcode for every hashing of the same plain data, salt, hashcode length, and iterations.
        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_SameInputsGenerateSameHash(string password)
        {
            //Arrange

            // Generate a salt.
            byte[] salt = SecurityService.GenerateSalt();
            // Convert the password to hex string.
            string hexPassword = UtilityService.ToHexString(password);

            //Act

            // Hash the same hex string with the same salt separately.
            string a = SecurityService.HashWithKDF(hexPassword, salt);
            string b = SecurityService.HashWithKDF(hexPassword, salt);

            //Assert

            // Both hashed passwords should match.
            Assert.IsTrue(a.Equals(b));
        }

        // KDF hash should generate different hashcodes for two different plain data with same salt, hashcode length, and iterations.
        [DataTestMethod]
        [DataRow("string1", "string2")]
        public void SecurityService_HashWithKDF_DifferentPasswordsGenerateDifferentHashes(string password1, string password2)
        {
            //Arrange

            // Generate a salt.
            byte[] salt = SecurityService.GenerateSalt();
            // Convert the password1 to hex string.
            string hexPassword1 = UtilityService.ToHexString(password1);
            // Convert the password2 to hex string.
            string hexPassword2 = UtilityService.ToHexString(password2);

            //Act

            // Hash the password1 hex string and password2 hex string with the same salt.
            string a = SecurityService.HashWithKDF(hexPassword1, salt);
            string b = SecurityService.HashWithKDF(hexPassword2, salt);

            //Assert

            // Both hashed passwords should not match.
            Assert.IsFalse(a.Equals(b));
        }

        // KDF hash should generate different hashcodes for two different salts with same plain data, hashcode length, and iterations.
        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_DifferentSaltsGenerateDifferentHashes(string password)
        {
            //Arrange

            // Generate a salt1.
            byte[] salt1 = SecurityService.GenerateSalt();
            // Generate a salt2.
            byte[] salt2 = SecurityService.GenerateSalt();
            // Convert the password to hex string.
            string hexPassword = UtilityService.ToHexString(password);

            //Act

            // Hash the same hex string password with different salts.
            string a = SecurityService.HashWithKDF(hexPassword, salt1);
            string b = SecurityService.HashWithKDF(hexPassword, salt2);

            //Assert

            // Both hashed passwords should not match.
            Assert.IsFalse(a.Equals(b));
        }

        // KDF hash should generate different hashcodes for two different hash lengths with same plain data, hashcode length, and iterations.
        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_DifferentHashLengthsGenerateDifferentHashes(string password)
        {
            //Arrange

            // Generate a salt.
            byte[] salt = SecurityService.GenerateSalt();
            // Byte length of the hashcode.
            int hashBytesLength = 32;
            // Convert the password to hex string.
            string hexPassword = UtilityService.ToHexString(password);

            //Act

            // Hash the same hex string password with same salt but different hashcode lengths.
            string a = SecurityService.HashWithKDF(hexPassword, salt, hashLength: hashBytesLength);
            string b = SecurityService.HashWithKDF(hexPassword, salt, hashLength: hashBytesLength - 16);

            //Assert

            // Both hashed passwords should not match.
            Assert.IsFalse(a.Equals(b));
        }

        // KDF hash should generate different hashcodes for two different iterations with same plain data, hashcode length, and iterations.
        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithKDF_DifferentIterationsGenerateDifferentHashes(string password)
        {
            //Arrange

            // Generate a salt.
            byte[] salt = SecurityService.GenerateSalt();
            // Number of iterations.
            int iterations = 10000;
            // Convert the password to hex string.
            string hexPassword = UtilityService.ToHexString(password);

            //Act

            // Hash the same hex string password with same salt but different iterations.
            string a = SecurityService.HashWithKDF(hexPassword, salt);
            string b = SecurityService.HashWithKDF(hexPassword, salt, iterations: iterations - 100);

            //Assert

            // Both hashed passwords should not match.
            Assert.IsFalse(a.Equals(b));
        }

        // KDF hash should generate same hashcode length for two different plain data lengths.
        [DataTestMethod]
        [DataRow("short", "looooooooooooooooooooooooooooooooooooooooong")]
        public void SecurityService_HashWithKDF_DifferentPasswordLengthsGenerateSameHashLength(string password1, string password2)
        {
            //Arrange

            // Generate a salt.
            byte[] salt = SecurityService.GenerateSalt();
            // Convert the password1 to hex string.
            string hexPassword1 = UtilityService.ToHexString(password1);
            // Convert the password2 to hex string.
            string hexPassword2 = UtilityService.ToHexString(password2);

            //Act

            //Hash the password1 and password2 with same salt.
            string a = SecurityService.HashWithKDF(hexPassword1, salt);
            string b = SecurityService.HashWithKDF(hexPassword2, salt);

            //Assert

            // Both hashed passwords should have same length. 
            Assert.IsTrue(a.Length == b.Length);
        }

        // Given a plain data, SHA1 should generate an expected hashcode.
        [DataTestMethod]
        [DataRow("test", "A94A8FE5CCB19BA61C4C0873D391E987982FBBD3")]
        public void SecurityService_HashWithSHA1_AcutalHashMatchesExpectedHash(string data, string expected)
        {
            //Arrange

            //Act

            // Hash the plain data.
            string actual = SecurityService.HashWithSHA1(data);

            //Assert

            // Hashed data should match the expected data.
            Assert.IsTrue(expected.Equals(actual));

        }

        // SHA1 should generate the same hashcode for every hashing of the same plain data.
        [DataTestMethod]
        [DataRow("string1")]
        public void SecurityService_HashWithSHA1_SameStringGeneratesSameHash(string string1)
        {
            //Arrange

            //Act

            // Hash the same plain data separately.
            string a = SecurityService.HashWithSHA1(string1);
            string b = SecurityService.HashWithSHA1(string1);

            //Assert

            // Both hashed data should match.
            Assert.IsTrue(a.Equals(b));
        }

        // SHA1 should generate different hashcodes for two different plain data.
        [DataTestMethod]
        [DataRow("string1", "string2")]
        public void SecurityService_HashWithSHA1_DifferentStringsGenerateDifferentHashes(string string1, string string2)
        {
            //Arrange

            //Act

            // Hash two different data. 
            string a = SecurityService.HashWithSHA1(string1);
            string b = SecurityService.HashWithSHA1(string2);

            //Assert

            // Both hashed data should not match.
            Assert.IsFalse(a.Equals(b));

        }

        // SHA1 should generate same hashcode length for two different plain data length.
        [DataTestMethod]
        [DataRow("short", "loooooooooooooooooooooooooooong")]
        public void SecurityService_HashWithSHA1_DifferentStringLengthsGenerateSameHashLength(string string1, string string2)
        {
            //Arrange

            //Act

            // Hash two different data.
            string a = SecurityService.HashWithSHA1(string1);
            string b = SecurityService.HashWithSHA1(string2);

            //Assert

            // Both hashed data should have same length.
            Assert.IsTrue(a.Length == b.Length);

        }
    }
}
