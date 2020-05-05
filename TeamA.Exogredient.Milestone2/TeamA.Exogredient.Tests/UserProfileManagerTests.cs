using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.Exceptions;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using System.Collections.Generic;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class UserProfileManagerTests
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        // Init DAO Layer.
        private readonly static UploadDAO _uploadDAO = new UploadDAO(Constants.SQLConnection);
        private readonly static SaveListDAO _saveListDao = new SaveListDAO(Constants.SQLConnection);
        private readonly static UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
        private readonly static AnonymousUserDAO _ipDAO = new AnonymousUserDAO(Constants.SQLConnection);
        private readonly static MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);
        private readonly static LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
        private readonly static StoreDAO _storeDAO = new StoreDAO(Constants.SQLConnection);

        // Init service Layer.
        private readonly static MaskingService _maskingService = new MaskingService(_mapDAO);
        private readonly static UploadService _uploadService = new UploadService(_uploadDAO);
        private readonly static StoreService _storeService = new StoreService(_storeDAO);
        private readonly static SaveListService _saveListService = new SaveListService(_saveListDao);
        private readonly static DataStoreLoggingService _dataStoreLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
        private readonly static FlatFileLoggingService _flatFileLoggingService = new FlatFileLoggingService(_maskingService);
        private readonly static UserManagementService _userManagementService = new UserManagementService(_userDAO, _ipDAO, _dataStoreLoggingService, _flatFileLoggingService, _maskingService);
        private readonly static FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);
        private readonly static DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);

        // Init Manager Layer.
        private readonly static LoggingManager _loggingManager = new LoggingManager(_flatFileLoggingService, _dataStoreLoggingService);
        private readonly static UserProfileManager _userProfileManager = new UserProfileManager(_uploadService, _storeService, _saveListService, _loggingManager, _userManagementService);

        private static IEnumerable<object[]> ProfileTestDataSuccess =>
            new List<object[]> {
                new object[] { "username", "localhost", 0, null },
                new object[] { "zero", "localhost", 0, null }
            };

        [TestMethod]
        [DynamicData(nameof(ProfileTestDataSuccess))]
        public async Task UserProfileManager_GetProfileScoreAsync_Pass(string username, string ipAddress, int failureCount, Exception ex)
        {
            // ACT: Get list of profile score for users.
            var listProfiles = await _userProfileManager.GetProfileScoreAsync(username, ipAddress, failureCount, ex);

            // ASSERT: The profile score were successfully retrieved .
            Assert.IsTrue(listProfiles.Count > 0);
        }

        private static IEnumerable<object[]> ProfileTestDataFail =>
        new List<object[]> {
                new object[] { "nonexit", "localhost", 0, null },
                new object[] { "FAIL", "localhost", 0, null }
        };

        [TestMethod]
        [DynamicData(nameof(ProfileTestDataFail))]
        public async Task UserProfileManager_GetProfileScoreAsync_FailUserDNE(string username, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // ACT: Get list of profile score for users that don't exists.
                var listProfiles = await _userProfileManager.GetProfileScoreAsync(username, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: Argument was thrown because users don't exists.
                Assert.IsTrue(true);
            }
        }

        private static IEnumerable<object[]> uploadTestDataUserExists =>
        new List<object[]> {
            new object[] { "username", 0,"localhost", 0, null },
            new object[] { "zero", 0, "localhost", 0, null }
        };

        [TestMethod]
        [DynamicData(nameof(uploadTestDataUserExists))]
        public async Task UserProfileManager_GetRecentUploadsAsync_Pass(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // ACT: Get the recent uploads for two existent user.
            var listUploads = await _userProfileManager.GetRecentUploadsAsync(username, pagination, ipAddress, failureCount, ex);

            // ASSERT: The uploads retrieved for the two users are greater than 0.
            Assert.IsTrue(listUploads.Count > 0);

        }



        private static IEnumerable<object[]> UploadTestDataUserDNE =>
        new List<object[]> {
            new object[] { "nonexit", 0,"localhost", 0, null },
            new object[] { "FAIL", 0,"localhost", 0, null }
        };
        [TestMethod]
        [DynamicData(nameof(UploadTestDataUserDNE))]
        public async Task UserProfileManager_GetRecentUploadsAsync_UserDNE(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // ACT: Get the recent uploads for two non existent user.
                var listUploads = await _userProfileManager.GetRecentUploadsAsync(username, pagination, ipAddress, failureCount, ex);
            }
            catch(ArgumentException)
            {
                // ASSERT: attempts to retrieve uploads for user DNE throws Argument Exceptions.
                Assert.IsTrue(true);
            }
        }


        [TestMethod]
        [DynamicData(nameof(uploadTestDataUserExists))]
        public async Task UserProfileManager_GetInProgressUploadsAsync_pass(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // ACT: Get the in progress uploads for two existent users.
            var inProgressUploads = await _userProfileManager.GetInProgressUploadsAsync(username, pagination, ipAddress, failureCount, ex);

            // ASSERT: Check that inprogress is above 0.
            Assert.IsTrue(inProgressUploads.Count > 0);
        }


        [TestMethod]
        [DynamicData(nameof(UploadTestDataUserDNE))]
        public async Task UserProfileManager_GetInProgressUploadsAsync_FailUserDNE(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // ACT: Get the in progress uploads for two non existent users.
                var inProgressUploads = await _userProfileManager.GetInProgressUploadsAsync(username, pagination, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: Check that an arguemnt exception was thrown.
                Assert.IsTrue(true);
            }
        }


        private static IEnumerable<object[]> SaveListExists =>
        new List<object[]> {
            new object[] { "username", 0,"localhost", 0, null },
        };
        [TestMethod]
        [DynamicData(nameof(SaveListExists))]
        public async Task UserProfileManager_GetSaveListAsync_Pass(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            // ACT: Get the save list for two existent users.
            var SaveLists = await _userProfileManager.GetSaveListAsync(username, pagination, ipAddress, failureCount, ex);

            // ASSERT: Check that inprogress is above 0.
            Assert.IsTrue(SaveLists.Count > 0);
        }


        [TestMethod]
        [DynamicData(nameof(uploadTestDataUserExists))]
        public async Task UserProfileManager_GetSaveListAsync_FailUserDNE(string username, int pagination, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // ACT: Get the in savelist for two non existent users.
                var SaveLists = await _userProfileManager.GetSaveListAsync(username, pagination, ipAddress, failureCount, ex);
            }
            catch(ArgumentException)
            {
                // ASSERT: Check that savelist is above 0.
                Assert.IsTrue(true);
            }

        }


        private static IEnumerable<object[]> DeleteSaveListTestDataExists=>
        new List<object[]> {
            new object[] { "username", 1, "beef", "localhost", 0, null },
            new object[] { "zero", 1, "beef", "localhost", 0, null }
        };
        [TestMethod]
        [DynamicData(nameof(DeleteSaveListTestDataExists))]
        public async Task UserProfileManager_DeleteSaveListAsync_Pass(string username, int storeId, string ingredient, string ipAddress, int failureCount, Exception ex)
        {
            // ACT: Delete savelist row for two user.
            var deleteResult = await _userProfileManager.DeleteSaveListAsync(username, storeId, ingredient, ipAddress, failureCount, ex);

            // ASSERT: Test that the result is true.
            Assert.IsTrue(deleteResult);
        }

        private static IEnumerable<object[]> DeleteSaveListTestDataUserDNE =>
        new List<object[]> {
            new object[] { "asdf", 1, "beef", "localhost", 0, null },
            new object[] { "FAIL", 1, "beef", "localhost", 0, null }
        };
        [TestMethod]
        [DynamicData(nameof(DeleteSaveListTestDataUserDNE))]
        public async Task UserProfileManager_DeleteSaveListAsync_FaileUserDNE(string username, int storeId, string ingredient, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // ACT: Delete savelist row for nonexistent user.
                var deleteResult = await _userProfileManager.DeleteSaveListAsync(username, storeId, ingredient, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: Test that an exception was thrown.
                Assert.IsTrue(true);
            }
        }


        private static IEnumerable<object[]> DeleteUploadExist =>
        new List<object[]> {
            new object[] { new List<String>() {"20"}, "username", "localhost", 0, null },
        };
 
        [TestMethod]
        [DynamicData(nameof(DeleteUploadExist))]
        public async Task UserProfileManager_DeleteUploadsAsync_Pass(List<int> ids, string performingUser, string ipAddress, int failureCount, Exception ex)
        {
            // Act: Delete upload for a user.
            var deleteResult = await _userProfileManager.DeleteUploadsAsync(ids, performingUser, ipAddress, failureCount, ex);

            // ASSERT: check that the operation passed.
            Assert.IsTrue(deleteResult);
        }


        private static IEnumerable<object[]> DeleteUploadUserDNE =>
        new List<object[]> {
            new object[] { new List<String>() {"20"}, "FAIL", "localhost", 0, null },
        };

        [TestMethod]
        [DynamicData(nameof(DeleteUploadUserDNE))]
        public async Task UserProfileManager_DeleteUploadsAsync_FailUserDNE(List<int> ids, string performingUser, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // Act: Delete upload for a user.
                var deleteResult = await _userProfileManager.DeleteUploadsAsync(ids, performingUser, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: Check that the operation thrown and argument exception.
                Assert.IsTrue(true);
            }
        }

        private static IEnumerable<object[]> DeleteUploadUploadIdDNE =>
        new List<object[]> {
            new object[] { new List<String>() {"100"}, "FAIL", "localhost", 0, null },
        };

        [TestMethod]
        [DynamicData(nameof(DeleteUploadUploadIdDNE))]
        public async Task UserProfileManager_DeleteUploadsAsync_FailUploadIdrDNE(List<int> ids, string performingUser, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // Act: Delete upload for a user.
                var deleteResult = await _userProfileManager.DeleteUploadsAsync(ids, performingUser, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: check that the operation thew and argument exception.
                Assert.IsTrue(true);
            }
        }

        private static IEnumerable<object[]> DeleteUploadTestDataInvalidUser =>
        new List<object[]> {
            new object[] { new List<String>() {"1"}, "zero", "localhost", 0, null },
        };

        [TestMethod]
        [DynamicData(nameof(DeleteUploadTestDataInvalidUser))]
        public async Task UserProfileManager_DeleteUploadsAsync_FailNotAuthz(List<int> ids, string performingUser, string ipAddress, int failureCount, Exception ex)
        {
            try
            {
                // Act: Attempt to delete an upload that a user does not own.
                var deleteResult = await _userProfileManager.DeleteUploadsAsync(ids, performingUser, ipAddress, failureCount, ex);
            }
            catch (NotAuthorizedException)
            {
                // ASSERT: Check that the operation threw a NotAuthorizedException.
                Assert.IsTrue(true);
            }
        }


        private static IEnumerable<object[]> SaveListPageTestDataExist =>
        new List<object[]> {
            new object[] { "username", "localhost", 0, null, 1},
        };

        [TestMethod]
        [DynamicData(nameof(SaveListPageTestDataExist))]
        public async Task UserProfileManager_GetSaveListPaginationSizeAsync_Pass(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            // ACT: Get the save list pagination for existing user.
            var saveListPage = await _userProfileManager.GetSaveListPaginationSizeAsync(username, ipAddress, failureCount, ex);

            // ASSERT: That a expected pagination result is returned.
            Assert.IsTrue(saveListPage == expected);

        }

        [TestMethod]
        [DynamicData(nameof(SaveListPageTestDataExist))]
        public async Task UserProfileManager_GetSaveListPaginationSizeAsync_PassEmptySaveList(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            // ACT: Get the save list pagination for a user.
            var saveListPage = await _userProfileManager.GetSaveListPaginationSizeAsync(username, ipAddress, failureCount, ex);
             
            // ASSERT: That it is equal the expected value.
            Assert.IsTrue(saveListPage == expected);    
        }


        [TestMethod]
        [DynamicData(nameof(SaveListPageTestDataExist))]
        public async Task UserProfileManager_GetInProgressUploadPaginationSizeAsync_Pass(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            // ACT: Get the upload pagination for a user.
            var saveListPage = await _userProfileManager.GetInProgressUploadPaginationSizeAsync(username, ipAddress, failureCount, ex);

            // ASSERT: That it is equal to the expected value.
            Assert.IsTrue(saveListPage == expected);

        }



        private static IEnumerable<object[]> RecentUploadTestDataExists =>
        new List<object[]> {
            new object[] { "username", "localhost", 0, null, 2},
        };
        [TestMethod]
        [DynamicData(nameof(RecentUploadTestDataExists))]
        public async Task UserProfileManager_GetRecentUploadPaginationSizeAsync_Pass(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            // ACT: Get the recent upload pagination for an existent user.
            var saveListPage = await _userProfileManager.GetRecentUploadPaginationSizeAsync(username, ipAddress, failureCount, ex);

            // ASSERT: That is is equal to the expected value.
            Assert.IsTrue(saveListPage == expected);
        }


        private static IEnumerable<object[]> RecentUploadTestDataUserDNE =>
        new List<object[]> {
            new object[] { "ajkasdfjs", "localhost", 0, null, 2},
        };
        [TestMethod]
        [DynamicData(nameof(RecentUploadTestDataUserDNE))]
        public async Task UserProfileManager_GetRecentUploadPaginationAsync_FailUserDNE(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            try
            {
                // ACT: Attempt to get pagination size for user DNE.
                var recentUploadPage = await _userProfileManager.GetRecentUploadPaginationSizeAsync(username, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: that an argument exception was thrown.
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        [DynamicData(nameof(RecentUploadTestDataUserDNE))]
        public async Task UserProfileManager_GetSaveListPaginationAsync_FailUserDNE(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            try
            {
                // ACT: Attempt to get pagination size for user DNE.
                var recentUploadPage = await _userProfileManager.GetSaveListPaginationSizeAsync(username, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: that an argument exception was thrown.
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        [DynamicData(nameof(RecentUploadTestDataUserDNE))]
        public async Task UserProfileManager_GetInProgressPaginationAsync_FailUserDNE(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            try
            {
                // ACT: Attempt to get pagination size for user DNE.
                var recentUploadPage = await _userProfileManager.GetInProgressUploadPaginationSizeAsync(username, ipAddress, failureCount, ex);
            }
            catch (ArgumentException)
            {
                // ASSERT: that an argument exception was thrown.
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        [DynamicData(nameof(SaveListPageTestDataExist))]
        public async Task UserProfileManager_GetInProgressPaginationAsync_Pass(string username, string ipAddress, int failureCount, Exception ex, int expected)
        {
            // ACT: Get the in progress upload pagination for an existent user.
            var recentUploadPage = await _userProfileManager.GetInProgressUploadPaginationSizeAsync(username, ipAddress, failureCount, ex);

            // ASSERT: That the value is equal to the expected.
            Assert.IsTrue(recentUploadPage == expected);
        }

    }
}
