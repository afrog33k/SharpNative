using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = 3;
            Action a = () => i = 4;
            Func<int> b = () => i;
            Foo(() => i = 6);
        }
        public static void Foo(Action a)
        {
        }
    }
}