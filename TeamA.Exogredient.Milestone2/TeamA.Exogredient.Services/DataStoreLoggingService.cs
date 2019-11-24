using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class DataStoreLoggingService
    {
        private readonly DataStoreLoggingDAO _dsLoggingDAO;

        public DataStoreLoggingService()
        {
            _dsLoggingDAO = new DataStoreLoggingDAO();
        }

        public async Task<bool> LogToDataStoreAsync(string timestamp, string operation, string identifier,
                                                    string ipAddress, string errorType)
        {
            string[] splitResult = timestamp.Split(' ');

            LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

            return await _dsLoggingDAO.CreateAsync(logRecord, splitResult[2]);
        }

        public async Task<bool> DeleteLogFromDataStoreAsync(string timestamp, string operation, string identifier,
                                                            string ipAddress, string errorType)
        {
            try
            {
                string[] splitResult = timestamp.Split(' ');

                LogRecord logRecord = new LogRecord(splitResult[0] + " " + splitResult[1], operation, identifier, ipAddress, errorType);

                string id = await _dsLoggingDAO.FindIdFieldAsync(logRecord, splitResult[2]);

                await _dsLoggingDAO.DeleteAsync(id, splitResult[2]);

                return true;
            }
            catch
            {
                return false;
            }
            
        }

    }
}
