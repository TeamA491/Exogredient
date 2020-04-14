using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class SaveListService
    {
        private readonly SaveListDAO _saveListDao;

        public SaveListService(SaveListDAO savelistDAO)
        {
            _saveListDao = savelistDAO;
        }

        /// <summary>
        /// Get the save list for a user.
        /// </summary>
        /// <param name="user">User to retrieve save list from.</param>
        /// <param name="pagination">Specific page to retrieve.</param>
        /// <returns>List of SaveListResult.</returns>
        public async Task<List<SaveListResult>> GetSaveListAsync(string user, int pagination)
        {
            return await _saveListDao.ReadyByUsernameAsync(user, pagination).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a save list for a user.
        /// </summary>
        /// <param name="username">User that is the owner of the save list.</param>
        /// <param name="storeId">StoreId associated with save listl</param>
        /// <param name="ingredient">Ingredient name for the save list.</param>
        /// <returns>bool represented wither the operation passed.</returns>
        public async Task<bool> DeleteSaveListAsync(string username, int storeId, string ingredient)
        {
            return await _saveListDao.DeleteByPKAsync(username, storeId, ingredient);
        }

        /// <summary>
        /// Get the pagination size for a user.
        /// </summary>
        /// <param name="username">user to retrieve size from.</param>
        /// <returns>int representing the pagination size.</returns>
        public async Task<int> GetPaginationSizeAsync(string username)
        {
            return await _saveListDao.GetPaginationSizeAsync(username).ConfigureAwait(false);
        }
    }
}
