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

        public async Task<bool> DeleteUploads(List<string> ids)
        {
            return await _uploadDao.DeleteByIdsAsync(ids).ConfigureAwait(false);   
        }

        public async Task<bool> ReportUpload(UploadRecord uploadRecord)
        {
            throw new NotImplementedException();
        }


        public async Task<List<ProfileScoreResult>> getUploadVotes(string username)
        {
            return await _uploadDao.ReadUploadVotes(username).ConfigureAwait(false);
        }

        public async Task<List<UploadResult>> GetRecentByUploader(string username, int pagination)
        {
            return await _uploadDao.ReadRecentByUploader(username, pagination).ConfigureAwait(false);
        }

        public async Task<List<UploadResult>> GetInProgressUploadsByUploader(string username, int pagination)
        {
            return await _uploadDao.ReadInProgressUploadsByUploader(username, pagination).ConfigureAwait(false);
        }

        public async Task<bool> CheckUploadOwner(List<string> ids, string owner)
        {
            return await _uploadDao.CheckUploadsOwner(ids, owner).ConfigureAwait(false);
        }

        public async Task<bool> CheckUploadsExistence(List<String> ids)
        {
            return await _uploadDao.CheckUploadsExistence(ids).ConfigureAwait(false);
        }

        public async Task<int> GetInProgressPaginationSize(string username)
        {
            return await _uploadDao.GetInProgressPaginationSize(username).ConfigureAwait(false);
        }

        public async Task<int> GetRecentPaginationSize(string username)
        {
            return await _uploadDao.GetRecentPaginationSize(username).ConfigureAwait(false);
        }
    }
}
