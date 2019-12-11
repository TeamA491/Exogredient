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
            Console.WriteLine(DateTime.UtcNow.ToString("ss:ffff"));

            Console.WriteLine(await UtilityService.IsCorruptedPasswordAsync("lsiaf72284kdf722h3i732h724h").ConfigureAwait(false));

            Console.WriteLine(DateTime.UtcNow.ToString("ss:ffff"));
        }
    }
}
