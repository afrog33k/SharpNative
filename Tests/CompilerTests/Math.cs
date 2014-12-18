using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = 3;
            i += 4;
            i -= 3;
            i *= 4;
            i %= 3;
            i = i + 1;
            i = i % sizeof(int);
            i = i - sizeof(byte);
            i = i * 100;
            double f = i / 3f;
            int hex = 0x00ff;
            i = (int)f;
            var z = (i & hex) == 5;
            var x = (int)(i / 3);
        }
    }
}