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
            //UserRecord record = new UserRecord("eli123", "eli gomez", "elithegolfer@gmail.com", "949-981-5506", "lskdajf;lsadjf", 1, "test", "sldjfsldfkj", 1, "123", 1, 1, 1, 1, 1);

            //MaskingService ms = new MaskingService(new MapDAO());

            //UserRecord maskedRecord = (UserRecord)await ms.Mask(record).ConfigureAwait(false);

            //Console.WriteLine(UtilityService.BytesToUTF8String(UtilityService.HexStringToBytes((string)maskedRecord.GetData()["name"])));

            string password = "test";
            string salt = "644EB06BF0F7A319";

            string hex = SecurityService.HashWithKDF(UtilityService.ToHexString(password), UtilityService.HexStringToBytes(salt));

            Console.WriteLine(hex);
        }
    }
}
