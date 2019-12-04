using System.IO;
using System.Net;

namespace TeamA.Exogredient.Services
{
    public static class FTPService
    {
        public static bool Send(string ftpURL, string ftpFolder, string sourceDirectory, string userName, string password)
        {
            byte[] fileBytes = null;

            // File to send on local machine.
            string archiveFilePath = sourceDirectory + ".7z";
            if (!File.Exists(archiveFilePath))
            {
                return false;
            }

            fileBytes = File.ReadAllBytes(archiveFilePath);

            try
            {
                // Create the FTP request. Set the folder and file name.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpURL + ftpFolder + Path.GetFileName(archiveFilePath));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Entering in FTP user credentials.
                request.Credentials = new NetworkCredential(userName, password);

                // Specify additional information about the FTP request.
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = false;
                request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }
            }
            catch (WebException ex)
            {
                return false;
            }
            return true;
        }
    }
}
