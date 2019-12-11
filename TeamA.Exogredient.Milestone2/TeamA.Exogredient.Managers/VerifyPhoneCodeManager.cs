using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class VerifyPhoneCodeManager
    {
        public static async Task<Result<bool>> VerifyPhoneCodeAsync(string username, string inputCode, string ipAddress,
                                                                    string phoneNumber, bool duringRegistration, int currentNumExceptions)
        {
            try
            {
                bool phoneVerificationSuccess = false;

                UserObject user = await UserManagementService.GetUserInfoAsync(username).ConfigureAwait(false);

                if (user.PhoneCodeFailures >= Constants.MaxPhoneCodeAttempts)
                {
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyPhoneOperation, username, ipAddress,
                                                  Constants.MaxPhoneTriesReachedLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.MaxPhoneTriesReachedUserMessage, phoneVerificationSuccess, false, currentNumExceptions);
                }

                string verificationStatus = await AuthenticationService.VerifyPhoneCodeAsync(phoneNumber, inputCode).ConfigureAwait(false);

                if (verificationStatus.Equals(Constants.TwilioAuthenticationApprovedString))
                {
                    phoneVerificationSuccess = true;
                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyPhoneOperation, username, ipAddress).ConfigureAwait(false);

                    if (duringRegistration)
                    {
                        await UserManagementService.MakeTempPermAsync(username).ConfigureAwait(false);
                    }

                    return UtilityService.CreateResult(Constants.VerifyPhoneSuccessUserMessage, phoneVerificationSuccess, false, currentNumExceptions);
                }
                else if (verificationStatus.Equals(Constants.TwilioAuthenticationPendingString))
                {
                    await UserManagementService.IncrementPhoneCodeFailuresAsync(username).ConfigureAwait(false);

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyPhoneOperation, username, ipAddress,
                                                  Constants.WrongPhoneCodeMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.WrongPhoneCodeMessage, phoneVerificationSuccess, false, currentNumExceptions);
                }
                else
                {
                    // Failed
                    await UserManagementService.IncrementPhoneCodeFailuresAsync(username).ConfigureAwait(false);

                    await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                                  Constants.VerifyPhoneOperation, username, ipAddress,
                                                  Constants.PhoneCodeExpiredLogMessage).ConfigureAwait(false);

                    return UtilityService.CreateResult(Constants.PhoneCodeExpiredUserMessage, phoneVerificationSuccess, false, currentNumExceptions);
                }
            }
            catch (Exception e)
            {
                await LoggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.VerifyPhoneOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await UserManagementService.NotifySystemAdminAsync($"{Constants.VerifyPhoneOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return UtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
