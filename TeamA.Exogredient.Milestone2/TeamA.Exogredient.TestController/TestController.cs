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

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main()
        {
            MaskingService ms = new MaskingService(new MapDAO());
            UserDAO dao = new UserDAO();

            UserRecord record1 = new UserRecord("eli123", "eli gomez", "elithegolfer@gmail.com", "949-981-5506", "lskdajf;lsadjf", 1, "test", "sldkd", 1, "123", 1, 1, 1, 1, 1);
            UserRecord maskedRecord1 = (UserRecord)await ms.MaskAsync(record1).ConfigureAwait(false);
            await dao.CreateAsync(maskedRecord1).ConfigureAwait(false);


            UserRecord record2 = new UserRecord("eli123456", "eli gomez", "elithegolfer2@gmail.com", "949-981-5507", "lskdajf;lsadjf", 1, "test", "sldkd", 1, "123", 1, 1, 1, 1, 1);
            UserRecord maskedRecord2 = (UserRecord)await ms.MaskAsync(record2).ConfigureAwait(false);
            await dao.CreateAsync(maskedRecord2).ConfigureAwait(false);

            //UserObject obj = await UserManagementService.GetUserInfoAsync("eli123").ConfigureAwait(false);
        }
    }
}
