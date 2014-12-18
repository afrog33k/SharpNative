using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            string s = "Blah";
            var list = new List<int>();
            if (s is string)
                Console.WriteLine("Yes");
            if (list is List<int>)
                Console.WriteLine("Yes");

            object o = s;
            string sss = o as string;
            Console.WriteLine(sss);
        }
    }
}