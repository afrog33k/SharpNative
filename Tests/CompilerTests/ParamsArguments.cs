using System;

namespace Foo
{
    public static class Bar
    {
        public static void Method1(params int[] p)
        {
        }
        public static void Method2(int i, params int[] p)
        {
        }
        public static void Method3(int i, int z, params int[] p)
        {
        }

        public static void Foo()
        {
            Method1(1);
            Method1(1, 2);
            Method1(1, 2, 3);
            Method1(1, 2, 3, 4);
            Method2(1);
            Method2(1, 2);
            Method2(1, 2, 3);
            Method2(1, 2, 3, 4);
            Method3(1, 2);
            Method3(1, 2, 3);
            Method3(1, 2, 3, 4);

        }
    }
}