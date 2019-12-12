using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System;
using TeamA.Exogredient.DataHelpers;
using System.Text;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UtilityServiceTests
    {
        [DataTestMethod]
        [DataRow("A3D1FF2CB29F5FDC", new byte[] { 163, 209, 255, 44, 178, 159, 95, 220 })]
        public void UtilityService_HexStringToBytes_GenerateCorrectByteArray(string hexString, byte[] expected)
        {
            // Act
            byte[] actual = UtilityService.HexStringToBytes(hexString);

            // Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [DataTestMethod]
        [DataRow(new byte[] { 104, 101, 108, 108, 111 }, "68656C6C6F")]
        public void UtilityService_BytesToHexString_GenerateCorrectHexString(byte[] bytes, string expected)
        {
            // Act
            string actual = UtilityService.BytesToHexString(bytes);

            // Assert
            Assert.IsTrue(actual.Equals(expected));
        }

        [DataTestMethod]
        [DataRow("testing", "74657374696E67")]
        public void UtilityService_ToHexString_GenerateCorrectHexString(string original, string expected)
        {
            // Act
            string actual = UtilityService.ToHexString(original);

            // Assert
            Assert.IsTrue(expected.Equals(actual));
        }

        [DataTestMethod]
        [DataRow("Jason", 5, -1)]
        [DataRow("David", 2000, 1)]
        public void UtilityService_CheckLength_WithinLengthSuccess(string name, int length, int min)
        {
            // if min = -1 we are not checking withing a range/ the string has to be equal to length.

            // Act
            bool result = UtilityService.CheckLength(name, length, min);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("Jason", 4, -1)]
        [DataRow("David", 3, 1)]
        [DataRow("aBc", 7, 4)]
        public void UtilityService_CheckLength_NotWithinLengthFail(string name, int length, int min)
        {
            // if min = -1 we are not checking withing a range/ the string has to be equal to length.

            // Act
            bool result = UtilityService.CheckLength(name, length, min);

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("asdsa12312#!@#")]
        [DataRow("...fas313ads[];'{}312")]
        public void UtilityService_CheckCharacters_OnlyANSCharactersSuccess(string name)
        {
            // Act
            bool result = UtilityService.CheckCharacters(name, Constants.CharSetsData[Constants.ANSNoAngle]);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("123123123")]
        public void UtilityService_CheckCharacters_OnlyNumericCharactersSuccess(string name)
        {
            // Act
            bool result = UtilityService.CheckCharacters(name, Constants.CharSetsData[Constants.Numeric]);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A")]
        [DataRow("lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com")]
        public void UtilityService_EmailFormatValidityCheck_ValidEmailFormatSuccess(string email)
        {
            // Act
            bool result = UtilityService.CheckEmailFormatValidity(email);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A", "a@a")]
        [DataRow("lmao123sda@gmail.com", "lmao123sda@gmail.com")]
        [DataRow("l.ma.o123sda@gmail.com", "lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com", "dasda.dsa#!@yahoo.com")]
        [DataRow("LoveThisClass123+lmaojk@gmail.com", "lovethisclass123@gmail.com")]
        public void UtilityService_CanonicalizingEmail_CanonicalizingEmailSuccess(string email, string expectedEmail)
        {
            // Act
            string actualEmail = UtilityService.CanonicalizeEmail(email);

            // Assert: Check if act matches with expected value.
            Assert.AreEqual(expectedEmail, actualEmail);
        }

        [DataTestMethod]
        [DataRow("exogredient")]
        [DataRow("ImTryingToHidexogredientasdasd")]
        [DataRow("EXOGREDIENT")]
        [DataRow("eXoGrEdIeNt")]
        public void UtilityService_ContainsContextSpecificWords_HasContextSuccess(string plaintextPassword)
        {
            // Act
            bool result = UtilityService.ContainsContextSpecificWords(plaintextPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("thatthisfromyour")]
        [DataRow("ashdfsakdffreeadf")]
        [DataRow("servicesthese")]
        [DataRow("POLICY")]
        public async Task UtilityService_ContainsDictionaryWordsAsync_HasDicionaryAsyncSuccess(string plaintextPassword)
        {
            // Act
            bool result = await UtilityService.ContainsDictionaryWordsAsync(plaintextPassword).ConfigureAwait(false);

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
        public async Task UtilityService_IsCorruptedPassword_IsCorruptedAsyncSuccess(string plaintextPassword)
        {
            // Act
            bool result = await UtilityService.IsCorruptedPasswordAsync(plaintextPassword).ConfigureAwait(false);

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
        public void UtilityService_ContainsRepetitionOrSequence_HasPatternSuccess(string plaintextPassword)
        {
            // Act
            bool result = UtilityService.ContainsRepetitionOrSequence(plaintextPassword);

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
        public void UtilityService_ContainsRepetitionOrSequence_NoReptitionFailure(string plaintextPassword)
        {
            // Act
            bool result = UtilityService.ContainsRepetitionOrSequence(plaintextPassword);

            // Assert
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void UtilityService_CurrentUnixTime_CurrentTimeMatchSuccess()
        {
            // Arrange:
            long expectedTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            // Act: 
            long actualTime = UtilityService.CurrentUnixTime();

            // Assert:
            Assert.AreEqual(expectedTime, actualTime);
        }

        [DataTestMethod]
        [DataRow("message", "data", true, 1)]
        public void UtilityService_CreateResult_CreateAccurateResult(string message, string data, bool exceptionOccured, int numException)
        {
            // Act
            Result<string> resultObject = UtilityService.CreateResult<string>(message, data, exceptionOccured, numException);

            // Assert: Check that the result we created matches the inputs. 
            bool result;
            if(resultObject.Message == message && resultObject.Data == data && 
                resultObject.ExceptionOccurred == exceptionOccured && resultObject.NumExceptions == numException)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(1, 3, 5)]
        public void UtilityService_TimespanToSeconds_ConvertToSecondsSuccessfull(int hours, int minutes, int seconds)
        {
            // Arrange: Get the expected seconds for the timespan object
            long expectedSeconds =(long) (new TimeSpan(hours, minutes, seconds)).TotalSeconds;

            TimeSpan ts = new TimeSpan(hours, minutes, seconds);
            // Act: Get the seconds for timespan object using the function.
            long actualSeconds = UtilityService.TimespanToSeconds(ts);

            // Assert:
            Assert.AreEqual(expectedSeconds, actualSeconds);
        }

        [TestMethod]
        public void UtilityService_BytesToUTF8String_ConvertSuccessfull()
        {
            // Arrange:
            string expectedString = "HelloWorld";
            byte[] bytes = Encoding.UTF8.GetBytes(expectedString);

            // Act:
            string actualString = UtilityService.BytesToUTF8String(bytes);

            // Assert:
            Assert.AreEqual(expectedString, actualString);
        }


    }
}
