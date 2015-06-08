module System.DateTime;

import System.Namespace;
import System.Runtime.CompilerServices.Namespace;
import System.Globalization.Namespace;
import System.Threading.Namespace;
alias System.DayOfWeek.DayOfWeek _DayOfWeek;

struct DateTime
{
    // Number of 100ns ticks per time unit
    private __gshared const long TicksPerMillisecond = cast(long )cast(long)10000;
    private __gshared const long TicksPerSecond = cast(long )DateTime.TicksPerMillisecond*cast(long)1000;
    private __gshared const long TicksPerMinute = cast(long )DateTime.TicksPerSecond*cast(long)60;
    private __gshared const long TicksPerHour = cast(long )DateTime.TicksPerMinute*cast(long)60;
    private __gshared const long TicksPerDay = cast(long )DateTime.TicksPerHour*cast(long)24;
    // Number of milliseconds per time unit
    private __gshared const int MillisPerSecond = cast(int )1000;
    private __gshared const int MillisPerMinute = cast(int )DateTime.MillisPerSecond*60;
    private __gshared const int MillisPerHour = cast(int )DateTime.MillisPerMinute*60;
    private __gshared const int MillisPerDay = cast(int )DateTime.MillisPerHour*24;
    // Number of days in a non-leap year
    private __gshared const int DaysPerYear = cast(int )365;
    // Number of days in 4 years
    private __gshared const int DaysPer4Years = cast(int )DateTime.DaysPerYear*4+1;
    // Number of days in 100 years
    private __gshared const int DaysPer100Years = cast(int )DateTime.DaysPer4Years*25-1;
    // Number of days in 400 years
    private __gshared const int DaysPer400Years = cast(int )DateTime.DaysPer100Years*4+1;
    // Number of days from 1/1/0001 to 12/31/1600
    private __gshared const int DaysTo1601 = cast(int )DateTime.DaysPer400Years*4;
    // Number of days from 1/1/0001 to 12/30/1899
    private __gshared const int DaysTo1899 = cast(int )DateTime.DaysPer400Years*4+DateTime.DaysPer100Years*3-367;
    // Number of days from 1/1/0001 to 12/31/9999
    private __gshared const int DaysTo10000 = cast(int )DateTime.DaysPer400Years*25-366;
    private __gshared const long MinTicks = cast(long )cast(long)0;
    private __gshared const long MaxTicks = cast(long )441796895990000000;
    private __gshared const long MaxMillis = cast(long )cast(long)DateTime.DaysTo10000*DateTime.MillisPerDay;
    // This is mask to extract ticks from m_ticks
    private __gshared const ulong TickMask = cast(ulong )cast(ulong)0x7FFFFFFFFFFFFFFFL;
    private __gshared const ulong UTCMask = cast(ulong )0x8000000000000000L;
    public __gshared DateTime MinValue;
    public __gshared DateTime MaxValue;
    private __gshared const int DatePartYear = cast(int )0;
    private __gshared const int DatePartDayOfYear = cast(int )1;
    private __gshared const int DatePartMonth = cast(int )2;
    private __gshared const int DatePartDay = cast(int )3;
    private __gshared Array_T!(int) DaysToMonth365;
    private __gshared Array_T!(int) DaysToMonth366;
    private __gshared Array_T!(int) timeBuffer;
    private ulong m_ticks = 0;
    /// Our origin is at 1601/01/01:00:00:00.000
	/// While desktop CLR's origin is at 0001/01/01:00:00:00.000.
	/// There are 504911232000000000 ticks between them which we are subtracting.
	/// See DeviceCode\PAL\time_decl.h for explanation of why we are taking
	/// year 1601 as origin for our HAL, PAL, and CLR.

    // static Int64 ticksAtOrigin = 504911232000000000;
    __gshared long ticksAtOrigin;

	
    public static int gettimeofday(int* time, int* timezome)
    {
		
		//Extern (Internal) Method Call
		return System_DateTime_gettimeofday(time ,timezome);
    }
	

    public DateTime Add(TimeSpan val)
    {
		return   DateTime(Cast!(long)(this.m_ticks)+val.Ticks);
    }

    private DateTime Add(double val, int scale)
    {
		return   DateTime((Cast!(long)(this.m_ticks)+cast(long)(val*scale*DateTime.TicksPerMillisecond+((val>=cast(double)0) ? (0.5) : (-0.5)))));
    }

    public DateTime AddDays(double val)
    {
		return Add(val, DateTime.MillisPerDay);
    }

    public DateTime AddHours(double val)
    {
		return Add(val, DateTime.MillisPerHour);
    }

    public DateTime AddMilliseconds(double val)
    {
		return Add(val, 1);
    }

    public DateTime AddMinutes(double val)
    {
		return Add(val, DateTime.MillisPerMinute);
    }

    public DateTime AddSeconds(double val)
    {
		return Add(val, DateTime.MillisPerSecond);
    }

    public DateTime AddTicks(long val)
    {
		return   DateTime(Cast!(long)(this.m_ticks)+val);
    }

    public static int Compare(DateTime t1, DateTime t2)
    {
		// Get ticks, clear UTC mask
		ulong t1_ticks = t1.m_ticks&DateTime.TickMask;
		ulong t2_ticks = t2.m_ticks&DateTime.TickMask;
		// Compare ticks, ignore the Kind property.
		if(t1_ticks>t2_ticks)
		{
			return 1;
		}
		if(t1_ticks<t2_ticks)
		{
			return -1;
		}
		// Values are equal
		return 0;
    }

    public int CompareTo(NObject val)
    {
		if(val is null)
		{
			return 1;
		}
		return DateTime.Compare(this, UNBOX!(DateTime)(val));
    }

    public static int DaysInMonth(int year, int month)
    {
		//Extern (Internal) Method Call
		return System_DateTime_DaysInMonth(year ,month);
		 
    }



    public bool Equals(NObject val)
    {
		if(IsCast!(Boxed!(DateTime))(val))
		{
			// Call compare for proper comparison of 2 DateTime objects
			// Since DateTime is optimized value and internally represented by int64
			// "this" may still have type int64.
			// Convertion to object and back is a workaround.
			NObject o = BOX!(DateTime)(this);
			DateTime thisTime = UNBOX!(DateTime)(o);
			return Compare(thisTime, UNBOX!(DateTime)(val))==0;
		}
		return false;
    }

    public static bool Equals(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)==0;
    }

    public DateTime Date() 
    {
		{
			// Need to remove UTC mask before arithmetic operations. Then set it back.
			if((this.m_ticks&DateTime.UTCMask)!=cast(ulong)0)
			{
				return   DateTime(Cast!(long)((((this.m_ticks&DateTime.TickMask)-(this.m_ticks&DateTime.TickMask)%DateTime.TicksPerDay)|DateTime.UTCMask)));
			}
			else
			{
				return   DateTime(Cast!(long)((this.m_ticks-this.m_ticks%DateTime.TicksPerDay)));
			}
        }

    }


    public int Day() 
    {
		{
			return GetDatePart(DateTime.DatePartDay);
        }

    }


    public _DayOfWeek DayOfWeek() 
    {
		{
			return _DayOfWeek(((this.m_ticks/DateTime.TicksPerDay+cast(ulong)1)%cast(ulong)7));
        }

    }


    public int DayOfYear() 
    {
		{
			return GetDatePart(DateTime.DatePartDayOfYear);
        }

    }

    /// Reduce size by calling a single method?


    public int Hour() 
    {
		{
			return Cast!(int)(((this.m_ticks/DateTime.TicksPerHour)%cast(ulong)24));
        }

    }


    public DateTimeKind Kind() 
    {
		{
			// If mask for UTC time is set - return UTC. If no maskk - return local.
			return ((this.m_ticks&DateTime.UTCMask)!=cast(ulong)0) ? (DateTimeKind.Utc) : (DateTimeKind.Local);
        }

    }


    public static DateTime SpecifyKind(DateTime value, DateTimeKind kind)
    {
		DateTime retVal =   DateTime(Cast!(long)(value.m_ticks));
		if(kind==DateTimeKind.Utc)
		{
			// Set UTC mask
			// Set UTC mask
			// Set UTC mask
			// Set UTC mask
			retVal.m_ticks=value.m_ticks|DateTime.UTCMask;
		}
		else
		{
			retVal.m_ticks=value.m_ticks&~DateTime.UTCMask;
		}
		return retVal;
    }

    public int Millisecond() 
    {
		{
			return Cast!(int)(((this.m_ticks/DateTime.TicksPerMillisecond)%cast(ulong)1000));
        }

    }


    public int Minute() 
    {
		{
			return Cast!(int)(((this.m_ticks/DateTime.TicksPerMinute)%cast(ulong)60));
        }

    }


    public int Month() 
    {
		{
			return 0;
        }

    }


    public static DateTime Now() 
    {
		{
			//Unsafe
			{
				//fixed() Scope
				{
					int* p = cast(int*)(&DateTime.timeBuffer[0]);
					//fixed() Scope
					{
						int* p2 = cast(int*)(&DateTime.timeBuffer[2]);
						{
							if(gettimeofday(p, p2)==0)
							{
								return   DateTime(p[0]*DateTime.TicksPerSecond+p[1]*10+p2[2]*DateTime.TicksPerMinute);
							}
						}
					}
				}
				throw  new NException();
			}
        }

    }


    public static DateTime UtcNow() 
    {
		{
			//Unsafe
			{
				//fixed() Scope
				{
					int* p = cast(int*)(&DateTime.timeBuffer[0]);
					{
						if(gettimeofday(p, null)==0)
						{
							return   DateTime(p[0]*DateTime.TicksPerSecond+p[1]*10, DateTimeKind.Utc);
						}
					}
				}
				throw  new NException();
			}
        }

    }


    public int Second() 
    {
		{
			return Cast!(int)(((this.m_ticks/DateTime.TicksPerSecond)%cast(ulong)60));
        }

    }


    public long Ticks() 
    {
		{
			return Cast!(long)((this.m_ticks&DateTime.TickMask))+DateTime.ticksAtOrigin;
        }

    }


    public TimeSpan TimeOfDay() 
    {
		{
			return   TimeSpan(Cast!(long)(((this.m_ticks&DateTime.TickMask)%DateTime.TicksPerDay)));
        }

    }


    public static DateTime Today() 
    {
		{
			return   DateTime();
        }

    }


    public int Year() 
    {
		{
			return 0;
        }

    }

    // Constructs a DateTime from a string. The string must specify a
    // date and optionally a time in a culture-specific or universal format.
    // Leading and trailing whitespace characters are allowed.
    // 

    public static DateTime Parse(String s)
    {
		//return (DateTimeParse.Parse(s, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None));
		return DateTime();
    }
    // Constructs a DateTime from a string. The string must specify a
    // date and optionally a time in a culture-specific or universal format.
    // Leading and trailing whitespace characters are allowed.
    // 

    public static DateTime Parse(String s, IFormatProvider provider)
    {
		//return (DateTimeParse.Parse(s, DateTimeFormatInfo.GetInstance(provider), DateTimeStyles.None));
		return DateTime();
    }

    public static DateTime Parse(String s, IFormatProvider provider, DateTimeStyles styles)
    {
		//   DateTimeFormatInfo.ValidateStyles(styles, "styles");
		//return (DateTimeParse.Parse(s, DateTimeFormatInfo.GetInstance(provider), styles));
		return DateTime.Now;
    }
    // Constructs a DateTime from a string. The string must specify a
    // date and optionally a time in a culture-specific or universal format.
    // Leading and trailing whitespace characters are allowed.
    // 

    public static DateTime ParseExact(String s, String format, IFormatProvider provider)
    {
		// return (DateTimeParse.ParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), DateTimeStyles.None));
		return DateTime.Now;
    }
    // Constructs a DateTime from a string. The string must specify a
    // date and optionally a time in a culture-specific or universal format.
    // Leading and trailing whitespace characters are allowed.
    // 

    public static DateTime ParseExact(String s, String format, IFormatProvider provider, DateTimeStyles style)
    {
		//DateTimeFormatInfo.ValidateStyles(style, "style");
		//return (DateTimeParse.ParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), style));
		return DateTime.Now;
    }

    public static DateTime ParseExact(String s, Array_T!(String) formats, IFormatProvider provider, DateTimeStyles style)
    {
		//DateTimeFormatInfo.ValidateStyles(style, "style");
		//return DateTimeParse.ParseExactMultiple(s, formats, DateTimeFormatInfo.GetInstance(provider), style);
		return DateTime.Now;
    }

    public TimeSpan Subtract(DateTime val)
    {
		return   TimeSpan(Cast!(long)((this.m_ticks&DateTime.TickMask))-Cast!(long)((val.m_ticks&DateTime.TickMask)));
    }

    public DateTime Subtract(TimeSpan val)
    {
		return  DateTime(cast(long)(m_ticks - cast(ulong)val.m_ticks));
		//return DateTime.Now;
    }

    public DateTime ToLocalTime()
    {
		throw  new NotImplementedException();
    }

    public String ToString()
    {
		//return DateTimeFormat.Format(this, null, DateTimeFormatInfo.CurrentInfo);
		return new String("");
    }

    public String ToString(String format)
    {
		//            return DateTimeFormat.Format(this, format, DateTimeFormatInfo.CurrentInfo);
		return new String("");
    }

    public String ToString(IFormatProvider provider, IConvertible __j = null)
    {
		//            return DateTimeFormat.Format(this, null, DateTimeFormatInfo.GetInstance(provider));
		return new String("");
    }

    public String ToString(String format, IFormatProvider provider, IFormattable __j = null)
    {
		//            return DateTimeFormat.Format(this, format, DateTimeFormatInfo.GetInstance(provider));
		return new String("");
    }

    public DateTime ToUniversalTime()
    {
		throw  new NotImplementedException();
    }

    public final DateTime  opBinary (string _op) (TimeSpan other)
		if(_op=="+")
		{ 
			return op_Addition(this,other); 
		}


    public final DateTime  opOpAssign (string _op) (TimeSpan other)
		if(_op=="+")
		{ 
			return op_Addition(this,other); 
		}


    public static DateTime  op_Addition(DateTime d, TimeSpan t)
    {
		//            return new DateTime((long)(d.m_ticks + (ulong)t.m_ticks));
		return DateTime.Now;
    }

    public final DateTime  opBinary (string _op) (TimeSpan other)
		if(_op=="-")
		{ 
			return op_Subtraction(this,other); 
		}


    public final DateTime  opOpAssign (string _op) (TimeSpan other)
		if(_op=="-")
		{ 
			return op_Subtraction(this,other); 
		}


    public static DateTime  op_Subtraction(DateTime d, TimeSpan t)
    {
		// return new DateTime((long)(d.m_ticks - (ulong)t.m_ticks));
		return DateTime.Now;
    }

    public final TimeSpan  opBinary (string _op) (DateTime other)
		if(_op=="-")
		{ 
			return op_Subtraction(this,other); 
		}


    public final TimeSpan  opOpAssign (string _op) (DateTime other)
		if(_op=="-")
		{ 
			return op_Subtraction(this,other); 
		}


    public static TimeSpan  op_Subtraction(DateTime d1, DateTime d2)
    {
		return d1.Subtract(d2);
    }

    public final bool  opEquals(DateTime other)
	{ 
			return op_Equality(this,other); 
	}


    public static bool  op_Equality(DateTime d1, DateTime d2)
    {
		return Compare(d1, d2)==0;
    }

  /*  public final bool  opEquals (string _op) (DateTime other)
		if(_op=="!=")
		{ 
			return op_Inequality(this); 
		}

*/
    public static bool  op_Inequality(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)!=0;
    }

    public final bool  opCmp (string _op) (DateTime other)
		if(_op=="<")
		{ 
			return op_LessThan(this); 
		}


    public static bool  op_LessThan(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)<0;
    }

    public final bool  opCmp (string _op) (DateTime other)
		if(_op=="<=")
		{ 
			return op_LessThanOrEqual(this); 
		}


    public static bool  op_LessThanOrEqual(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)<=0;
    }

    public final bool  opCmp (string _op) (DateTime other)
		if(_op==">")
		{ 
			return op_GreaterThan(this); 
		}


    public static bool  op_GreaterThan(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)>0;
    }

    public final bool  opCmp (string _op) (DateTime other)
		if(_op==">=")
		{ 
			return op_GreaterThanOrEqual(this); 
		}


    public static bool  op_GreaterThanOrEqual(DateTime t1, DateTime t2)
    {
		return Compare(t1, t2)>=0;
    }

    private int GetDatePart(int part)
    {
		ulong ticks = this.m_ticks;
		// n = number of days since 1/1/0001
		int n = Cast!(int)((ticks/DateTime.TicksPerDay));
		// y400 = number of whole 400-year periods since 1/1/0001
		int y400 = n/DateTime.DaysPer400Years;
		// n = day number within 400-year period
		// n = day number within 400-year period
		// n = day number within 400-year period
		n-=y400*DateTime.DaysPer400Years;
		// y100 = number of whole 100-year periods within 400-year period
		int y100 = n/DateTime.DaysPer100Years;
		// Last 100-year period has an extra day, so decrement result if 4
		if(y100==4)
		{
			y100=3;
		}
		// n = day number within 100-year period
		// n = day number within 100-year period
		// n = day number within 100-year period
		n-=y100*DateTime.DaysPer100Years;
		// y4 = number of whole 4-year periods within 100-year period
		int y4 = n/DateTime.DaysPer4Years;
		// n = day number within 4-year period
		// n = day number within 4-year period
		// n = day number within 4-year period
		n-=y4*DateTime.DaysPer4Years;
		// y1 = number of whole years within 4-year period
		int y1 = n/DateTime.DaysPerYear;
		// Last year has an extra day, so decrement result if 4
		if(y1==4)
		{
			y1=3;
		}
		// If year was requested, compute and return it
		if(part==DateTime.DatePartYear)
		{
			return y400*400+y100*100+y4*4+y1+1;
		}
		// n = day number within year
		// n = day number within year
		// n = day number within year
		n-=y1*DateTime.DaysPerYear;
		// If day-of-year was requested, return it
		if(part==DateTime.DatePartDayOfYear)
		{
			return n+1;
		}
		// Leap year calculation looks different from IsLeapYear since y1, y4,
		// and y100 are relative to year 1, not year 0
		bool leapYear = y1==3&&(y4!=24||y100==3);
		Array_T!(int) days = (leapYear) ? (DateTime.DaysToMonth366) : (DateTime.DaysToMonth365);
		// All months have less than 32 days, so n >> 5 is a good conservative
		// estimate for the month
		int m = n>>5+1;
		// m = 1-based month number
		while (n>=days[m])
		{
			m++;
		}
		// If month was requested, return it
		if(part==DateTime.DatePartMonth)
		{
			return m;
		}
		// Return 1-based day-of-month
		return n-days[m-1]+1;
    }

    public TypeCode GetTypeCode(IConvertible __j = null)
    {
		return TypeCode.DateTime;
    }
    /// <internalonly/>


    bool ToBoolean(IFormatProvider provider, IConvertible __j = null)
    {
		// throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Boolean"));
		return false;
    }
    /// <internalonly/>


    wchar ToChar(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Char"));
		return ' ';
    }
    /// <internalonly/>


    byte ToSByte(IFormatProvider provider, IConvertible __j = null)
    {
		// throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "SByte"));
		return cast(byte) cast(byte)0;
    }
    /// <internalonly/>


    ubyte ToByte(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Byte"));
		return cast(ubyte) cast(ubyte)0;
    }
    /// <internalonly/>


    short ToInt16(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Int16"));
		return cast(short) cast(short)0;
    }
    /// <internalonly/>


    ushort ToUInt16(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "UInt16"));
		return cast(ushort) cast(ushort)0;
    }
    /// <internalonly/>


    int ToInt32(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Int32"));
		return 0;
    }
    /// <internalonly/>


    uint ToUInt32(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "UInt32"));
		return cast(uint) cast(uint)0;
    }
    /// <internalonly/>


    long ToInt64(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Int64"));
		return cast(long) cast(long)0;
    }
    /// <internalonly/>


    ulong ToUInt64(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "UInt64"));
		return cast(ulong) cast(ulong)0;
    }
    /// <internalonly/>


    float ToSingle(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Single"));
		return cast(float) cast(float)0;
    }
    /// <internalonly/>


    double ToDouble(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Double"));
		return cast(double) cast(double)0;
    }
    /// <internalonly/>


    Decimal ToDecimal(IFormatProvider provider, IConvertible __j = null)
    {
		//            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", "DateTime", "Decimal"));
		return cast(Decimal) cast(Decimal)0;
    }

    DateTime ToDateTime(IFormatProvider provider, IConvertible __j = null)
    {
		//            return this;
		return DateTime.Now;
    }
    /// <internalonly/>


    NObject ToType(Type type, IFormatProvider provider, IConvertible __j = null)
    {
		//            return Convert.DefaultToType((IConvertible)this, type, provider);
		return cast(NObject) null;
    }

    public static void Main()
    {
    }

    public  void __init(long ticks)
    {
		if(((ticks&cast(long)(DateTime.TickMask))<DateTime.MinTicks)||((ticks&cast(long)(DateTime.TickMask))>DateTime.MaxTicks))
		{
			//throw  new ArgumentOutOfRangeException(String("ticks"), String("Ticks must be between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks."));
		}
		this.m_ticks=cast(ulong)(ticks);
    }

    public  void __init(long ticks, DateTimeKind kind)
    {
		__init(ticks);
		if(kind==DateTimeKind.Local)
		{
			this.m_ticks&=~DateTime.UTCMask;
		}
		else
		{
			this.m_ticks|=DateTime.UTCMask;
		}
    }

    public  void __init(int year, int month, int day)
    {
		__init(year, month, day, 0, 0, 0);
    }

    public  void __init(int year, int month, int day, int hour, int minute, int second)
    {
		__init(year, month, day, hour, minute, second, 0);
    }

    public  void __init(int year, int month, int day, int hour, int minute, int second, int millisecond)
    {
		throw  new NotImplementedException();
    }
    void __init(){}//default xtor
    static DateTime opCall(U...)(U args_)
    {
		DateTime s;
		s.__init(args_);
		return s;
    }

    static this()
    {
		MinValue =   DateTime(DateTime.MinTicks);

		MaxValue =   DateTime(DateTime.MaxTicks);

		DaysToMonth365 = new Array_T!(int) (__CC!(int[])([0,31,59,90,120,151,181,212,243,273,304,334,365]));

		DaysToMonth366 = new Array_T!(int) (__CC!(int[])([0,31,60,91,121,152,182,213,244,274,305,335,366]));

		timeBuffer = new Array_T!(int)(4);

		ticksAtOrigin = cast(long)0;

    }

	public static class __Boxed_ : Boxed!(DateTime) ,System.Namespace.IFormattable ,System.Namespace.IConvertible
	{
		import std.traits;

		public String ToString(String format, IFormatProvider provider, IFormattable __j = null)
		{
			return __Value.ToString(format, provider);
		}

		public TypeCode GetTypeCode(IConvertible __j = null)
		{
			return __Value.GetTypeCode();
		}

		bool ToBoolean(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToBoolean(provider);
		}

		wchar ToChar(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToChar(provider);
		}

		byte ToSByte(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToSByte(provider);
		}

		ubyte ToByte(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToByte(provider);
		}

		short ToInt16(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToInt16(provider);
		}

		ushort ToUInt16(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToUInt16(provider);
		}

		int ToInt32(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToInt32(provider);
		}

		uint ToUInt32(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToUInt32(provider);
		}

		long ToInt64(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToInt64(provider);
		}

		ulong ToUInt64(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToUInt64(provider);
		}

		float ToSingle(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToSingle(provider);
		}

		double ToDouble(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToDouble(provider);
		}

		Decimal ToDecimal(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToDecimal(provider);
		}

		DateTime ToDateTime(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToDateTime(provider);
		}

		public String ToString(IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToString(provider);
		}

		NObject ToType(Type type, IFormatProvider provider, IConvertible __j = null)
		{
			return __Value.ToType(type, provider);
		}

		this()
		{
			super(DateTime.init);
		}
		public override String ToString()
		{
			return __Value.ToString();
		}

		this(ref DateTime value)
		{
			super(value);
		}

		U opCast(U)()
			if(is(U:DateTime))
			{
				return __Value;
			}

		U opCast(U)()
			if(!is(U:DateTime))
			{
				return this;
			}

		auto opDispatch(string op, Args...)(Args args)
		{
			enum name = op;
			return __traits(getMember, __Value, name)(args);
		}

		public override Type GetType()
		{
			return __Value.GetType();
		}
	}

	public __Boxed_ __Get_Boxed()
	{
		return new __Boxed_(this);
	}
	alias __Get_Boxed this;

	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}