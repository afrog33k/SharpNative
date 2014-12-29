//Partial classes test
using System;

namespace CSharpPlayground
{

    public partial class MyPartial
    {
        public int Foo { get; set; }
    }

   public partial class MyPartial
    {
        public int Bar { get; set; }
    }

    public class PartialExample
    {
        public MyPartial foobar = new MyPartial();

        public PartialExample()
        {
            foobar.Foo = 1;
            foobar.Bar = 2;
        }

        static void Main()
        {
            var part =  new PartialExample();
            Console.WriteLine(part.foobar.Foo + part.foobar.Bar);
        }
    }

   
}
