using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Collections.Generic;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class MaskingServiceTests
    {
        private readonly MaskingService _maskingService = new MaskingService(new MapDAO());
        private readonly IPAddressDAO _ipDAO = new IPAddressDAO();
        private readonly UserDAO _userDAO = new UserDAO();
        private readonly MapDAO _mapDAO = new MapDAO();

        [TestMethod]
        public async Task MaskingService_MaskAsync_WorksWithDifferentRecords()
        {
            LogRecord lrecord = new LogRecord("test", "test", "test", "test", "test");
            IPAddressRecord irecord = new IPAddressRecord("test");

            LogRecord lrecordMasked = (LogRecord)await _maskingService.MaskAsync(lrecord, false).ConfigureAwait(false);
            IPAddressRecord irecordMasked = (IPAddressRecord)await _maskingService.MaskAsync(irecord, false).ConfigureAwait(false);

            Assert.IsTrue(lrecordMasked.IsMasked());
            Assert.IsTrue(irecordMasked.IsMasked());

            try
            {
                await _mapDAO.DeleteByIdsAsync(new List<string>() { _maskingService.MaskString("test") }).ConfigureAwait(false);
            }
            catch
            { }
        }

        [TestMethod]
        public async Task MaskingService_MaskAsync_AllNecessaryColumnsAreDifferent()
        {
            UserRecord record = new UserRecord("username", "name", "email", "9499815506", "password", Constants.EnabledStatus,
                                                Constants.CustomerUserType, "salt", UtilityService.CurrentUnixTime(), "89456",
                                                UtilityService.CurrentUnixTime(), 5, UtilityService.CurrentUnixTime(), 4, 7);

            IDictionary<string, object> data = record.GetData();

            UserRecord maskedRecord = (UserRecord)await _maskingService.MaskAsync(record, false).ConfigureAwait(false);

            Assert.IsTrue(maskedRecord.IsMasked());

            IDictionary<string, object> maskedData = maskedRecord.GetData();

            foreach (KeyValuePair<string, object> pair in maskedData)
            {
                if (Constants.UserDAOIsColumnMasked[pair.Key])
                {
                    Assert.IsFalse(data[pair.Key].Equals(pair.Value));
                    await _mapDAO.DeleteByIdsAsync(new List<string>() { pair.Value.ToString() }).ConfigureAwait(false);
                }
                else
                {
                    Assert.IsTrue(data[pair.Key].Equals(pair.Value));
                }
            }
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", "Customer", "12345678", "196.168.40.42")]
        public async Task MaskingService_UnMaskAsync_WorksWithDifferentObjects(bool isTemp, string username, string name, string email,
                                                                               string phoneNumber, string password, string userType, string salt, string ipAddress)
        {
            await UserManagementService.CreateIPAsync(ipAddress).ConfigureAwait(false);

            UserRecord userRecord = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, userType,
                                             salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            await UserManagementService.CreateUserAsync(isTemp, userRecord).ConfigureAwait(false);

            string id = username;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                id = _maskingService.MaskString(username);
            }

            UserObject rawUser = (UserObject)await _userDAO.ReadByIdAsync(id).ConfigureAwait(false);

            id = ipAddress;

            if (Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOIPColumn])
            {
                id = _maskingService.MaskString(ipAddress);
            }

            // Cast the return result of asynchronously reading by the ip address into the IP object.
            IPAddressObject rawIP = (IPAddressObject)await _ipDAO.ReadByIdAsync(id).ConfigureAwait(false);

            IPAddressObject ip = (IPAddressObject)await _maskingService.UnMaskAsync(rawIP).ConfigureAwait(false);
            UserObject user = (UserObject)await _maskingService.UnMaskAsync(rawUser).ConfigureAwait(false);

            Assert.IsTrue(ip.IsUnMasked());
            Assert.IsTrue(user.IsUnMasked());

            await UserManagementService.DeleteIPAsync(ipAddress).ConfigureAwait(false);
            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow(false, "username", "mr.DROP TABLE", "blahblah@gmail.com", "1234567891", "password", "Customer", "12345678")]
        public async Task MaskingService_UnMaskAsync_AllDataIsTheSame(bool isTemp, string username, string name, string email,
                                                                          string phoneNumber, string password, string userType, string salt)
        {
            UserRecord userRecord = new UserRecord(username, name, email, phoneNumber, password, Constants.EnabledStatus, userType,
                                                   salt, Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

            await UserManagementService.CreateUserAsync(isTemp, userRecord).ConfigureAwait(false);

            string id = username;

            if (Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn])
            {
                id = _maskingService.MaskString(username);
            }

            UserObject rawUser = (UserObject)await _userDAO.ReadByIdAsync(id).ConfigureAwait(false);
            UserObject user = (UserObject)await _maskingService.UnMaskAsync(rawUser).ConfigureAwait(false);

            Assert.IsTrue(user.IsUnMasked());

            if (user.Username.Equals(username) && user.Name.Equals(name) && user.Email.Equals(email) && user.PhoneNumber.Equals(phoneNumber)
                && user.Password.Equals(password) && user.UserType.Equals(userType) && user.Salt.Equals(salt))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail();
            }

            await UserManagementService.DeleteUserAsync(username).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("test1")]
        [DataRow("test2")]
        public void MaskingService_MaskString_InputChanges(string input)
        {
            string result = _maskingService.MaskString(input);

            Assert.IsFalse(input.Equals(result));
        }

        [DataTestMethod]
        [DataRow("test1", "test1actual", 1, 2)]
        [DataRow("test2", "test2actual", 4, 42)]
        public async Task MaskingService_UpdateOccurrencesAsync_OccurrencesChange(string hash, string actual, int originalOccurrences, int updatedOccurrences)
        {
            MapRecord record = new MapRecord(hash, actual, originalOccurrences);

            await _mapDAO.CreateAsync(record).ConfigureAwait(false);

            await _maskingService.UpdateOccurrencesAsync(hash, updatedOccurrences).ConfigureAwait(false);

            MapObject obj = (MapObject)await _mapDAO.ReadByIdAsync(hash).ConfigureAwait(false);

            Assert.IsTrue(obj.Occurrences.Equals(updatedOccurrences));

            await _mapDAO.DeleteByIdsAsync(new List<string>() { hash }).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("test1", "test1actual", 56)]
        [DataRow("test2", "test2actual", 4)]
        public async Task MaskingService_DecrementOccurrencesAsync_OccurrencesDecremented(string hash, string actual, int originalOccurrences)
        {
            MapRecord record = new MapRecord(hash, actual, originalOccurrences);

            await _mapDAO.CreateAsync(record).ConfigureAwait(false);

            await _maskingService.DecrementOccurrencesAsync(hash).ConfigureAwait(false);

            MapObject obj = (MapObject)await _mapDAO.ReadByIdAsync(hash).ConfigureAwait(false);

            Assert.IsTrue(obj.Occurrences.Equals(originalOccurrences - 1));

            await _mapDAO.DeleteByIdsAsync(new List<string>() { hash }).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("test1", "test1actual", 1)]
        [DataRow("test2", "test2actual", 1)]
        public async Task MaskingService_DecrementOccurrencesAsync_OneOccurrenceEntriesDeleted(string hash, string actual, int originalOccurrences)
        {
            MapRecord record = new MapRecord(hash, actual, originalOccurrences);

            await _mapDAO.CreateAsync(record).ConfigureAwait(false);

            await _maskingService.DecrementOccurrencesAsync(hash).ConfigureAwait(false);

            Assert.IsFalse(await _mapDAO.CheckHashExistenceAsync(hash).ConfigureAwait(false));
        }
    }
}