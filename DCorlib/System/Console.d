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
		write(BOX!(value).ToString());		
}

public static void Write(T)(T value) if(is(T==double))
{
		write(BOX!(value).ToString());		
}


public static void Write(T)(T value) if(is(T==int))
{
		write(value);		
}

public static void Write(T)(T value) if(is(T==wchar))
{
	write(value);		
}


public static void WriteLine(T)(T value) if(is(T==wchar))
{

		writeln(value);		
}

public static void WriteLine(short value)
{
	writeln(value);
}


public static void Write(short value)
{
	write(value);
}


public static void WriteLine(wchar value)
{

	writeln(value);		
}

public static void Write(wchar value)
{
	write(value);		
}


public static void WriteLine(T)(T value) if(is(T==int))
{
	writeln(value);		
}

public static void WriteLine(T)(T value) if(is(T==double))
{
		writeln(BOX!(T)(value).ToString());		
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
	writeln(value);		//TODO: find out why dmd 2.067-b4 toString errors
}


public static void WriteLine(wchar[] value)
{
	writeln(cast(wstring)value);		
}

public static void WriteLine(wstring value)
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

	public static void WriteLine(Array_T!(wchar) anything)//if(!is(T==double) && !is(T==float) && !is(T==int)) 
	{
		writeln(anything.Items);
	}

	//Generic
	public static void WriteLine(T)(T anything) if(!is(T==double) && !is(T==float) && !is(T==int)&&!is(T:NObject)) 
	{
		writeln(anything);
	}
	
	public static void Write(T)(T anything) if(!is(T==double) && !is(T==float) && !is(T==int) &&!is(T:NObject)) 
	{
		write(anything);
	}
	
	
	//	public static void WriteLine(Boolean aString)
	//	{
	//		writeln(aString);
	//	}
	
	public static void WriteLine(String aString) // All raw string classes should be aware of .Text property
	{
		if(aString !is null)
		writeln(aString.Text);
		else
			writeln();
	}

	public static void Write(String aString) // All raw string classes should be aware of .Text property
	{
		if(aString !is null)
			write(aString.Text);
			
	}

	public static void WriteLine(NObject aString) // All raw string classes should be aware of .Text property
	{
		if(aString !is null)
		writeln(aString.ToString().Text);
		else
			writeln();

	}

	public static void Write(NObject aString) // All raw string classes should be aware of .Text property
	{
		if(aString !is null)
		write(aString.ToString().Text);
	}

	


	public static void WriteLine(String format, Array_T!(NObject) objs) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, objs);
		writeln(aString.Text);
	}

	public static void WriteLine(String format,  NObject a, NObject b, NObject c) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, __ARRAY!(NObject)([a,b,c]));
		writeln(aString.Text);
	}

	public static void WriteLine(String format,  NObject a) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, __ARRAY!(NObject)([a]));
		writeln(aString.Text);
	}

	public static void Write(String format,  NObject a) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, __ARRAY!(NObject)([a]));
		write(aString.Text);
	}

	public static void WriteLine(String format,  NObject a, NObject b) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format,  __ARRAY!(NObject)([a,b]));
		writeln(aString.Text);
	}

//	public static void WriteLine(String format, NObject[] objs ...) // All raw string classes should be aware of .Text property
//	{
//		String aString = String.Format(format, objs);
//		writeln(aString.Text);
//	}

	public static void Write(String format, Array_T!(NObject) objs) // All raw string classes should be aware of .Text property
	{
		String aString = String.Format(format, objs);
		write(aString.Text);
	}


//	public static void Write(String format, NObject[] objs ...) // All raw string classes should be aware of .Text property
//	{
//		String aString = String.Format(format, objs);
//		write(aString.Text);
//	}
	
	public static void WriteLine(int value)
	{
		writeln(value);
	}
}

