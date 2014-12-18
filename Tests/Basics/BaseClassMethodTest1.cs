//Class Method Test 1  - base class calls working
using System;

class A
{
    public virtual void Test(string name)
    {
        Console.WriteLine("A.Test");
        Console.WriteLine(name);
    }
}

class B : A
{
    public override void Test(string name)
    {
        Console.WriteLine("B.Test");
        base.Test(name);
    }
}

//If we add C order of definitions becomes a problem

class Program
{
    static void Main()
    {


        // Compile-time type is A.
        // Runtime type is B.
        A ref2 = new B();
        ref2.Test("Coogar");


    }
}