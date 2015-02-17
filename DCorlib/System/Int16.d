module System.Int16;
import System.Namespace;
import std.conv;

class Int16 : Boxed!short
{
		public static const short MaxValue = short.max;
		public static const short MinValue = short.min;

	/*this()
	{
		// Constructor code
	}*/

	this(short value = 0)
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

