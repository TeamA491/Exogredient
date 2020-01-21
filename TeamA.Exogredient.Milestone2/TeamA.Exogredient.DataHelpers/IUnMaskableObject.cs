using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// Defines the methods needed for an unmaskable object.
    /// </summary>
    public interface IUnMaskableObject
    {
        Type[] GetParameterTypes();
        List<Tuple<object, bool>> GetMaskInformation();
        void SetToUnMasked();
        bool IsUnMasked();
    }
}
