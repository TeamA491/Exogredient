using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendPhoneCodeManager
    {
        public static async Task<Result<bool>> SendPhoneCodeAsync(string username, string phoneNumber, string ipAddress, int currentNumExceptions)
        {
            try
            {
                await AuthenticationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SendPhoneCodeSuccessUserMessage, true, false, currentNumExceptions);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await UserManagementService.NotifySystemAdminAsync($"{Constants.SendPhoneCodeOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
