using System;
using System.Linq;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int[] e = new int[] { 0, 1, 2, 3 };
            Console.WriteLine(e.First());
            Console.WriteLine(e.First(o => o == 1));
            Console.WriteLine(e.ElementAt(2));
            Console.WriteLine(e.Last());
            Console.WriteLine(e.Select(o => o).Count());
            Console.WriteLine(e.Where(o => o > 0).Count() + 2);
            Console.WriteLine(e.Count(o => true) + 2);

            var dict = e.ToDictionary(o => o, o => 555);
            e.OfType<int>();
            e.OrderBy(o => 4);
            e.OrderBy(o => ""z"");
        }
    }
}