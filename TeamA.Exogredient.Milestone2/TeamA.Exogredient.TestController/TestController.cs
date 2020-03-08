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
            UploadDAO ud = new UploadDAO(Constants.SQLConnection);
            List<ProfileScoreResult> listprofile = await ud.ReadUploadVotes("username").ConfigureAwait(false);
            foreach(ProfileScoreResult ps in listprofile)
            {
                Console.WriteLine(ps.UploadUpvote + " " + ps.UploadDownvote);
            }


            //Console.WriteLine(ud.ReadUploadVotes("username"));
        }
    }
}
