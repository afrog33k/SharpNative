using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
            var ar = new int[] { 1, 2, 3 };

            foreach(var i in ar)
                Console.WriteLine(i);

            Console.WriteLine(ar[1]);
            Console.WriteLine(ar.Length);
            Console.WriteLine(new List<string>().Count);
        }
    }
}