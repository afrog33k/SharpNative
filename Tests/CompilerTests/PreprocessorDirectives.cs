using System;

namespace Blargh
{
    public class SomeClass
    {
#if CSNative
        Some raw text here;
#endif

        public SomeClass()
        {
#if CSNative
            Console.WriteLine(""CsNative1"");
#else
            Console.WriteLine("not1");
#endif
#if CSNative //comment
            Console.WriteLine("CsNative12");
#else

            Console.WriteLine("not2");
#if nope
            Console.WriteLine("CsNative13");
#endif

#endif
            Console.WriteLine("outside");

#if CSNative
            Console.WriteLine("CsNative14");
#endif

            if (true)
            {
#if CSNative
            Console.WriteLine("In if");
#endif

            }
        }
    }
}