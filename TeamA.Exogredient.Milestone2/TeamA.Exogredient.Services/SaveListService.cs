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

        public async Task<List<SaveListResult>> GetSaveList(string user, int pagination)
        {
            return await _saveListDao.ReadyByUsername(user, pagination).ConfigureAwait(false);
        }

        public async Task<bool> DeleteSaveList(string username, int storeId, string ingredient)
        {
            return await _saveListDao.DeleteByPK(username, storeId, ingredient);
        }
    }
}
