module System.Text.StringBuilder;
import System.Namespace;
import System.Text.Namespace;
import std.conv;

class StringBuilder : NObject
{
	private static const int defaultMaxCapacity = Int32.MaxValue;
		private static const int defaultInitialCapacity = 16;

		
		private int capacity;
		private wstring data ="";
	
	public void Insert(int index, wstring value)
	{

	}

	public void Insert(int index, string value)
	{

	}
	
	public this() 
	{
	 	this(defaultInitialCapacity, defaultMaxCapacity);
	}
	
	public this(int initialCapacity) 
	{
		this(initialCapacity, defaultMaxCapacity);
	}

	public this(int initialCapacity, int maxCapacity) 
	{
		this.capacity = initialCapacity;
	
		if(capacity<2)
			capacity =2;
	
		this.data =[];// new wchar[capacity];
	}

	public this(String value)
	{
		this((value !is null) ? value.Length : defaultInitialCapacity, defaultMaxCapacity);

		if (value !is null) 
		{
				data = cast(wstring)(value);
		}
		
	}

	
	
	public	int Length() @property {
		return cast(int) data.length;
	}



	

	
		public override string toString()
		{
			return to!string(data[0..data.length]);
		}

		public override String ToString() 
		{
		//	Console.WriteLine(data);
			return new String(data);
		//	return String("");
		}

		public String ToString(int startIndex, int length) 
		{
			if (startIndex < 0 || length < 0 || startIndex + length > this.Length) {
				throw new ArgumentOutOfRangeException();
			}
			return  new String((data[startIndex..(startIndex+length)]));
		}

		public int Capacity() @property
		{
			
				return this.capacity;
			
		}

	

		public StringBuilder Append(String value) {
			//Console.WriteLine("Appending " ~ value.Text);

			data ~=  value.Text;

			//Console.WriteLine("Value is: " ~ data);
			return this;
		}

		public StringBuilder AppendLine(String value) {
			//Console.WriteLine("Appending " ~ value.text);

			data = data ~ cast(wstring)value ~ "\r\n";

			//Console.WriteLine("Value is: " ~ data);
			return this;
		}

		public StringBuilder Append(String value, int startIndex, int count) {
			//Console.WriteLine("Appending " ~ cast(string)value);
			
			if (value is null) {
				return this;
			}
			if (startIndex < 0 || count < 0 || value.Length < startIndex + count) 
			{
				throw new ArgumentOutOfRangeException();
			}
			//this.EnsureSpace(count);
			//for (int i = 0; i < count; i++) {
			//	this.data[this.length++] = value[startIndex + i];
			//}
			data = data ~ (cast(wstring)value)[startIndex..(count+startIndex)];
			return this;
		}


		public StringBuilder Append(char value) {
		
			data = data ~ value;
			return this;
		}

	public StringBuilder Append(char value, int repeatCount) {
			if (repeatCount < 0) {
				throw new ArgumentOutOfRangeException();
			}
			//EnsureSpace(repeatCount);
			for (int i = 0; i < repeatCount; i++) {
				this.data = data~value;
			}
			return this;
		}

		public StringBuilder Append(char[] value) {
			if (value == null) {
				return this;
			}
			
			data = data ~ cast(wstring)value;
			return this;
		}

		public StringBuilder Append(wstring value) {
			if (value == null) {
				return this;
			}
			
			data = data ~ value;
			return this;
		}

		public StringBuilder Append(NObject value) {
			if (value is null) {
				return this;
			}
			return Append(value.ToString());
		}

		public StringBuilder Append(bool value) {
			return Append(to!wstring(value));
		}

		public StringBuilder Append(byte value) {
			return Append(to!wstring(value));
		}

	/*	public StringBuilder Append(decimal value) {
			return Append(to!string(value));
		}*/

		public StringBuilder Append(double value) {
			return Append(to!wstring(value));
		}

		public StringBuilder Append(short value) {
			return Append(to!wstring(value));
		}

		public StringBuilder Append(int value) {
			return Append(to!wstring(value));
		}

		public StringBuilder Append(long value) {
			return Append(to!wstring(value));
		}

		//public StringBuilder Append(sbyte value) {
		//	return Append(to!string(value));
		//}

		public StringBuilder Append(float value) {
			return Append(to!wstring(value));
		}

	/*	public StringBuilder Append(ushort value) {
			return Append(to!string(value));
		}

		public StringBuilder Append(uint value) {
			return Append(to!string(value));
		}

		public StringBuilder Append(ulong value) {
			return Append(to!string(value));
		}*/



	public String Substring(int index, int length)
	{
		return  new String(text[index..(length+index)]);//(index,length);
	}

	final char opIndex(int index) {
		if (index >= text.length)
			throw new ArgumentOutOfRangeException( new String("index"));
		
		return text[index];
	}
	
	String opBinary(string op)(String rhs)
	{
		static if (op == "+") 
			return  String(text ~ rhs.text);	
		static if (op == "+=") 
		{
			text = text ~ rhs.text;
			return  text;	
		}
	}
	
	string opCast(T : wstring)() const
	{
		return text;
	}

	string opCast(T : String)() const
	{
		return ToString();
	}
	
	public static String Concat(String self, String other)
	{
		self = new String(self.text ~ other.text);
		return self;
	}
}


