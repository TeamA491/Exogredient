using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class VerifyEmailCodeManager
    {
        public static async Task<Result<bool>> VerifyEmailCodeAsync(string username, string inputCode, string ipAddress, int currentNumExceptions)
        {
            try
            {
                bool emailVerificationSuccess = false;

                UserObject user = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                if (user.EmailCodeFailures >= Constants.MaxEmailCodeAttempts)
                {
                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, username, ipAddress,
                                                  Constants.MaxEmailTriesReachedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.MaxEmailTriesReachedUserMessage, emailVerificationSuccess, false, currentNumExceptions);
                }

                long maxValidTimeSeconds = UtilityService.TimespanToSeconds(Constants.EmailCodeMaxValidTime);
                long currentUnix = UtilityService.CurrentUnixTime();

                if (user.EmailCodeTimestamp + maxValidTimeSeconds < currentUnix)
                {
                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, username, ipAddress,
                                                  Constants.EmailCodeExpiredLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.EmailCodeExpiredUserMessage, emailVerificationSuccess, false, currentNumExceptions);
                }

                if (user.EmailCode.Equals(inputCode))
                {
                    emailVerificationSuccess = true;
                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, username, ipAddress).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.VerifyEmailSuccessUserMessage, emailVerificationSuccess, false, currentNumExceptions);
                }
                else
                {
                    await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, username, ipAddress,
                                                  Constants.WrongEmailCodeMessage).ConfigureAwait(false);

                    await UserManagementService.IncrementEmailCodeFailuresAsync(username).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.WrongEmailCodeMessage, emailVerificationSuccess, false, currentNumExceptions);
                }
            }
            catch (Exception e)
            {
                await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.VerifyEmailOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await UserManagementService.NotifySystemAdminAsync($"{Constants.VerifyEmailOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
