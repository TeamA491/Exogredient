using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// Defines the methods needed for a maskable record.
    /// </summary>
    public interface IMaskableRecord
    {
        Type[] GetParameterTypes();
        List<Tuple<object, bool>> GetMaskInformation();
        void SetToMasked();
        bool IsMasked();
    }
}
