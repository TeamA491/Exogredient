using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class StringUtilityServiceTests
    {

        [TestInitialize]
        public void Init()
        {
        }

        [DataTestMethod]
        [DataRow("Jason", 5, -1)]
        [DataRow("David", 2000, 1)]
        public void RegistrationService_CheckLength_WithinLengthSuccess(string name, int length, int min)
        {
            bool result = StringUtilityService.CheckLength(name, length, min);
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("asdsa12312#!@#")]
        [DataRow("...fas313ads[];'{}312")]
        public void RegistrationService_CheckIfANSCharacters_OnlyANSCharactersSuccess(string name)
        {
            bool result = StringUtilityService.CheckIfANSCharacters(name);
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("123123123")]
        public void RegistrationService_CheckIfNumericCharacters_OnlyNumericCharactersSuccess(string name)
        {
            bool result = StringUtilityService.CheckIfNumericCharacters(name);
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A")]
        [DataRow("lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com")]
        public void RegistrationService_EmailFormatValidityCheck_ValidEmailFormatSuccess(string email)
        {
            bool result = StringUtilityService.EmailFormatValidityCheck(email);
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("a@A")]
        [DataRow("lmao123sda@gmail.com")]
        [DataRow("dasda.dsa#!@yahoo.com")]
        [DataRow("LoveThisClass123+lmaojk@gmail.com")]
        public void RegistrationService_CanonicalizingEmail_CanonicalizingEmailSuccess(string email)
        {
            string canonicalizedEmail = StringUtilityService.CanonicalizingEmail(email);

            bool result = false;

            if (canonicalizedEmail.Equals("a@a") || canonicalizedEmail.Equals("lmao123sda@gmail.com") ||
                canonicalizedEmail.Equals("dasda.dsa#!@yahoo.com") || canonicalizedEmail.Equals("lovethisclass123@gmail.com"))
            {
                result = true;
            }

            Assert.IsTrue(result);
        }


    }
}
