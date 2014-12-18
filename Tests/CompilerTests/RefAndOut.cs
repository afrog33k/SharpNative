using System;
using System.Text;
        
namespace Blargh
{
    public class Foo
    {
        public Foo()
        {
            int x;
            TestOut(out x);
            x = 3;
            var s = x.ToString();
            int i = 1;
            TestRef(ref i);
            i = 5;
            new StringBuilder(i);
            Func<int> fun = () => x;
        }
        
        public void TestRef(ref int i)
        {
            var sb = new StringBuilder(i);
            i = 4;
        }
        public void TestOut(out int i)
        {
            i = 4;
            var sb = new StringBuilder(i);
        }
        
    }
}