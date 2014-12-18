using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = -3;
            Console.WriteLine(""false "" + i.IsFour());
            i += 6;
            var b = i.IsFour();
            Console.WriteLine(""true "" + b);
            Utilities.IsFour(5);
        }

        public static bool IsFour(this int i)
        {
            return i == 4;
        }
    }
}