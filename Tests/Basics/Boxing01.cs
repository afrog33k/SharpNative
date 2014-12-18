using System;

class Boxing
{



 public static void Main()
	{
	  string aName = "Failed"; 
		int i = 123;
		object o = i;
		
		Console.WriteLine(i);
		Console.WriteLine(o); 
		
		o =  564;
		i = (int)o;  
		
		try
		{
		aName = (string) o; //aName is null, should throw an exception

		Console.WriteLine(aName); 
		}
		catch(InvalidCastException ex)
		{
			Console.WriteLine("couldn't cast");
		}

		o = "griselda";

		aName = (string) o;
		
		Console.WriteLine(aName);

		
		Console.WriteLine(i);
		Console.WriteLine(o);

	}

 
	
}

