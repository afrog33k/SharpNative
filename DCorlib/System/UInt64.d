module System.UInt64;
import System.Namespace;
import std.conv;

class UInt64 : Boxed!ulong
{
		public static const ulong MaxValue = ulong.max;
		public static const ulong MinValue = ulong.min;

	/*this()
	{
		// Constructor code
	}*/

	this(ulong value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return __Value.stringof;
	}

	public static String ToString(ulong value)
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

