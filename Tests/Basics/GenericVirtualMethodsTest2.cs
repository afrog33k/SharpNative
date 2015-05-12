using System;

class Primes
{

    public interface IBar
    {
        string DoIt<T1,T2>(T1 a, T2 b);
    }

    public class Bar : IBar
    {
        public string DoIt<T1,T2>(T1 a, T2 b)
        {
            return string.Format("I did it with {0} and {1}", a, b);
        }
    }


    public static void Main()
    {
        IBar bar = new Bar();
        string result = bar.DoIt(42, 69);
        Console.WriteLine(result);
    } 
    
}