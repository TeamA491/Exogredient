using System;
using TeamA.Exogredient.TestDAO;
using TeamA.Exogredient.TestRecord;

namespace TestController
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TestDAO td = new TestDAO();

            TestRecord tr = new TestRecord(tc: "test2");

            td.Create(tr);
        }
    }
}
