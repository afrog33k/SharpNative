module System.Int64;
import System.Namespace;
import std.conv;

class Int64 : Boxed!long
{
		public static const long MaxValue = long.max;
		public static const long MinValue = long.min;

	/*this()
	{
		// Constructor code
	}*/

	this(long value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		//return to!string(__Value);
		return (__Value.stringof);//TODO on dmd 2.067-b4 to!string crashes ????
	}

	public static String ToString(long value)
	{
		return new String(to!wstring(value));
	}

	public override String ToString()
	{
		import std.array:appender;
		import std.format;
		//Almost there ;)
		auto writer = appender!string();
		formattedWrite(writer,"%d",__Value);
		return new String(to!wstring(writer.data));
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

}

