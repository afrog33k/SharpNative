using System;
using System.Text;

namespace Blargh
{

    public class Foo
    {
        public void Bar(int a, int b, int c, int d = 3)
        {
        }

        public Foo()
        {
            Bar(1,2,3,4);
            Bar(1,2,3);
            Bar(a: 1, b: 2, c: 3, d: 4);
            Bar(a: 1, b: 2, c: 3);
            Bar(a: 1, c: 3, b: 2);
            Bar(1, c: 3, b: 2);
            Bar(1, 2, c: 3, d: 4);
        }
    }
}