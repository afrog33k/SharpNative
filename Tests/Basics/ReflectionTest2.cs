//http://www.dotnetperls.com/getmethod
using System;
using System.Reflection;

static class Methods
{
    public static void Inform(string parameter)
    {
	Console.WriteLine("Inform:parameter={0}", parameter);
    }
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
    }
}
