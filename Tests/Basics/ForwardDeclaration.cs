using System;
using System.Collections.Generic;
using System.Text;

namespace ForwardDeclaration
{
class ClassA
    {
        ClassB b;
        public ClassA()
        {
            b = new ClassB(this);
        }

        public string SayHello()
        {
            return b.SayHello();
        }
    }

    class ClassB
    {        
        ClassA a;

        public ClassB(ClassA a)
        {
            this.a = a;
        }

        public string SayHello()
        {
            return "Hello I am B";
        }
    }


    class Program
    {
        static void Main()
        {
            Program p = new Program();
            p.Run();
        }

        public void Run()
        {
            ClassA a = new ClassA();
            string hello = a.SayHello();
            Console.WriteLine(hello);
        }
    }
}
