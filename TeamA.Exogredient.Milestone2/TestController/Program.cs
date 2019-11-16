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

            TestRecord tr = new TestRecord(id: 32, tc: null);

            // THIS IS HOW YOU USE A DATA ACCESS OBJECT WITH A RECORD OBJECT

            //td.Create(tr);

            //td.DeleteByIDs(new List<int>() { 1 });

            //Console.WriteLine(string.Join("||",(td.ReadByIDs(new List<int>() { 2,3 })).ToArray()));

            td.Update(42, tr);
        }
    }
}
