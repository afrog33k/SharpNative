using System;

namespace Blargh
{
    public static class Utilities
    {
        [ThreadStatic]
        public static string NoInit;

        [ThreadStatic]
        public static string Init = "initval";
    }
}