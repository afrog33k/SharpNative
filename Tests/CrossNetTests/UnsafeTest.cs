/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class UnsafeTest
    {
        const int SIZE = 10000;

        public static void Main() 
  {
    Console.WriteLine(Test(750000));
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
                unsafe
                {
                    fixed (int * p = array)
                    {
                        int* buffer = p;
                        int count = array.Length;
                        while (count-- != 0)
                        {
                            counter += *buffer;
                            ++buffer;
                        }
                    }
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
