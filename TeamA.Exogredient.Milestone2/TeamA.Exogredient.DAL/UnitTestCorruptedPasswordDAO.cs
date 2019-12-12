using System;
using System.Collections.Generic;

namespace TeamA.Exogredient.DAL
{
    public class UnitTestCorruptedPasswordDAO
    {
        private readonly List<string> corruptedPassword;

        public UnitTestCorruptedPasswordDAO()
        {
            corruptedPassword = new List<string>()
            {
                   "7C4A8D09CA3762AF61E59520943DC26494F8941B",
                    "F7C3BC1D808E04732ADF679965CCC34CA7AE3441",
                    "B1B3773A05C0ED0176787A4F1574FF0075F7521E",
                    "5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8",
                    "3D4F2BF07DC1BE38B20CD6E46949A1071F9D0E3D",
                    "7C222FB2927D828AF22F592134E8932480637C0D",
                    "6367C48DD193D56EA7B0BAAD25B19455E529F5EE",
                    "20EABE5D64B0E216796E834F52D61FD0B70332FC",
                    "E38AD214943DAAD1D64C102FAEC29DE4AFE9DA3D",
                    "8CB2237D0679CA88DB6464EAC60DA96345513964",
                    "01B307ACBA4F54F55AAFC33BB06BBBF6CA803E9A",
                    "601F1889667EFAEBB33B8C12572835DA3F027F78",
                    "C984AED014AEC7623A54F0591DA07A85FD4B762D",
                    "EE8D8728F435FD550F83852AABAB5234CE1DA528",
                    "7110EDA4D09E062AA5E4A390B0A572AC0D2C0220",
                    "B80A9AED8AF17118E51D4D0C2D7872AE26E2109E",
                    "B0399D2029F64D445BD131FFAA399A42D2F8E7DC",
                    "40BD001563085FC35165329EA1FF5C5ECBDBBEEF",
                    "AB87D24BDC7452E55738DEB5F868E1F16DEA5ACE",
                    "AF8978B1797B72ACFFF9595A5A2A373EC3D9106D"
            };
        }
            
        public List<string> Read()
        {
            return new List<string>(corruptedPassword);
        }
    }
}
