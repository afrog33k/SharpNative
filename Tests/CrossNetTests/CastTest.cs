/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{

    class CastTest
    {
        interface I1
        {
        }

        class C1 : I1
        {
        }

        interface I2 : I1
        {
        }

        class C2 : C1, I2
        {
        }

        interface I3 : I2
        {
        }

        class C3 : C2, I3
        {
        }

        interface I4 : I3
        {
        }

        class C4 : C3, I4
        {
        }

        interface I5 : I4
        {
        }

        class C5 : C4, I5
        {
        }

        interface I6 : I5
        {
        }

        class C6 : C5, I6
        {
        }

        interface I7 : I6
        {
        }

        class C7 : C6, I7
        {
        }

        interface I8 : I7
        {
        }

        // Derives from 8 interfaces, 7 implementations
        //  The cast comparison should be going slower and slower...
        class C8 : C7, I8
        {
        }

          public static void Main()
            {
            Console.WriteLine(Test(66));
            }

        public static bool Test(int N)
        {
            object o = new C8();

            for (int i = 0; i < N; ++i)
            {
                for (int k = 0; k < 1000 * 1000; ++k)
                {
                    if (o is I1 == false)
                    {
                        return (false);
                    }
                    if (o is I2 == false)
                    {
                        return (false);
                    }
                    if (o is I3 == false)
                    {
                        return (false);
                    }
                    if (o is I4 == false)
                    {
                        return (false);
                    }
                    if (o is I5 == false)
                    {
                        return (false);
                    }
                    if (o is I6 == false)
                    {
                        return (false);
                    }
                    if (o is I7 == false)
                    {
                        return (false);
                    }
                    if (o is I8 == false)
                    {
                        return (false);
                    }


                    if (o is C1 == false)
                    {
                        return (false);
                    }
                    if (o is C2 == false)
                    {
                        return (false);
                    }
                    if (o is C3 == false)
                    {
                        return (false);
                    }
                    if (o is C4 == false)
                    {
                        return (false);
                    }
                    if (o is C5 == false)
                    {
                        return (false);
                    }
                    if (o is C6 == false)
                    {
                        return (false);
                    }
                    if (o is C7 == false)
                    {
                        return (false);
                    }
                    if (o is C8 == false)
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }
    }
}
