using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// Defines common functionality between SQLRecords, mainly the ability to get the internal data.
    /// </summary>
    public interface ISQLRecord
    {
        IDictionary<string, object> GetData();
    }
}
