using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class UploadRecord : ISQLRecord, IMaskableRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();
        private bool _masked = false;

        public IDictionary<string, object> GetData()
        {
            throw new NotImplementedException();
        }

        public List<Tuple<object, bool>> GetMaskInformation()
        {
            throw new NotImplementedException();
        }

        public Type[] GetParameterTypes()
        {
            throw new NotImplementedException();
        }

        public bool IsMasked()
        {
            throw new NotImplementedException();
        }

        public void SetToMasked()
        {
            throw new NotImplementedException();
        }
    }
}
