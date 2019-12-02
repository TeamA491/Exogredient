﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main(string[] args)
        {
            //await AuthenticationService.SendCallVerificationAsync("9499815506");
            string result = await AuthenticationService.VerifyPhoneCodeAsync("9499815506", "0738");
            Console.WriteLine(result);
        }
    }
}
