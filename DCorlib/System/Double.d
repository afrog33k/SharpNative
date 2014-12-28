module System.Double;
import System.Namespace;
import std.conv;

class Double : NObject
{

  public static const double MinValue = -1.7976931348623157E+308;
  public static const double MaxValue = 1.7976931348623157E+308;
  public static const double Epsilon = 4.9406564584124650E-308; // E-324 (.net) .. d cannot store that size
  public static const double NegativeInfinity = -1.0/(0.0);
  public static const double PositiveInfinity = 1.0/(0.0);
  public static const double NaN = 0.0/0.0;



	this()
	{
		// Constructor code
	}

	public override string toString() {
		return "";
	}

	public static String ToString(double value)
	{
		return new String(to!wstring(value));
	}

}

