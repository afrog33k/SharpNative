using System;

namespace Blargh
{
    public static class Utilities
    {
        static int Foo;
        public static void Main()
        {
            Blargh.Utilities.Foo = 4;
            Console.WriteLine(int.MaxValue);
            Console.WriteLine(int.MinValue);
            Console.WriteLine(short.MaxValue);
            Console.WriteLine(short.MinValue);
            Console.WriteLine(ushort.MaxValue);
            Console.WriteLine(ushort.MinValue);
            Console.WriteLine(uint.MaxValue);
            Console.WriteLine(uint.MinValue);
            string s = ""123"";
            Console.WriteLine(int.Parse(s) + 1);
            float.Parse(s);
            double.Parse(s);
        }
    }
}