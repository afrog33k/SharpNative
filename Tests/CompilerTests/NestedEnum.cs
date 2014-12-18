namespace Blargh
{
    class Foo
    {
        public enum TestEnum
        {
            One, Two, Three
        }

        public Foo()
        {
            var i = TestEnum.One;
            i.ToString();
        }
    }
}