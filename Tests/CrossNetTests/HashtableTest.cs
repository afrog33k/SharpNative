/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class HashtableTest
    {

		public static void Main()
		{
			Console.WriteLine(Test (1200));
		}


        public static bool Test(int N)
        {
            Hashtable hash1 = new Hashtable();
            Hashtable hash2 = new Hashtable();

            for (int i = 0; i <= 9999; i++)
            {
                hash1.Add("foo_" + i.ToString(), i);
            }

            for (int i = 0; i < N; i++)
            {
                IDictionaryEnumerator it = hash1.GetEnumerator();
                while (it.MoveNext())
                {
                    if (hash2.ContainsKey(it.Key))
                    {
                        int v1 = (int)hash1[it.Key];
                        int v2 = (int)hash2[it.Key];
                        hash2[it.Key] = v1 + v2;
                    }
                    else
                    {
                        hash2.Add(it.Key, hash1[it.Key]);
                    }
                }

                // Make sure that the result is as expected
                if (hash1["foo_0"].ToString() != "0")
                {
                    return (false);
                }
                if (hash1["foo_3705"].ToString() != "3705")
                {
                    return (false);
                }
                if (hash1["foo_9999"].ToString() != "9999")
                {
                    return (false);
                }
                if (hash2["foo_0"].ToString() != "0")
                {
                    return (false);
                }
                if (hash2["foo_3705"].ToString() != (3705 * (i + 1)).ToString())
                {
                    return (false);
                }
                if (hash2["foo_9999"].ToString() != (9999 * (i + 1)).ToString())
                {
                    return (false);
                }
            }

            return (true);
        }
    }
}
