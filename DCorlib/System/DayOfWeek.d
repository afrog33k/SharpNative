module System.DayOfWeek;

import System.Namespace;

struct DayOfWeek// Enum
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
	public enum DayOfWeek Sunday = 0;
	public enum DayOfWeek Monday = 1;
	public enum DayOfWeek Tuesday = 2;
	public enum DayOfWeek Wednesday = 3;
	public enum DayOfWeek Thursday = 4;
	public enum DayOfWeek Friday = 5;
	public enum DayOfWeek Saturday = 6;


	DayOfWeek opBinary(string op)(DayOfWeek rhs)
	{
		return mixin("DayOfWeek(__Value "~op~" rhs.__Value)");
	}
	bool opEquals(const DayOfWeek a)
	{
		return a.__Value == this.__Value;
	}
	bool opEquals(const int a)
	{
		return a == this.__Value;
	}
	public string toString()
	{
		if (this == Sunday.__Value)
		{
			return "Sunday";
		}
		if (this == Monday.__Value)
		{
			return "Monday";
		}
		if (this == Tuesday.__Value)
		{
			return "Tuesday";
		}
		if (this == Wednesday.__Value)
		{
			return "Wednesday";
		}
		if (this == Thursday.__Value)
		{
			return "Thursday";
		}
		if (this == Friday.__Value)
		{
			return "Friday";
		}
		if (this == Saturday.__Value)
		{
			return "Saturday";
		}
		return std.conv.to!string(GetType().FullName.Text);
	}
}