using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using Twilio;
using Twilio.Rest.Preview.AccSecurity.Service;

namespace TeamA.Exogredient.Services
{
    public class AuthenticationService
    {
        UserDAO _userDao;
        SecurityService _securityService;

        private readonly string _sendingEmail = "exogredient.system@gmail.com";
        private readonly string _sendingEmailPassword = Environment.GetEnvironmentVariable("SYSTEM_EMAIL_PASSWORD", EnvironmentVariableTarget.User);
        private readonly string _twilioAccountSID = "AC94d03adc3d2da651c16c82932c29b047";
        private readonly string _twilioPathServiceSID = "VAa9682f046b6f511b9aa1807d4e2949e5";
        private readonly string _twilioAuthorizationToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN", EnvironmentVariableTarget.User);

        public AuthenticationService()
        {
            _userDao = new UserDAO();
            _securityService = new SecurityService();
        }

        public async Task<bool> DisableUserNameAsync(string userName)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (await _userDao.IsUserNameDisabledAsync(userName))
                {
                    return false;
                    //throw new Exception("The username is already disabled!");
                }
                
                UserRecord disabledUser = new UserRecord(userName, disabled: "1");
                await _userDao.UpdateAsync(disabledUser);

                return true;
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }

        public async Task<bool> EnableUserNameAsync(string userName)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (! (await _userDao.IsUserNameDisabledAsync(userName)))
                {
                    return false;
                    //throw new Exception("The username is already enabled!");
                }
                UserRecord disabledUser = new UserRecord(userName, disabled: "0");
                await _userDao.UpdateAsync(disabledUser);

                return true;
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }


        public async Task<bool> AuthenticateAsync(string userName, byte[] encryptedPassword, byte[] aesKeyEncrypted, byte[] aesIV)
        {
            try
            {
                if (! (await _userDao.UserNameExistsAsync(userName)))
                {
                    return false;
                }
                if (await _userDao.IsUserNameDisabledAsync(userName))
                {
                    // HACK make custom exception
                    throw new Exception("This username is locked! To enable, contact the admin");
                }

                RSAParameters privateKey = SecurityService.GetRSAPrivateKey();
                byte[] aesKey = _securityService.DecryptRSA(aesKeyEncrypted, privateKey);
                string hexPassword = _securityService.DecryptAES(encryptedPassword, aesKey, aesIV);
                
                Tuple<string, string> saltAndPassword = await _userDao.GetStoredPasswordAndSaltAsync(userName);

                string storedPassword = saltAndPassword.Item1;
                string saltString = saltAndPassword.Item2;

                byte[] saltBytes = _securityService.HexStringToBytes(saltString);
                string hashedPassword = _securityService.HashPassword(hexPassword, saltBytes, 100, 32);


                if (storedPassword.Equals(hashedPassword)) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
                //throw e;
            }
        }


        /*
        public SendPhoneVerification(string phoneNumber)
        {

        }

        public generateJWT()
        {

        }
        */

        public async Task<bool> SendCallVerificationAsync(string phoneNumber)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

        }

        public async Task<bool> VerifyPhoneCodeAsync(string phoneNumber, string phoneCode)
        {
            try
            {
                string accountSID = _twilioAccountSID;
                string authorizationToken = _twilioAuthorizationToken;

                TwilioClient.Init(accountSID, authorizationToken);


                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: $"+1{phoneNumber}",
                    code: $"{phoneCode}",
                    pathServiceSid: _twilioPathServiceSID
                );


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

        }

        public async Task<bool> SendEmailVerificationAsync(string emailAddress)
        {
            try
            {
                var message = new MimeMessage();
                var bodyBuilder = new BodyBuilder();

                message.From.Add(new MailboxAddress(_sendingEmail));
                message.To.Add(new MailboxAddress($"{emailAddress}"));

                message.Subject = "Exogredient Account Verification";

                Random generator = new Random();
                string emailCode = generator.Next(100000, 1000000).ToString();

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
                await client.AuthenticateAsync(_sendingEmail, _sendingEmailPassword);
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
