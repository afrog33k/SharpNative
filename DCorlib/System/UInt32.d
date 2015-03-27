module System.UInt32;
import System.Namespace;
import std.conv;

class UInt32 : Boxed!uint
{
		public static const uint MaxValue = uint.max;
		public static const uint MinValue = uint.min;

	/*this()
	{
		// Constructor code
	}*/

	this(uint value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return __Value.stringof;
	}

	public static String ToString(int value)
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

