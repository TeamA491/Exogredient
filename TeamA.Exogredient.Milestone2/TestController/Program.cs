using System;
using System.Collections.Generic;
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

            TestRecord tr = new TestRecord(tc: "test3");

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            Console.WriteLine(string.Join("||",(td.ReadByIDs(new List<int>() { 2,3 })).ToArray()));
        }
    }
}
