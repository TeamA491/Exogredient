﻿using System;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class DataStoreLoggingService
    {
        private static readonly DataStoreLoggingDAO _dsLoggingDAO;

        static DataStoreLoggingService()
        {
            _dsLoggingDAO = new DataStoreLoggingDAO();
        }

        public static async Task<bool> LogToDataStoreAsync(string timestamp, string operation, string identifier,
                                                           string ipAddress, string errorType)
        {
            try
            {
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                return await _dsLoggingDAO.CreateAsync(logRecord, splitResult[2]).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> DeleteLogFromDataStoreAsync(string timestamp, string operation, string identifier,
                                                                   string ipAddress, string errorType)
        {
            try
            {
                string[] splitResult = timestamp.Split(' ');

                if (splitResult.Length != 3)
                {
                    throw new ArgumentException(Constants.TimestampFormatIncorrect);
                }

                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                string id = await _dsLoggingDAO.FindIdFieldAsync(logRecord, splitResult[2]).ConfigureAwait(false);

                await _dsLoggingDAO.DeleteAsync(id, splitResult[2]).ConfigureAwait(false);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
