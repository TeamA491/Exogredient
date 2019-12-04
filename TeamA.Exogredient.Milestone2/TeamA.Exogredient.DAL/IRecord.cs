using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public interface IRecord
    {
        IDictionary<string, object> GetData();
    }
}
