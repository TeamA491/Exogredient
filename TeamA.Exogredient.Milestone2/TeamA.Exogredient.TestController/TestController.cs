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
using System.Reflection.Metadata;
using System.Linq;
using WeCantSpell.Hunspell;

namespace TeamA.Exogredient.TestController
{
    class TestController
    {
        public async static Task Main()
        {
            //UploadDAO ud = new UploadDAO(Constants.SQLConnection);
            //List<ProfileScoreResult> listprofile = await ud.ReadUploadVotes("username").ConfigureAwait(false);
            //foreach(ProfileScoreResult ps in listprofile)
            //{
            //    Console.WriteLine(ps.UploadUpvote + " " + ps.UploadDownvote);
            //}


            //string killcomma = "asdfsdf,";
            //Console.WriteLine(killcomma.TrimEnd(new char[] { ',' }));


            var ud = new UploadDAO(Constants.SQLConnection);
            //Console.WriteLine(await ud.DeleteByIdsAsync(new List<string>() { "20", "21", "22" }));
            //Console.WriteLine(await ud.CheckUploadsExistence(new List<string>() { "1", "2", "3","23" }));

            //var userD = new UserDAO(Constants.SQLConnection);
            //Console.WriteLine(await userD.ReadUserType("testuser"));


            //var asasd = "one";
            //Console.WriteLine(asasd.Equals("one"));

            Console.WriteLine(await ud.GetRecentPaginationSize("username").ConfigureAwait(false));
        }
    }
}
