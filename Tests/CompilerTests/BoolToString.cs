using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void Foo()
        {
            var b = true;
            var s = """" + b;
            s = b + """";
            s = b.ToString();
        }
    }
}