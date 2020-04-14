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
            throw new NotImplementedException();
        }

        public async Task<bool> SaveProgessUpload(UploadRecord uploadRecord)
        {
            // Set column in_progress in uploadRecord. 
            throw new NotImplementedException();
        }

        public async Task<IDataObject> ContinueProgressUpload(String id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the uploads using a list of uploads.
        /// </summary>
        /// <param name="ids">Ids of the upload to delete.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUploadsAsync(List<string> ids)
        {
            return await _uploadDao.DeleteByIdsAsync(ids).ConfigureAwait(false);   
        }

        public async Task<bool> ReportUpload(UploadRecord uploadRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all the upload vote's for a user.
        /// </summary>
        /// <param name="username">User to get votes from.</param>
        /// <returns>List of ProfileScoreResult.</returns>
        public async Task<List<ProfileScoreResult>> getUploadVotesAsync(string username)
        {
            return await _uploadDao.ReadUploadVotesAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the recent uploads by a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> GetRecentByUploaderAsync(string username, int pagination)
        {
            return await _uploadDao.ReadRecentByUploaderAsync(username, pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the In Progress upload for a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <param name="pagination">Pagination to specify which page to retrieve.</param>
        /// <returns>List of UploadResult.</returns>
        public async Task<List<UploadResult>> GetInProgressUploadsByUploaderAsync(string username, int pagination)
        {
            return await _uploadDao.ReadInProgressUploadsByUploaderAsync(username, pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Check that a user is the owner of a list of uploads.
        /// </summary>
        /// <param name="ids">Ids of the uploads to check.</param>
        /// <param name="owner">User to perform operation on.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadOwnerAsync(List<string> ids, string owner)
        {
            return await _uploadDao.CheckUploadsOwnerAsync(ids, owner).ConfigureAwait(false);
        }

        /// <summary>
        /// Check that an upload exists.
        /// </summary>
        /// <param name="ids">Ids of the upload to check.</param>
        /// <returns>bool representing whether the operation passed.</returns>
        public async Task<bool> CheckUploadsExistenceAsync(List<String> ids)
        {
            return await _uploadDao.CheckUploadsExistenceAsync(ids).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the pagination size for in progress uploads of a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <returns>int representing pagination size.</returns>
        public async Task<int> GetInProgressPaginationSizeAsync(string username)
        {
            return await _uploadDao.GetInProgressPaginationSizeAsync(username).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the pagination size for recent uploads of a user.
        /// </summary>
        /// <param name="username">User to perform operation on.</param>
        /// <returns>int representing pagination size.</returns>
        public async Task<int> GetRecentPaginationSizeAsync(string username)
        {
            return await _uploadDao.GetRecentPaginationSizeAsync(username).ConfigureAwait(false);
        }
    }
}
