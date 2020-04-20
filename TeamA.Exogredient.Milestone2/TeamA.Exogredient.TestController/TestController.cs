using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;
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

            var snapshot = await d.ReadOneSnapshotAsync(tries, 2020, 4).ConfigureAwait(false);
            Console.WriteLine(snapshot._month);

            var snapshotList = await d.ReadMultiSnapshotAsync(tries, 2020).ConfigureAwait(false);

            foreach (var snaps in snapshotList)
            {
                Console.WriteLine(snapshot._month);
            }
        }
    }
}
