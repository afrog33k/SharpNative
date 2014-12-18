using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Statements
{
    class Program
    {
        static void Main()
        {
            Program p = new Program();
            p.For();
            p.While();
            p.DoWhile();
        }

        public void For()
        {
            Console.WriteLine("Testing for");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
            }
        }

        public void While()
        {
            Console.WriteLine("Testing While");
            int i = 0;
            while (i < 5)
            {
                Console.WriteLine(i);
                i++;
            }
        }

        public void DoWhile()
        {
            Console.WriteLine("Testing dowhile");
            int i = 0;
            do
            {
                Console.WriteLine(i);
                i++;
            }
            while (i < 5);
        }
    }
}
