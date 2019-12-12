using System;
using System.IO;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.AppConstants;
using System.Threading.Tasks;

namespace TeamA.Exogredient.ArchivingConsoleApp
{
    public class ArchivingScript
    {
        public static async Task Main(string[] args)
        {
            // Make sure only days, source Directory, and targetDirectory are entered into program
            if (args.Length != 4)
            {
                await UserManagementService.NotifySystemAdminAsync(Constants.InvalidArgs, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                return;
            }

            DateTime currentTime = DateTime.Now;
            int days;
            string targetDirectory = "";
            
            try
            {
                 days = Convert.ToInt32(args[1]);
            }
            catch
            {
                await UserManagementService.NotifySystemAdminAsync(Constants.FirstArgumentNotInt, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                return;
            }

            string sourceDir;

            if (Directory.Exists(args[2]))
            {
                sourceDir = args[2];
            }
            else
            {
                await UserManagementService.NotifySystemAdminAsync(Constants.SourceDirectoryDNE, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                return;
            }

            if (Directory.Exists(args[3]))
            {
                targetDirectory = args[3];
            }

            targetDirectory += currentTime.ToString(Constants.NamingFormatString);
            bool result = false;

            // FetchLogs will return true if files of the proper age are found in the source directory. 
            if (FileFetchingService.FetchLogs(sourceDir, targetDirectory, days))
            {
               if(CompressionService.Compress(Constants.SevenZipPath, sourceDir, targetDirectory))
                {
                  result = await FTPService.SendAsync(Constants.FTPUrl, "", targetDirectory, Constants.FTPUsername, Constants.FTPpassword).ConfigureAwait(false);
                }
            }
            
            // Notify system admin if Archiving fails to run successfully. 
            if(result == false)
            {
                await UserManagementService.NotifySystemAdminAsync("Archiving failed on" + currentTime, Constants.SystemAdminEmailAddress).ConfigureAwait(false);
            }
        }
    }
}
