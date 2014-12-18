using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Func<int, int> f1 = x => x + 5;
            Console.WriteLine(f1(3));
            Func<int, int> f2 = x => { return x + 6; };
            Console.WriteLine(f2(3));

            List<Action> actions = new List<Action>();
            actions.Add(() => { });
        }
    }
}