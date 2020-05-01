using System;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class SendPhoneCodeManager
    {

        private readonly LoggingManager _loggingManager;
        private readonly VerificationService _verificationService;

        public SendPhoneCodeManager(LoggingManager loggingManager,
                                    VerificationService verificationService)
        {
            _loggingManager = loggingManager;
            _verificationService = verificationService;
        }

        public async Task<Result<bool>> SendPhoneCodeAsync(string username, string phoneNumber, string ipAddress,
                                                           int currentNumExceptions)
        {
            try
            {
                await _verificationService.SendCallVerificationAsync(username, phoneNumber).ConfigureAwait(false);

                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress).ConfigureAwait(false);

                return SystemUtilityService.CreateResult(Constants.SendPhoneCodeSuccessUserMessage, true, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                _ = _loggingManager.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString),
                                              Constants.SendPhoneCodeOperation, username, ipAddress, e.Message).ConfigureAwait(false);

                if (currentNumExceptions + 1 >= Constants.MaximumOperationRetries)
                {
                    await SystemUtilityService.NotifySystemAdminAsync($"{Constants.SendPhoneCodeOperation} failed a maximum number of times for {username}.", Constants.SystemAdminEmailAddress).ConfigureAwait(false);
                }

                return SystemUtilityService.CreateResult(Constants.SystemErrorUserMessage, false, true);
            }
        }
    }
}
