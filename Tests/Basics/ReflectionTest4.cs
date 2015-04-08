//Shows getting base class methods along side methods in current class, and invoking them virtually
using System;
using System.Reflection;

 class Methods
{
    public static void Inform(string parameter)
    {
    Console.WriteLine("Inform:parameter={0}", parameter);
    }
 public static void Inform2(int parameter)
    {
    Console.WriteLine("Inform:parameter={0}", parameter);
    }
 public override string ToString()
    {
        return "yo";
    }
 /*
   public override bool Equals(Object obj)
    {
       return false;
    }
   
    public virtual int GetHashCode()
    {
        return -1;
    }
    
    public  Type GetType(){
        return typeof(int);
    }*/
}

class Program
{
    static void Main()
    {
    // Name of the method we want to call.
    string name = "Inform";

    // Call it with each of these parameters.
    string[] parameters = { "Sam", "Perls" };

    // Get MethodInfo.
    Type type = typeof(Methods);
    MethodInfo info = type.GetMethod(name);

    // Loop over parameters.
    foreach (string parameter in parameters)
    {
        info.Invoke(null, new object[] { parameter });
    }
    
    // get all public static methods of MyClass type
MethodInfo[] methodInfos = typeof(Methods).GetMethods();
// sort methods by name
Array.Sort(methodInfos,
        delegate(MethodInfo methodInfo1, MethodInfo methodInfo2)
        { return methodInfo1.Name.CompareTo(methodInfo2.Name); });

// write method names
foreach (MethodInfo methodInfo in methodInfos)
{
  Console.WriteLine(methodInfo.Name);
  if(methodInfo.Name =="GetType")
    {
  //      Console.WriteLine(methodInfo.Invoke(new Methods(),null));
    }
}
    }
}
