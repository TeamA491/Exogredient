using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Managers
{
    public class UserProfileManager
    {
        private readonly UploadService _uploadService;
        private readonly StoreManagementService _storeManagementService;
        private readonly SaveListService _saveListService;

        public UserProfileManager(UploadService uploadService, StoreManagementService storeManagementService, SaveListService saveListService)
        {
            _uploadService = uploadService;
            _storeManagementService = storeManagementService;
            _saveListService = saveListService;
        }

        public async Task<List<ProfileScoreResult>> GetProfileScore(string username)
        {
            return await _uploadService.getUploadVotes(username).ConfigureAwait(false);
        }

        public string GetSavedUploadDraft()
        {
            throw new NotImplementedException();
        }

        public string GetSaveList()
        {
            throw new NotImplementedException();
        }

    }
}
