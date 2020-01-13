using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public class MaskingService
    {
        private readonly MapDAO _mapDAO;

        public MaskingService(MapDAO dao)
        {
            _mapDAO = dao;
        }

        public async Task<MapObject> UnMask(string hashInput)
        {
            return (MapObject)await _mapDAO.ReadByIdAsync(hashInput).ConfigureAwait(false);
        }

        public async Task<bool> Mask(string plainInput)
        {
            string result = SecurityService.HashWithHMACSHA256(SecurityService.HashWithHMACSHA256(plainInput));

            MapRecord record = new MapRecord(result, plainInput);

            return await _mapDAO.CreateAsync(record).ConfigureAwait(false);
        }
    }
}
