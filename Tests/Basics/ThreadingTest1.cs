using System;
using System.Threading;
 
namespace threading
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = new Thread(myFun);
            t.Start();
            t.Join();
            Console.WriteLine("Main thread Running");
           
        }
 
        static void myFun()
        {
            Console.WriteLine("Running other Thread");
        }
    }
}