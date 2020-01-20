using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    public class MaskingService
    {
        private readonly MapDAO _mapDAO;

        public MaskingService(MapDAO mdao)
        {
            _mapDAO = mdao;
        }

        public async Task<IUnMaskableObject> UnMaskAsync(IUnMaskableObject obj)
        {
            if (!obj.IsUnMasked())
            {
                List<Tuple<object, bool>> info = obj.GetMaskInformation();

                object[] parameters = new object[info.Count];

                for (int i = 0; i < info.Count; i++)
                {
                    Tuple<object, bool> data = info[i];

                    if (data.Item2)
                    {
                        if (data.Item1 is string)
                        {
                            MapObject map = (MapObject)await _mapDAO.ReadByIdAsync(data.Item1.ToString()).ConfigureAwait(false);

                            parameters[i] = map.Actual;
                        }
                        else
                        {
                            MapObject map = (MapObject)await _mapDAO.ReadByIdAsync(data.Item1.ToString()).ConfigureAwait(false);

                            parameters[i] = Int32.Parse(map.Actual);
                        }

                        await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.MapTableReadFromOperation,
                                                      Constants.SystemIdentifier, Constants.LocalHost).ConfigureAwait(false);
                    }
                    else
                    {
                        parameters[i] = data.Item1;
                    }
                }

                IUnMaskableObject resultObject = (IUnMaskableObject)obj.GetType().GetConstructor(obj.GetParameterTypes()).Invoke(parameters);

                resultObject.SetToUnMasked();

                return resultObject;
            }
            else
            {
                return obj;
            }
        }

        public async Task<IMaskableRecord> MaskAsync(IMaskableRecord record)
        {
            if (!record.IsMasked())
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
                            string hashString;

                            if (data.Item1 is string)
                            {
                                string hash = SecurityService.HashWithSHA256(SecurityService.HashWithSHA256(input));
                                parameters[i] = hash;
                                hashString = hash;
                            }
                            else
                            {
                                int count = Int32.Parse(Environment.GetEnvironmentVariable("COUNT", EnvironmentVariableTarget.User));
                                int hash = count;
                                Environment.SetEnvironmentVariable("COUNT", (count + 1).ToString(), EnvironmentVariableTarget.User);
                                parameters[i] = hash;
                                hashString = hash.ToString();
                            }

                            if (!await _mapDAO.CheckHashExistenceAsync(hashString).ConfigureAwait(false))
                            {
                                MapRecord mapRecord = new MapRecord(hashString, input, 1);
                                await _mapDAO.CreateAsync(mapRecord).ConfigureAwait(false);
                            }
                            else
                            {
                                MapObject mapObj = (MapObject)await _mapDAO.ReadByIdAsync(hashString).ConfigureAwait(false);
                                await UpdateOccurrencesAsync(hashString, mapObj.Occurrences + 1).ConfigureAwait(false);
                            }

                            //await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.MapTableModifiedOperation,
                            //                          Constants.SystemIdentifier, Constants.LocalHost).ConfigureAwait(false);
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

                resultRecord.SetToMasked();

                return resultRecord;
            }
            else
            {
                return record;
            }
        }

        public async Task<bool> UpdateOccurrencesAsync(string hashInput, int occurrences)
        {
            MapRecord map = new MapRecord(hashInput, occurrences: occurrences);

            return await _mapDAO.UpdateAsync(map).ConfigureAwait(false);
        }

        public async Task<bool> DecrementOccurrencesAsync(string hashInput)
        {
            if (!await _mapDAO.CheckHashExistenceAsync(hashInput).ConfigureAwait(false))
            {
                // TODO: exception message
                throw new ArgumentException();
            }

            MapObject mapObj = (MapObject)await _mapDAO.ReadByIdAsync(hashInput).ConfigureAwait(false);

            if (mapObj.Occurrences - 1 == 0)
            {
                return await _mapDAO.DeleteByIdsAsync(new List<string>() { hashInput }).ConfigureAwait(false);
            }
            else
            {
                MapRecord record = new MapRecord(hashInput, occurrences: mapObj.Occurrences - 1);

                return await _mapDAO.UpdateAsync(record).ConfigureAwait(false);
            }
        }

        public async Task<bool> DecrementMappingForDeleteAsync(IUnMaskableObject maskedObject)
        {
            if (!maskedObject.IsUnMasked())
            {
                List<Tuple<object, bool>> info = maskedObject.GetMaskInformation();

                for (int i = 0; i < info.Count; i++)
                {
                    Tuple<object, bool> data = info[i];

                    if (data.Item2)
                    {
                        await DecrementOccurrencesAsync(data.Item1.ToString()).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DecrementMappingForUpdateAsync(IMaskableRecord updateRecord, IUnMaskableObject maskedObject)
        {
            List<Tuple<object, bool>> recordInfo = updateRecord.GetMaskInformation();
            List<Tuple<object, bool>> objectInfo = maskedObject.GetMaskInformation();

            for (int i = 0; i < recordInfo.Count; i++)
            {
                Tuple<object, bool> recordData = recordInfo[i];
                Tuple<object, bool> objectData = objectInfo[i];

                if (recordData.Item2)
                {
                    // Masked Column
                    bool beingUpdated = true;

                    if (recordData.Item1 is int)
                    {
                        if ((int)recordData.Item1 == -1)
                        {
                            beingUpdated = false;
                        }
                    }
                    if (recordData.Item1 is string)
                    {
                        if (recordData.Item1 == null)
                        {
                            beingUpdated = false;
                        }
                    }
                    if (recordData.Item1 is long)
                    {
                        if ((long)recordData.Item1 == -1)
                        {
                            beingUpdated = false;
                        }
                    }

                    if (beingUpdated)
                    {
                        await DecrementOccurrencesAsync(objectData.Item1.ToString()).ConfigureAwait(false);
                    }
                }
            }

            return true;
        }
    }
}