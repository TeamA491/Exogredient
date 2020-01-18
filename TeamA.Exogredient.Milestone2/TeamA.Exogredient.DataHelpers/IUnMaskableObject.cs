using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public interface IUnMaskableObject
    {
        Type[] GetParameterTypes();
        List<Tuple<object, bool>> GetMaskInformation();
        void SetToUnMasked();
        bool IsUnMasked();
    }
}
