//
// System.Decimal.cs
//
// Represents a floating-point decimal data type with up to 29 
// significant digits, suitable for financial and commercial calculations.
//
// Authors:
//   Martin Weindel (martin.weindel@t-online.de)
//   Marek Safar (marek.safar@gmail.com)
//
// (C) 2001 Martin Weindel
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
// Copyright (C) 2014 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//Adapted from above

module System.Decimal;
import System.Namespace;

struct Decimal 
{

	public static class __Boxed_: Boxed!Decimal
	{
	}

	public const static Decimal MinValue = 0;//-79228162514264337593543950335;
	public const static Decimal MaxValue =  1;//79228162514264337593543950335;
	
	public const static Decimal MinusOne = -1;
	public const static Decimal One = 1;
	public const static Decimal Zero = 0;
	
	private static const Decimal MaxValueDiv10 = (cast(int)MaxValue / 10);
	
	// some constants
	private const static uint MAX_SCALE = 28;
	private const static uint SIGN_FLAG = 0x80000000;
	private const static int SCALE_SHIFT = 16;
	private const static uint RESERVED_SS32_BITS = 0x7F00FFFF;
	
	// internal representation of decimal
	private uint flags =0;
	private uint hi = 0;
	private uint lo = 0;
	private uint mid = 0;


	int opCast() const
	{
		return lo;
	}

	void opAssign(int value)
	{
		Decimal dec;
		this.lo = value;
		this.flags =0;
		this.hi  =0;
		this.mid = 0;
	}

	static Decimal op_Explicit(float a)
	{
		return Decimal(a);
	}

	public this (int lo, int mid, int hi, bool isNegative, byte scale)
	{
		//unchecked 
		{
			this.lo = cast(uint) lo;
			this.mid = cast(uint) mid;
			this.hi = cast(uint) hi;
			
//			if (scale > MAX_SCALE) 
//				throw new ArgumentOutOfRangeException (Locale.GetText ("scale must be between 0 and 28"));
			
			flags = scale;
			flags <<= SCALE_SHIFT;
			if (isNegative) 
				flags |= SIGN_FLAG;
		}
	}
	
	public this (int value) 
	{
		//unchecked 
		{
			hi = mid = 0;
			if (value < 0) 
			{
				flags = SIGN_FLAG;
				lo = (cast(uint)~value) + 1;
			}
			else 
			{
				flags = 0;
				lo = cast(uint) value;
			}
		}
	}

	//@CLSCompliant(false)
	public this (uint value) 
	{
		lo = value;
		flags = hi = mid = 0;
	}
	
	public this (long value) 
	{
		//unchecked 
		{
			hi = 0;
			if (value < 0) 
			{
				flags = SIGN_FLAG;
				ulong u = (cast(ulong)~value) + 1;
				lo = cast(uint)u;
				mid = cast(uint)(u >> 32);
			}
			else 
			{
				flags = 0;
				ulong u = cast(ulong)value;
				lo = cast(uint)u;
				mid = cast(uint)(u >> 32);
			}
		}
	}
	
	//@CLSCompliant(false)
	public this (ulong value) 
	{
		//unchecked 
		{
			flags = hi = 0;
			lo = cast(uint)value;
			mid = cast(uint)(value >> 32);
		}
	}
	
	public this (float value) 
	{
/*#if false
		//
		// We cant use the double2decimal method
		// because it incorrectly turns the floating point
		// value 1.23456789E-25F which should be:
		//    0.0000000000000000000000001235
		// into the incorrect:
		//   0.0000000000000000000000001234
		//
		//    The code currently parses the double value 0.6 as
		//    0.600000000000000
		//
		// And we have a patch for that called (trim
		if (double2decimal (out this, value, 7) != 0)
			throw new OverflowException ();
#else
		if (value > (float)Decimal.MaxValue || value < (float)Decimal.MinValue ||
		    float.IsNaN (value) || float.IsNegativeInfinity (value) || float.IsPositiveInfinity (value)) {
			throw new OverflowException (Locale.GetText (
				"Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", value));
		}
		
		// we must respect the precision (double2decimal doesn't)
		Decimal d = Decimal.Parse (value.ToString (CultureInfo.InvariantCulture),
		                           NumberStyles.Float, CultureInfo.InvariantCulture);*/
	//	flags = d.flags;
	//	hi = d.hi;
	//	lo = d.lo;
	//	mid = d.mid;
/*#endif*/
	}
	
	public this (double value) 
	{

		/*if (value > cast(double)Decimal.MaxValue || value < cast(double)Decimal.MinValue ||
		    double.IsNaN (value) || double.IsNegativeInfinity (value) || double.IsPositiveInfinity (value)) {
			throw new OverflowException (Locale.GetText (
				"Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", value));
		}
		// we must respect the precision (double2decimal doesn't)
		Decimal d = Decimal.Parse (value.ToString (CultureInfo.InvariantCulture),
		                           NumberStyles.Float, CultureInfo.InvariantCulture);
		flags = d.flags;
		hi = d.hi;
		lo = d.lo;
		mid = d.mid;*/

	}
	
	public this (int[] bits) 
	{
		if (bits == null) 
		{
//			throw new ArgumentNullException (Locale.GetText ("bits is a null reference"));
		}

		if (bits.length != 4) 
		{
//			throw new ArgumentException (Locale.GetText ("bits does not contain four values"));
		}

		//unchecked 
		{
			lo = cast(uint) bits[0];
			mid = cast(uint) bits[1];
			hi = cast(uint) bits[2];
			flags = cast(uint) bits[3];
			byte scale = cast(byte)(flags >> SCALE_SHIFT);
			if (scale > MAX_SCALE || (flags & RESERVED_SS32_BITS) != 0) 
			{
//				throw new ArgumentException (Locale.GetText ("Invalid bits[3]"));
			}
		}
	}
	
	public static Decimal FromOACurrency (long cy)
	{
//		return cast(Decimal)cy / cast(Decimal)10000;
		return Decimal();
	}
	
	public static int[] GetBits (Decimal d) 
	{
		//unchecked 
		{
			return  [cast(int)d.lo, cast(int)d.mid, cast(int)d.hi, cast(int)d.flags ];
		}
	}
	
	public static Decimal Negate (Decimal d) 
	{
		d.flags ^= SIGN_FLAG;
		return d;
	}
	
	public static Decimal Add (Decimal d1, Decimal d2) 
	{
//		if (decimalIncr ( d1,  d2) == 0)
//			return d1;
//		else
//			throw new OverflowException (Locale.GetText ("Overflow on adding decimal number"));
		return Decimal();
	}
	
	public static Decimal Subtract (Decimal d1, Decimal d2) 
	{
//		d2.flags ^= SIGN_FLAG;
//		int result = decimalIncr ( d1,  d2);
//		if (result == 0)
//			return d1;
//		else
//			throw new OverflowException (Locale.GetText ("Overflow on subtracting decimal numbers ("+result+")"));
		return Decimal();
	}
	
	public int GetHashCode () 
	{
		return cast(int) (flags ^ hi ^ lo ^ mid);
	}



	static Decimal op_Implicit_Decimal(int value)
	{
		Decimal aDec =  Decimal(value,0,0,false,1);
//		Console.WriteLine(aDec);
		return aDec;
//		return Decimal(value);

	}

//	private uint flags;
//	private uint hi;
//	private uint lo;
//	private uint mid;

	Decimal opBinary(string op)(Decimal other)
	{
		if(op=="+")
		{
			return Decimal(this.lo+other.lo,this.mid+other.mid,this.hi+other.hi,false,1);
		}

		if(op=="-")
		{
			return Decimal(this.lo-other.lo,this.mid+other.mid,this.hi+other.hi,false,1);

		}

	}

	Decimal opBinary(string op)(int other)
	{
		if(op=="+")
		{
			return Decimal(this.lo+other,0,0,false,1);
		}
		
		if(op=="-")
		{
			return Decimal(this.lo-other,0,0,false,1);

			
		}
		
	}

	Decimal opBinaryRight(string op)(int other)
	{
		if(op=="+")
		{
			return Decimal(this.lo+other,0,0,false,1);
		}
		
		if(op=="-")
		{
			return Decimal(this.lo-other,0,0,false,1);
			
			
		}
		
	}

	string toString() // Temp fix for test-9
	{
		return std.conv.to!(string)(lo);
	}

	bool opEquals(Decimal other)
	{
		return this.lo==other.lo && this.hi==other.hi && this.mid == other.mid;// && this.flags == other.flags;
	}
}

