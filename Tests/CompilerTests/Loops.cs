using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            while (true)
            {
                Console.WriteLine("hi");
                break;
            }
            
            while (true)
                Console.WriteLine("nobreak");

            for (int i=0;i<50;i=i+1)
                Console.WriteLine(i);

            do
            {
                Console.WriteLine("Dowhile");
            }
            while (false);

            while (true)
            {
                if (4 == 5)
                    continue;
                
            }

            while (true)
            {
                Console.WriteLine(1);
                break;
                Console.WriteLine(2);
                continue;
                Console.WriteLine(3);

            }

            while (true)
            {
                switch (4)
                {
                    case 4: break;
                }
            }
        }
    }
}