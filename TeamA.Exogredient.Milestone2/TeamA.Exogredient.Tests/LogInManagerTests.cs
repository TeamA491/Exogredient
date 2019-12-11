using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Managers;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class LogInManagerTests
    {
        UserDAO userDAO = new UserDAO();

        [TestMethod]
        [DataRow("loginTest", "155A62BD4E0E7E4464C960BB79072EAD6B3522CBC3929021421DC4AAE308FFE1",
            "340947CCA61EFB8FC3AC60C4983B6B23854B7BE6C8EBBBA5DCDE90150E47DC4A16BA77598591F33F332D98AA22DE1FD7DBC" +
            "2FB9C58CA3395A3D62C2CC0239920102858CFE7365224EE357602B3ABA7378FD07F3CABA7AA5160AF51719ACBE74CA75822087" +
            "01942A41DF0217228C634331DDA6D1F262B87D7564E0B7479BA953F", "03C0A197753CB1D7DD421D4B80985434", "127.1.1")]
        public async Task LogInManager_LogInAsync_SuccessfulLogin(string username, string encryptedPassword,
                                                          string encryptedAESKey, string aesIV, string ipAddress)
        {
            //Arrange
                                                    //correctpassword 
            //UserRecord userRecord = new UserRecord(username, "firstname", "lastname", "email", "1234567890", "ACF75FE646B404B92050B75D41AFBB482316E91C6119A34E49CAF8105DE3EF57", 0, "usertype", "E63F5130A5C31E3F", 0, "ecode", 0, 0, 0, 0, 0);
            //await userDAO.CreateAsync(userRecord);
            
            //Act
            Result<bool> result = await LogInManager.LogInAsync(username,encryptedPassword,encryptedAESKey,aesIV,ipAddress,0);

            //Assert
            Assert.IsTrue(result.Data);

            //Clean up
            await userDAO.DeleteByIdsAsync(new List<string>{ username });
        }

        [TestMethod]
        [DataRow("loginTest", "155A62BD4E0E7E4464C960BB79072EAD6B3522CBC3929021421DC4AAE308FFE1",
            "340947CCA61EFB8FC3AC60C4983B6B23854B7BE6C8EBBBA5DCDE90150E47DC4A16BA77598591F33F332D98AA22DE1FD7DBC" +
            "2FB9C58CA3395A3D62C2CC0239920102858CFE7365224EE357602B3ABA7378FD07F3CABA7AA5160AF51719ACBE74CA75822087" +
            "01942A41DF0217228C634331DDA6D1F262B87D7564E0B7479BA953F", "03C0A197753CB1D7DD421D4B80985434", "127.1.1")]
        public async Task LogInManager_LogInAsync_UnsuccessfulLogin(string username, string encryptedPassword,
                                                          string encryptedAESKey, string aesIV, string ipAddress)
        {
            //Arrange
            //correctpassword 
            //UserRecord userRecord = new UserRecord(username, "firstname", "lastname", "email", "1234567890", "ACF75FE646B404B92050B75D41AFBB482316E91C6119A34E49CAF8105DE3EF57", 0, "usertype", "E63F5130A5C31E3F", 0, "ecode", 0, 0, 0, 0, 0);
            //await userDAO.CreateAsync(userRecord);

            //Act
            Result<bool> result = await LogInManager.LogInAsync("wrongusername", encryptedPassword, encryptedAESKey, aesIV, ipAddress,0);

            //Assert
            Assert.IsFalse(result.Data);

            //Clean up
            await userDAO.DeleteByIdsAsync(new List<string> { username });
        }
    }
}
