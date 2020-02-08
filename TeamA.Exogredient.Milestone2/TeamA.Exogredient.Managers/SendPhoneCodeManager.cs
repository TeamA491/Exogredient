using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendPhoneCodeManager
    {

        private readonly LoggingService _loggingService;
        private readonly AuthenticationService _authenticationService;
        private readonly VerificationService _verificationService;

        public SendPhoneCodeManager(LoggingService loggingService, AuthenticationService authenticationService,
                                    VerificationService verificationService)
        {
            _loggingService = loggingService;
            _authenticationService = authenticationService;
            _verificationService = verificationService;

        }

        public async Task<Result<bool>> SendPhoneCodeAsync(string username, string phoneNumber, string ipAddress,
                                                           int currentNumExceptions)
        {
            try
            {
                await _verificationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

                await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.SendPhoneCodeSuccessUserMessage, true, false, currentNumExceptions);
            }
            catch (Exception e)
            {
                await _loggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.SendPhoneCodeOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true, currentNumExceptions + 1);
            }
        }
    }
}
