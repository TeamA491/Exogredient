using Google.Cloud.Vision.V1;
using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using System.Drawing;
using UploadController;
using System.IO;
using Image = Google.Cloud.Vision.V1.Image;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.Managers;
using System.Text;
using System.Reflection;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.CRUD;
using System.Reflection.Metadata;
using System.Linq;
using WeCantSpell.Hunspell;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main()
        {
            //    MapDAO mdao = new MapDAO(Constants.MapSQLConnection);
            //    LogDAO ldao = new LogDAO(Constants.NOSQLConnection);
            //    StoreDAO sdao = new StoreDAO(Constants.SQLConnection);
            //    AnonymousUserDAO adao = new AnonymousUserDAO(Constants.SQLConnection);
            //    UploadDAO updao = new UploadDAO(Constants.SQLConnection);
            //    UserDAO udao = new UserDAO(Constants.SQLConnection);

            //    MaskingService mservice = new MaskingService(mdao);

            //    DataStoreLoggingService dsls = new DataStoreLoggingService(ldao, mservice);
            //    FlatFileLoggingService ffls = new FlatFileLoggingService(mservice);

            //    LoggingManager lmanager = new LoggingManager(ffls, dsls);
            //    GoogleImageAnalysisService gias = new GoogleImageAnalysisService();
            //    StoreService sservice = new StoreService(sdao);
            //    UploadService uservice = new UploadService(updao);
            //    UserManagementService umanage = new UserManagementService(udao, adao, dsls, ffls, mservice);

            //    UploadManager manager = new UploadManager(lmanager, gias, sservice, uservice, umanage);

            //    Bitmap bmp = new Bitmap(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_185009.jpg");
            //    byte[] byteArray = File.ReadAllBytes(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_185009.jpg");

            // DIRECTIONS: comment out the admin check in this method to create the admin

            //await umanage.CreateUserAsync(false, new UserRecord("System", "testname", "testemail@email.com", "9498675309", "password", 0, Constants.AdminUserType, "salt", Constants.NoValueLong, "090909", 0, 0, 0, 0, 0)).ConfigureAwait(false);

            //============================================================================

            //await uservice.ContinueUploadProgressAsync(11).ConfigureAwait(false);

            //await gias.AnalyzeAsync(Image.FromFile(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_185009.jpg"), Constants.ExogredientCategories).ConfigureAwait(false);


            //UploadPost post = new UploadPost(byteArray, "packaged/bottled products", "thesmokinggun42", "172.88.196.101", DateTime.Now, "Nutter Butter", "deez nutz", 5, 4.20, "item");



            var userdao = new UserDAO(Constants.SQLConnection);
            var anonUserdao = new AnonymousUserDAO(Constants.SQLConnection);
            LogDAO _logDAO = new LogDAO(Constants.NOSQLConnection);
            MapDAO _mapDAO = new MapDAO(Constants.MapSQLConnection);

            MaskingService _maskingService = new MaskingService(_mapDAO);

            DataStoreLoggingService _dsLoggingService = new DataStoreLoggingService(_logDAO, _maskingService);
            FlatFileLoggingService _ffLoggingService = new FlatFileLoggingService(_maskingService);

            var usrmngser = new UserManagementService(userdao, anonUserdao, _dsLoggingService, _ffLoggingService, _maskingService);


            usrmngser.CreateUserAsync(false, new UserRecord("System","sys", Constants.SystemAdminEmailAddress, "5625555555", "123123", 0, "Admin","12345678", 0, "0", 0, 0, 0,0,0));


        }
    }
}
