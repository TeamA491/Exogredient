using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// FTP service is used to transfer a file from one machine to a 
    /// FTP server. 
    /// </summary>
    public static class FTPService
    {
        /// <summary>
        /// The SendAsync function takes a file from the local machine 
        /// and tries to send it to a FTP server. 
        /// </summary>
        /// <param name="ftpURL">The url of the FTP server.</param>
        /// <param name="ftpFolder">The destination folder in the FTP server. </param>
        /// <param name="sourceDirectory"> The directory in which the desired file is located</param>
        /// <param name="userName"> Crendential for FTP server. </param>
        /// <param name="password"> Credential for FTP server. </param>
        /// <returns>A boolean that confirms if sending was successful.</returns>
        public static async Task<bool> SendAsync(string ftpURL, string ftpFolder, string sourceDirectory, string userName, string password)
        {
            // File to send on local machine.
            string archiveFilePath = sourceDirectory + Constants.SevenZipFileExtension;

            if (!File.Exists(archiveFilePath))
            {
                throw new ArgumentException(Constants.FTPfileNotFound);
            }

            byte[] fileBytes = File.ReadAllBytes(archiveFilePath);

            // Create the FTP request. Set the folder and file name.
            FtpWebRequest request = WebRequest.Create(ftpURL + ftpFolder + Path.GetFileName(archiveFilePath)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Create a variable for FTP responses.
            FtpWebResponse response;

            // Entering in FTP user credentials.
            request.Credentials = new NetworkCredential(userName, password);

            // Specify additional information about the FTP request.
            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = false;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length).ConfigureAwait(false);
                }
            }
            catch(WebException e)
            {
                response = e.Response as FtpWebResponse;
                if(response != null)
                {
                    throw new WebException(Constants.FTPinvalidCredentials);
                }
            }
            return true;
        }
    }
}
