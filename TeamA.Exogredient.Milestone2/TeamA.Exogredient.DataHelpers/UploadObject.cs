using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadObject : IDataObject, IUnMaskableObject
    {
        public List<Tuple<object, bool>> GetMaskInformation()
        {
            throw new NotImplementedException();
        }

        public Type[] GetParameterTypes()
        {
            throw new NotImplementedException();
        }

        public bool IsUnMasked()
        {
            throw new NotImplementedException();
        }

        public void SetToUnMasked()
        {
            throw new NotImplementedException();
        }
    }
}
