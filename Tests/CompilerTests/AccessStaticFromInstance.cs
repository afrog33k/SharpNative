using System;

namespace Blargh
{
    public class Foo
    {
        static int StaticField = 1;
        static void StaticMethod() { }
        public void Method()
        {
            Console.WriteLine(StaticField);
            StaticMethod();
        }
    }
}