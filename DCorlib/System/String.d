module System.String;
import System.Namespace;
import System.Text.Namespace;



class StringHelper
{
	private static int ParseDecimal(String str,  ref int ptr) 
	{
		int p = ptr;
		int n = 0;
		while (true)
		{
			wchar c = str[p];
			if (c<'0'||c>'9')
			{
				break;
			}
			n=n*10+c-'0';
			++p;
		}
		if (p==ptr)
		{
			return (-1);
		}
		ptr=p;
		return (n);
	}

	private static void ParseFormatSpecifier(String str,  ref int ptr,  ref int n,  ref int width,  ref bool leftAlign,  ref String format) 
	{
		try
		{
			n=ParseDecimal(str, ptr);
			if (n<0)
			{
				throw ( new FormatException( (new String ("Input string was not in a correct format."))));
			}
			if (str[ptr]==',')
			{
				++ptr;
				while (System.Namespace.Char.IsWhiteSpace(str[ptr]))
				{
					++ptr;
				}
				int start = ptr;
				format=str.Substring(start, ptr-start);
				leftAlign=(str[ptr]=='-');
				if (leftAlign)
				{
					++ptr;
				}
				width=ParseDecimal(str, ptr);
				if (width<0)
				{
					throw ( new FormatException( (new String ("Input string was not in a correct format."))));
				}
			}
			else
			{
				width=0;
				leftAlign=false;
				format= (new String (""));
			}
			if (str[ptr]==':')
			{
				int start = ++ptr;
				while (str[ptr]!='}')
				{
					++ptr;
				}
				format=CsNative.NullStringCheck(format)+CsNative.NullStringCheck(str.Substring(start, ptr-start));
			}
			else
			{
				format= null;
			}
			if (str[ptr++]!='}')
			{
				throw ( new FormatException( (new String ("Input string was not in a correct format."))));
			}
		}
		catch(
			  IndexOutOfRangeException __ex)    {
				  throw ( new FormatException( (new String ("Input string was not in a correct format."))));
			  }
	}



	
	public static void FormatHelper(StringBuilder result, String format, Array_T!(NObject) args) 
	{
		FormatHelper( result,  format,  args.Items);
	}

	public static void FormatHelper(StringBuilder result, String format, NObject[] args) 
	{
		if (format is null||args is null)
		{
			throw ( new ArgumentNullException());
		}
		int ptr = 0;
		int start = 0;
		int formatLen = format.Length;
		while (ptr<formatLen)
		{
			wchar c = format[ptr++];
			if (c=='{')
			{
				auto end = ptr-start-1;
				auto aString = format.text;
				//Console.WriteLine("Appending " ~ format.text[start..(start+ptr)-1]);
				result.Append(format, start, ptr-start-1);
				if (format[ptr]=='{')
				{
					start=ptr++;
					continue;
				}

				int n = 0;
				int width = 0;
				bool leftAlign = false;
				String  argFormat = null;
				ParseFormatSpecifier(format, ptr, n, width, leftAlign, argFormat);
				if (n>=args.length)
				{
					throw ( new FormatException( (new String ("Index (zero based) must be greater than or equal to zero and less than the size of the argument list."))));
				}
				NObject  arg = args[n];
				String  str = null;
				if (arg is null)
				{
					str=new String("");
				}
				else
				{
					str=arg.ToString();
				}

				if (width>str.Length)
				{
					int padLen = width-str.Length;
					if (leftAlign)
					{
						result.Append(str);
						result.Append(' ', padLen);
					}
					else
					{
						result.Append(' ', padLen);
						result.Append(str);
					}
				}
				else
				{
					result.Append(str);
				}

				start=ptr;

			}
			else if (c=='}')
			{
				if (ptr<formatLen&&format[ptr]=='}')
				{
					result.Append(format, start, ptr-start-1);
					start=ptr++;
				}
				else
				{
					throw ( new FormatException( (new String ("Input string was not of the correct format."))));
				}
			}
		}
		if (start<formatLen)
		{
			result.Append(format, start, formatLen-start);
		}
		//Console.WriteLine("Final string: " ~ result.ToString().text);

	}

}



class String : NObject
{
	wstring text;
	//	char[] text;
	//	this(char[] source)
	//	{
	//		// Constructor code
	//		text = source;
	//	}

	public wstring Text() @property const
	{
		return text;
	}

	//	static const __typeID = typeid(wstring);

	public override int GetHashCode()
	{
		//TODO: need better impl
		if(text is null)
			return -1;

		import xxhash;
		uint hashed = xxhashOf((text));
		return cast(int) hashed;
//		return cast(int)text.toHash;
		//enum type = typeid(wstring);

		//auto hash =  cast(int)type.getHash(&text);
		//return hash;
	}

	public __gshared String Empty = new String("");
	public __gshared String Null =  new String(cast(string)null);

	static this()
	{
		//Empty =  new String("");
	}

	this()
	{
		text = "";
	}
	this(wstring source)
	{
		text = source;
	}

	this(string source)
	{
		text = std.conv.to!(wstring)(source);
	}

	public static bool IsNullOrEmpty(String value) 
	{
		return (value is null) || (value.Length == 0);
	}

	public bool StartsWith(String str) {
		//Console.WriteLine(new String("str = {0}, ss = {1}") , str , this.Substring(0, str.Length) );
		return this.Substring(0, str.Length).Equals(str);
	}

	public bool EndsWith(String str) {
		return this.Substring(this.Length - str.Length, str.Length) == str;
	}


	//#region Format Methods

	public static String Format(String format, NObject obj0) {
		return Format(null, format, [ obj0 ]);
	}

	public static String Format(String format, NObject obj0, NObject obj1) {
		return Format(null, format, [ obj0, obj1 ]);
	}

	public static String Format(String format, NObject obj0, NObject obj1, NObject obj2) {
		return Format(null, format, [ obj0, obj1, obj2 ]);
	}


	public static String Format(String format, Array_T!(NObject) args1) { // "have to rename argument named args"
		return Format(null, format, args1);
	}

	public static String Format(String format, NObject[] args1 ...) { // "have to rename argument named args"
		return Format(null, format, args1);
	}

	public static String Format(IFormatProvider provider, String format, Array_T!(NObject) args1) {
		StringBuilder sb = new StringBuilder();
		StringHelper.FormatHelper(sb, format, args1);
		return sb.ToString();
	}

	public static String Format(IFormatProvider provider, String format, NObject[] args1 ...) {
		StringBuilder sb = new StringBuilder();
		StringHelper.FormatHelper(sb, format, args1);
		return sb.ToString();
	}

	//#endregion

	Array_T!(wchar) ToCharArray()
	{
		return  new Array_T!(wchar)(__CC(cast(wchar[])text)); // Make this dup later
	}


	public	int Length() @property {
		return cast(int) text.length;
	}



	public override String ToString()
	{
		return this;
		//return new String(text[0..text.length]);
	}

	public String Substring(int index, int length)
	{
		return new String(text[index..length]);//(index,length);
	}



	bool IEquatable_T_Equals (String rhs)
	{
		return Equals(rhs);
	}

	override bool Equals (NObject rhs)
    {
		
        return text == (rhs.ToString()).text;
    }

	override bool  opEquals(Object rhs)
	{
		 if(cast(String)rhs)
		{
			return text==(cast(String)(rhs)).text;
		}
		return super.opEquals(rhs);
			//Console.WriteLine("opEquals");
		//	return text==(rhs.text);
		//return false;
	
	}

	final wchar opIndex(int index) {
		if (index >= text.length)
			throw new ArgumentOutOfRangeException(new String("index"));

		return text[index];
	}

	String opOpAssign(wstring op)(String rhs)
	{
		//Console.WriteLine(op);
		static if (op == "+") 
		{
			text = text ~ rhs.text;
			return  this; 
		}
	}

	String opBinary(string op)(String rhs)
	{
		//Console.WriteLine(op);
		static if (op == "+") 
		{
			return  new String(text ~ rhs.text);	
		}
		//static if (op == "+=") 
		//{
		//	text = text ~ rhs.text;
		//	return  this;	
		//}
	}


	String[] Split(Array_T!wchar chars)
	{
		return null;
	}



	//seems we can overload just like c# yay ;)
	String opBinaryRight(string op)(NObject lhs)
	{
		static if (op == "+") 
			return  new String(lhs.ToString().text ~ text);  

	}



	String opBinary(string op)(NObject rhs)
	{
		//Console.WriteLine(op);
		static if (op == "+") 
			return  new String(text ~ rhs.ToString().text);  
		static if (op == "+=") 
		{
			text = text ~ rhs.ToString().text;
			return  text; 
		}
	}

	final T opCast(T)() if(is(T:wstring))
	{
		return text;
	}

	final T opCast(T)() if(is(T:string))
	{
		import std.string;
		return std.conv.to!(char[])(text);
	}

	final U opCast(U)() if(is(U:wchar*))
    {

		return cast(U) text;
    }

	final U opCast(U)() if(is(U:char*))
    {
		import std.string;
		return cast(U) std.conv.to!(char[])(text).toStringz;
    }

	final U opCast(U)()
		if(!is(U:char*) && !is(U:wchar*) && !is(U:string) && !is(U:wstring))
		{
			return cast(U)this;
		}

	public static String Concat(String self, String other)
	{
		self = new String(self.text ~ other.text);
		return self;
	}

	public static String Concat(Array_T!String array)
	{
		wstring self = "";
		foreach(item; array)
			self = self ~ item.ToString().text;
		return new String(self);
	}

	override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}


