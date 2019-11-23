using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DAL
{
    public class TestRecord
    {
        public int Id { get; }
        public string TestColumn { get; }

        public TestRecord(int id = -1, string tc = "")
        {
            Id = id;
            TestColumn = tc;
        }
    }
}
