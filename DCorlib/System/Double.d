module System.Double;
import System.Namespace;
import std.conv;

class Double : Boxed!double
{

  public static const double MinValue = -1.7976931348623157E+308;
  public static const double MaxValue = 1.7976931348623157E+308;
  public static const double Epsilon = 4.9406564584124650E-308; // E-324 (.net) .. d cannot store that size
  public static const double NegativeInfinity = -1.0/(0.0);
  public static const double PositiveInfinity = 1.0/(0.0);
  public static const double NaN = 0.0/0.0;



	/*this()
	{
		// Constructor code
	}*/

	this(double value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static String ToString(double value)
	{
		return new String(to!wstring(value));
	}

	
	

	public override String ToString()
	{
		import core.stdc.stdio;
		import std.array:appender;
		import std.format;
		//Almost there ;)
		auto writer = appender!string();
		formattedWrite(writer,"%G",__Value);
		return _S(writer.data);
		//char[256] buf;
		//buf.length = 256;
		//vsprintf(cast(char*)buf, cast(const(char*))"%.17g",cast(char*) __Value);
	//	//buf.length = core.stdc.string.strlen(cast(char*)buf);

		//printf( "%.*s", cast(char*)buf );
		//return _S("");//; new String(buf);
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}


}

