using System;
using System.IO;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
            var usingMe = new MemoryStream();
            using (usingMe)
            {
                Console.WriteLine("In using");
                return;
            }
        }
    }
}