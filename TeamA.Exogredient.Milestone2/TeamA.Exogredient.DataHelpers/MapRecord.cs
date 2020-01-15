﻿using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class MapRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public MapRecord(string hash, string actual)
        {
            _data.Add(Constants.MapDAOHashColumn, hash);
            _data.Add(Constants.MapDAOActualColumn, actual);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}