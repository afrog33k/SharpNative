//SharpLang
//Works
//Simple Generic Function
using System;


public interface GenericInterface //Generic Interfaces cannot be statically compiled ...
{
    T RTest<T>(T test);
}

public static class Program
{

    public class Test : GenericInterface
    {
        public T RTest<T>(T test)
        {
            return test;
        }
    }

    public static void Main()
    {
        var spec = new Test();
        System.Console.WriteLine(spec.RTest(32));
        System.Console.WriteLine(spec.RTest("Test1"));
    }
}