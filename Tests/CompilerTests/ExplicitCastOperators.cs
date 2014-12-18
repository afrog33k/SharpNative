using System;

namespace Foo
{
    public class Bar
    {
        public static explicit operator string(Bar value)
        {
            return ""blah"";
        }

        public static void Foo()
        {
            var b = new Bar();
            var s = (string)b;
    
        }
    }
}