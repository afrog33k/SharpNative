module System.DateTimeKind;

import System.Namespace;
import System.Runtime.CompilerServices.Namespace;
import System.Threading.Namespace;
import System.Globalization.Namespace;

struct DateTimeKind// Enum
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

	public enum DateTimeKind Utc = 1;
	public enum DateTimeKind Local = 2;


	DateTimeKind opBinary(string op)(DateTimeKind rhs)
	{
		return mixin("DateTimeKind(__Value "~op~" rhs.__Value)");
	}
	bool opEquals(const DateTimeKind a)
	{
		return a.__Value == this.__Value;
	}
	bool opEquals(const int a)
	{
		return a == this.__Value;
	}
	public string toString()
	{
		if (this == Utc.__Value)
		{
			return "Utc";
		}
		if (this == Local.__Value)
		{
			return "Local";
		}
		return std.conv.to!string(GetType().FullName.Text);
	}
}
