using System;

namespace Blargh
{
    public class SomeClass
    {
        public static void Foo(int i1, int i2 = 4, string s1 = "\"hi\"")
        {
        	Console.WriteLine(i1);
			Console.WriteLine(i2);
			Console.WriteLine(s1);			
        } 

        public static void Main() 
        {
            Foo(4);
            Foo(5, 6);
            Foo(6, 7, "\"eight\"");
        }
    }
}