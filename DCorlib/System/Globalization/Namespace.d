module System.Globalization.Namespace;
import std.stdio;
import std.string;
import System.Namespace;
import std.conv;


import System.Namespace;

public class DateTimeFormatInfo
{
}





public class CultureInfo : NObject
{
}
struct DateTimeStyles// Enum
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
  public enum DateTimeStyles None = 0x00000000;
  public enum DateTimeStyles AllowLeadingWhite = 0x00000001;
  public enum DateTimeStyles AllowTrailingWhite = 0x00000002;
  public enum DateTimeStyles AllowInnerWhite = 0x00000004;
  public enum DateTimeStyles AllowWhiteSpaces = DateTimeStyles.AllowLeadingWhite|DateTimeStyles.AllowInnerWhite|DateTimeStyles.AllowTrailingWhite;
  public enum DateTimeStyles NoCurrentDateDefault = 0x00000008;
  public enum DateTimeStyles AdjustToUniversal = 0x00000010;
  public enum DateTimeStyles AssumeLocal = 0x00000020;
  public enum DateTimeStyles AssumeUniversal = 0x00000040;
  public enum DateTimeStyles RoundtripKind = 0x00000080;


  DateTimeStyles opBinary(string op)(DateTimeStyles rhs)
  {
    return mixin("DateTimeStyles(__Value "~op~" rhs.__Value)");
  }
  bool opEquals(const DateTimeStyles a)
  {
    return a.__Value == this.__Value;
  }
  bool opEquals(const int a)
  {
    return a == this.__Value;
  }
  public string toString()
  {
    if (this == None.__Value)
    {
      return "None";
    }
    if (this == AllowLeadingWhite.__Value)
    {
      return "AllowLeadingWhite";
    }
    if (this == AllowTrailingWhite.__Value)
    {
      return "AllowTrailingWhite";
    }
    if (this == AllowInnerWhite.__Value)
    {
      return "AllowInnerWhite";
    }
    if (this == AllowWhiteSpaces.__Value)
    {
      return "AllowWhiteSpaces";
    }
    if (this == NoCurrentDateDefault.__Value)
    {
      return "NoCurrentDateDefault";
    }
    if (this == AdjustToUniversal.__Value)
    {
      return "AdjustToUniversal";
    }
    if (this == AssumeLocal.__Value)
    {
      return "AssumeLocal";
    }
    if (this == AssumeUniversal.__Value)
    {
      return "AssumeUniversal";
    }
    if (this == RoundtripKind.__Value)
    {
      return "RoundtripKind";
    }
    return std.conv.to!string(GetType().FullName.Text);
  }
}