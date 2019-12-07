using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class VerifyEmailCodeManager
    {
        public static async Task<Result<bool>> VerifyEmailCodeAsync(string username, string inputCode, string ipAddress)
        {
            try
            {
                bool emailVerificationSuccess = false;

                UserObject user = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                if (user.EmailCodeFailures > Constants.MaxEmailCodeAttempts)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.MaxEmailTriesReachedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.MaxEmailTriesReachedUserMessage, emailVerificationSuccess);
                }

                long maxValidTimeSeconds = UtilityService.TimespanToSeconds(Constants.EmailCodeMaxValidTime);
                long currentUnix = UtilityService.CurrentUnixTime();

                if (user.EmailCodeTimestamp + maxValidTimeSeconds < currentUnix)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.EmailCodeExpiredLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.EmailCodeExpiredUserMessage, emailVerificationSuccess);
                }

                if (user.EmailCode.Equals(inputCode))
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, Constants.AnonymousUserIdentifier, ipAddress).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.VerifyEmailSuccessUserMessage, emailVerificationSuccess);
                }
                else
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyEmailOperation, Constants.AnonymousUserIdentifier, ipAddress,
                                                  Constants.WrongEmailCodeMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.WrongEmailCodeMessage, emailVerificationSuccess);
                }
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.VerifyEmailOperation, Constants.AnonymousUserIdentifier, ipAddress, e.Message).ConfigureAwait(false);

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false);
            }
        }
    }
}
