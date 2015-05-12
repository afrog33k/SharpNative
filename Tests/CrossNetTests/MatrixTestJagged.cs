/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class MatrixTest
    {
        static int SIZE = 30;

        public static int[][] mkmatrix(int rows, int cols)
        {
            int count = 1;
            int[][] m = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                m[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    m[i][j] = count;
                    ++count;            // To fix painful Reflector's bug
                }
            }
            return (m);
        }

        public static void mmult(int rows, int cols,
                              int[][] m1, int[][] m2, int[][] m3)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int val = 0;
                    for (int k = 0; k < cols; k++)
                    {
                        val += m1[i][k] * m2[k][j];
                    }
                    m3[i][j] = val;
                }
            }
        }

           public static void Main() 
  {
    Console.WriteLine(Test(50000));
  }

        public static bool Test(int N)
        {
            int[][] m1 = mkmatrix(SIZE, SIZE);
            int[][] m2 = mkmatrix(SIZE, SIZE);
            int[][] mm = new int[SIZE][];
            
            for(int h=0;h<SIZE;h++)
                mm[h] = new int[SIZE];

            for (int i = 0; i < N; i++)
            {
                
                mmult(SIZE, SIZE, m1, m2, mm);

                if (mm[0][0] != 270165)
                {
                    return (false);
                }

                if (mm[2][7] != 1070820)
                {
                    return (false);
                }

                if (mm[17][5] != 7019790)
                {
                    return (false);
                }

                if (mm[25][12] != 10355745)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
