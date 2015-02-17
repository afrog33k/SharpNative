module System.SByte;
import System.Namespace;
import std.conv;

class SByte : Boxed!byte
{
		public static const byte MaxValue = byte.max;
		public static const byte MinValue = byte.min;

	/*this()
	{
		// Constructor code
	}*/

	this(byte value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static String ToString(byte value)
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

