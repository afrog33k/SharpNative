using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void Foo(byte i){ }
        public static void Foo(short i){ }
        public static void Foo(int i){ }
        public static void Foo(long i){ }
        public static void Foo(float i){ }
        public static void Foo(double i){ }
        public static void Foo(ushort i){ }
        public static void Foo(uint i){ }
        public static void Foo(ulong i){ }

        static Utilities()
        {
            byte b = 1;
            short s = 1;
            int i = 1;
            long l = 1;
            float f = 1;
            double d = 1;
            ushort us = 1;
            uint ui = 1;
            ulong ul = 1;
            Foo(b); Foo(s); Foo(i); Foo(l); Foo(f); Foo(d); Foo(us); Foo(ui); Foo(ul);
        }
    }
}