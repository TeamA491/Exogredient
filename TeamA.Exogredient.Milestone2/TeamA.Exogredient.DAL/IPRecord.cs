using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public class IPRecord
    {
        IDictionary<string, string> data = new Dictionary<string, string>();

        public IPRecord(string userName, string timestamp = null)
        {
            data.Add("username", userName);
            data.Add("timestamp", timestamp);
        }

        public IDictionary<string, string> GetData()
        {
            return data;
        }
    }
}
