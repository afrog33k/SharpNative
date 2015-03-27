using System;
     using System.Threading;
     using System.Reflection;
     
     namespace DelegateTypeInference
     {
     	
       class Program
       {
         static void Main(string[] args)
        {
          A(new ThreadStart(ShowTime));
          A(delegate { ShowTime(); });
          A(ShowTime);
          A(() => ShowTime());
          A(() => { ShowTime(); Console.WriteLine("boo"); });
    
          B(a => ShowTime());
        //  C((a, b) => { ShowTime(); return null; });
        }
    
        static void ShowTime()
        { Console.WriteLine("10-10-2014 7:58pm"); }
    
        static void A(ThreadStart t)
        { t(); }
    
        static void B(WaitCallback w)
        { w(null); }
    
        /*static void C(ModuleResolveEventHandler m)
        { Module mod = m(null, null); }*/
      }
    }