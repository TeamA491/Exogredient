using System;
using System.IO;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public static class SystemUtilityService
    {
        /// <summary>
        /// Asynchronously check if string <paramref name="input"/> has been corrupted in previous password breaches.
        /// </summary>
        /// <param name="input">The string to test (string)</param>
        /// <returns>Task (bool) indicating whether the string is contained in the index.</returns>
        public static async Task<bool> IsCorruptedPasswordAsync(string input)
        {
            // Hash the input with sha1.
            string passwordSha1 = SecurityService.HashWithSHA1(input);

            string lineInput = "";

            using (StreamReader reader = new StreamReader(Constants.CorruptedPasswordsPath))
            {
                // Asynchronously read every line from the text file and compare it to the input.
                // If the input hash matches, return true. Return false if it was not found.
                while ((lineInput = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (passwordSha1.Equals(lineInput.Trim()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// Creates a result object from the <paramref name="message"/>, with the <paramref name="data"/>,
        /// whether an <paramref name="exceptionOccurred"/>, and storing the <paramref name="numExceptions"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data in the object.</typeparam>
        /// <param name="message">The message to store in the result (string)</param>
        /// <param name="data">The data to store in the result (T)</param>
        /// <param name="exceptionOccurred">The fact of whether an exception occurred (bool)</param>
        /// <param name="numExceptions">The number of exceptions that have occurred within a logical execution (int)</param>
        /// <returns>The resulting Result(T)</returns>
        public static Result<T> CreateResult<T>(string message, T data, bool exceptionOccurred, int numExceptions)
        {
            Result<T> result = new Result<T>(message)
            {
                Data = data,
                ExceptionOccurred = exceptionOccurred,
                NumExceptions = numExceptions
            };

            return result;
        }


        /// <summary>
        /// Asynchronously uses gmail smtp to send an email to the system administrator.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="body">The message to send.</param>
        /// <param name="sysAdminEmailAddress">The email of system admin</param>
        /// <returns>Returns true if the operation is successfull and false if it failed.</returns>
        public static async Task<bool> NotifySystemAdminAsync(string body, string sysAdminEmailAddress)
        {
            // Creates a thread for each day containing which exceptions maxed out.
            string title = DateTime.UtcNow.ToString(Constants.NotifySysAdminSubjectFormatString);

            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            // From the system email address to the sysAdminEmailAddress.
            message.From.Add(new MailboxAddress($"{Constants.SystemEmailAddress}"));
            message.To.Add(new MailboxAddress($"{sysAdminEmailAddress}"));

            message.Subject = title;

            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            // Create the SMTP client with the default certificate validation callback to prevent man in the middle attacks.
            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
            };

            // Connect over google SMTP, provide credentials, and asynchronously send and disconnect the client.
            await client.ConnectAsync(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
            await client.AuthenticateAsync($"{Constants.SystemEmailAddress}", $"{Constants.SystemEmailPassword}").ConfigureAwait(false);
            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();

            return true;
        }

        

        

       

    }
}
