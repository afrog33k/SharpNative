using System;

namespace CsNative
{
    public static class Utilities
    {
        public static void Main()
        {
            string notInitialized;
            int myNum = 0;
            notInitialized = "InitMe!";

            if (myNum > 4)
                myNum = 2;
            else if (notInitialized == "asdf")
                myNum = 1;
            else
                myNum = 999;

            Console.WriteLine(myNum == 999 ? "One" : "Two");
        }
    }
}