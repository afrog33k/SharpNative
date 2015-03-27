//http://www.dotnetperls.com/reflection-field
using System;
using System.Reflection;

 class ReflectionTest
{
 	//Order of declaration is important for this version of reflection
 	 //  int Age = 90; //need to specifically support this
    public string _name = "Peter File";
   // public const int Height = 100; //c# orders consts after others (reflection)
 	
    public static int Width;
    public static int Weight;
    public static string Name;
    public static int Height = 100; //c# orders consts after others (reflection)
    
    
  

    public static void Write()
    {
	Type type = typeof(ReflectionTest); // Get type pointer
	FieldInfo[] fields = type.GetFields(); // Obtain all fields
	foreach (var field in fields) // Loop through fields
	{
	    string name = field.Name; // Get string name
	    object temp = field.GetValue(new ReflectionTest(){_name= name + "kabiito"}); // Get value
	    if (temp is int) // See if it is an integer.
	    {
		int value = (int)temp;
		Console.Write(name);
		Console.Write(" (int) = "); 
		Console.WriteLine(value);
	    } 
	    else if (temp is string) // See if it is a string.
	    {
		string value = temp as string;
		Console.Write(name);
		Console.Write(" (string) = ");
		Console.WriteLine(value);
	    }
	}
    }
}

class Program
{
    static void Main()
    {
    	//int i = 9;
   for(int i=0;i < 100; i++)
    {
	//ReflectionTest.Height = 100+i; // Set value
	ReflectionTest.Width = 50+i; // Set value
	ReflectionTest.Weight = 300+i; // Set value
	ReflectionTest.Name = "Perl" + i; // Set value
	ReflectionTest.Write(); // Invoke reflection methods
	Console.WriteLine((50+i).GetType());
	Console.WriteLine((50.0+i).GetType());
	Console.WriteLine(("Perl").GetType());
	Console.WriteLine(("Perl" + i).GetType());

	}
    }
}