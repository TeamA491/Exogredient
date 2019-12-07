using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendEmailCodeManager
    {
        public static async Task<Result<bool>> SendPhoneCodeAsync(string username, string emailAddress, string ipAddress)
        {
            try
            {
                await AuthenticationService.SendEmailVerificationAsync(username, emailAddress).ConfigureAwait(false);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendEmailCodeOperation, username, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SendEmailCodeSuccessUserMessage, true);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendEmailCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
