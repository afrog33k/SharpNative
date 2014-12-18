/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public static class GcTest
    {
        public class Toggle
        {
            public bool state = true;
            public Toggle(bool start_state)
            {
                this.state = start_state;
            }

            public bool value()
            {
                return (this.state);
            }

            public Toggle activate()
            {
                this.state = !this.state;
                return (this);
            }
        }

        public class NthToggle : Toggle
        {
            int count_max = 0;
            int counter = 0;

            public NthToggle(bool start_state, int max_counter)
                : base(start_state)
            {
                this.count_max = max_counter;
                this.counter = 0;
            }
            public new NthToggle activate()
            {
                this.counter += 1;
                if (this.counter >= this.count_max)
                {
                    this.state = !this.state;
                    this.counter = 0;
                }
                return (this);
            }
        }

        public static bool Test(int N)
        {
            for (int loop = 0; loop < N; ++loop)
            {
                Toggle mainToggle = new Toggle(true);
                for (int i = 0; i < 5; i++)
                {
                    sVolatileValue = mainToggle.activate().value();
                }

                // Create temp objects (that are going to be collected soon)
                for (int i = 0; i < NUM_TOGGLE; i++)
                {
                    Toggle toggle = new Toggle(true);
                }

                NthToggle nthToggle = new NthToggle(true, 3);
                for (int i = 0; i < 8; i++)
                {
                    sVolatileValue = nthToggle.activate().value();
                }

                for (int i = 0; i < NUM_TOGGLE; i++)
                {
                    NthToggle toggle = new NthToggle(true, 3);
                }

                // To make sure local variables are traced correctly...
                for (int i = 0; i < 5; i++)
                {
                    sVolatileValue = mainToggle.activate().value();
                }

                for (int i = 0; i < 8; i++)
                {
                    sVolatileValue = nthToggle.activate().value();
                }
            }

            return (true);
        }

		public static void Main()
		{
			Test (400);
		}

        public static int NUM_TOGGLE = 1000 * 1000;
        public static volatile bool sVolatileValue;
    }
}
