using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    public class StoreObject: IDataObject
    {
        public int StoreId { get; }
        public string Owner { get; }
        public string StoreName { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public string StoreDescription { get; }
    }
}
