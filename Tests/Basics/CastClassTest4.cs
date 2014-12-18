//CastClass Test 4 based on 2 - Passes, but interface support are not yet written, so fails,
// we are using new here but with a different return type ... fails

using System;

namespace CsNativeVisual.Tests
{
    interface Bah
    {
        B H();
    }
    class A : Bah
    {
        public int F() { return 1; }
        public virtual int G() { return 2; }
        public B H() { return new B(); }
    }
    class B : A
    {
        public new int F() { return 3; }
        public override int G() { return 4; }
        public new int H() { return 11; }

        public override string ToString()
        {
            return "B"; // We do this as namespace/modules in d differ from C# (Should probably make the compiler generate default ToStrings for classes that dont override ?)
        }
    }
    class Test
    {
        static public void Main()
        {
            int result = 0;
            B b = new B();
            A a = b;

            Console.WriteLine(a.H());
            Console.WriteLine(((A)b).H());
            Console.WriteLine(((B)a).H());

            Console.WriteLine(result);
        }
    };
};
