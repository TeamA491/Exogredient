using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.DataHelpers
{
    public interface IMaskableRecord
    {
        Type[] GetParameterTypes();
        List<Tuple<object, bool>> GetMaskInformation();
    }
}
