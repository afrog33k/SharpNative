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
		return to!string(__Value);
	}

	public static String ToString(ulong value)
	{
		return new String(to!wstring(value));
	}

	public override String ToString()
	{
		return new String(to!wstring(this.toString));
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

}

