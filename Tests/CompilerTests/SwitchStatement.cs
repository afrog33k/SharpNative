using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
            string s = "Blah";
            switch (s)
            {
                case "NotMe": Console.WriteLine(5); break;
                case "Box": Console.WriteLine(4); break;
                case "Blah": 
                case "Blah2": Console.WriteLine(3); break;
                default: throw new InvalidOperationException();
            }
        }
    }
}