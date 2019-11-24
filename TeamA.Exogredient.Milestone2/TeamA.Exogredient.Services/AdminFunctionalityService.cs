using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Services
{
    public class AdminFunctionalityService
    {
        public async Task<bool> NotifySystemAdminAsync(string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("exogredient.system@gmail.com");
                mail.To.Add("TEAMA.CS491@gmail.com");
                mail.Subject = "Test Mail";
                mail.Body = $"{message}";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("exogredient.system@gmail.com", "z8Bids@l%dkHF!");
                SmtpServer.EnableSsl = true;

                await SmtpServer.SendMailAsync(mail);

                SmtpServer.Dispose();

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
