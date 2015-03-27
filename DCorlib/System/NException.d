module System.NException;

import std.conv;
import System.Namespace;

// we have to give it another name ... damn :(
public class NException : Exception  
{

	public NObject __Boxed_;
	alias  __Boxed_ this;

	NException _innerException;

	public NException InnerException() @property
	{
		return _innerException;
	}

	public this(System.Runtime.Serialization.Namespace.SerializationInfo info, System.Runtime.Serialization.Namespace.StreamingContext context)
	{
		super("Exception");
		//super(info, context);
	}

	public this(String message, NException innerException)
  	{
  		_innerException = innerException;
    	super(cast(string)cast(wstring)message);

	}

	public this() @safe
	{
    	super("Exception");
	}

	public this(string value) @safe
	{
		super(value);
	}

	public this(String message)
	{
		super(cast(string)cast(wstring)message);
	}

	public String Message() @property
	{
		return new String(msg);
	}

	public String _Exception_Message() @property
	{
		return new String(msg);
	}

	public String StackTrace() @property
	{
		return new String(file ~":"~ to!string(line) ~"\r\n" ~ info.toString);
	}

	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

	public  String ToString()
	{
		return _Exception_Message();
	}
}