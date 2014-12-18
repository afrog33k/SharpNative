/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    class VirtualTest
    {
        class BaseClass
        {
            public virtual int Function(int a)
            {
                return (-1);
            }
        }

        class MyClass : BaseClass
        {
            public override int Function(int a)
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
            BaseClass myClass0 = new MyClass();
            BaseClass myClass1 = new MyClass();
            BaseClass myClass2 = new MyClass();
            BaseClass myClass3 = new MyClass();
            BaseClass myClass4 = new MyClass();
            BaseClass myClass5 = new MyClass();
            BaseClass myClass6 = new MyClass();
            BaseClass myClass7 = new MyClass();
            BaseClass myClass8 = new MyClass();
            BaseClass myClass9 = new MyClass();
            BaseClass myClassA = new MyClass();
            BaseClass myClassB = new MyClass();
            BaseClass myClassC = new MyClass();
            BaseClass myClassD = new MyClass();
            BaseClass myClassE = new MyClass();
            BaseClass myClassF = new MyClass();

            for (int i = 0; i < N; ++i)
            {
                for (int k = 0; k < 1000 * 1000; ++k)
                {
                    // Note that this benchmark will spend a lot of time in the loop code
                    //  This actually will make it a closer of a real code
                    //  (i.e. we are never doing 1 million interface call in a row, there is always
                    //  something else around. So I guess this would be a good demonstration of a real worst case).
                    int result = myClass0.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass1.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass2.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass3.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass4.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass5.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass6.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass7.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass8.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClass9.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassA.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassB.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassC.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassD.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassE.Function(42);
                    if (result != 42)
                    {
                        return (false);
                    }

                    result = myClassF.Function(42);
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
