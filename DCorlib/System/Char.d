module System.Char;
import System.Namespace;
import std.conv;
import std.ascii;

class Char : Boxed!wchar
{

	public static const wchar MaxValue = wchar.max;
	public static const wchar MinValue = wchar.min;

// Note that this array must be ordered, because binary searching is used on it.
		 static const wchar[] WhiteChars = [
			cast(wchar) 0x9, cast(wchar) 0xA, cast(wchar) 0xB, cast(wchar) 0xC, cast(wchar) 0xD,
			cast(wchar) 0x85, cast(wchar) 0x1680, cast(wchar) 0x2028, cast(wchar) 0x2029,
			cast(wchar) 0x20, cast(wchar) 0xA0, cast(wchar) 0x2000, cast(wchar) 0x2001,
			cast(wchar) 0x2002, cast(wchar) 0x2003, cast(wchar) 0x2004, cast(wchar) 0x2005,
			cast(wchar) 0x2006, cast(wchar) 0x2007, cast(wchar) 0x2008, cast(wchar) 0x2009,
			cast(wchar) 0x200A, cast(wchar) 0x200B, cast(wchar) 0x3000, cast(wchar) 0xFEFF ];


public static bool IsWhiteSpace(wchar c) {
			// TODO: Make this use Array.BinarySearch() when implemented
			for (int i = 0; i < WhiteChars.length; i++) {
				if (WhiteChars[i] == c) {
					return true;
				}
			}
			return false;
		}

public static bool IsWhiteSpace(String str, int index) {
			if (str is null) {
				throw new ArgumentNullException(new String("str"));
			}
			if (index < 0 || index >= str.Length) {
				throw new ArgumentOutOfRangeException(new String("index"));
			}
			return IsWhiteSpace(str[index]);
		}

		public static bool IsDigit(wchar c)
		{
			return isDigit(c);
		}


	this(wchar value = 0)
    {
        this.__Value = value;
    }

	public override string toString() {
		return to!string(__Value);
	}

	public static String ToString(wchar value)
	{
		return new String(to!wstring(value));
	}

	public override String ToString()
	{
		import std.array:appender;
		import std.format;
		//Almost there ;)
		auto writer = appender!string();
		formattedWrite(writer,"%c",__Value);
		return new String(to!wstring(writer.data));
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

}

