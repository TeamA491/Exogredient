using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Registration.Tests
{
    [TestClass]
    public class RegistrationUnitTests
    {
        [TestMethod]
        public void TestMethod5()
        {
            RegistrationService rs = new RegistrationService();

            Assert.IsTrue(rs.CheckScope(true));
        }

        [TestMethod]
        public void TestMethod4()
        {
            RegistrationService rs = new RegistrationService();

            Assert.IsTrue(rs.CheckLength("Jason", 5));
        }

        [TestMethod]
        public void TestMethod3()
        {
            RegistrationService rs = new RegistrationService();

            Assert.IsTrue(rs.CheckIfANSCharacters("asdsa12312#!@#"));
        }

        [TestMethod]
        public void TestMethod2()
        {
            RegistrationService rs = new RegistrationService();

            Assert.IsTrue(rs.CheckIfNumericCharacters("12312312"));
        }

        [TestMethod]
        public void TestMethod1()
        {
            RegistrationService rs = new RegistrationService();

            Assert.IsTrue(rs.EmailFormatValidityCheck("a@"));
        }
    }
}
