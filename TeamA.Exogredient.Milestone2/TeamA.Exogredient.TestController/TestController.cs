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

        public static int Add(int a, int b=12345)
        {
            MethodInfo mInfo = typeof(TestController).GetMethod("Add");
            Console.WriteLine(mInfo.GetParameters()[0].);
            return a + b;
        }

        public async static Task Main()
        {
            Add(6787, 2233);

        }
    }
}
