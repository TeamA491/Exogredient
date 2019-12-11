using System;
using System.Collections.Generic;
using System.Data;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    public class UnitTestIPAddressDAO
    {
        Dictionary<string, IPAddressRecord> IPAddress;

        public UnitTestIPAddressDAO()
        {
            IPAddress = new Dictionary<string, IPAddressRecord>();
        }


        public bool Create(ISQLRecord record)
        {
            IPAddressRecord ipRecord;
            try
            {
                ipRecord = (IPAddressRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.IPCreateInvalidArgument);
            }

            
            IDictionary<string, object> recordData = ipRecord.GetData();

            foreach (KeyValuePair<string, object> pair in recordData)
            {
                if (pair.Value is string)
                {
                    if (pair.Value == null)
                    {
                        throw new NoNullAllowedException(Constants.IPRecordNoNull);
                    }
                }

                if (pair.Value is int || pair.Value is long)
                {
                    if (pair.Value.Equals(-1))
                    {
                        throw new NoNullAllowedException(Constants.IPRecordNoNull);
                    }
                }
            }

            IPAddress.Add((string)recordData[Constants.IPAddressDAOIPColumn], ipRecord);

            return true;
        }

        public bool DeleteByIds(List<string> idsOfRows)
        {
            foreach (string ip in idsOfRows)
            {
                if (!CheckIPExistence(ip))
                {
                    throw new ArgumentException(Constants.IPDeleteDNE);
                }
                IPAddress.Remove(ip);
            }

            return true;
        }

        public IDataObject ReadById(string id)
        {
            if (!CheckIPExistence(id))
            {
                throw new ArgumentException(Constants.IPDeleteDNE);
            }
            IDictionary<string, object> data = IPAddress[id].GetData();

            return new IPAddressObject((string)data[Constants.IPAddressDAOIPColumn],
                                       (long)data[Constants.IPAddressDAOtimestampLockedColumn],
                                       (int)data[Constants.IPAddressDAOregistrationFailuresColumn],
                                       (long)data[Constants.IPAddressDAOlastRegFailTimestampColumn]);
        }

        public bool Update(ISQLRecord record)
        {
            IPAddressRecord ipRecord;
            try
            {
                ipRecord = (IPAddressRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.IPUpdateInvalidArgument);
            }

            IDictionary<string, object> newRecordData = ipRecord.GetData();
            IDictionary<string, object> existingRecordData = IPAddress[(string)newRecordData[Constants.IPAddressDAOIPColumn]].GetData();

            if (!CheckIPExistence((string)newRecordData[Constants.IPAddressDAOIPColumn]))
            {
                throw new ArgumentException(Constants.IPUpdateDNE);
            }

            foreach (KeyValuePair<string, object> pair in newRecordData)
            {
                if (pair.Value is int && (int)pair.Value != -1)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
                else if (pair.Value is long && (long)pair.Value != (long)-1)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
                else if (!(pair.Value is int || pair.Value is long) && pair.Value != null)
                {
                    existingRecordData[pair.Key] = pair.Value;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the ip exists.
        /// </summary>
        /// <param name="ipAddress"> ip address to be checked </param>
        /// <returns> true if ip address exists, otherwise false </returns>
        public bool CheckIPExistence(string ipAddress)
        {
            return IPAddress.ContainsKey(ipAddress);
        }
    }
}
