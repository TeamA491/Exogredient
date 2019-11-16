using System;

namespace TeamA.Exogredient.TestRecord
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
