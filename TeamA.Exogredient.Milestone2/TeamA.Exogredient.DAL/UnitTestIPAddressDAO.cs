using System;
using System.Collections.Generic;
using System.Data;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DataHelpers;

namespace TeamA.Exogredient.DAL
{
    /// <summary>
    /// DAO for the data store containing IP Address information.
    /// </summary>
    public class UnitTestIPAddressDAO
    {
        // Datastore for IP Addresses.
        Dictionary<string, IPAddressRecord> IPAddress;

        public UnitTestIPAddressDAO()
        {
            IPAddress = new Dictionary<string, IPAddressRecord>();
        }


        /// <summary>
        /// Creates the <paramref name="record"/> in the data store.
        /// </summary>
        /// <param name="record">The record to insert (ISQLRecord)</param>
        /// <returns> bool whether the function executed without exception.</returns>
        public bool Create(ISQLRecord record)
        {
            // Convert the record to IPAddressRecord
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
                // Check for null values in the data (string == null, numeric == -1), and throw a NoNullAllowedException
                // if one is found.
                if (pair.Value is int)
                {
                    if ((int)pair.Value == -1)
                    {
                        throw new NoNullAllowedException(Constants.IPRecordNoNull);
                    }
                }
                if (pair.Value is string)
                {
                    if (pair.Value == null)
                    {
                        throw new NoNullAllowedException(Constants.IPRecordNoNull);
                    }
                }
                if (pair.Value is long)
                {
                    if ((long)pair.Value == -1)
                    {
                        throw new NoNullAllowedException(Constants.IPRecordNoNull);
                    }
                }
            }

            // Add to the datastore
            IPAddress.Add((string)recordData[Constants.AnonymousUserDAOIPColumn], ipRecord);

            return true;
        }


        /// <summary>
        /// Delete all the objects referenced by the <paramref name="idsOfRows"/>.
        /// </summary>
        /// <param name="idsOfRows">The list of ids of rows to delete (List(string))</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public bool DeleteByIds(List<string> idsOfRows)
        {
            foreach (string ip in idsOfRows)
            {
                // Check if the IP exists.
                if (!CheckIPExistence(ip))
                {
                    throw new ArgumentException(Constants.IPDeleteDNE);
                }
                // Remove the IP from the datastore.
                IPAddress.Remove(ip);
            }

            return true;
        }

        /// <summary>
        /// Read the information in the data store pointed to by the <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the row to read (string)</param>
        /// <returns>(IDataObject) the information represented as an object</returns>
        public IDataObject ReadById(string id)
        {
            // Check if the IP exists.
            if (!CheckIPExistence(id))
            {
                throw new ArgumentException(Constants.IPDeleteDNE);
            }
            // Extract the data of the IP Address.
            IDictionary<string, object> data = IPAddress[id].GetData();

            // Return an IP object that contains the data.
            return new IPAddressObject((string)data[Constants.AnonymousUserDAOIPColumn],
                                       (long)data[Constants.AnonymousUserDAOtimestampLockedColumn],
                                       (int)data[Constants.AnonymousUserDAOregistrationFailuresColumn],
                                       (long)data[Constants.AnonymousUserDAOlastRegFailTimestampColumn]);
        }

        /// <summary>
        /// Update the <paramref name="record"/> in the data store based on the values that are not null inside it.
        /// </summary>
        /// <param name="record">The record containing the information to update (ISQLRecord)</param>
        /// <returns>(bool) whether the function executed without exception</returns>
        public bool Update(ISQLRecord record)
        {
            // Convert the record to IPAddressRecord.
            IPAddressRecord ipRecord;
            try
            {
                ipRecord = (IPAddressRecord)record;
            }
            catch
            {
                throw new ArgumentException(Constants.IPUpdateInvalidArgument);
            }

            // Extract the data of the record.
            IDictionary<string, object> newRecordData = ipRecord.GetData();
            // Get the IP Address of the record.
            string ipAddress = (string)newRecordData[Constants.AnonymousUserDAOIPColumn];

            // Check if the IP Address exists.
            if (!CheckIPExistence(ipAddress))
            {
                throw new ArgumentException(Constants.IPUpdateDNE);
            }

            // Extract the data of the corresponding exisitng record.
            IDictionary<string, object> existingRecordData = IPAddress[ipAddress].GetData();

            foreach (KeyValuePair<string, object> pair in newRecordData)
            {
                // Update only the values where the record value is not null (string == null, numeric == -1).
                if (pair.Value is int)
                {
                    if ((int)pair.Value != -1)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
                if (pair.Value is string)
                {
                    if (pair.Value != null)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
                if (pair.Value is long)
                {
                    if ((long)pair.Value != -1)
                    {
                        existingRecordData[pair.Key] = pair.Value;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the <paramref name="ipAddress"/> exists.
        /// </summary>
        /// <param name="ipAddress"> ip address to be checked </param>
        /// <returns> true if ip address exists, otherwise false </returns>
        public bool CheckIPExistence(string ipAddress)
        {
            return IPAddress.ContainsKey(ipAddress);
        }
    }
}
