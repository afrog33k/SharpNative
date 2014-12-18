/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class RecursiveTest
    {

		public static void Main()
		{
			Console.WriteLine(Test (1));
		}


        public static bool Test(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                int iResult = Ack(3, 10);
                if (iResult != 8189)
                {
                    return (false);
                }

                iResult = Fib(40);
                if (iResult != 102334155)
                {
                    return (false);
                }

                iResult = Tak(30, 20, 10);
                if (iResult != 11)
                {
                    return (false);
                }

                double dResult = Fib(40.0);
                if (dResult != 102334155.0)
                {
                    return (false);
                }

                dResult = Tak(30.0, 20.0, 10.0);
                if (dResult != 11.0)
                {
                    return (false);
                }
            }
            return (true);
        }

        public static int Ack(int m, int n)
        {
            if (m == 0)
            {
                return n + 1;
            }
            if (n == 0)
            {
                return Ack(m - 1, 1);
            }
            else
            {
                return Ack(m - 1, Ack(m, n - 1));
            }
        }

        public static int Fib(int n)
        {
            if (n < 2)
            {
                return n;
            }
            else
            {
                return Fib(n - 2) + Fib(n - 1);
            }
        }

        public static int Tak(int x, int y, int z)
        {
            if (y < x)
            {
                return Tak(Tak(x - 1, y, z), Tak(y - 1, z, x), Tak(z - 1, x, y));
            }
            else
            {
                return z;
            }
        }

        public static double Fib(double n)
        {
            if (n < 2.0)
            {
                return n;
            }
            else
            {
                return Fib(n - 2.0) + Fib(n - 1.0);
            }
        }

        public static double Tak(double x, double y, double z)
        {
            if (y < x)
            {
                return Tak(Tak(x - 1.0, y, z), Tak(y - 1.0, z, x), Tak(z - 1.0, x, y));
            }
            else
            {
                return z;
            }
        }
    }
}
