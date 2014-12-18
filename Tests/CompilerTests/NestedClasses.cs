using System;
using System.Text;

namespace Blargh
{

    public class Outer
    {
        public class Inner
        {
            public int InnerField;
            public Inner()
            {
                InnerField = 9;
            }
        }

        public Outer()
        {
            var i = new Inner();
            i.InnerField = 4;
        }
    }
}