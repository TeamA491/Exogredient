using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Services
{
    public static class FTPService
    {
        public static async Task<bool> SendAsync(string ftpURL, string ftpFolder, string sourceDirectory, string userName, string password)
        {
            // File to send on local machine.
            string archiveFilePath = sourceDirectory + ".7z";
            if (!File.Exists(archiveFilePath))
            {
                return false;
            }

            byte[] fileBytes = File.ReadAllBytes(archiveFilePath);

            try
            {
                // Create the FTP request. Set the folder and file name.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpURL + ftpFolder + Path.GetFileName(filePath));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Entering in FTP user credentials.
                request.Credentials = new NetworkCredential(userName, password);

                // Specify additional information about the FTP request.
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = false;
                request.EnableSsl = false;

                using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }
            }
            catch (WebException ex)
            {
                await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length).ConfigureAwait(false);
                requestStream.Close();
            }

            return true;
        }
    }
}
