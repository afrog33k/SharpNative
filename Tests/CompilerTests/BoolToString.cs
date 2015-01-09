using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
            var b = true;
            var s = "\"\"" + b;
            s = b + "\"\"";
            s = b.ToString();
            Console.WriteLine(s);
        }
    }
}