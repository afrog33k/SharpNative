module System.Int32;
import System.Namespace;
import std.conv;

class Int32 : NObject
{
		public static const int MaxValue = 0x7fffffff;
		public static const int MinValue = -2147483648;

	this()
	{
		// Constructor code
	}

	public override string toString() {
		return "";
	}

	public static String ToString(int value)
	{
		return new String(to!wstring(value));
	}

}

