using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public interface IMasterFlatFileDAO
    {
        void Create(object record, string folderName, string fileName);

        void Delete(object record, string folderName, string fileName);
    }
}
