using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendPhoneCodeManager
    {
        public static async Task<Result<bool>> SendPhoneCodeAsync(string username, string phoneNumber, string ipAddress)
        {
            try
            {
                await AuthenticationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SendPhoneCodeSuccessUserMessage, true);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
