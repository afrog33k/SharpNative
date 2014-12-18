/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class ForeachOnArrayTest
    {
        const int SIZE = 10000;


public static void Main()
{
			Test(1250000);
}
        public static bool Test(int N)
        {
            int[] array = new int[SIZE];
            for (int j = 0; j < SIZE; ++j)
            {
                array[j] = j;
            }

            for (int i = 0; i < N; ++i)
            {
                int counter = 0;
                foreach (int value in array)
                {
                    counter += value;
                }

                if (counter != (SIZE * (SIZE - 1)) / 2)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
