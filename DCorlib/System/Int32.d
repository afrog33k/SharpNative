module System.Int32;
import System.Namespace;
import std.conv;

class Int32 : Boxed!int
{
		public static const int MaxValue = 0x7fffffff;
		public static const int MinValue = -2147483648;

		

	/*this()
	{
		// Constructor code
	}*/

	this(int value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static int Parse(String s)
	{
		return to!int(s.Text);
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

