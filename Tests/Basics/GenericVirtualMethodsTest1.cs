using System;
using System.Text;

internal class Program
{
   // [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Test()
    {
        var b = GetA();
        b.GenericVirtual<string>();
        b.GenericVirtual<int>();
        b.GenericVirtual<StringBuilder>();
        b.GenericVirtual<int>();
        b.GenericVirtual<StringBuilder>();
        b.GenericVirtual<string>();
        b.NormalVirtual();
    }

  //  [MethodImpl(MethodImplOptions.NoInlining)]
    private static IBase GetA()
    {
        return new B();
    }

    interface IBase
    {
        void GenericVirtual<T>();
        void NormalVirtual();
    }

    private class A: IBase
    {
        public virtual void GenericVirtual<T>()
        {
              Console.WriteLine("GenericVirtual - A");
        }

        public virtual void NormalVirtual()
        {
              Console.WriteLine("NormalVirtual - A");
        }
    }

    private class B : A
    {
        public override void GenericVirtual<T>()
        {
            base.GenericVirtual<T>();
            Console.WriteLine("Generic virtual: {0}", typeof(T).FullName);
        }

        public override void NormalVirtual()
        {
            base.NormalVirtual();
            Console.WriteLine("Normal virtual");
        }
    }

    public static void Main(string[] args)
    {
    
        Test();
        Test();
    }
}