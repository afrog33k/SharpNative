using System;

namespace Blargh
{
    public partial class Foo
    {
        partial void NoOther();
        partial void Other();
    }

    partial class Foo
    {
        partial void Other()
        {
            Console.WriteLine();
        }
    }
}