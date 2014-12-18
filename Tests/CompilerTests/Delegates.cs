using System;

namespace Blargh
{
    public delegate int NamespaceDlg();
    public delegate T TemplatedDelegate<T>(T arg, int arg2);

    public static class Utilities
    {
        public static Action StaticAction;
        public delegate int GetMahNumber(int arg);

        public static void SomeFunction(GetMahNumber getit, NamespaceDlg getitnow, TemplatedDelegate<float> unused)
        {
            Console.WriteLine(getit(getitnow()));
            var a = new[] { getitnow };
            a[0]();
            StaticAction();
            Utilities.StaticAction();
            Blargh.Utilities.StaticAction();
        }
    }
}