using System;
using System.Collections.Generic;
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

        public async Task<IMaskableRecord> Mask(IMaskableRecord record)
        {
            List<Tuple<object, bool>> info = record.GetMaskInformation();

            object[] parameters = new object[info.Count];

            for (int i = 0; i < info.Count; i++)
            {
                Tuple<object, bool> data = info[i];

                if (data.Item2)
                {
                    bool computeHash = true;

                    if (data.Item1 is int)
                    {
                        if ((int)data.Item1 == -1)
                        {
                            computeHash = false;
                        }
                    }
                    if (data.Item1 is string)
                    {
                        if (data.Item1 == null)
                        {
                            computeHash = false;
                        }
                    }
                    if (data.Item1 is long)
                    {
                        if ((long)data.Item1 == -1)
                        {
                            computeHash = false;
                        }
                    }

                    if (computeHash)
                    {
                        string input = data.Item1.ToString();

                        string hash = SecurityService.HashWithSHA256(SecurityService.HashWithSHA256(input));

                        MapRecord mapRecord = new MapRecord(hash, input);

                        if (!await _mapDAO.CheckHashExistenceAsync(hash).ConfigureAwait(false))
                        {
                            await _mapDAO.CreateAsync(mapRecord).ConfigureAwait(false);
                        }

                        parameters[i] = hash;
                    }
                    else
                    {
                        parameters[i] = data.Item1;
                    }
                }
                else
                {
                    parameters[i] = data.Item1;
                }
            }

            IMaskableRecord resultRecord = (IMaskableRecord)record.GetType().GetConstructor(record.GetParameterTypes()).Invoke(parameters);

            return resultRecord;
        }
    }
}
