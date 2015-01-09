using System;

namespace Foo
{
    public class Bar
    {
        public static explicit operator string(Bar value)
        {
            return "\"blah\"";
        }

        public static void Main()
        {
            var b = new Bar();
            var s = (string)b;
            Console.WriteLine(s);
			Console.WriteLine(b);
	            
        }
        
        public string ToString()
        {
        	return "Foo.Bar";
        }
    }
}