//Demonstrates calling attempting to call a base method from a derived class

using System;
class Base
{
    public virtual void Foo() { Console.WriteLine("Base"); }
}
class Derived : Base
{
    public override void Foo() { Console.WriteLine("Derived"); }

public static void Main()
{
    Base b = new Base();
	
    Derived d = new Derived();
    var baset=typeof(Base);
     baset.GetMethod("Foo").Invoke(b, null); 
    baset.GetMethod("Foo").Invoke(d, null); 


    // D does what is expected here but C# calls the most derived method :( ... we have to emulate this
//Roslyn smudges this up
//typeof(Base).GetMethod("Foo").Invoke(d, null); // need to fix it

    //Correct approach for C# i.e. what I'm doing by default in D
    
    //For later fix
     /*   var method = typeof(Base).GetMethod("Foo");
        var ftn = method.MethodHandle.GetFunctionPointer();
        var func = (Action)Activator.CreateInstance(typeof(Action), d, ftn);
        func();*/
 
}
}