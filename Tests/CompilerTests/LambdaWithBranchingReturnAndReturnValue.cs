using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Func<bool, bool, int> z = (a, b) => 
            {
                Console.WriteLine(1);
                if (a && b)
                    return 1;
                Console.WriteLine(2);
                if (a)
                {
                    Console.WriteLine(3);
                    return 2;
                }
                Console.WriteLine(4);
                return 3;
            };
        }
    }
}