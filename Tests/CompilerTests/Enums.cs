using System;

namespace Blargh
{
    public enum MostlyNumbered
    {
        One = 1,
        Two = 2,
        Three = 3,
        Unnumbered,
        SomethingElse = 50
    }
    public enum UnNumbered
    {
        One, Two, Three
    }
    static class Clazz
    {
        public static void Methodz()
        {
            var f = MostlyNumbered.One;
            var arr = new UnNumbered[] { UnNumbered.One, UnNumbered.Two, UnNumbered.Three };
            var i = (int)f;
            var e = (MostlyNumbered)Enum.Parse(typeof(MostlyNumbered), ""One"");
            var s = e.ToString();
            s = e + ""asdf"";
            s = ""asdf"" + e;
            MostlyNumbered? n = MostlyNumbered.Two;
            var s2 = n.ToString();
            s2 = ""asdf"" + n;
            var vals = Enum.GetValues(typeof(MostlyNumbered));
        }
    }
}