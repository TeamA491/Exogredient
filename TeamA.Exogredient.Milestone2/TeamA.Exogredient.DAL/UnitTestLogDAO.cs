using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class UnitTestLogDAO
    {

        Dictionary<string, List<LogRecord>> Logs;

        public UnitTestLogDAO()
        {
            Logs = new Dictionary<string, List<LogRecord>>();
        }

        public bool Create(INOSQLRecord record, string yyyymmdd)
        {
            LogRecord logRecord;
            try
            {
                logRecord = (LogRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.LogCreateInvalidArgument);
            }

            if (Logs.ContainsKey(yyyymmdd))
            {
                Logs[yyyymmdd].Add(logRecord);
            }
            else
            {
                Logs.Add(yyyymmdd, new List<LogRecord>{logRecord});
            }

            return true;
            
        }

        public bool Delete(string uniqueId, string yyyymmdd)
        {
            List<LogRecord> oneDayLogs = Logs[yyyymmdd];
            int index = 0;
            foreach(LogRecord log in oneDayLogs)
            {
                if (log.Identifier.Equals(uniqueId))
                {
                    oneDayLogs.RemoveAt(index);
                    return true;
                }
                index++;
            }
            throw new ArgumentException(Constants.LogDeleteDNE);
        }
    }
}
