using System;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TeamA.Exogredient.AppConstants;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;
//using Twilio.Rest.Preview.AccSecurity.Service; //TODO

namespace TeamA.Exogredient.Services
{
    public class VerificationService
    {
        private readonly UserManagementService _userManagementService;

        public VerificationService(UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Asynchronously makes a phone call via Twilio to the <paramref name="phoneNumber"/> of the user
        /// indicated by the <paramref name="username"/>. The robot should ask for keyboard
        /// input from the user before reading them their 4 digit code, verifying a human answered the call.
        /// </summary>
        /// <param name="username">The username of the user to call (string)</param>
        /// <param name="phoneNumber">The phone number to call, without the 1 (string)</param>
        /// <returns>true if the function completes without an exception (bool)</returns>
        public async Task<bool> SendCallVerificationAsync(string username, string phoneNumber)
        {
            // Initiate the API with the credentials defined in the constants file.
            string accountSID = Constants.TwilioAccountSID;
            string authorizationToken = Constants.TwilioAuthToken;

            TwilioClient.Init(accountSID, authorizationToken);

            // Asynchronously send the call to the phone number, via the call channel, using the path service SID.
            await VerificationResource.CreateAsync(
                to: $"+1{phoneNumber}",
                channel: Constants.TwilioCallChannel,
                pathServiceSid: Constants.TwilioPathServiceSID
            ).ConfigureAwait(false);

            // Reset the user's phone code failures (incase of a re-send).
            await _userManagementService.UpdatePhoneCodeFailuresAsync(username, 0).ConfigureAwait(false);

            return true;
        }


        /// <summary>
        /// Asynchronously verify the phone code that the user inputted via Twilio.
        /// </summary>
        /// <param name="phoneNumber">The phone number the verification code was sent to (string)</param>
        /// <param name="phoneCode">User-inputtted phone code, attempt to match what was sent (string)</param>
        /// <returns>String indicating whether the verification was approved, is still pending, or failed</returns>
        public async Task<string> VerifyPhoneCodeAsync(string phoneNumber, string phoneCode)
        {
            // Must catch twilio.exceptions.twilioapiexception if they try to verify after expiration time or if the phone number
            // was not used in a verification.
            try
            {
                // Initiate the API with the credentials defined in the constants file.
                string accountSID = Constants.TwilioAccountSID;
                string authorizationToken = Constants.TwilioAuthToken;

                TwilioClient.Init(accountSID, authorizationToken);

                // Asynchronously attempt to verify the inputted code that was sent to the phone number, using the path service SID.
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: $"+1{phoneNumber}",
                    code: $"{phoneCode}",
                    pathServiceSid: Constants.TwilioPathServiceSID
                ).ConfigureAwait(false);

                // Return the verification status.
                return verificationCheck.Status;
            }
            catch (TwilioException)
            {
                return Constants.TwilioAuthenticationFailString;
            }
        }

        /// <summary>
        /// Asynchronously send an email code to <paramref name="emailAddress"/> of the user
        /// indicated by the <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the user to send the email to (string)</param>
        /// <param name="emailAddress">The email address to send the email to (string)</param>
        /// <returns>Task (bool) whether the function executed without exception</returns>
        public async Task<bool> SendEmailVerificationAsync(string username, string emailAddress)
        {
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            // From the system email address to the emailAddress.
            message.From.Add(new MailboxAddress(Constants.SystemEmailAddress));
            message.To.Add(new MailboxAddress($"{emailAddress}"));

            message.Subject = Constants.EmailVerificationSubject;

            // Generate a code the length of what's defined in the constant file.
            Random generator = new Random();
            string emailCode = generator.Next((int)Math.Pow(10, Constants.EmailCodeLength - 1),
                                              (int)Math.Pow(10, Constants.EmailCodeLength)).ToString();

            // Timestamp the email code was generated at.
            long emailCodeTimestamp = TimeUtilityService.CurrentUnixTime();

            // Store the code and timestamp in the data store.
            await _userManagementService.StoreEmailCodeAsync(username, emailCode.ToString(), emailCodeTimestamp).ConfigureAwait(false);

            // Html body for the email code.
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

            // Create the SMTP client with the default certificate validation callback to prevent man in the middle attacks.
            //var client = new SmtpClient
            //{
            //    ServerCertificateValidationCallback = (s, c, h, e) => MailService.DefaultServerCertificateValidationCallback(s, c, h, e)
            //};
            var client = new SmtpClient();

            // Connect over google SMTP, provide credentials, and asynchronously send and disconnect the client.
            await client.ConnectAsync(Constants.GoogleSMTP, Constants.GoogleSMTPPort, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
            await client.AuthenticateAsync(Constants.SystemEmailAddress, Constants.SystemEmailPassword).ConfigureAwait(false);
            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();

            return true;
        }

    }
}
