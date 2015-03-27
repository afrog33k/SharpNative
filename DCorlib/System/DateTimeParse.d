module System.DateTimeParse;

import System.Namespace;
import System.Globalization.Namespace;
alias System.Globalization.Namespace Globalization;

class DateTimeParse : NObject
{

    public static DateTime ParseExact(String s, String format, Globalization.DateTimeFormatInfo dateTimeFormatInfo, Globalization.DateTimeStyles style)
    {
		throw  new NotImplementedException();
    }

    public static DateTime ParseExactMultiple(String s, Array_T!(String) formats, Globalization.DateTimeFormatInfo dateTimeFormatInfo, Globalization.DateTimeStyles style)
    {
		throw  new NotImplementedException();
    }

    public static DateTime Parse(String s, Globalization.DateTimeFormatInfo dateTimeFormatInfo, Globalization.DateTimeStyles dateTimeStyles)
    {
		throw  new NotImplementedException();
    }

	public override String ToString()
	{
		return GetType().FullName;
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}