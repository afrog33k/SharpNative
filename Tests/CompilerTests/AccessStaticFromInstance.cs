using System;

namespace Blargh
{
    public class Foo
    {
        static int StaticField = 1;
        static void StaticMethod() 
        { 
			Console.WriteLine("Static Method");
        }
        public  static void Main()
        {
            Console.WriteLine(StaticField);
            StaticMethod();
        }
    }
}