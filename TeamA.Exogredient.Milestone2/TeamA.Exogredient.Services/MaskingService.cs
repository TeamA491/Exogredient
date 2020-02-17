using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// Provides functions for masking and unmasking objects, as well as interfacing with the MapDAO.
    /// </summary>
    public class MaskingService
    {
        private readonly MapDAO _mapDAO;
        /// <summary>
        /// Creates the masking service with its dependencies initialized.
        /// </summary>
        /// <param name="mdao">The MapDAO to new up and pass into the object.</param>
        public MaskingService(MapDAO mdao)
        {
            _mapDAO = mdao;
        }

        /// <summary>
        /// Takes in a IMaskableRecord and masks all the data necessary based on which columns are masked in
        /// the DAO.
        /// </summary>
        /// <param name="record">The record to mask.</param>
        /// <param name="isCreate">Whether this is being used for a create (lets the function know whether to increment
        /// the mapping).</param>
        /// <returns>(IMaskableRecord) the masked version of the record</returns>
        public async Task<IMaskableRecord> MaskAsync(IMaskableRecord record, bool isCreate)
        {
            // If the record is already masked we will simply return what was passed.
            if (!record.IsMasked())
            {
                // Get the mask information: a list of tuples<object, bool>, the data and whether it's column is masked.
                List<Tuple<object, bool>> info = record.GetMaskInformation();

                // The list of objects to pass to the constructor.
                object[] parameters = new object[info.Count];

                for (int i = 0; i < info.Count; i++)
                {
                    // Loop through the list and get all individual tuples.
                    Tuple<object, bool> data = info[i];

                    // If the data's column is masked...
                    if (data.Item2)
                    {
                        // Only compute the hash if data is being inserted into the table (i.e. not null or -1).
                        bool computeHash = true;

                        if (data.Item1 is int)
                        {
                            if ((int)data.Item1 == -1)
                            {
                                computeHash = false;
                            }
                        }
                        if (data.Item1 == null)
                        {
                            computeHash = false;
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
                            // Convert everything to a string including numerical values (to store in the "actual" column of the MapDAO).
                            string input = data.Item1.ToString();
                            string hashString;

                            // If the data is a string, simply double hash the input.
                            if (data.Item1 is string)
                            {
                                string hash = SecurityService.HashWithSHA256(SecurityService.HashWithSHA256(input));
                                parameters[i] = hash;
                                hashString = hash;
                            }
                            else
                            {
                                // Otherwise get the hash is the count of the amount of times this code block has been reached.
                                // Our current approach to hashing integer values:
                                int count = Int32.Parse(Environment.GetEnvironmentVariable("COUNT", EnvironmentVariableTarget.User));
                                int hash = count;
                                parameters[i] = hash;
                                hashString = hash.ToString();
                                Environment.SetEnvironmentVariable("COUNT", (count + 1).ToString(), EnvironmentVariableTarget.User);
                            }

                            // If this is the first time this hash string has been seen, create the record in the MapDAO with an occurrence of 1.
                            if (!await _mapDAO.CheckHashExistenceAsync(hashString).ConfigureAwait(false))
                            {
                                MapRecord mapRecord = new MapRecord(hashString, input, 1);
                                await _mapDAO.CreateAsync(mapRecord).ConfigureAwait(false);
                            }
                            else if (isCreate)
                            {
                                // Otherwise, if this function is being used in a creation, increment the occurrences. Do nothing otherwise.
                                MapObject mapObj = await _mapDAO.ReadByIdAsync(hashString).ConfigureAwait(false) as MapObject;
                                await UpdateOccurrencesAsync(hashString, mapObj.Occurrences + 1).ConfigureAwait(false);
                            }

                            // HACK: should log here but this ruins the tests... (ASK VONG)

                            //await LoggingService.LogAsync(DateTime.UtcNow.ToString(Constants.LoggingFormatString), Constants.MapTableModifiedOperation,
                            //                          Constants.SystemIdentifier, Constants.LocalHost).ConfigureAwait(false);
                        }
                        else
                        {
                            // If the computation was not needed, keep the same data.
                            parameters[i] = data.Item1;
                        }
                    }
                    else
                    {
                        // If the column was not masked, keep the same data.
                        parameters[i] = data.Item1;
                    }
                }

                // Create a record of the type that was passed to this function by getting the constructor and invoking it with
                // the collected parameters.
                IMaskableRecord resultRecord = (IMaskableRecord)record.GetType().GetConstructor(record.GetParameterTypes()).Invoke(parameters);

                resultRecord.SetToMasked();

                return resultRecord;
            }
            else
            {
                return record;
            }
        }

        /// <summary>
        /// Takes in an IUnMaskableObject and unmasks it based on whether certain columns are masked.
        /// </summary>
        /// <param name="obj">The object to unmask.</param>
        /// <returns>The unmasked IUnMaskableObject</returns>
        public async Task<IUnMaskableObject> UnMaskAsync(IUnMaskableObject obj)
        {
            // If the object is already unmasked we will simply return what was passed.
            if (!obj.IsUnMasked())
            {
                // Get the mask information: a list of tuples<object, bool>, the data and whether it's column is masked.
                List<Tuple<object, bool>> info = obj.GetMaskInformation();

                // The list of objects to pass to the constructor.
                object[] parameters = new object[info.Count];

                for (int i = 0; i < info.Count; i++)
                {
                    // Loop through the list and get all individual tuples.
                    Tuple<object, bool> data = info[i];

                    // If the data's column is masked...
                    if (data.Item2)
                    {
                        // If the data is a string simply add the actual value to the parameters.
                        if (data.Item1 is string)
                        {
                            MapObject map = await _mapDAO.ReadByIdAsync(data.Item1.ToString()).ConfigureAwait(false) as MapObject;

                            parameters[i] = map.Actual;
                        }
                        else
                        {
                            // Otherwise we must parse the string actual value into an integer and add that to the parameters.
                            MapObject map = await _mapDAO.ReadByIdAsync(data.Item1.ToString()).ConfigureAwait(false) as MapObject;

                            parameters[i] = Int32.Parse(map.Actual);
                        }
                    }
                    else
                    {
                        parameters[i] = data.Item1;
                    }
                }

                // Create an object of the type that was passed to this function by getting the constructor and invoking it with
                // the collected parameters.
                IUnMaskableObject resultObject = (IUnMaskableObject)obj.GetType().GetConstructor(obj.GetParameterTypes()).Invoke(parameters);

                resultObject.SetToUnMasked();

                return resultObject;
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Masks a string using our current implementation.
        /// </summary>
        /// <param name="input">The input to mask.</param>
        /// <returns>The masked value of the string.</returns>
        public string MaskString(string input)
        {
            return SecurityService.HashWithSHA256(SecurityService.HashWithSHA256(input));
        }

        /// <summary>
        /// Updates the occurrences of the hash in the Map table.
        /// </summary>
        /// <param name="hashInput">The hash to update.</param>
        /// <param name="occurrences">The new occurrences to store in the MapDAO.</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public async Task<bool> UpdateOccurrencesAsync(string hashInput, int occurrences)
        {
            if (!await _mapDAO.CheckHashExistenceAsync(hashInput).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.HashNotInTable);
            }

            MapRecord map = new MapRecord(hashInput, occurrences: occurrences);

            return await _mapDAO.UpdateAsync(map).ConfigureAwait(false);
        }

        /// <summary>
        /// Decrements the occurrences of the hash in the Map table.
        /// </summary>
        /// <param name="hashInput">The hash to update.</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public async Task<bool> DecrementOccurrencesAsync(string hashInput)
        {
            if (!await _mapDAO.CheckHashExistenceAsync(hashInput).ConfigureAwait(false))
            {
                throw new ArgumentException(Constants.HashNotInTable);
            }

            MapObject mapObj = await _mapDAO.ReadByIdAsync(hashInput).ConfigureAwait(false) as MapObject;

            // If the new occurrences will be 0, we have to delete it from the Map table.
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

        /// <summary>
        /// Decrements the mappings of a masked object in the Map table, specifically during a delete operation.
        /// </summary>
        /// <param name="maskedObject">The object to decrement the mappings of</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public async Task<bool> DecrementMappingForDeleteAsync(IUnMaskableObject maskedObject)
        {
            if (maskedObject.IsUnMasked())
            {
                throw new ArgumentException(Constants.DecrementDeleteUnmasked);
            }

            List<Tuple<object, bool>> info = maskedObject.GetMaskInformation();

            for (int i = 0; i < info.Count; i++)
            {
                Tuple<object, bool> data = info[i];

                // If the column is masked, decrement the occurrences of the hash.
                if (data.Item2)
                {
                    await DecrementOccurrencesAsync(data.Item1.ToString()).ConfigureAwait(false);
                }
            }

            return true;
        }

        /// <summary>
        /// Decrements the mappings of a masked object in the Map table, specifically during an update operation,
        /// where a record used for the update must be passed in as well to determine whether columns are being updated.
        /// </summary>
        /// <param name="updateRecord">The record being used for the update.</param>
        /// <param name="maskedObject">The object to decrement the mappings for.</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public async Task<bool> DecrementMappingForUpdateAsync(IMaskableRecord updateRecord, IUnMaskableObject maskedObject)
        {
            if (maskedObject.IsUnMasked())
            {
                throw new ArgumentException(Constants.DecrementUpdateUnmasked);
            }

            List<Tuple<object, bool>> recordInfo = updateRecord.GetMaskInformation();
            List<Tuple<object, bool>> objectInfo = maskedObject.GetMaskInformation();

            for (int i = 0; i < recordInfo.Count; i++)
            {
                Tuple<object, bool> recordData = recordInfo[i];
                Tuple<object, bool> objectData = objectInfo[i];

                // If the column is masked...
                if (recordData.Item2)
                {
                    // The data is being updated if the corresponding record data is not null or -1.
                    bool beingUpdated = true;

                    if (recordData.Item1 is int)
                    {
                        if ((int)recordData.Item1 == -1)
                        {
                            beingUpdated = false;
                        }
                    }
                    if (recordData.Item1 == null)
                    {
                        beingUpdated = false;
                    }
                    if (recordData.Item1 is long)
                    {
                        if ((long)recordData.Item1 == -1)
                        {
                            beingUpdated = false;
                        }
                    }

                    // If the data is being updated and it is not the primary key (first entry in the mask information),
                    // decrement the occurrences of the hash.
                    if (beingUpdated && i != 0)
                    {
                        await DecrementOccurrencesAsync(objectData.Item1.ToString()).ConfigureAwait(false);
                    }
                }
            }

            return true;
        }
    }
}