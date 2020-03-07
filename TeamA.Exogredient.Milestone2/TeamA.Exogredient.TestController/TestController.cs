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

namespace TeamA.Exogredient.TestController
{
    class TestController
    {

        public async static Task Main()
        {
            var stemmed = StringUtilityService.Stem("airplanes");
            Console.WriteLine(stemmed);
            Console.WriteLine(StringUtilityService.AutoCorrectWord(stemmed,
                "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US.dic",
                "/Users/charleskwak/MyPersonal/Git/Exogredient/TeamA.Exogredient.Milestone2/en_US.aff"));
        }
    }
}
