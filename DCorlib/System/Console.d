module System.Console;
import System.Namespace;
import std.stdio;
import std.string;


public class Console :  NObject
{

	//T DoIt(T)()
	//{
	//	return cast(T)"just did";
	//}
	this()
	{
		// Constructor code
	}
	
	// Non-generic
	
	//	public static void WriteLine(Object anything)
	//	{
	//		writeln(anything);
	//	

		//if(is(T==double)
		//{
		//	return format("%.14f", n);
		//}
public static void Write(T)(T value) if(is(T==float))
{
		write(value.toString());		
}

public static void Write(T)(T value) if(is(T==double))
{
		write(value.toString());		
}


public static void Write(T)(T value) if(is(T==int))
{
		write(value.toString());		
}

public static void WriteLine(T)(T value) if(is(T==int))
{
		writeln(value.toString());		
}

public static void WriteLine(T)(T value) if(is(T==double))
{
		writeln(value.toString());		
}


public static void WriteLine(T)(T value) if(is(T==long))
{
		writeln(value);		
}

public static void WriteLine(T)(T value) if(is(T==ulong))
{
		writeln(value);		
}
public static void WriteLine(long value)
{
		writeln(value);		
}
public static void WriteLine(T)(T value) if(is(T==float))
{
	writeln(value.toString());		
}


public static void WriteLine(char[] value)
{
	writeln(cast(string)value);		
}

public static void WriteLine(string value)
{
	writeln(value);		
}


	public static void WriteLine(bool value)
	{
		if(value)
			writeln("True");
		else
			writeln("False");
	}

	public static void Write(bool value)
	{
		if(value)
			write("True");
		else
			write("False");
	}



	

	public static void WriteLine()
	{
		writeln();
	}

	public static void WriteLine(Array_T!(char) anything)//if(!is(T==double) && !is(T==float) && !is(T==int)) 
	{
		writeln(anything.Items);
	}

	//Generic
	public static void WriteLine(T)(T anything) if(!is(T==double) && !is(T==float) && !is(T==int)) 
	{
		writeln(anything);
	}
	
	public static void Write(T)(T anything) if(!is(T==double) && !is(T==float) && !is(T==int)) 
	{
		write(anything);
	}
	
	
	//	public static void WriteLine(Boolean aString)
	//	{
	//		writeln(aString);
	//	}
	
	public static void WriteLine(String aString) // All raw string classes should be aware of .Text property
	{
		writeln(aString.Text);
	}

	public static void Write(String aString) // All raw string classes should be aware of .Text property
	{
		write(aString.Text);
	}

	public static void WriteLine(NObject aString) // All raw string classes should be aware of .Text property
	{
		writeln(aString.ToString().Text);
	}

	public static void Write(NObject aString) // All raw string classes should be aware of .Text property
	{
		write(aString.ToString().Text);
	}


	public static void WriteLine(String format, NObject[] objs ...) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, objs);
		writeln(aString.Text);
	}

	public static void Write(String format, NObject[] objs ...) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, objs);
		write(aString.Text);
	}
	
	public static void WriteLine(int value)
	{
		writeln(value);
	}
}

