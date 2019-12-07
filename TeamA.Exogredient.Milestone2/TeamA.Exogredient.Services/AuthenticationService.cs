using System;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Preview.AccSecurity.Service;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public static class AuthenticationService
    {
        private static readonly UserDAO _userDAO;

        static AuthenticationService()
        {
            _userDAO = new UserDAO();
        }

        public static async Task<bool> SendCallVerificationAsync(string username, string phoneNumber)
        {
            string accountSID = Constants.TwilioAccountSID;
            string authorizationToken = Constants.TwilioAuthToken;

            TwilioClient.Init(accountSID, authorizationToken);

            await VerificationResource.CreateAsync(
                to: $"+1{phoneNumber}",
                channel: "call",
                pathServiceSid: Constants.TwilioPathServiceSID
            ).ConfigureAwait(false);

            await UserManagementService.UpdatePhoneCodeFailuresAsync(username, 0).ConfigureAwait(false);

            return true;
        }

        public static async Task<string> VerifyPhoneCodeAsync(string phoneNumber, string phoneCode)
        {
            // Must catch twilio.exceptions.twilioapiexception if they try to verify after expiration time
            try
            {
                string accountSID = Constants.TwilioAccountSID;
                string authorizationToken = Constants.TwilioAuthToken;

                TwilioClient.Init(accountSID, authorizationToken);


                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: $"+1{phoneNumber}",
                    code: $"{phoneCode}",
                    pathServiceSid: Constants.TwilioPathServiceSID
                ).ConfigureAwait(false);

                return verificationCheck.Status;
            }
            catch (TwilioException)
            {
                return Constants.TwilioExpiredReturnString;
            }
        }

        public static async Task<bool> SendEmailVerificationAsync(string username, string emailAddress)
        {
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            message.From.Add(new MailboxAddress(Constants.SystemEmailAddress));
            message.To.Add(new MailboxAddress($"{emailAddress}"));

            message.Subject = "Exogredient Account Verification";

            Random generator = new Random();
            string emailCode = generator.Next(100000, 1000000).ToString();
            long emailCodeTimestamp = UtilityService.CurrentUnixTime();

            await UserManagementService.StoreEmailCodeAsync(username, emailCode, emailCodeTimestamp).ConfigureAwait(false);

            bodyBuilder.HtmlBody = @"<td valign=""top"" align=""center"" bgcolor=""#0d1121"" style=""padding:35px 70px 30px;"" class=""em_padd""><table align=""center"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">" +
                @"<tr>" +
                @"</tr>" +
                @"<tr>" +
                @"<td height = ""15"" style = ""font-size:0px; line-height:0px; height:15px;"" > &nbsp;</td>" +
                            @"</tr>" +
                            @"<tr>" +
                            $@"<td align = ""center"" valign = ""top"" style = ""font-family:'Open Sans', Arial, sans-serif; font-size:18px; line-height:22px; color:#fbeb59; letter-spacing:2px; padding-bottom:12px;"">YOUR EMAIL VERIFICATION CODE IS: {emailCode}</td>" +
                @"</tr>" +
                @"<tr>";

            message.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
            };

            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
            await client.AuthenticateAsync(Constants.SystemEmailAddress, Constants.SystemEmailPassword).ConfigureAwait(false);
            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();

            return true;
        }

    }
}
