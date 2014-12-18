module System.Char;
import System.Namespace;
import std.conv;
import std.ascii;
class Char : NObject
{

// Note that this array must be ordered, because binary searching is used on it.
		 static const char[] WhiteChars = [
			cast(char) 0x9, cast(char) 0xA, cast(char) 0xB, cast(char) 0xC, cast(char) 0xD,
			cast(char) 0x85, cast(char) 0x1680, cast(char) 0x2028, cast(char) 0x2029,
			cast(char) 0x20, cast(char) 0xA0, cast(char) 0x2000, cast(char) 0x2001,
			cast(char) 0x2002, cast(char) 0x2003, cast(char) 0x2004, cast(char) 0x2005,
			cast(char) 0x2006, cast(char) 0x2007, cast(char) 0x2008, cast(char) 0x2009,
			cast(char) 0x200A, cast(char) 0x200B, cast(char) 0x3000, cast(char) 0xFEFF ];


public static bool IsWhiteSpace(char c) {
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

		public static bool IsDigit(char c)
		{
			return isDigit(c);
		}


	this()
	{
		// Constructor code
	}

	public override string toString() {
		return "";
	}

	public static String ToString(char value)
	{
		return new String(to!string(value));
	}

	static this()
	{
		
	}

}

