using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class SnapShotServiceTests
    {
        private static readonly MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);
        private static readonly LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
        private static readonly UploadDAO _uploadDAO = new UploadDAO(Constants.SQLConnection);
        private static readonly UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
        private static readonly SnapshotDAO _snapshotDAO = new SnapshotDAO(Constants.NOSQLConnection);

        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
        private static readonly DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
        private static readonly FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);
        private static readonly SnapshotService _snapshotService = new SnapshotService(_logDAO, _userDAO, _uploadDAO, _snapshotDAO);

        private static readonly LoggingManager _loggingManager = new LoggingManager(_ffLoggingService, _dsLoggingService);
        private static readonly ReadSnapshotManager _readSnapshotManager = new ReadSnapshotManager(_loggingManager, _snapshotService);

        [DataTestMethod]
        [DataRow(2020, 2, 29)]
        [DataRow(2020, 3, 31)]
        [DataRow(2020, 4, 30)]
        public void SnapshotService_GetDaysInMonth_Pass(int year, int month, int expected)
        {
            var actual = _snapshotService.GetDaysInMonth(year, month);
            // Checking if the amount of days match the actual amount of days.
            Assert.AreEqual(expected, actual);
        }

        private static IEnumerable<object[]> userDict =>
        new List<object[]> {
            new object[] { new Dictionary<string, int>() { {"jason", 13 }, {"david", 25 }, { "eli", 17 }, {"bob", 15 }, {"charlie", 13 }, {"hovic", 21 },
                { "abe", 17 }, {"daniel", 25 }, {"elliot", 53 }, {"peter", 22 }, { "fa", 14 }, {"ju", 18 }}  }
        };
        [TestMethod]
        [DynamicData(nameof(userDict))]
        public void SnapshotService_DropPairInDictTillTen_Pass(Dictionary<string, int> users)
        {
            var droppedList = _snapshotService.DropTillTen(users);
            // Check if the amount of pairs in dictionary is less than 11. 
            Assert.IsTrue(droppedList.Count < 11);
        }

    }
}
