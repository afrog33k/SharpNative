/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class EventsTest
    {
        const int NUM_LISTENERS = 5;
        const int REPEAT = 100000;

        public delegate void EventDelegate(int a, int b);

        public class MyClass
        {
            public void Listener(int a, int b)
            {
                ++sCounter;
            }

            public static int sCounter = 0;
        }

        class MyEventClass
        {
            public void RaiseEvent(int a, int b)
            {
                if (mEvent != null)
                {
                    mEvent(a, b);
                }
            }

            public event EventDelegate mEvent;
        }

		public static void Main()
		{
			Console.WriteLine(Test (1700));
		}

        public static bool Test(int N)
        {
            MyEventClass c = new MyEventClass();
            for (int i = 0; i < NUM_LISTENERS; ++i)
            {
                MyClass l = new MyClass();
                c.mEvent += new EventDelegate(l.Listener);
            }

            for (int i = 0; i < N; ++i)
            {
                MyClass.sCounter = 0;

                for (int j = 0; j < REPEAT; ++j)
                {
                    c.RaiseEvent(42, 21);
                }

                if (MyClass.sCounter != NUM_LISTENERS * REPEAT)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
