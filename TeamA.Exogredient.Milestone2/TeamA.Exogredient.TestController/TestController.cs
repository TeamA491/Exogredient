using Google.Cloud.Vision.V1;
using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using System.Drawing;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main()
        {
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
