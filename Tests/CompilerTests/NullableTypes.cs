using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int? nullableInt = new Nullable<int>();
            float d = 3;
            var cond = nullableInt.HasValue ? (float?)null : ((float)d);
            Console.WriteLine(nullableInt.HasValue);
            int? withValue = new Nullable<int>(8);
            Console.WriteLine(withValue.Value);
            int? implicitNull = null;
            implicitNull = null;
            int? implicitValue = 5;
            implicitValue = 8;
            Foo(3);
            int? n = (int?)null;
			n.ToString();
            var s = ""hi "" + n;
        }

        public static int? Foo(int? i)
        {
            return 4;
        }
    }
}