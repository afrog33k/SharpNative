module System.Single;
import System.Namespace;
import std.conv;

class Single : NObject
{
	this()
	{
		// Constructor code
	}

	public override string toString() {
		return "";
	}

	public static String ToString(float value)
	{
		return new String(to!string(value));
	}

}

