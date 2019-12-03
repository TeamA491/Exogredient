using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using Twilio;
using Twilio.Rest.Preview.AccSecurity.Service;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class AuthenticationService
    {
        private const string _systemEmailAddress = Constants.SystemEmailAddress;
        private static readonly string _systemEmailPassword = Constants.SystemEmailPassword;
        private const string _twilioAccountSID = Constants.TwilioAccountSID;
        private const string _twilioPathServiceSID = Constants.TwilioPathServiceSID;
        private static readonly string _twilioAuthorizationToken = Constants.TwilioAuthToken;

        private static readonly UserDAO _userDAO;

        static AuthenticationService()
        {
            _userDAO = new UserDAO();
        }

        /// <summary>
        /// Check if the username and the password are correct.
        /// </summary>
        /// <param name="userName"> the username used for login</param>
        /// <param name="encryptedPassword"> the password used for login encrypted </param>
        /// <param name="aesKeyEncrypted"> AES key used for encrypting the password </param>
        /// <param name="aesIV"> AES Initialization Vector used for encrypting the password </param>
        /// <returns> true if the username and password are correct, false otherwise </returns>
        public static async Task<bool> AuthenticateAsync(string userName, byte[] encryptedPassword, byte[] aesKeyEncrypted, byte[] aesIV)
        {
            // Check if the username exists.
            if (!(await _userDAO.CheckUserExistenceAsync(userName)))
            {
                return false;
            }
            // Check if the username is disabled.
            if (await _userDAO.CheckIfUserDisabledAsync(userName))
            {
                throw new InvalidOperationException("This username is locked! To enable, contact the admin");
            }

            byte[] privateKey = SecurityService.GetRSAPrivateKey();
            byte[] aesKey = SecurityService.DecryptRSA(aesKeyEncrypted, privateKey);
            // Decrypt the encrypted password.
            string hexPassword = SecurityService.DecryptAES(encryptedPassword, aesKey, aesIV);

            // Get the password and the salt stored corresponding to the username.
            Tuple<string, string> saltAndPassword = await _userDAO.GetStoredPasswordAndSaltAsync(userName);
            string storedPassword = saltAndPassword.Item1;
            string saltString = saltAndPassword.Item2;

            // Convert the salt to byte array.
            byte[] saltBytes = StringUtilityService.HexStringToBytes(saltString);
            //Number of iterations for has && length of the hash in bytes.
            // Hash the decrypted password with the byte array of salt.
            string hashedPassword = SecurityService.HashWithKDF(hexPassword, saltBytes);

            //Check if the stored password matches the hashed password
            if (storedPassword.Equals(hashedPassword))
            {
                // TODO Uncomment when GenerateJWS is implemented
                //string token = CreateToken(userName);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> SendCallVerificationAsync(string phoneNumber)
        {
            string accountSID = _twilioAccountSID;
            string authorizationToken = _twilioAuthorizationToken;

            TwilioClient.Init(accountSID, authorizationToken);

            var verification = await VerificationResource.CreateAsync(
                to: $"+1{phoneNumber}",
                channel: "call",
                pathServiceSid: _twilioPathServiceSID
            );

            return true;
        }

        public static async Task<bool> VerifyEmailCodeAsync(string username, string emailCodeInput, TimeSpan maxCodeValidTime)
        {
            Tuple<string, string> emailCodeInformation = await _userDAO.GetEmailCodeAndTimestamp(username);
            string emailCode = emailCodeInformation.Item1;
            string emailCodeTimestamp = emailCodeInformation.Item2;
            UserRecord record;

            if (emailCodeTimestamp.Equals(""))
            {
                return false;
            }

            if (StringUtilityService.CurrentTimePastDatePlusTimespan(emailCodeTimestamp, maxCodeValidTime))
            {
                record = new UserRecord(username, emailCode: "", emailCodeTimestamp: "");
                await _userDAO.UpdateAsync(record);
                return false;
            }
            
            if (!emailCodeInput.Equals(emailCode))
            {
                return false;
            }

            record = new UserRecord(username, emailCode: "", emailCodeTimestamp: "");
            await _userDAO.UpdateAsync(record);

            return true;
        }

        public static async Task<bool> VerifyPhoneCodeAsync(string phoneNumber, string phoneCode)
        {
            string accountSID = _twilioAccountSID;
            string authorizationToken = _twilioAuthorizationToken;

            TwilioClient.Init(accountSID, authorizationToken);


            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: $"+1{phoneNumber}",
                code: $"{phoneCode}",
                pathServiceSid: _twilioPathServiceSID
            );


            if (verificationCheck.Status.Equals("approved"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> SendEmailVerificationAsync(string username, string emailAddress)
        {
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            message.From.Add(new MailboxAddress(_systemEmailAddress));
            message.To.Add(new MailboxAddress($"{emailAddress}"));

            message.Subject = "Exogredient Account Verification";

            Random generator = new Random();
            string emailCode = generator.Next(100000, 1000000).ToString();
            string emailCodeTimestamp = DateTime.UtcNow.ToString("hh:mm:ss MM-dd-yyyy UTC");

            await UserManagementService.StoreEmailCode(username, emailCode, emailCodeTimestamp);

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

            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_systemEmailAddress, _systemEmailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();

            return true;
        }

    }
}
