module System.Int32;
import System.Namespace;
import std.conv;

class Int32 : Boxed!int,  IComparable, IComparable__G!(int)
{
		public static const int MaxValue = 0x7fffffff;
		public static const int MinValue = -2147483648;


	int CompareTo(NObject obj, IComparable __j = null)
	{
		auto other = UNBOX!(int)(obj);
		return __Value - other;
	}
		int CompareTo(int other, IComparable__G!int __j = null)
		{
		
			return __Value - other;
		}

	/*this()
	{
		// Constructor code
	}*/

	this(int value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return __Value.stringof;
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

