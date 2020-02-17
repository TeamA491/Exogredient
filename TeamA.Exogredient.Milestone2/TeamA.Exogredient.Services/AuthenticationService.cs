using System;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Twilio;
using Twilio.Exceptions;
using TeamA.Exogredient.AppConstants;
using Twilio.Rest.Verify.V2.Service;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// Contains functions relating to authenticating the user's provided information.
    /// </summary>
    public class AuthenticationService: IAuthenticationService
    {

        public bool Authenticate(object existing, object credentials)
        {
            AuthenticationDTO exisitngDTO = existing as AuthenticationDTO;
            AuthenticationDTO credentialsDTO = credentials as AuthenticationDTO;

            return exisitngDTO.Equals(credentialsDTO);
        }
    }
}
