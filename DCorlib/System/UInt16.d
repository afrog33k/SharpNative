module System.UInt16;
import System.Namespace;
import std.conv;

class UInt16 : Boxed!ushort
{
		public static const ushort MaxValue = ushort.max;
		public static const ushort MinValue = ushort.min;

	/*this()
	{
		// Constructor code
	}*/

	this(ushort value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static String ToString(int value)
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

