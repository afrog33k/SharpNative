using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
        	
        	Console.WriteLine(" hey " + 0);
        	
            Console.WriteLine("hello-".GetHashCode());
            Console.WriteLine("hell".GetHashCode());
            Console.WriteLine("hekllo".GetHashCode());
            Console.WriteLine("yo lo".GetHashCode());

            if ("hello" == "hello")
            {
                Console.WriteLine("they are the same");
            }

            var test = "abc";

            switch (test)
            {
                case "yolo":
                    Console.WriteLine();
                    break;
                case "abc":
                    Console.WriteLine("abc");
                    break;

            }


            string s = @"50\0";
            Console.WriteLine(s.IndexOf("0"));
           

            foreach(string s3 in new string[] { "Hello" })
                s3.Substring(4, 5);

            int i = 4;
            string si = i.ToString();
            if (si.StartsWith("asdf"))
                Console.WriteLine(4);
        }
    }
}
