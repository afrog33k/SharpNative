using System;

namespace Blargh
{
    public class Top
    {
        public Top(int i) { }
    }

    public class Derived : Top
    {
        public Derived() : base(4) { }
    }
}