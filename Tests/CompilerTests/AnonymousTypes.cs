using System;
using System.Text;

namespace Blargh
{

    public class Foo
    {
        public static void Main()
        {
            var i = new { Field1 = 3, Field2 = new StringBuilder() };
            Console.WriteLine(i.Field1);
        }
    }
}