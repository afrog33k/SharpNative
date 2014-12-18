using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Action<bool, bool> z = (a, b) => 
            {
                Console.WriteLine(1);
                if (a && b)
                    return;
                Console.WriteLine(2);
                if (a)
                {
                    Console.WriteLine(3);
                    return;
                }
                Console.WriteLine(4);
            };
        }
    }
}