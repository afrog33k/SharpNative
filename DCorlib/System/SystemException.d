module System.SystemException;

import System.Namespace;
import System.Runtime.Serialization.Namespace;

static class SystemException : System.Namespace.NException
{

	public this()
	{
		super(_S("System Exception"));
	}

	public this(String message)
	{
		super(message);
	}

	public this(String message, System.Namespace.NException innerException)
	{
		super(message, innerException);
	}

	public this(System.Runtime.Serialization.Namespace.SerializationInfo info, System.Runtime.Serialization.Namespace.StreamingContext context)
	{
		super(info, context);
	}

	public override String ToString()
	{
		return GetType().FullName;
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}