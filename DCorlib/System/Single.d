module System.Single;
import System.Namespace;
import std.conv;

class Single :  Boxed!float
{
	public static const float MaxValue = float.max;
	public static const float MinValue = float.min_normal;

	/*this()
	{
		// Constructor code
	}*/

	this(float value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static String ToString(float value)
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

