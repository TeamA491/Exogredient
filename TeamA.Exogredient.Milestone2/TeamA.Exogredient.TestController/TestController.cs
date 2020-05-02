using Google.Cloud.Vision.V1;
using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using System.Drawing;
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
            var map = new MapDAO(Constants.MapSQLConnection);
            var mask = new MaskingService(map);
            var flat = new FlatFileLoggingService(mask);

            var log = new LogDAO(Constants.NOSQLConnection);
            var data = new DataStoreLoggingService(log, mask);
            var logman = new LoggingManager(flat, data);

            var user = new UserDAO(Constants.SQLConnection);
            var upload = new UploadDAO(Constants.SQLConnection);
            var snapdao = new SnapshotDAO(Constants.NOSQLConnection);
            var snap = new SnapshotService(log, user, upload, snapdao);
            var c = new CreateSnapshotManager(logman, snap);

            var d = new ReadSnapshotManager(logman, snap);


            int tries = 0;
            var fact = await c.CreateSnapshotAsync(tries, 2020, 4).ConfigureAwait(false);

            //var logdao = new LogDAO(Constants.NOSQLConnection);
            //var mapdao = new MapDAO(Constants.MapSQLConnection);
            //var mask = new MaskingService(mapdao);
            //var ffLogging = new FlatFileLoggingService(mask);
            //var dsLogging = new DataStoreLoggingService(logdao, mask);
            //var loggingManager = new LoggingManager(ffLogging, dsLogging);

            ////await loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.DownvoteOperation + "/15", "something", "35.214.178.5").ConfigureAwait(false);

            //await loggingManager.LogAsync("00:21:29:91 UTC 20200311", Constants.RegistrationOperation, "abe", "136.0.159.132").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200322", Constants.RegistrationOperation, "charlie", "107.164.247.172").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200305", Constants.RegistrationOperation, "daniel", "69.163.219.16").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200322", Constants.RegistrationOperation, "david", "98.149.88.14").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200314", Constants.RegistrationOperation, "eli", "99.203.74.100").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200407", Constants.RegistrationOperation, "hovic", "192.229.74.20").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200403", Constants.RegistrationOperation, "jason", "45.38.49.155").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200424", Constants.RegistrationOperation, "spectrum", "35.214.213.131").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200412", Constants.RegistrationOperation, "google", "23.240.33.197").ConfigureAwait(false);
            //await loggingManager.LogAsync("00:21:29:91 UTC 20200414", Constants.RegistrationOperation, "something", "107.164.247.172").ConfigureAwait(false);
            GoogleImageAnalysisService gias = new GoogleImageAnalysisService();


            MapDAO mdao = new MapDAO(Constants.MapSQLConnection);
            MaskingService mservice = new MaskingService(mdao);
            FlatFileLoggingService fflservice = new FlatFileLoggingService(mservice);
            LogDAO ldao = new LogDAO(Constants.NOSQLConnection);
            DataStoreLoggingService dsloggingservice = new DataStoreLoggingService(ldao, mservice);

            LoggingManager lmanager = new LoggingManager(fflservice, dsloggingservice);

            StoreDAO sdao = new StoreDAO(Constants.SQLConnection);
            AnonymousUserDAO adao = new AnonymousUserDAO(Constants.SQLConnection);
            DataStoreLoggingService dsls = new DataStoreLoggingService(ldao, mservice);
            StoreService sservice = new StoreService(sdao);
            UploadDAO updao = new UploadDAO(Constants.SQLConnection);
            UploadService uservice = new UploadService(updao);

            UserDAO udao = new UserDAO(Constants.SQLConnection);

            FlatFileLoggingService ffls = new FlatFileLoggingService(mservice);

            UserManagementService umanage = new UserManagementService(udao, adao, dsls, ffls, mservice);

            UploadManager manager = new UploadManager(lmanager, gias, sservice, uservice, umanage);


            //UserRecord urecord = new UserRecord("thesmokinggun42", "eli gomez", "elithegolfer@gmail.com", "9499815506", "fausto42", 0, "admin",
            //                                    "salt", 5, "6787", 5, 5, 5, 5, 5);

            //await umanage.CreateUserAsync(false, urecord).ConfigureAwait(false);

            await manager.CreateUploadAsync(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_185009.jpg", "packaged/bottled products", "thesmokinggun42", "172.88.196.101", DateTime.Now, "Nutter Butter", "These are some snacks", 5, 4.52, "item", 0);
            await manager.DraftUploadAsync(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_184614.jpg", "packaged/bottled products", "thesmokinggun42", "172.88.196.101", price: 5.6789);

            //Console.WriteLine(await sdao.FindStoreAsync(33.834045, -118.168047972222).ConfigureAwait(false));

            //var data = await gias.AnalyzeAsync(Image.FromFile(@"C:\Users\Eli\Desktop\Test\CVS\IMG_20200420_185009.jpg"), Constants.ExogredientCategories).ConfigureAwait(false);

            //foreach (var s in data.Suggestions)
            //{
            //    Console.WriteLine(s);
            //}

            //Console.WriteLine(Constants.GoogleApiKey);
        }
    }
}
