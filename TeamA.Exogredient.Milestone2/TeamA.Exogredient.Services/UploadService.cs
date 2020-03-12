using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class UploadService
    {
        private readonly UploadDAO _uploadDao;
        public UploadService(UploadDAO uploadDAO)
        {
            _uploadDao = uploadDAO;
        }

        public async Task<bool> CreateUpload(UploadRecord uploadRecord)
        {
            return await _uploadDao.CreateAsync(uploadRecord).ConfigureAwait(false);
        }

        public async Task<bool> SaveProgessUpload(UploadRecord uploadRecord)
        {
            // Set column in_progress in uploadRecord. 
            return await _uploadDao.CreateAsync(uploadRecord).ConfigureAwait(false);
        }

        public async Task<IDataObject> ContinueProgressUpload(String id)
        {
            return await _uploadDao.ReadByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<bool> DeleteUploads(List<String> id)
        {
            return await _uploadDao.DeleteByIdsAsync(id).ConfigureAwait(false);   
        }

        public async Task<bool> ReportUpload(UploadRecord uploadRecord)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataObject> ReadByIngredientName(string ingredientName)
        {
            throw new NotImplementedException();
        }
        public async Task<List<ProfileScoreResult>> getUploadVotes(string username)
        {
            return await _uploadDao.ReadUploadVotes(username).ConfigureAwait(false);
        }

        public async Task<List<UploadResult>> ReadRecentByUploader(string username, int pagination)
        {
            return await _uploadDao.ReadRecentByUploader(username, pagination).ConfigureAwait(false);
        }

    }
}
