/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    class DictionaryTest
    {

		public static void Main()
		{
			Console.WriteLine(Test (150));
		}

        public static bool Test(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                const int NUM_VALUES = 10 * 1024;
                Dictionary<int, double> dic = new Dictionary<int, double>(NUM_VALUES);
                for (int j = 0; j < NUM_VALUES; ++j)
                {
                    dic.Add(j, (double)j);
                }

                for (int j = 0; j < 100; ++j)
                {
                    for (int k = 0; k < NUM_VALUES; ++k)
                    {
                        double result = dic[k];
                        if (result != (double)k)
                        {
                            return (false);
                        }
                    }
                }
            }
            return (true);
        }

        // Make sure dictionary compiles for both primitives, structures and classes
        public bool CompileCheck1()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Abc", "Def");
            string result = d["Abc"];
            return (result == "Def");
        }

        struct Abc
        {
            public Abc(int b)
            {
                mB = b;
            }

            int mB;
        }

        struct Def
        {
            public Def(int a)
            {
                mA = a;
            }

            public static bool operator ==(Def left, Def right)
            {
                return (left.mA == right.mA);
            }

            public static bool operator !=(Def left, Def right)
            {
                return (left.mA != right.mA);
            }

            public override bool Equals(object obj)
            {
                if (obj is Def == false)
                {
                    return (false);
                }
                Def d = (Def)obj;
                return (mA == d.mA);
            }

            public override int GetHashCode()
            {
                return (mA);
            }

            int mA;
        }

        public bool CompileCheck2()
        {
            Dictionary<Abc, Def> d = new Dictionary<Abc, Def>();
            d.Add(new Abc(10), new Def(20));
            Def result = d[new Abc(10)];
            return (result == new Def(20));
        }
    }
}
