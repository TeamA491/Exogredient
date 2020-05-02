//using System;
//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TeamA.Exogredient.DataHelpers;
//using TeamA.Exogredient.AppConstants;
//using TeamA.Exogredient.Services;
//using TeamA.Exogredient.DAL;

//namespace TeamA.Exogredient.Tests
//{
//    [TestClass]
//    public class VerificationServiceTests
//    {
//        private static readonly UserDAO _userDAO = new UserDAO(Constants.SQLConnection);
//        private static readonly AnonymousUserDAO _ipDAO = new AnonymousUserDAO(Constants.SQLConnection);
//        private static readonly LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
//        private static readonly MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);
//        private static readonly MaskingService _maskingService = new MaskingService(_mapDAO);
//        private static readonly DataStoreLoggingService _dsLog = new DataStoreLoggingService(_logDAO, _maskingService);
//        private static readonly FlatFileLoggingService _ffLog = new FlatFileLoggingService(_maskingService);

//        private static readonly UserManagementService _userManagementService = new UserManagementService(_userDAO,_ipDAO,_dsLog,_ffLog,_maskingService);
//        //TODO ELI YOURE MISSING AN ARGUMENT
//        private static readonly VerificationService _verificationService = new VerificationService(_userManagementService);

//        [DataTestMethod]
//        [DataRow("9499815506", "eli")]
//        public async Task VerificationService_SendCallVerificationAsync_CallSuccessful(string phoneNumber, string username)
//        {
//            try
//            {
//                // Arrange: Create user 
//                UserRecord user = new UserRecord(username, "eli", "test@gamil.com", phoneNumber, "asdasd", Constants.EnabledStatus, Constants.CustomerUserType,
//                                                 "123123", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);

//                await _userManagementService.CreateUserAsync(false, user).ConfigureAwait(false);
//            }
//            catch
//            { }

//            bool result = await _verificationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

//            Assert.IsTrue(result);

//            await _userManagementService.DeleteUserAsync(username).ConfigureAwait(false);
//        }

//        [DataTestMethod]
//        [DataRow("elithegolfer@gmail.com", "eli")]
//        public async Task AuthenticationService_SendEmailVerificationAsync_EmailSuccessful(string emailAddress, string username)
//        {
//            try
//            {
//                // Arrange: Create user 
//                UserRecord user = new UserRecord(username, "eli", emailAddress, "5625555555", "asdasd", Constants.EnabledStatus, Constants.CustomerUserType,
//                                        "123123", Constants.NoValueLong, Constants.NoValueString, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueLong, Constants.NoValueInt, Constants.NoValueInt);
//                await _userManagementService.CreateUserAsync(false, user).ConfigureAwait(false);
//            }
//            catch (Exception E)
//            {
//                Console.WriteLine(E.Message);
//            }

//            bool result = await _verificationService.SendEmailVerificationAsync("eli", emailAddress).ConfigureAwait(false);

//            Assert.IsTrue(result);

//            await _userManagementService.DeleteUserAsync(username).ConfigureAwait(false);
//        }
//    }
//}
