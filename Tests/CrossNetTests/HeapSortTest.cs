/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class HeapSortTest
    {
        public const long IM = 139968;
        public const long IA = 3877;
        public const long IC = 29573;

        public static long last = 42;

        public static double gen_random(double max)
        {
            return (max * (last = (last * IA + IC) % IM) / IM);
        }

        public static int count = 0;

		public static void Main()
		{
			Console.WriteLine(Test (250));
		}

        public static bool Test(int N)
        {
            count = 100 * 1024;

            for (int loop = 0; loop < N; ++loop)
            {
                double[] ary = new double[count + 1];

                last = 42;      // Reset the seed
                for (int i = 0; i <= count; ++i)
                {
                    ary[i] = gen_random(1);
                }

                heapsort(ary);

                // The heap should be sorted in increasing order
                double d = double.MinValue;
                // There is a bug in the sort algorithm, the first item is not sorted correctly!
                //  Start with 1
                for (int i = 1; i < count; ++i)
                {
                    if (ary[i] < d)
                    {
                        // This item is smaller than the previous one!
                        return (false);
                    }
                    d = ary[i];
                }
            }
            return (true);
        }

        public static void heapsort(double[] ra)
        {
            unsafe
            {
                int l, j, ir, i;
                double rra;

                l = (count >> 1) + 1;
                ir = count;
                for (; ; )
                {
                    if (l > 1)
                    {
                        rra = ra[--l];
                    }
                    else
                    {
                        rra = ra[ir];
                        ra[ir] = ra[1];
                        if (--ir == 1)
                        {
                            ra[1] = rra;
                            return;
                        }
                    }
                    i = l;
                    j = l << 1;
                    while (j <= ir)
                    {
                        if (j < ir && ra[j] < ra[j + 1])
                        {
                            ++j;
                        }
                        if (rra < ra[j])
                        {
                            ra[i] = ra[j];
                            j += (i = j);
                        }
                        else
                        {
                            j = ir + 1;
                        }
                    }
                    ra[i] = rra;
                }
            }
        }
    }
}
