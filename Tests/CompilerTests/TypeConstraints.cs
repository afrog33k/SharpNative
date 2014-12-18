using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction<T>() where T : class, IComparable<T>
        {
        }
    }
}