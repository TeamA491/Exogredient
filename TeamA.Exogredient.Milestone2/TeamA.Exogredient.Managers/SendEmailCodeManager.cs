using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendEmailCodeManager
    {
        private readonly LoggingManager _loggingManager;
        private readonly VerificationService _verificationService;

        public SendEmailCodeManager(LoggingManager loggingManager,
                                    VerificationService verificationService)
        {
            _loggingManager = loggingManager;
            _verificationService = verificationService;
        }

        public async Task<Result<bool>> SendEmailCodeAsync(string username, string emailAddress, string ipAddress,
                                                           int currentNumExceptions)
        {
            try
            {
                await _verificationService.SendEmailVerificationAsync(username, emailAddress).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.SendEmailCodeOperation, username, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.SendEmailCodeSuccessUserMessage, true, false);
            }
            catch (Exception e)
            {
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                             Constants.SendEmailCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.SendEmailCodeOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true);
            }
        }
    }
}
