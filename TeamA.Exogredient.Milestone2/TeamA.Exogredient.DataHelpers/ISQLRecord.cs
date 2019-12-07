using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    public interface ISQLRecord
    {
        IDictionary<string, object> GetData();
    }
}
