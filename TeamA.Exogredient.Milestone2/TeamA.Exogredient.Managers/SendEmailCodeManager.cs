using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendEmailCodeManager
    {
        public static async Task<Result<bool>> SendPhoneCodeAsync(string username, string emailAddress, string ipAddress, int currentNumExceptions)
        {
            try
            {
                await AuthenticationService.SendEmailVerificationAsync(username, emailAddress).ConfigureAwait(false);

                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendEmailCodeOperation, username, ipAddress).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SendEmailCodeSuccessUserMessage, true, false, currentNumExceptions);
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendEmailCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await UserManagementService.NotifySystemAdminAsync($"{Constants.SendEmailCodeOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
