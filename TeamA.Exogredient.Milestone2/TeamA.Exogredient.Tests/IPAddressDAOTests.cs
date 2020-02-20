using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class IPAddressDAOTests
    {
        IPAddressDAO ipDAO = new IPAddressDAO(Constants.SQLConnection);
        string NonExistingIP = "192.1.1";

        private bool DataEquals(IPAddressRecord ipRecord, IPAddressObject ipObject)
        {
            IDictionary<string, object> recordData = ipRecord.GetData();
            Console.WriteLine(recordData[Constants.IPAddressDAOIPColumn]);
            Console.WriteLine(recordData[Constants.IPAddressDAOtimestampLockedColumn]);
            Console.WriteLine(recordData[Constants.IPAddressDAOregistrationFailuresColumn]);
            Console.WriteLine(recordData[Constants.IPAddressDAOlastRegFailTimestampColumn]);
            Console.WriteLine(ipObject.IP);
            Console.WriteLine(ipObject.TimestampLocked);
            Console.WriteLine(ipObject.RegistrationFailures);
            Console.WriteLine(ipObject.LastRegFailTimestamp);

            if (((string)recordData[Constants.IPAddressDAOIPColumn]).Equals(ipObject.IP) &&
                ((long)recordData[Constants.IPAddressDAOtimestampLockedColumn]).Equals(ipObject.TimestampLocked) &&
                ((int)recordData[Constants.IPAddressDAOregistrationFailuresColumn]).Equals(ipObject.RegistrationFailures) &&
                ((long)recordData[Constants.IPAddressDAOlastRegFailTimestampColumn]).Equals(ipObject.LastRegFailTimestamp))
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
        public async Task IPAddressDAO_CreateAsync_SuccessfulCreation(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            // Act

            // Create IP Address.
            await ipDAO.CreateAsync(ipRecord);
            IPAddressObject ipObject = (IPAddressObject)await ipDAO.ReadByIdAsync(ip).ConfigureAwait(false);
            // Check if the data created is correct.
            bool correctDataResult = DataEquals(ipRecord, ipObject);

            // Assert

            // The data should be correct.
            Assert.IsTrue(correctDataResult);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip}).ConfigureAwait(false);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_CreateAsync_UnsuccessfulCreation(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            bool result = false;

            // Act

            try
            {
                // Create the same IP Address twice.
                await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);
                await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);
            }
            catch(Exception)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_DeleteByIdsAsync_SuccessfulDeletion(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP address.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Act

            // Delete the IP address.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
            // Check if the IP exists and set the result accordingly.
            bool result = await ipDAO.CheckIPExistenceAsync(ip);

            // Assert

            // The result should be false.
            Assert.IsFalse(result);

        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_DeleteByIdsAsync_UnsuccessfulDeletion(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP address.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            bool result = false;

            // Act
            try
            {
                // Delete an non-existing IP.
                await ipDAO.DeleteByIdsAsync(new List<string>() { NonExistingIP }).ConfigureAwait(false);
            }
            catch(ArgumentException)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_ReadByIdAsync_SuccessfulRead(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Act

            // Read the IP.
            IPAddressObject ipObject = (IPAddressObject)await ipDAO.ReadByIdAsync(ip).ConfigureAwait(false);
            // Check if the read IP is correct and set the result accordingly.
            bool result = DataEquals(ipRecord, ipObject);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_ReadByIdAsync_UnsuccessfulRead(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);
            bool result = false;

            // Act
            try
            {
                // Read a non-existing IP.
                IPAddressObject ipObject = (IPAddressObject)await ipDAO.ReadByIdAsync(NonExistingIP).ConfigureAwait(false);
            }
            catch(ArgumentException)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_UpdateAsync_SuccessfulUpdate(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Prepare a new data to update.
            IPAddressRecord updatedRecord = new IPAddressRecord(ip, 1, 1, 1);

            // Act

            // Update the data of the IP.
            await ipDAO.UpdateAsync(updatedRecord);
            // Read the IP. 
            IPAddressObject ipObject = (IPAddressObject)await ipDAO.ReadByIdAsync(ip);
            // Check if the IP was updated correctly and set the result to true. 
            bool result = DataEquals(updatedRecord, ipObject);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_UpdateAsync_UnsuccessfulUpdate(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create the IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);
            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Prepare a new data to update but with a wrong IP address.
            IPAddressRecord updatedRecord = new IPAddressRecord(NonExistingIP, 1, 1, 1);
            bool result = false;

            // Act

            try
            {
                // Update the IP address.
                await ipDAO.UpdateAsync(updatedRecord).ConfigureAwait(false);
            }
            catch(Exception)
            {
                // Catch the exception and set the result to true.
                result = true;
            }

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }


        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_CheckIPExistenceAsync_IPExists(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Act

            // Check if the IP exists and set the result accordingly.
            bool result = await ipDAO.CheckIPExistenceAsync(ip).ConfigureAwait(false);

            // Assert

            // The result should be true.
            Assert.IsTrue(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("127.1.1", 0, 0, 0)]
        public async Task IPAddressDAO_CheckIPExistenceAsync_IPNonExists(string ip, long timestampLocked, int registrationFailures,
                               long lastRegFailTimestamp)
        {
            // Arrange

            // Create an IP.
            IPAddressRecord ipRecord =
                new IPAddressRecord(ip, timestampLocked, registrationFailures, lastRegFailTimestamp);

            await ipDAO.CreateAsync(ipRecord).ConfigureAwait(false);

            // Act

            // Check if the IP exists and set the result accordingly.
            bool result = await ipDAO.CheckIPExistenceAsync(NonExistingIP).ConfigureAwait(false);

            // Assert

            // The result should be false.
            Assert.IsFalse(result);

            // Clean up

            // Delete the IP.
            await ipDAO.DeleteByIdsAsync(new List<string>() { ip }).ConfigureAwait(false);
        }
    }
}
