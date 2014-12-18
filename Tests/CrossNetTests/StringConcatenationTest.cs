/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class StringConcatenationTest
    {
        const int SIZE = 10000;

            public static void Main() 
  {
    Console.WriteLine(Test(160));
  }
        public static bool Test(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                string str = i.ToString();
                for (int j = 0; j < SIZE; ++j)
                {
                    str += "_";
                }

                if (str.Length != SIZE + i.ToString().Length)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
