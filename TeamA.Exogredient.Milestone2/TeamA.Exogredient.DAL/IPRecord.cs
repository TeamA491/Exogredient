using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DAL
{
    public class IPRecord
    {
        IDictionary<string, string> data = new Dictionary<string, string>();

        public IPRecord(string ip, string timestamp = null)
        {
            data.Add(Constants.IPAddressDAOIPColumn, ip);
            data.Add(Constants.IPAddressDAOTimestampLockedColumn, timestamp);
        }

        public IDictionary<string, string> GetData()
        {
            return data;
        }
    }
}
