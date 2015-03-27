module System.TypeCode;

import System.Namespace;
struct TypeCode// Enum
{
	public int __Value;
	alias __Value this;
	public enum __IsEnum = true; // Identifies struct as enum
	public this(int value)
	{
		__Value = value;
	}

	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
	public enum TypeCode Empty = 0;
	public enum TypeCode __CSObject = 1;
	public enum TypeCode DBNull = 2;
	public enum TypeCode Boolean = 3;
	public enum TypeCode Char = 4;
	public enum TypeCode SByte = 5;
	public enum TypeCode Byte = 6;
	public enum TypeCode Int16 = 7;
	public enum TypeCode UInt16 = 8;
	public enum TypeCode Int32 = 9;
	public enum TypeCode UInt32 = 10;
	public enum TypeCode Int64 = 11;
	public enum TypeCode UInt64 = 12;
	public enum TypeCode Single = 13;
	public enum TypeCode Double = 14;
	public enum TypeCode Decimal = 15;
	public enum TypeCode DateTime = 16;
	public enum TypeCode String = 18;


	TypeCode opBinary(string op)(TypeCode rhs)
	{
		return mixin("TypeCode(__Value "~op~" rhs.__Value)");
	}
	bool opEquals(const TypeCode a)
	{
		return a.__Value == this.__Value;
	}
	bool opEquals(const int a)
	{
		return a == this.__Value;
	}
	public string toString()
	{
		if (this == Empty.__Value)
		{
            return "Empty";
		}
		if (this == __CSObject.__Value)
		{
            return "__CSObject";
		}
		if (this == DBNull.__Value)
		{
            return "DBNull";
		}
		if (this == Boolean.__Value)
		{
            return "Boolean";
		}
		if (this == Char.__Value)
		{
            return "Char";
		}
		if (this == SByte.__Value)
		{
            return "SByte";
		}
		if (this == Byte.__Value)
		{
            return "Byte";
		}
		if (this == Int16.__Value)
		{
            return "Int16";
		}
		if (this == UInt16.__Value)
		{
            return "UInt16";
		}
		if (this == Int32.__Value)
		{
            return "Int32";
		}
		if (this == UInt32.__Value)
		{
            return "UInt32";
		}
		if (this == Int64.__Value)
		{
            return "Int64";
		}
		if (this == UInt64.__Value)
		{
            return "UInt64";
		}
		if (this == Single.__Value)
		{
            return "Single";
		}
		if (this == Double.__Value)
		{
            return "Double";
		}
		if (this == Decimal.__Value)
		{
            return "Decimal";
		}
		if (this == DateTime.__Value)
		{
            return "DateTime";
		}
		if (this == String.__Value)
		{
            return "String";
		}
		return std.conv.to!string(GetType().FullName.Text);
	}
}