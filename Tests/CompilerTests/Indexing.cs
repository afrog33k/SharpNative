using System;
using System.Collections.Generic;

namespace Blargh
{
    public class Foo
    {
        public Foo()
        {
            var dict = new Dictionary<int, int>();
            dict[3] = 4;
            var i = dict[3];
            var array = new int[3];
            array[0] = 1;
            var str = "hello";
            var c = str[2];
            var list = new List<int>();
            i = list[0];
        }
    }
}