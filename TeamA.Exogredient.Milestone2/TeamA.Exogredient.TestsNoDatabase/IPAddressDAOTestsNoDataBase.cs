using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.TestsNoDatabase
{
    [TestClass]
    public class IPAddressDAOTestsNoDataBase
    {
        string NonExistingIP = "192.1.1";

        private bool DataEquals(IPAddressRecord ipRecord, IPAddressObject ipObject)
        {
            IDictionary<string, object> recordData = ipRecord.GetData();

            if (recordData[Constants.IPAddressDAOIPColumn].Equals(ipObject.IP) &&
                recordData[Constants.IPAddressDAOtimestampLockedColumn].Equals(ipObject.TimestampLocked) &&
                recordData[Constants.IPAddressDAOregistrationFailuresColumn].Equals(ipObject.RegistrationFailures) &&
                recordData[Constants.IPAddressDAOlastRegFailTimestampColumn].Equals(ipObject.LastRegFailTimestamp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_Create_SuccessfulCreation(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange
            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            // Act

            // Create IP Address.
            ipDAO.Create(ipRecord);
            IPAddressObject ipObject = (IPAddressObject)ipDAO.ReadById(ip);
            // Check if the data created is correct.
            bool correctDataResult = DataEquals(ipRecord, ipObject);

            // Assert

            // The data should be correct.
            Assert.IsTrue(correctDataResult);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_Create_UnsuccessfulCreation(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange
            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            bool result = false;

            // Act

            try
            {
                // Create the same IP Address twice.
                ipDAO.Create(ipRecord);
                ipDAO.Create(ipRecord);
            }
            catch (ArgumentException)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_DeleteByIds_SuccessfulDeletion(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP address.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);

            // Act

            // Delete the IP address.
            ipDAO.DeleteByIds(new List<string>() { ip });
            // Check if the IP exists and set the result accordingly.
            bool result = ipDAO.CheckIPExistence(ip);

            // Assert

            // The result should be false.
            Assert.IsFalse(result);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_DeleteByIds_UnsuccessfulDeletion(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP address.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);

            bool result = false;

            // Act
            try
            {
                // Delete an non-existing IP.
                ipDAO.DeleteByIds(new List<string>() { NonExistingIP });
            }
            catch (ArgumentException)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_ReadById_SuccessfulRead(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);

            // Act

            // Read the IP.
            IPAddressObject ipObject = (IPAddressObject)ipDAO.ReadById(ip);
            // Check if the read IP is correct and set the result accordingly.
            bool result = DataEquals(ipRecord, ipObject);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_ReadById_UnsuccessfulRead(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);
            bool result = false;

            // Act
            try
            {
                // Read a non-existing IP.
                IPAddressObject ipObject = (IPAddressObject)ipDAO.ReadById(NonExistingIP);
            }
            catch (ArgumentException)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_Update_SuccessfulUpdate(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);

            // Prepare a new data to update.
            IPAddressRecord updatedRecord = new IPAddressRecord(ip, 1, 1, 1);

            // Act

            // Update the data of the IP.
            ipDAO.Update(updatedRecord);
            // Read the IP. 
            IPAddressObject ipObject = (IPAddressObject)ipDAO.ReadById(ip);
            // Check if the IP was updated correctly and set the result to true. 
            bool result = DataEquals(ipRecord, ipObject);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_Update_UnsuccessfulUpdate(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create the IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            ipDAO.Create(ipRecord);

            // Prepare a new data to update but with a wrong IP address.
            IPAddressRecord updatedRecord = new IPAddressRecord(NonExistingIP, 1, 1, 1);
            bool result = false;

            // Act

            try
            {
                // Update the IP address.
                ipDAO.Update(updatedRecord);
            }
            catch (Exception)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }


        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_CheckIPExistence_IPExists(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            ipDAO.Create(ipRecord);

            // Act

            // Check if the IP exists and set the result accordingly.
            bool result = ipDAO.CheckIPExistence(ip);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public void IPAddressDAO_CheckIPExistence_IPNonExists(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            UnitTestIPAddressDAO ipDAO = new UnitTestIPAddressDAO();
            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            ipDAO.Create(ipRecord);

            // Act

            // Check if the IP exists and set the result accordingly.
            bool result = ipDAO.CheckIPExistence(NonExistingIP);

            // Assert

            // The result should be false.
            Assert.IsFalse(result);
        }

    }
}
