/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class InterfaceTest
    {
        interface IMyInterface
        {
            int Function(int a);
        }

        class MyClass : IMyInterface
        {
            public int Function(int a)
            {
                return (a);
            }
        }

		public static void Main()
		{
			Console.WriteLine(Test (100));
		}

        public static bool Test(int N)
        {
            IMyInterface myInterface0 = new MyClass();
            IMyInterface myInterface1 = new MyClass();
            IMyInterface myInterface2 = new MyClass();
            IMyInterface myInterface3 = new MyClass();
            IMyInterface myInterface4 = new MyClass();
            IMyInterface myInterface5 = new MyClass();
            IMyInterface myInterface6 = new MyClass();
            IMyInterface myInterface7 = new MyClass();
            IMyInterface myInterface8 = new MyClass();
            IMyInterface myInterface9 = new MyClass();
            IMyInterface myInterfaceA = new MyClass();
            IMyInterface myInterfaceB = new MyClass();
            IMyInterface myInterfaceC = new MyClass();
            IMyInterface myInterfaceD = new MyClass();
            IMyInterface myInterfaceE = new MyClass();
            IMyInterface myInterfaceF = new MyClass();

            for (int i = 0; i < N; ++i)
            {
                for (int k = 0; k < 1000 * 1000; ++k)
                {
                    int result;

                    // Note that this benchmark will spend a lot of time in the loop code
                    //  This actually will make it a closer of a real code
                    //  (i.e. we are never doing 1 million interface call in a row, there is always
                    //  something else around. So I guess this would be a good demonstration of a real worst case).
                    result = myInterface0.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface1.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface2.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface3.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface4.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface5.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface6.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface7.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface8.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterface9.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceA.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceB.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceC.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceD.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceE.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myInterfaceF.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }
    }
}
