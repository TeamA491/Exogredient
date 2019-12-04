using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class StringUtilityServiceTests
    {
        [DataTestMethod]
        [DataRow("A3D1FF2CB29F5FDC", new byte[] { 163, 209, 255, 44, 178, 159, 95, 220 })]
        public void SecurityService_HexStringToBytes_GenerateCorrectByteArray(string hexString, byte[] expected)
        {
            // Act
            byte[] actual = StringUtilityService.HexStringToBytes(hexString);

            // Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [DataTestMethod]
        [DataRow(new byte[] { 104, 101, 108, 108, 111 }, "68656C6C6F")]
        public void StringUtilityService_BytesToHexString_GenerateCorrectHexString(byte[] bytes, string expected)
        {
            // Act
            string actual = StringUtilityService.BytesToHexString(bytes);

            // Assert
            Assert.IsTrue(actual.Equals(expected));
        }

        [DataTestMethod]
        [DataRow("testing", "74657374696E67")]
        public void StringUtilityService_ToHexString_GenerateCorrectHexString(string original, string expected)
        {
            // Act
            string actual = StringUtilityService.ToHexString(original);

            // Assert
            Assert.IsTrue(expected.Equals(actual));
        }

        [DataTestMethod]
        [DataRow("Jason", 5, -1)]
        [DataRow("David", 2000, 1)]
        public void StringUtilityService_CheckLength_WithinLengthSuccess(string name, int length, int min)
        {
            // if min = -1 we are not checking withing a range/ the string has to be equal to length.

            // Act
            bool result = StringUtilityService.CheckLength(name, length, min);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("Jason", 4, -1)]
        [DataRow("David", 3, 1)]
        [DataRow("aBc", 7, 4)]
        public void StringUtilityService_CheckLength_NotWithinLengthFail(string name, int length, int min)
        {
            // if min = -1 we are not checking withing a range/ the string has to be equal to length.

            // Act
            bool result = StringUtilityService.CheckLength(name, length, min);

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("asdsa12312#!@#")]
        [DataRow("...fas313ads[];'{}312")]
        public void StringUtilityService_CheckIfANSCharacters_OnlyANSCharactersSuccess(string name)
        {
            // Act
            bool result = StringUtilityService.CheckIfANSCharacters(name);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("123123123")]
        public void StringUtilityService_CheckIfNumericCharacters_OnlyNumericCharactersSuccess(string name)
        {
            // Act
            bool result = StringUtilityService.CheckIfNumericCharacters(name);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A")]
        [DataRow("lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com")]
        public void StringUtilityService_EmailFormatValidityCheck_ValidEmailFormatSuccess(string email)
        {
            // Act
            bool result = StringUtilityService.EmailFormatValidityCheck(email);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A")]
        [DataRow("lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com")]
        [DataRow("LoveThisClass123+lmaojk@gmail.com")]
        public void StringUtilityService_CanonicalizingEmail_CanonicalizingEmailSuccess(string email)
        {

            // Act
            string canonicalizedEmail = StringUtilityService.CanonicalizingEmail(email);


            // Check if act matches with expected value.
            bool result = false;

            if (canonicalizedEmail.Equals("a@a") || canonicalizedEmail.Equals("lmao123sda@gmail.com") ||
                canonicalizedEmail.Equals("dasda.dsa#!@yahoo.com") || canonicalizedEmail.Equals("lovethisclass123@gmail.com"))
            {
                result = true;
            }


            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("exogredient")]
        [DataRow("ImTryingToHidexogredientasdasd")]
        [DataRow("EXOGREDIENT")]
        [DataRow("eXoGrEdIeNt")]
        public void StringUtilityService_ContainsContextSpecificWords_HasContextSuccess(string plaintextPassword)
        {
            // Act
            bool result = StringUtilityService.ContainsContextSpecificWords(plaintextPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("thatthisfromyour")]
        [DataRow("ashdfsakdffreeadf")]
        [DataRow("servicesthese")]
        [DataRow("POLICY")]
        public async Task StringUtilityService_ContainsDictionaryWordsAsync_HasDicionaryAsyncSuccess(string plaintextPassword)
        {
            // Act
            bool result = await StringUtilityService.ContainsDictionaryWordsAsync(plaintextPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("123456")]
        [DataRow("123456789")]
        [DataRow("qwerty")]
        [DataRow("password")]
        [DataRow("password1")]
        [DataRow("abc123")]
        public async Task StringUtilityService_IsCorruptedPassword_IsCorruptedAsyncSuccess(string plaintextPassword)
        {
            // Act
            bool result = await StringUtilityService.IsCorruptedPassword(plaintextPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("111111111111111111")]
        [DataRow("aaaaaaaaaaaaaaaaaa")]
        [DataRow("987654321098765432")]
        [DataRow("abcdefghijklmnopqr")]
        [DataRow("ABCDEFGHIJKLMNOPQR")]
        [DataRow("zyxwvutsrqponmlkji")]
        [DataRow("ZYXWVUTSRQPONMLKJI")]
        [DataRow("ImTrying1234ToBeSneaky")]
        [DataRow("SneakySneakyasdfhhlsadfjls987asdf")]
        [DataRow("aabbccddeee")]
        [DataRow("!!!!!!!!!!!@@@@@@@")]
        [DataRow("%%%%%%%%%%%%%%%%%%")]
        [DataRow("<<<<<<<<<<<<<<<<<<")]
        [DataRow("::s:::::::::::::::")]
        [DataRow("\"\"\"\"\"\"\"\"\"\"\"\"\"\"")]
        [DataRow("912")]
        public void StringUtilityService_ContainsRepetitionOrSequence_HasPatternSuccess(string plaintextPassword)
        {
            // Act
            bool result = StringUtilityService.ContainsRepetitionOrSequence(plaintextPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("05830672945710")]
        [DataRow("imGoingTopass3489512")]
        [DataRow("ABadPasswordThisIs")]
        [DataRow("121212121223232323")]
        [DataRow("thisshouldnotcontainreptitions")]
        [DataRow("34895019")]
        public void StringUtilityService_ContainsRepetitionOrSequence_NoReptitionFalse(string plaintextPassword)
        {
            // Act
            bool result = StringUtilityService.ContainsRepetitionOrSequence(plaintextPassword);

            // Assert
            Assert.IsFalse(result);

        }

        [DataTestMethod]
        [DataRow("A3D1FF2CB29F5FDC", new byte[] { 163, 209, 255, 44, 178, 159, 95, 220 })]
        public void StringUtilityService_HexStringToBytes_GenerateCorrectByteArray(string hexString, byte[] expected)
        {
            //Arrange

            //Act
            byte[] actual = StringUtilityService.HexStringToBytes(hexString);

            //Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

    }
}
