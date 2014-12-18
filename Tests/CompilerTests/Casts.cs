using System;

namespace Blargh
{
#if !CSNative
    public static class Utilities
    {
        public static T As<T>(this object o)
        {
            return (T)o;
        }
    }
#endif

    public static class Test
    {
        public static void SomeFunction()
        {
            var a = DateTime.Now.As<String>();
            object o = 4;
            var b = (byte)(short)o;
        }
    }
}