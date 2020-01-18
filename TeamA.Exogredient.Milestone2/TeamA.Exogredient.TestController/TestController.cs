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

            //UserRecord record = new UserRecord("eli123", "eli gomez", "elithegolfer@gmail.com", "949-981-5506", "lskdajf;lsadjf", 1, "test", "sldkd", 1, "123", 1, 1, 1, 1, 1);

            //UserRecord maskedRecord = (UserRecord)await ms.MaskAsync(record).ConfigureAwait(false);

            //UserDAO dao = new UserDAO();
            //await dao.CreateAsync(maskedRecord).ConfigureAwait(false);

            //Console.WriteLine(UtilityService.BytesToUTF8String(UtilityService.HexStringToBytes((string)maskedRecord.GetData()["name"])));

            UserObject obj = await UserManagementService.GetUserInfoAsync("eli123").ConfigureAwait(false);

            Console.WriteLine(obj.Email);

            UserObject unmasked = (UserObject)await ms.UnMaskAsync(obj).ConfigureAwait(false);

            Console.WriteLine(unmasked.Email);
        }
    }
}
