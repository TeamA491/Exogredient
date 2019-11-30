using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Services
{
    public class AdminFunctionalityService
    {
        private readonly string _sendingEmail = "exogredient.system@gmail.com";
        private readonly string _sendingEmailPassword = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD", EnvironmentVariableTarget.User);
        private readonly string _receivingEmail = "TEAMA.CS491@gmail.com";

        /// <summary>
        /// Uses gmail smtp to send mail from a known email address to any other address.
        /// Email subject is the current day in UTC.
        /// </summary>
        /// <param name="message">The message you want to send.</param>
        /// <returns>A bool representing whether the process succeeded.</returns>
        public async Task<bool> NotifySystemAdminAsync(string body)
        {
            try
            {
                string title = DateTime.UtcNow.ToString("MM-dd-yyyy");

                var message = new MimeMessage();
                var bodyBuilder = new BodyBuilder();

                message.From.Add(new MailboxAddress($"{_sendingEmail}"));
                message.To.Add(new MailboxAddress($"{_receivingEmail}"));

                message.Subject = title;

                Random generator = new Random();
                string emailCode = generator.Next(100000, 1000000).ToString();

                bodyBuilder.HtmlBody = body;

                message.Body = bodyBuilder.ToMessageBody();

                var client = new SmtpClient
                {
                    ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
                };

                await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync($"{_sendingEmail}", $"{_sendingEmailPassword}");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                client.Dispose();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
