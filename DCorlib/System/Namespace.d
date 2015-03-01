module System.Namespace;
import std.stdio;
import System.Boolean;
import System.String;
import std.conv;
import std.traits;
import std.typecons;

alias std.typecons.scoped StructAlloc;
alias System.NObject.NObject NObject;
alias System.ICloneable.ICloneable ICloneable;
alias System.Boolean.Boolean Boolean;

alias System.Byte.Byte Byte;
alias System.SByte.SByte SByte;

alias System.Int16.Int16 Int16;
alias System.UInt16.UInt16 UInt16;

alias System.Int32.Int32 Int32;
alias System.UInt32.UInt32 UInt32;

alias System.Int64.Int64 Int64;
alias System.UInt64.UInt64 UInt64;


alias System.Double.Double Double;
alias System.Single.Single Single;
alias System.Char.Char Char;
alias System.Decimal.Decimal Decimal;


alias System.String.String String;
alias System.Array.Array Array;
alias System.Array_T.Array_T Array_T;
alias System.Console.Console Console;
alias System.Math.Math Math;
alias System.GC.GC GC;

alias System.EmptyArray_T.EmptyArray_T EmptyArray_T;

alias System.Activator.Activator Activator;

import std.string;

import core.stdc.stdio;
import core.stdc.stdlib;
import core.sys.posix.dlfcn;

alias std.functional.toDelegate __ToDelegate;

//TODO: Improve this to reuse strings
public static String _S(wstring text)
{
	return new String((text));
}


public static String _S(string text) 
{
	return new String((text));
}

public static String _S(String text)
{
	return text;
}


//Locale Helper
struct Locale
{
	String GetText(String aString)
	{
		return aString;
	}

	String GetText(string aString)
	{
		return _S(aString);
	}

	String GetText(wstring aString)
	{
		return _S(aString);
	}
}

class __UNBOUND: NObject // object used to keep unbound generic types, for sharing and reflection
{
	NObject __Value;
	public this(NObject object)
	{
		__Value=object;
	}

	T opCast(T)() // __UNBOUND doesn't exist ;)
	{
		return cast(T) __Value;
	}
}

template __Tuple(E...)
{
	alias __Tuple = E;
}

static void __dummyFunc()
{
	
}

class IntPtr//:Boxed!(int*)
{
	int* m_value;
	public __gshared const  IntPtr Zero = new IntPtr(0);

	this(long pointer)
	{

		m_value = cast(int*)pointer;
	}

	static IntPtr op_Explicit(long pointer)
	{
		
		return new IntPtr(pointer);
	}



	/*class __Boxed_:Boxed!(IntPtr)
	{
		this(ref IntPtr ptr)
		{
			super(ptr);
		}
	}*/

}


public int Hash(const(String) aString)
{
	if(aString is null)
		return -1;

	import xxhash;
	
	uint hashed = xxhashOf(aString.Text);
	return cast(int) hashed;
}


public int Hash(String aString)
{
	if(aString is null)
		return -1;

	return aString.GetHashCode();
}

@trusted pure nothrow
public int Hash(in const(string) aString)
{
	if(aString is null)
		return -1;
	
	import xxhash;
	
	uint hashed = xxhashOf(aString);
	return cast(int) hashed;
}


//public int Hash(wstring aString)
//{
//	if(aString is null)
//		return -1;
//	
//	return aString.GetHashCode();
//}

public struct Nullable_T(T) //where T : struct
{
	private bool hasValue_ =false; 
	private T value_=T.init; 

	/*static Nullable opCall(T value) {
		Optional self;
		self.value_ = value;
		self.hasValue_ = true;
		return self;
	}*/


	void opAssign(T value)
	{
		value_ = value;
		hasValue_ = true;
	}

	void opAssign(Object value = null)
	{
		if(value is null)
		{
			value_ = T.init;
			hasValue_ = false;
		}
		hasValue_ = false;
	}

	int opCmp(Nullable_T!(T) other) {
		if (HasValue) {
			if (other.hasValue_)
				return typeid(T).compare(&value_, &other.value_);
			return 1;
		}
		if (other.HasValue)
			return -1;
		return 0;
	}

	int opEquals(Nullable_T!(T) other) {
		if (HasValue) {
			if (other.HasValue)
				return typeid(T).equals(&value_, &other.value_);
			return false;
		}
		if (other.HasValue)
			return false;
		return true;
	}

	/*public this(T value) 
	{
		this.value_ = value;
		this.hasValue_ = true;
	}   */     

	public bool HasValue() @property 
	{
		
			return hasValue_;
		
	} 

	public T Value() @property {
		
			if (!HasValue) 
			{
				throw new NException(_S("Nullable object must have a value."));
				//ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_NoValue);
			}
			return value_;
		
	}

	public T GetValueOrDefault() 
	{
		return value_;
	}

	public T GetValueOrDefault(T defaultValue) 
	{
		return HasValue ? value_ : defaultValue;
	}

	/*public override bool Equals(NObject other)  
	{
		if (!HasValue) return other == null;
		if (other == null) return false;
		return value.Equals(other);
	}*/

	public  int GetHashCode() 
	{
		return HasValue ? cast(int)value_.toHash : 0;
	}

	public  String ToString() 
	{
		return HasValue ? _S(value_.toString) : String.Empty;
	}

	public void Dispose(IDisposable j=null)
	{
		value_ = T.init;
		hasValue_ = false;
	}

	//alias value_ this;

	/*public static implicit operator Nullable<T>(T value) {
		return new Nullable<T>(value);
	}

	public static explicit operator T(Nullable<T> value) {
		return value.Value;
	}*/

	// The following already obsoleted methods were removed:
	//   public int CompareTo(object other)
	//   public int CompareTo(Nullable<T> other)
	//   public bool Equals(Nullable<T> other)
	//   public static Nullable<T> FromObject(object value)
	//   public object ToObject()
	//   public string ToString(string format)
	//   public string ToString(IFormatProvider provider)
	//   public string ToString(string format, IFormatProvider provider)

	// The following newly obsoleted methods were removed:
	//   string IFormattable.ToString(string format, IFormatProvider provider)
	//   int IComparable.CompareTo(object other)
	//   int IComparable<Nullable<T>>.CompareTo(Nullable<T> other)
	//   bool IEquatable<Nullable<T>>.Equals(Nullable<T> other)
}

/*
string __GetCsName(T)
{

	static if(is(T==class))
	{
		return __TypeOf!(T).Fu
	}
	//instead of manually doing this we are going to use boxed names
/*	if(T.stringof == "int")
		return "System.Int32";

	if(T.stringof == "long")
		return "System.Int64";

	if(T.stringof == "double")
		return "System.Double";

	if(T.stringof == "float")
		return "System.Single";
*/
//	return T.stringof;
//}

public static String ToString(T)(T basicType) // Should just implement this on all basic types
{
	return BOX!(T)(basicType).ToString();
}

public static __IA!(T) __CC(T)(T a) // create container
{
		__IA!(T) rr =  __IA!(T)(); // Need to find a way to get rid of new, this is an extra allocation ... ?
		rr.A = a;
		return rr;
}


struct __IA(T) //Internal Array Struct, to make array creation easier
{
	T A;

	final T opCast(T)()
	{
		return A;	
	}

	//alias A this; //Looks like this has to be done manually :(
	
}

//Better Properties ... cant really use these ... lead to a host of problems

__Property!(T) __CProperty(T)( void delegate(T) setter, T delegate() getter)
{
	auto property = __Property!(T).init;
	property._setter = setter;
	property._getter = getter;
	return property;
}

struct __Property(T)
{
	void delegate(T) _setter;
	T delegate() _getter;

//	this( void delegate(T) setter, T delegate() getter)
//	{
//		_setter = setter;
//		_getter = getter;
//	}


	T __getValue()
	{
		return cast(T)_getter();
	}
	
	alias __getValue this;

	T opCall(T)()
	{
		return _getter();
	}

	T opCall(T)(T value)
	{
		_setter(value);
		return _getter();
	}

//	__Property!(T) opCall(T)( void delegate(T) setter, T delegate() getter)
//	{
//	            		_setter = setter;
//	            		_getter = getter;
//		return this;
//	}


	/*auto opCall(U...)(U args)
	{
		//Console.WriteLine("called opCall");
		static if(args.length ==1)
		{
		//	Console.WriteLine("called setter");
			
			_setter(args);
		}
		static if(args.length==0)
		{
		//	Console.WriteLine("called getter");
			
			return _getter();
		}
	}*/
	
	bool opEquals(T other)
	{
		return _getter() == other;
	}
	
	T opAssign(T value)
	{
		_setter(value);
		return _getter();
	}
	
	T opBinary(string op)(T rhs)
	{
		return mixin("_getter() "~op~" rhs");
		/* static if (op == "+") return _getter() + rhs;
      static if (op == "-") return _getter() - rhs;
      */
	}
	
	T opBinaryRight(string op,R)(R lhs)
	if(!is(R:T))
	{
		return mixin("lhs "~op~" _getter()");
	}
	
	string toString()
	{
		return _getter().toString;
	}
	
//	auto opDispatch(U...)(U args)
//	{
//		return _getter()(args);
//	}
	
	T opUnary(string s)() if (s == "-")
	{
		return -(_getter()); 
	}
	
	T opUnary(string s)() if (s == "++")
	{
		auto value = _getter() + 1; //Weird C# behaviour
		_setter(_getter() + 1);
		return value; 
	}
	
	T opUnary(string s)() if (s == "--")
	{
		auto value = _getter() - 1; //Weird C# behaviour
		_setter(_getter() - 1);
		return value; 
	}
	
	T opUnary(string s)() if (s == "*")
	{
		return *_getter(); 
	}
	
}


Array_T!T __ARRAY(T)(T[] values)
{
	return new Array_T!(T)(__CC!(T[])(values));
}

/*bool __IsInteger(T)()
{
	return (is(T==int) || is(T==double) || is(T==float) || is(T==uint));
}
*/

//Provides .Net's (default() keyword)
T __Default(T)()
if(is(T==class))
{
		return cast(T)null;
}


T __Default(T)()
if(is(T==int) || is(T==double) || is(T==float) || is(T==uint) || is(T==wchar))
{

		return cast(T)0;
}

T __Default(T)()
if(is(T==struct))
{
	return T.init;
}

__Void __Default(T)()
if(is(T==void))
{
	return __Void.init;
}


T __Default(T)()
if(!(is(T==int) || is(T==double) || is(T==float) || is(T==uint) || is(T==wchar)|| is(T==struct) || is(T==class) || is(T==void)))
{

	return T.init;
}

	T __TypeNew(T,U...)(U args)
    if(is(T==class))
    {
		//Console.WriteLine("__TypeNew Class " ~ T.stringof);
		static if(__traits(compiles,new T(args)))
		{
			return new T(args);
		}
		else //abstract class
		{
			return null;
		}
    }

    T __TypeNew(T,U...)(U args)
    if(is(T==struct))
    {
	//Console.WriteLine("__TypeNew Struct " ~ T.stringof);
		static if(__traits(compiles,T(args)))
		{
		return T(args);
		//	return T.__init(args);
		}
		return __Default!(T)();
    }

	T __TypeNew(T,U...)(U args)
    if(!is(T==struct)&&!is(T==class)&&!is(T==void))
    {
	//Console.WriteLine("__TypeNew !Struct !Class " ~ T.stringof);
		
		return __Default!(T)();
    }

	__Void __TypeNew(T,U...)(U args)
    if(is(T==void))
    {
		//Console.WriteLine("__TypeNew !Struct !Class " ~ T.stringof);

		return __Default!(T)();
    }

//http://forum.dlang.org/thread/50992AC2.5020807@webdrake.net ... Simen Kjaeraas
template __TINFO( T ) { //InstantiationInfo
    static if ( is( T t == U!V, alias U, V... ) ) {
        alias V[0] P;
    } else {
        static assert(false, T.stringof ~ " is not a template type  
instantiation.");
    }
}

public static double MinValue(double value)
{
	return double.min_normal;
}


class Delegate:NObject
{
}

class __Delegate(T): Delegate
{
	T dFunc;
    __Delegate!T[] funcs;


	this()
	{
		dFunc = null;
		funcs = null;
	}

	this(typeof(T.funcptr) func) // Allows direct storage of functions too
	{
		dFunc = __ToDelegate(func);
		funcs = null;
	}

	this(T func)
	{
		dFunc = func;
		funcs = null;
	}

	this(__Delegate!(T) other)
	{
		dFunc = other.dFunc;
		funcs = other.funcs;
	}

	T Function()
	{
		return dFunc;
	}

	T opCast(T)()
	if(is(R==T))
	{
		return dFunc;
	}

	NObject opCast(R)()
	if(is(R==class))
	{
		return this;
	}

	R opCast(R)()
	if(!is(R==class) &&!is(R==T))
	{
			return cast(R)this;
	}


	ReturnType!T opCall(U ...)(U args)
	{

		//Console.WriteLine(dFunc is null);
		//Console.WriteLine(ReturnType!T.stringof);
		
		// Ideally, you shouldnt expect a result from multiple delegates ... thats crazy
		if(dFunc !is null)
		{
			static if(!is(ReturnType!T == void))
				return dFunc(args);
			else
				dFunc(args);
		}


		if(funcs !is null)
		{
			 foreach (func; funcs)
        	{
            	if(func !is null)
            		func(args);
        	}
		}

		static if(!is(ReturnType!T == void))
			return ReturnType!T.init;
		

		//assert(0);
	}


	ReturnType!T Invoke(U ...)(U args)
	{
		static if(!is(ReturnType!T == void))
			return opCall(args);
		else
			opCall(args);
	}

	

	

	 void attach(__Delegate!T func) {
        if (func)
            funcs ~= func;
    }

    void detach(__Delegate!T func) {
        ulong i = -1;
        //Console.WriteLine("Removing " ~ (&(func.Function)).toString);
	//	Console.WriteLine(((cast(void*)func.Function)).toString);

        for (int j=0;j< funcs.length; j++)
        {
			auto h = funcs[j];
        //	Console.WriteLine((&(h.Function)).toString);
        	//Console.WriteLine(((cast(void*)h.Function)).toString);

            if (h.Function is func.Function)
            {
        	//Console.WriteLine("Found it! j==" ~ j.toString);

                i = j;
                break;
            }
        }

        if (i != -1)
            funcs = funcs[0..cast(uint)i] ~ funcs[cast(uint)i+1..$];

		if(dFunc !is null && dFunc==func.Function) // Function could be the primary one we added during init ;)
		{
        //	Console.WriteLine("Found it!");
			dFunc = null;
		}
        
    }

   

    void opAddAssign(__Delegate!T func) {
        attach(func);
    }

    void opSubAssign(__Delegate!T func) {
        detach(func);
    }

	
}

class __Event(T):NObject //if(is(T==delegate))
{
   // T handler_t;

    T[] handlers;
    NObject owner;
    Action_T!(T) attacher = null;
    Action_T!(T) detacher = null;

	this()
	{

	}

    this(Action_T!(T) _attacher, Action_T!(T) _detacher) {
    	attacher = _attacher;
    	detacher = _detacher;
    }
    this(NObject o) { owner = o; }

    void attach(T handler) {
        if (handler)
        {
        	if(attacher !is null)
        		attacher(handler); //TODO: fix this attacher(handler);
        	else
            	handlers ~= handler;
        }
    }

    void detach(T handler) {
      if(detacher !is null)
      		detacher(handler); //TODO: fix this detacher(handler);
      else
      {
        ulong i = -1;
        foreach (j, h; handlers)
        {
            if (h is handler)
            {
                i = j;
                break;
            }
        }

        if (i > -1)
            handlers = handlers[0..i] ~ handlers[i+1..$];
    	}
    }

    void Invoke(U...)(U u) 
    //if(!is(u[0] : EventArgs))
    { 
    	//auto e = new EventArgs(owner);
    	//Invoke(new EventArgs(owner), u); 
    	 // call all handlers
        foreach (handler; handlers) //TODO fix this
        {
            //if (handler)
            //    handler(e);
            if(handler)
            	handler(u);
        }

    }
    
    //void Invoke(U...)(EventArgs e,U u)
    //if(!is(u[0] : EventArgs))
    //{
    //    // call all handlers
    //    foreach (handler; handlers) //TODO fix this
    //    {
    //        //if (handler)
    //        //    handler(e);
    //        if(handler)
    //        	handler(u);
    //    }
    //}

    void opCall(U...)(U u)
	{
		Invoke(u);
	}

    void opAddAssign(T handler) 
    {
        attach(handler);
    }

    void opSubAssign(T handler) 
    {
        detach(handler);
    }

}

class EventArgs {
    NObject source;
    bool handled;

    void handle() { handled = true; }

    this() {}
    this(NObject s) {
        source = s;
    }
}


class Random
{

import std.random;
	std.random.Random random;

	this() 
	{
		this(0);
	}

	this(int seed) //Need proper implementation
	{
//		random = new std.random.Random();
	}

	public	int Next()
	{
		return uniform(cast(int)0, int.max, random);
	}

public	double NextDouble()
	{
		return uniform(cast(double)0, cast(double)int.max,random);
	}
}


public static bool Equals(T)(T object, NObject other)
//if(!is(T==R) && !is(T==class) && is(R:NObject))
{
//	Console.WriteLine("Equals o, obj");
	return object == UNBOX!(T)(other);
}

//boxed.Value.opEquals(this.Value)
//Allows equals comparison between most basic types
public static bool Equals(T)(T object, T other)
//if(is(T==R))
{
	Console.WriteLine("comparing two eq");
//
//	static if(is(T:NObject) && is(R:NObject))
//	{
//		Console.WriteLine("comparing two n,n");
//
//		if(object == other)
//		{
//			return true;
//		}
//
//		return object.Equals(other);
//	}
//
//	static if(is(T==R))
//	{
//		Console.WriteLine("comparing two steuc");
//		return object.opEquals(other);
//	}
//
//	static if(is(R==class) && !is(T==class))
//	{
//		Console.WriteLine("comparing two c, nc");
//
//		return object == UNBOX!(T)(other);
//	}

	return object == other;
}


public static string toString(T)(T value) if(!is(T==struct) && !is(T==double) && !is(T==float) && !is(T==int)) 
{

	//if(is(T==double))
	//{
	//	return toString_D(value);
	//}

	//if(is(T==float))
	//{
	//	return toString_D(value);
	//}

	return to!string(value);
}

public static string toString(T)(T value) if(is(T==int))
{
	return to!string(value);
}

public static string toString(T)(T value) if(is(T==struct))
{
	return to!string(value);
}


public static string toString(T)(T value) if(is(T==bool))
{
	return value?"True":"False";
}
public static string toString(T)(T value) if(is(T==double))
{
	auto str = format("%.11f", value);


	if(str.lastIndexOf('.')!=-1)
	{
		while(str.length > 0 && str[str.length-1]=='0')
			str.length = str.length-1;
	}

	if(str.length>0 && str[str.length-1]=='.')
		str.length = str.length-1;
	return str;
}


public static string toString(T)(T value) if(is(T==float))
{
	auto str = format("%.3f", value);

	if(str.lastIndexOf('.')!=-1)
	{
		while(str.length > 0 && str[str.length-1]=='0')
			str.length = str.length-1;
	}

	if(str.length>0 && str[str.length-1]=='.')
		str.length = str.length-1;
	return str;
}

// we have to give it another name ... damn :(
public class NException : Exception  
{
	
	
	/*public Exception E;
	
	alias  E this;*/

	public NObject __Boxed_;
	alias  __Boxed_ this;

	NException _innerException;

	public NException InnerException() @property
	{
		return _innerException;
	}

	public this(String message, NException innerException)
  	{
  		_innerException = innerException;
    	super(cast(string)cast(wstring)message);
  
	}

	public this() @safe
	{
    	super("Exception");
	}

	public this(string value) @safe
	{
		super(value);
	}

	public this(String message)
	{
			super(cast(string)cast(wstring)message);
	}

	public String Message() @property
	{
		return new String(msg);
	}

	public String _Exception_Message() @property
	{
		return new String(msg);
	}

	public String StackTrace() @property
	{
		return new String(file ~":"~ to!string(line) ~"\r\n" ~ info.toString);
	}

	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

	public  String ToString()
	{
		return _Exception_Message();
	}
}

/*
double random() {
  synchronized {
    static Random rand;
    if (rand is null)
      rand = new Random;
    return rand.nextDouble();
  }
}

// Based on ran3 algorithm.
class Random {

  enum SEED = 161803398;
  enum BITS = 1000000000;

  private int[56] seedList_;
  private int next_, nextp_;

  this() {
    this(GetTickCount());
  }

  this(int seed) {
    int j = SEED - abs(seed);
    seedList_[55] = j;
    int k = 1;
    for (int c = 1; c < 55; c++) {
      int i = (21 * c) % 55;
      seedList_[i] = k;
      k = j - k;
      if (k < 0)
        k += BITS;
      j = seedList_[i];
    }

    for (int c = 1; c <= 4; c++) {
      for (int d = 1; d <= 55; d++) {
        seedList_[d] -= seedList_[1 + (d + 30) % 55];
        if (seedList_[d] < 0)
          seedList_[d] += BITS;
      }
    }

    nextp_ = 21;
  }

  int Next() {
    if (++next_ >= 56)
      next_ = 1;
    if (++nextp_ >= 56)
      nextp_ = 1;
    int result = seedList_[next_] - seedList_[nextp_];
    if (result < 0)
      result += BITS;
    seedList_[next_] = result;
    return result;
  }

  int Next(int max) {
    return cast(int)(Sample() * max);
  }

  int Next(int min, int max) {
    int range = max - min;
    if (range < 0) {
      long lrange = cast(long)(max - min);
      return cast(int)(cast(long)(Sample() * cast(double)lrange) + min);
    }
    return cast(int)(Sample() * range) + min;
  }

  double NextDouble() {
    return sample();
  }

  protected double Sample() {
    return next() * (1.0 / BITS);
  }

}
*/
public class Attribute: NObject
{

}



public enum PlatformID 
{
		Win32S = 0,
		Win32Windows = 1,
		Win32NT = 2,
		WinCE = 3,
		Unix = 4
}




		public  class Version : NObject
		{//, IComparable, IComparable<Version>, IEquatable<Version> {
		int _Major, _Minor, _Build, _Revision;

		private static const int UNDEFINED = -1;

		private void CheckedSet(int defined, int major, int minor, int build, int revision) 
		{
			// defined should be 2, 3 or 4

			if (major < 0) {
				throw new ArgumentOutOfRangeException(new String("major"));
			}
			this._Major = major;

			if (minor < 0) {
				throw new ArgumentOutOfRangeException(new String("minor"));
			}
			this._Minor = minor;

			if (defined == 2) {
				this._Build = UNDEFINED;
				this._Revision = UNDEFINED;
				return;
			}

			if (build < 0) {
				throw new ArgumentOutOfRangeException(new String("build"));
			}
			this._Build = build;

			if (defined == 3) {
				this._Revision = UNDEFINED;
				return;
			}

			if (revision < 0) {
				throw new ArgumentOutOfRangeException(new String("revision"));
			}
			this._Revision = revision;
		}

		public this() 
		{
			CheckedSet(2, 0, 0, -1, -1);
		}

		/*public this(String version) 
		{
			int n;
			string[] vals;
			int major = -1, minor = -1, build = -1, revision = -1;

			if (version == null) {
				throw new ArgumentNullException("version");
			}

			vals = version.Split('.');
			n = vals.Length;

			if (n < 2 || n > 4) {
				throw new ArgumentException("There must be 2, 3 or 4 components in the version string.");
			}

			if (n > 0) {
				major = int.Parse(vals[0]);
			}
			if (n > 1) {
				minor = int.Parse(vals[1]);
			}
			if (n > 2) {
				build = int.Parse(vals[2]);
			}
			if (n > 3) {
				revision = int.Parse(vals[3]);
			}

			CheckedSet(n, major, minor, build, revision);
		}*/

		public this(int major, int minor) 
		{
			CheckedSet(2, major, minor, 0, 0);
		}

		public this(int major, int minor, int build) 
		{
			CheckedSet(3, major, minor, build, 0);
		}

		public this(int major, int minor, int build, int revision) 
		{
			CheckedSet(4, major, minor, build, revision);
		}

		public int Build() @property 
		{
				return _Build;
		}

		public int Major() @property 
		{
				return _Major;
		}

		public int Minor() @property 
		{
				return _Minor;
		}

		public int Revision() @property
		{
				return _Revision;
		}

		public short MajorRevision() @property 
		{
				return cast(short)(_Revision >> 16);
		}

		public short MinorRevision() @property 
		{
			
				return cast(short)_Revision;
			
		}

		public override NObject ICloneable_Clone() 
		{
			if (_Build == -1) {
				return new Version(_Major, _Minor);
			} else if (_Revision == -1) {
				return new Version(_Major, _Minor, _Build);
			} else {
				return new Version(_Major, _Minor, _Build, _Revision);
			}
		}

		public int CompareTo(NObject version_) 
		{
			if (version_ is null) {
				return 1;
			}
			if (!(IsCast!(Version)(version_) )) {
				throw new ArgumentException(new String("Argument to Version.CompareTo must be a Version."));
			}
			return this.CompareTo(cast(Version)version_);
		}

		public override bool Equals(NObject obj) 
		{
			return this.Equals(cast(Version)obj);
		}

		public int CompareTo(Version v) 
		{
			if (v is null) 
			{
				return 1;
			}
			if (this._Major > v._Major) 
			{
				return 1;
			} 
			else if (this._Major < v._Major) 
			{
				return -1;
			}
			if (this._Minor > v._Minor) 
			{
				return 1;
			} 
			else if (this._Minor < v._Minor) 
			{
				return -1;
			}
			if (this._Build > v._Build) 
			{
				return 1;
			} else if (this._Build < v._Build) 
			{
				return -1;
			}
			if (this._Revision > v._Revision) 
			{
				return 1;
			} 
			else if (this._Revision < v._Revision) 
			{
				return -1;
			}
			return 0;
		}

		public bool Equals(Version x) 
		{
			return ((x !is null) &&
				(x._Major == _Major) &&
				(x._Minor == _Minor) &&
				(x._Build == _Build) &&
				(x._Revision == _Revision));
		}

		public override int GetHashCode() const
		{
			return (_Revision << 24) | (_Build << 16) | (_Minor << 8) | _Major;
		}

		// <summary>
		//   Returns a stringified representation of the version, format:
		//   major.minor[.build[.revision]]
		// </summary>
		public override String ToString() 
		{
			String mm = _Major.ToString() + new String(".") + _Minor.ToString();

			if (_Build != UNDEFINED) {
				mm = mm + new String(".") + _Build.ToString();
			}
			if (_Revision != UNDEFINED) {
				mm = mm + new String(".") + _Revision.ToString();
			}

			return mm;
		}

		// <summary>
		//    LAME: This API is lame, since there is no way of knowing
		//    how many fields a Version object has, it is unfair to throw
		//    an ArgumentException, but this is what the spec claims.
		//
		//    ie, Version a = new Version (1, 2);  a.ToString (3) should
		//    throw the expcetion.
		// </summary>
		public String ToString(int fields) 
		{
			if (fields == 0) {
				return String.Empty;
			}
			if (fields == 1) {
				return _Major.ToString();
			}
			if (fields == 2) {
				return _Major.ToString() + new String(".") + _Minor.ToString();
			}
			if (fields == 3) {
				if (_Build == UNDEFINED) {
					throw new ArgumentException
						(new String("fields is larger than the number of components defined in this instance."));
				}
				return _Major.ToString() + new String(".") + _Minor.ToString() + new String(".") + _Build.ToString();
			}
			if (fields == 4) {
				if (_Build == UNDEFINED || _Revision == UNDEFINED) {
					throw new ArgumentException
						(new String("fields is larger than the number of components defined in this instance."));
				}
				return _Major.ToString() + new String(".") + _Minor.ToString() + new String(".") + _Build.ToString() + new String(".") + _Revision.ToString();
			}
			throw new ArgumentException(new String("Invalid fields parameter: ") + fields.ToString());
		}

		/*public static bool operator ==(Version v1, Version v2) {
			return Equals(v1, v2);
		}

		public static bool operator !=(Version v1, Version v2) {
			return !Equals(v1, v2);
		}

		public static bool operator >(Version v1, Version v2) {
			return v1.CompareTo(v2) > 0;
		}

		public static bool operator >=(Version v1, Version v2) {
			return v1.CompareTo(v2) >= 0;
		}

		public static bool operator <(Version v1, Version v2) {
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator <=(Version v1, Version v2) {
			return v1.CompareTo(v2) <= 0;
		}*/

		// a very gentle way to construct a Version object which takes 
		// the first four numbers in a string as the version
		static Version CreateFromString(String info) {
			int major = 0;
			int minor = 0;
			int build = 0;
			int revision = 0;
			int state = 1;
			int number = UNDEFINED; // string may not begin with a digit

			if (info is null) 
			{
				return new Version(0, 0, 0, 0);
			}
			for (int i = 0; i < info.Length; i++) {
				wchar c = info[i];
				if (Char.IsDigit(c)) {
					if (number < 0) {
						number = (c - '0');
					} else {
						number = (number * 10) + (c - '0');
					}
				} else if (number >= 0) {
					// assign
					switch (state) {
						case 1:
							major = number;
							break;
						case 2:
							minor = number;
							break;
						case 3:
							build = number;
							break;
						case 4:
							revision = number;
							break;
						default:
							break;
					}
					number = -1;
					state++;
				}
				// ignore end of string
				if (state == 5)
					break;
			}

			// Last number
			if (number >= 0) {
				switch (state) {
					case 1:
						major = number;
						break;
					case 2:
						minor = number;
						break;
					case 3:
						build = number;
						break;
					case 4:
						revision = number;
						break;
					default:
							break;
				}
			}
			return new Version(major, minor, build, revision);
		}
	}


public class OperatingSystem : NObject{

		private PlatformID platformID;
		private  System.Namespace.Version _version;

		public this(PlatformID platformID,  System.Namespace.Version _version) 
		{
			if (_version is null) {
				throw new ArgumentNullException(new String("version"));
			}
			this.platformID = platformID;
			this._version = _version;
		}

		public PlatformID Platform() @property 
		{
				return this.platformID;
		}

		public  System.Namespace.Version Version() @property
		{
			
				return this._version;
			
		}

		public String ServicePack() @property
		{
			
				return String.Empty;
			
		}

		public String VersionString() @property
		{
			
				return ToString();
			
		}

		public override String ToString() {
			string str;

			switch (this.platformID) {
				case PlatformID.Win32NT:
					str = "Microsoft Windows NT";
					break;
				case PlatformID.Win32S:
					str = "Microsoft Win32S";
					break;
				case PlatformID.Win32Windows:
					str = "Microsoft Windows 98";
					break;
				case PlatformID.WinCE:
					str = "Microsoft Windows CE";
					break;
				case PlatformID.Unix:
					str = "Unix";
					break;
				default:
					str = "<unknown>";
					break;
			}

			return new String(str ~ " " ~ cast(string)(this._version.ToString()) ~ " (DNA)");
		}

		//#region ICloneable Members

		public override NObject ICloneable_Clone() 
		{
			return cast(OperatingSystem)this.ICloneable_Clone();
		}

		//#endregion
	}


Type GetType(T)(T c)
{
	return __GetBoxedType!(T)();
}



class Boxed (T) : NObject
{
	

    this(T value = T.init)
    {
        this.__Value = value;
    }

    //U opCast(U)()
   	//if(is(U == T))
    //{ 
    //	return Value; 
    //}

	//U opCast(U)()
 //  	if(!is(U == T))
 //   { 
 //   	return cast(U) this; 
 //   }    
	alias __Value this;

    public:
    T __Value;

    public override string toString()
	{
		static if(is(T==bool))
		{
			return __Value?"True":"False";
		}

		//if(is(T==double))
		//{
		//	return Value.toString;
		//}

		//if(is(T==float))
		//{
		//	return Value.toString;
		//}
		//static if(!is(T==double) && !is(T==long) && !is(T==byte))
		{
			return to!string(__Value);
		}

		
		//else
		{
			//Console.WriteLine("integer:");
	//		return Value.toString;
		}
	}

	public override String ToString()
	{
		return new String(to!wstring(this.toString));
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));

	}

}

class CsNative
{
public	static String NullStringCheck(String object)
	{
		if(object is null)
			return String.Empty;
		else
			return object.ToString();
	}

}
/**
 * <code>bool delegate(T a, T b)</code>
 */
template EqualityComparison(T) {
	alias bool delegate(T a, T b) EqualityComparison;
}

/**
 * <code>int delegate(T a, T b)</code>
 */
template Comparison(T) {
	alias int delegate(T a, T b) Comparison;
}

/**
 * <code>bool delegate(T obj)</code>
 */
template Predicate(T) {
	alias bool delegate(T) Predicate;
}

/**
 * <code>TOutput delegate(TInput input)</code>
 */
template Converter(TInput, TOutput) {
	alias TOutput delegate(TInput) Converter;
}

template Func_T_TResult( T , TResult )
{
	alias __Delegate!(TResult delegate(T arg) ) Func_T_TResult;
}


template Action_T( T )
{
	alias __Delegate!(void delegate(T arg) ) Action_T;
}


alias __Delegate!(void delegate()) Action;


/**
 * <code>void delegate(T obj)</code>
 */
 //alias Delegate(void delegate(T obj)) Action;
//template Action(T) {
//	alias void delegate(T obj) Action;
//}

template Func_TResult( TResult )
{
	alias __Delegate!(TResult delegate() ) Func_TResult;
}

 //alias Delegate(TResult delegate(T obj)) Func_T_TResult;
//template Func_T_TResult(T, TResult) {
//	alias TResult delegate(T input) Func_T_TResult;
//}

static Boxed!T BOX(T)(T value)
if(is(T == struct) && !__traits(compiles,T.__IsEnum==true))
{
	return new T.__Boxed_(value);
}

static BoxedEnum!T BOX(T)(T value)
if(is(T==struct) && __traits(compiles,T.__IsEnum==true))
{
	//Console.WriteLine(__traits(compiles,T.__IsEnum==true));
	return new BoxedEnum!(T)(value);
}
static T BOX(T)(NObject value)
if(is(T == class))
{
	return cast(T)value;
}

static T BOX(T)(Object value)
if(is(T == class))
{
	return cast(T)value;
}


static Boxed!(T) BOX(T)(T value)
if(!is(T==class) && !is(T==struct) &&!is(T==enum))
{

	auto boxedPrimitive = __BOXPrimitive(value);
	if(boxedPrimitive !is null)
		return cast(Boxed!(T))boxedPrimitive;

	return new Boxed!(T)(value);
}

static Boxed!__Void BOX(T)(__Void value)
if(is(T == void))
{
	return new Boxed!(__Void);
}

Type __GetBoxedType(T)()
if(is(T==wchar))
{
	return __TypeOf!(System.Char.Char);
}

Type __GetBoxedType(T)()
if(is(T==float))
{
	return  __TypeOf!(System.Single.Single);
}

Type __GetBoxedType(T)()
if(is(T==double))
{
	return  __TypeOf!(System.Double.Double);
}

Type __GetBoxedType(T)()
if(is(T==byte))
{
	return  __TypeOf!(System.SByte.SByte);
}

Type __GetBoxedType(T)()
if(is(T==ubyte))
{
	return  __TypeOf!(System.Byte.Byte);
}

Type __GetBoxedType(T)()
if(is(T==int))
{
	return __TypeOf!(System.Int32.Int32);
}

Type __GetBoxedType(T)()
if(is(T==uint))
{
	return __TypeOf!(System.UInt32.UInt32);
}

Type __GetBoxedType(T)()
if(is(T==long))
{
	return  __TypeOf!(System.Int64.Int64);
}

Type __GetBoxedType(T)()
if(is(T==ulong))
{
	return  __TypeOf!(System.UInt64.UInt64);
}

Type __GetBoxedType(T)()
if(is(T==struct) && !__traits(compiles,T.__IsEnum==true)) //Enums are now just structs with lots of magic
{
	auto boxedStruct= __TypeOf!(T.__Boxed_);
	return boxedStruct;
}

Type __GetBoxedType(T)()
if(is(T==struct) && __traits(compiles,T.__IsEnum==true)) //Enums are now just structs with lots of magic
{
	return __TypeOf!(BoxedEnum!(T));
}

Type __GetBoxedType(T)()
if(is(T==class))
{
	return __TypeOf!(T);
}

Type __GetBoxedType(T)()
	if( !is(T==class) && (!is(T==enum))&& (!is(T==struct))&& (!is(T==ulong))&& (!is(T==long)) && (!is(T==uint))&& (!is(T==int))&& (!is(T==ubyte)) && (!is(T==byte)) && (!is(T==double))&& (!is(T==wchar))
	&& !is(T==float) && !__traits(compiles,T.__IsEnum==true) )
{
	return __TypeOf!(T);
}


NObject __BOXPrimitive(T)(T value)
{
	static if(is(T==float))
	{
		return new System.Single.Single(value);
	}
	
	static if(is(T==double))
	{
		return new System.Double.Double(value);
	}
	
	
	static if(is(T==byte))
	{
		return new System.SByte.SByte(value);
	}
	
	static if(is(T==ubyte))
	{
		return new System.Byte.Byte(value);
	}
	
	static if(is(T==int))
	{
		return new System.Int32.Int32(value);
	}
	
	static if(is(T==uint))
	{
		return new System.UInt32.UInt32(value);
	}
	
	
	
	static if(is(T==long))
	{
		return new System.Int64.Int64(value);
	}
	
	static if(is(T==ulong))
	{
		return new System.UInt64.UInt64(value);
	}

	

//Enums are now just structs with lots of magic

	static if(is(T==struct) && __traits(compiles,T.__IsEnum==true))
	//	static if(is(T==enum))
		{
			return new BoxedEnum!(T)(value);
		}

	return null; // This is not a primitive
}

//}

//template UNBOX(T)
//{
//  static T UNBOX(Boxed!(T) boxed)
//{
//	return  boxed.Value;
//}

static T UNBOX(T,U)(U nobject) 
{
	static if(is(T==class)) // This should never happen /// how did you box a class and why ?
	{
		return  cast(T) nobject;
	}
	
	//Im probably going to need to genericise the Boxing operation and all assignments to sth of type object will have to do a cast
	//static if(is(U:Boxed!(T))) // Generics saved the day ... phew faster comparisons prevent casting issues ... not always applicable e.g. we have to use typeid or object
	{
		return (Cast!(Boxed!(T))(nobject)).__Value;//(cast(T) cast(void*) obj)

	}
	//Comparisons can work if we use typeid .... but that slows down things significantly ... so this could be an option ...
	//throw new InvalidCastException(new String("Cannot cast " ~ nobject.classinfo.name ~ " to " ~ T.stringof));
	//{

	//}
//import std.traits;

	//Console.WriteLine(TypeTuple!(T));
	//Console.WriteLine(TemplateArgsOf!(typeof(nobject)));
	//Console.WriteLine(Object.classinfo.getHash(cast(void*)typeid(nobject)));


//writeln(typeid(nobject));
//assert(is(TemplateArgsOf!(typeof(nobject)) == TypeTuple!(T)));
//guess we have to cheat and use traits

 //	if(typeof(nobject).classinfo == typeid(Boxed!(T)).classinfo ) //Typeid is slew exponential even
 	//if(is(TemplateArgsOf!(typeof(nobject)) == TypeTuple!(T)))
	//{
	//	return (cast(Boxed!(T))nobject).Value;
	//}

	//throw new InvalidCastException(new String("Cannot cast " ~ nobject.classinfo.name ~ " to " ~ T.stringof));


}
//}



	//bool IsCast(T, U)(U obj)  if (is(T == class) && is(U == class))
 //  {
 //      return (cast(T) cast(void*) obj) !is null;
 //  }

   bool IsCast(T, U)(U obj)  if (is(T == class))// && is(U :NObject))
   {


   		if(is(U:T))
   		{
   			return true;
   		}

   		return (cast(T)  obj) !is null;

    //   return cast(T) obj !is null;//(cast(T) cast(void*) obj) !is null;
   }


   //bool IsCast(T, U)(U obj)  if (is(T == class) && is(U == interface))
   //{
   //    return  is(T:U);//(cast(T) cast(void*) obj) !is null;
   //}

static int[int] ifaceMap; // too slow
struct IntIntMap
{
	int[] aArray;

	void Set(int key,int val)
	{
		aArray[key] = val;
	}

	int Get(int key)
	{
		return aArray[key];
	}
}

bool IsCast(T, U)(U obj)  if (is(T == interface))// && is(U :NObject))
   {
   	import std.traits;
   	if(obj !is null)
   	{
   		//import std.algorithm;
   		//enum int ifaceID = T.__id[0];

   		//int[] objIDS = obj.__get_id();

   		//int objId = objIDS[0];

   		//int* val;//ifaceID in ifaceMap;
   		////ifaceMap[ifaceID]=ifaceMap[ifaceID] & 1;
   		//if((val = ifaceID in ifaceMap) !is null)
   		////{
   		////if(val !is null)
   		//if(*val&objId)
   		//{
   		//	//Console.WriteLine("found in map");
   		//	return true;
   		//}
   //	}
   	//	else
   	//	{
   			//Console.WriteLine("not found in map");

   	//	}
   	

   		//static if(ifaceID==objIDS[0])
   		//return true;

   		//int objID = obj.__get_id()[0];

  // 		if(ifaceMap[ifaceID[objID]].containsKey(objID))
		//{
		//	return true;
   		//writeln("ifaceID = " ~ to!string(T.__id[0]));
   		//writeln(typeid(obj));

  // 		}
  // 		else
		//{
   		//auto found = false;
   		//foreach(i;objIDS)
   		//{
   		//	if(i==ifaceID)
   		//		found = true;

   		//	//writeln("objID = " ~ to!string(i));
   		//}
   		////if(!(ifaceID in ifaceMap))
   		//	//ifaceMap[ifaceID] =0;
   		//ifaceMap[ifaceID]|=objId;

  // 		if(found)
  // 			ifaceMap[ifaceID] ~= objID;

   		//return found;
  // 		}
  //Better algo
  		//return typeof(obj).__implements!(ifaceID);
   		//writeln("iscastiface");
   		if(is(U:T))
   		{
   			return true;
   		}
   	}

   		return (cast(T)  obj) !is null;
       //return cast(T) obj !is null; //is(U:T);//(cast(T) cast(void*) obj) !is null;
   }

//template IsCast(T)
//{
	//static bool IsCast(T,U)(U object)
	//{
		
	//	//TODO: add exception if not castable
	//	//static if(is(T == class) && !is(U == class))
	//	//{
	//	//	return false;
	//	//}

	//	//static if(!is(T == class) && is(U == class))
	//	//{
	//	//	return false;
	//	//}

	//	return cast(T)(object) !is null;
	//}
	
	static T Cast(T)(NObject object)
	{
		auto result = AsCast!(T)(object);
		static if(is(T==class))
		{
		if(result is null && object !is null)
		{
			throw new InvalidCastException(new String(("Cannot cast " ~ object.stringof ~ " to " ~ T.stringof)));
		}
		}
		return result;
	}

	static T Cast(T,U)(U object)
	if((!is(U==class) &&!is(U==interface))&&!(is(T==interface) && is(U==struct)))//is(U==enum) || is(U==struct))
	{
		return cast(T)object;
	}

	

	static T Cast(T,U)(U object)
	if(is(U==struct) && is(T==interface))
	{
	//	Console.WriteLine("Converting struct to interface");
		return cast(T)BOX!(U)(object); //UNBOX!(T)(cast(NObject)object);
	}

	static T Cast(T,U)(U object)
	if(!is(U:NObject) && (is(U==class) ||is(U==interface) ))// && !is(U==enum) && !is(U==struct))
	{
		
		auto result = AsCast!(T,U)(object);
		if(result is null)// && object !is null) this cannot work with everything
		{
			throw new InvalidCastException(new String(("Cannot cast " ~ object.stringof ~ " to " ~ T.stringof)));
		}
		return result;
	}

	static T AsCast(T)(NObject object)
	{
		static if(is(T==class) || is(T==interface))
		{
			return cast(T)(object);
		}
		else
			return UNBOX!(T)(object);
	}
	
static T AsCast(T,U)(U object)
if(is(U==interface) && is(T==struct))
{
return UNBOX!(T)(cast(NObject)object);
}


static T AsCast(T,U)(U object)
if(is(U==struct) && is(T==interface))
{
	Console.WriteLine("Converting struct to interface");
	return cast(T)BOX!(U)(object); //UNBOX!(T)(cast(NObject)object);
}


static T AsCast(T,U)(U object)
if(is(U==interface) && is(T==class))
{
	return cast(T)object;
}
	static T AsCast(T,U)(U object)
	if(!is(U:NObject)&&!is(U==interface))
	{
		static if(is(U:T))
		{
			return object;
		}
		
		/*static if((U is NObject) && is(T==class))
		{
			return object;
		}*/

		

		return cast(T)(object);
	}

	//static T AsCast(T)(NObject object)
	//{

	//	static if(is(T==class))
	//	{
	//		if(cast(T)(object) is null && object !is null)
	//		{
	//			throw new InvalidCastException(new String("Cannot cast " ~ object.classinfo.name ~ " to " ~ T.classinfo.name));
	//		}
	//		return cast(T)(object);
	//	}

		
	//	return (cast(Boxed!(T))object).Value;
		
	//}
//}


//static import System;
//import System.String;






alias void* Handle;

size_t offsetof(alias F)() {
	return F.offsetof;
}

struct Struct(T...) {
	
	T fields;
	
}

/**
 * $(MSDN system.idisposable, MSDN System.IDisposable)
 */
interface IDisposable {
	
	void Dispose(IDisposable j=null);

}

/**
 * Calls dispose on obj on block() completion.
 */
void __Using(IDisposable obj, void delegate() usingblock) {
	try {
		usingblock();
	}
	finally {
		if (obj !is null)
			obj.Dispose();
	}
}

// Used by cloneObject.
extern(C) private Object _d_newclass(ClassInfo info);

// Creates a shallow copy of an object.
Object cloneObject(Object obj) {
	if (obj is null)
		return null;
	
	ClassInfo ci = obj.classinfo;
	size_t start = Object.classinfo.init.length;
	size_t end = ci.init.length;
	
	Object clone = _d_newclass(ci);
	(cast(void*)clone)[start .. end] = (cast(void*)obj)[start .. end];
	return clone;
}




/**
 * The exception thrown when one of the arguments provided to a method is not valid.
 */
class ArgumentException : NException {
	
	private static const E_ARGUMENT = "Value does not fall within the expected range.";
	
	private String paramName_;
	
	this() {
		super(E_ARGUMENT);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
	this(String message, String paramName) {
		super(cast(string)message);
		paramName_ = paramName;
	}
	
	final String paramName() {
		return paramName_;
	}
	
}

/**
 * The exception thrown when a null reference is passed to a method that does not accept it as a valid argument.
 */
class ArgumentNullException : ArgumentException {
	
	private static const E_ARGUMENTNULL = "Value cannot be null.";
	
	this() {
		super(new String(E_ARGUMENTNULL));
	}


	this(String paramName) {
		super(new String(E_ARGUMENTNULL), paramName);
	}
	
	this(String paramName, String message) {
		super(message,  paramName);
	}
	
}

class IndexOutOfRangeException : NException {

private static const E_INDEOUTOFRANGE = "Array index is out of range.";
		public this() {
			super(E_INDEOUTOFRANGE);
			} 

		public this(String message) {
			super(cast(string)message);
			}

		//public IndexOutOfRangeException(String message, Exception innerException)
		//	: base(message, innerException) {
		//}

	}

/**
 * The exception that is thrown when the value of an argument passed to a method is outside the allowable range of values.
 */
class ArgumentOutOfRangeException : ArgumentException {
	
	private static const E_ARGUMENTOUTOFRANGE = "Index was out of range.";
	
	this() {
		super(new String(E_ARGUMENTOUTOFRANGE));
	}
	
	this(String paramName) {
		super(new String(E_ARGUMENTOUTOFRANGE), paramName);
	}
	
	this(String paramName, String message) {
		super(message, paramName);
	}
	
}

/**
 * The exception thrown when the format of an argument does not meet the parameter specifications of the invoked method.
 */
class FormatException : NException {
	
	private static const E_FORMAT = "The value was in an invalid format.";
	
	this() {
		super(E_FORMAT);
	}
	
	this(String message) {
		super(std.conv.to!(string)(message.Text));
	}
	
}

/**
 * The exception thrown for invalid casting.
 */
class InvalidCastException : NException {
	
	private static const E_INVALIDCAST = "Specified cast is not valid.";
	
	this() {
		super(E_INVALIDCAST);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

class ApplicationException : NException
{
}

/**
 * The exception thrown when a method call is invalid.
 */
class InvalidOperationException : NException {
	
	private static const E_INVALIDOPERATION = "Operation is not valid.";
	
	this() @safe {
		super(E_INVALIDOPERATION);
	}
	
	this(String message) @safe {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when a requested method or operation is not implemented.
 */
class NotImplementedException : NException {
	
	private static const E_NOTIMPLEMENTED = "The operation is not implemented.";
	
	this() {
		super(E_NOTIMPLEMENTED);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when an invoked method is not supported.
 */
class NotSupportedException : NException {
	
	private static const E_NOTSUPPORTED = "The specified method is not supported.";
	
	this() {
		super(E_NOTSUPPORTED);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when there is an attempt to dereference a null reference.
 */
class NullReferenceException : NException {
	
	private static const E_NULLREFERENCE = "Object reference not set to an instance of an object.";
	
	this() {
		super(E_NULLREFERENCE);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when the operating system denies access.
 */
class UnauthorizedAccessException : NException {
	
	private static const E_UNAUTHORIZEDACCESS = "Access is denied.";
	
	this() {
		super(E_UNAUTHORIZEDACCESS);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when a security error is detected.
 */
class SecurityException : NException {
	
	private static const E_SECURITY = "Security error.";
	
	this() {
		super(E_SECURITY);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown for errors in an arithmetic, casting or conversion operation.
 */
class ArithmeticException : NException {
	
	private static const E_ARITHMETIC = "Overflow or underflow in arithmetic operation.";
	
	this() {
		super(E_ARITHMETIC);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}

/**
 * The exception thrown when an arithmetic, casting or conversion operation results in an overflow.
 */
class OverflowException : ArithmeticException {
	
	private enum E_OVERFLOW = "Arithmetic operation resulted in an overflow.";
	
	this() {
		super(new String(E_OVERFLOW));
	}
	
	this(String message) {
		super(message);
	}
	
}

/* Use core.exception.OutOfMemoryError */
//deprecated
class OutOfMemoryException : NException {
	
	private enum E_OUTOFMEMORY = "Out of memory.";
	
	this() {
		super(E_OUTOFMEMORY);
	}
	
	this(String message) {
		super(cast(string)message);
	}
	
}


//Basic System.Type, to support for enums for now

import System.__Internal.Namespace;
import System.Reflection.Internal;
import System.Reflection.Namespace;


public class Type:NObject
{
	public String Name;
	public String FullName;
	public Type BaseType;
	public bool IsInterface;

	abstract Array_T!(Type)  FindInterfaces(TypeFilter filter,
	                                        NObject filterCriteria);

	abstract Array_T!(Type) GetInterfaces();

	public NObject Create()
	{
		return null;
	}

	TypeMetadata __Meta;
	public NObject __GetValue( String fieldname, NObject underlyingtype=null)
	{
		return null;
	}

	public Array_T!(String) GetMembers(string name="")
	{
		return null;
	}

	public Array_T!(String) GetMember(String name)
	{
		return null;
	}

	public MethodInfo GetMethod(String name)
	{
		return null;
	}

	public Array_T!(FieldInfo) GetFields(string name="")
	{
		return null;
	}

	public void __SetValue(string fieldname, NObject value)
	{

	}

	override bool Equals(NObject other)
	{
//		Console.WriteLine(this);
//
//		Console.WriteLine(other);



		auto otherType = cast(Type) other;



		if(otherType is null)
		{
			if(other !is null)
				otherType = other.GetType();
		}

		if(otherType is null)
			return false;

//		Console.WriteLine(this.IsInterface);
		
//		Console.WriteLine(otherType.IsInterface);


		auto isequal =  this.FullName.Hash == otherType.FullName.Hash;

		if(isequal)
		   return true;


		{
			auto interfaces =this.GetInterfaces();
			if(interfaces !is null && interfaces.Length > 0)
			{
				for(int i=0;i<interfaces.Length;i++)
				{
					if(interfaces[i].FullName.Hash == otherType.FullName.Hash)
						return true;
				}
			}
		}


		if(this.IsInterface)
		{
			auto interfaces =otherType.GetInterfaces();
			if(interfaces !is null && interfaces.Length > 0)
			{
				for(int i=0;i<interfaces.Length;i++)
				{
					if(interfaces[i].FullName.Hash == this.FullName.Hash)
						return true;
				}
			}
		}


		return false;
	}
}

struct __Void //Special Type for void
{

}

struct DBNull
{
	NObject obj;
	alias obj this;
}


static Type[TypeInfo] CachedTypes;// = ["":""];



public static Type_T!(T.__Boxed_) __TypeOf(T)(string csName=null)
if(is(T==struct) && !__traits(compiles, T.__IsEnum==true))
{

	auto info = typeid(T);

	if(info in CachedTypes)
	{

		return cast(Type_T!(T.__Boxed_)) CachedTypes[info];
	}

	auto type= new Type_T!(T.__Boxed_)(csName);

	CachedTypes[info] = type;

	return type;
}


public static Type_T!(T) __TypeOf(T)(string csName=null)
if(is(T==struct) && __traits(compiles, T.__IsEnum==true))
{

	auto info = typeid(T);

	if(info in CachedTypes)
	{

		return cast(Type_T!(T)) CachedTypes[info];
	}

	auto type= new Type_T!(T)(csName);

	CachedTypes[info] = type;

	return type;
}



public static Type_T!(T) __TypeOf(T)(string csName=null)
if(!is(T==struct))
{

	auto  info = typeid(T);

	if(info in CachedTypes)
	{
		return cast(Type_T!(T)) CachedTypes[info];
	}

	auto type= new Type_T!(T)(csName);
	
	CachedTypes[info] = type;
	
	return type;
}

public class BoxedEnum(T): Enum
{
	T Value;
	this(T value)
	{
		Value = value;
	}

	override string toString()
	{
		return Value.toString;
	}

	override String ToString()
	{
		return _S(Value.toString);
	}

	override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}

public class Enum : NObject
{
	public static Array_T!(String) GetNames(Type type)
	{
		//writeln("Calling get members");
		return  type.GetMembers();
	}

	public static long Parse(T)(Type_T!(T) type, String name)
	{
		enum allMembers = __traits(allMembers, T);

		if(type is null) 
			throw new ArgumentNullException(_S("type"));
		if(name is null)
			throw new ArgumentNullException(_S("name"));

		//auto temp = __TypeNew!(T);

		auto members =  type.GetMembers();
		auto mname = name.Text;

		foreach(member; members) 
		{
			if(member==name)
			{
				return type.GetMember!(long)(name);
				//return __traits(getMember,T,mname);
			}
		}


		/*for(int i = 0; i < members.Length; i++)
		{
			if(members[i]==name)
			{
				
				return __traits(getMember,temp,name.Text);
			}
		}*/

		throw new ArgumentException(_S(""));
	}


}

//PInvoke Support

public static void __FreeNativeLibrary(void * handle)
{
	version(darwin)
	{
		dlclose(handle);
	}
}

static void*[wstring] __dllMap;

public static void *__LoadNativeLibrary(wstring libName)
{

	void** handleIn = libName in __dllMap;

	if(handleIn !is null)
		return __dllMap[libName];

	version(darwin)
	{
		void* handle = dlopen(cast(char*)(std.conv.to!(char[])(libName)), RTLD_LAZY);
		if (!handle)
		{
			//throw new Exception("dlopen error: " ~ dlerror());
			printf("dlopen error: %s\n", dlerror());
			exit(1);
		}
	//	writeln("successfully loaded " ~ libName);
		__dllMap[libName] = handle;
		return handle;
	}
	version(Windows)
	{
		import core.runtime, core.sys.windows.windows;
		void* handle = Runtime.loadLibrary(std.conv.to!(char[])(libName));
		if (!handle)
		{
			//throw new Exception("dlopen error: " ~ dlerror());
			printf("failed to load library");
			exit(1);
		}
		//writeln("successfully loaded " ~ libName);
		__dllMap[libName] = handle;
		return handle;

	}
	return null;
}

static void*[string] __dllFuncMap;


public static void *__LoadLibraryFunc(void* library, wstring funcName)
{
	version(darwin)
	{
		char* error = dlerror();
		auto func= dlsym(library, std.conv.to!(char[])(funcName).toStringz);
		if (error)
		{
			printf("dlsym error: %s - %s\n", error, cast(char*)"glutInit");
			exit(1);
		}

		return func;
	}
	version(Windows)
	{
	import core.runtime, core.sys.windows.windows;
	//writeln(library);
	//writeln(GetProcAddress(library, "MessageBoxW".toStringz));
	return GetProcAddress(library, std.conv.to!(char[])(funcName).toStringz);//cast(const(char)*)std.conv.to!(char[])(funcName));
	}
	return null;
}

static  void* __DllImportMap[wstring];



static void __SetupDllImport(wstring name)
{
	__DllImportMap[name] = __LoadNativeLibrary(name);
	//writeln(__DllImportMap[name]);
}


static void __FreeDllImports()
{
	foreach(lib ; __DllImportMap)
	{
        	__FreeNativeLibrary(lib);
	}
}

static void __SetupSystem() // Called before entering "main"
{

}

static void __EndSystem() // Called after ending "main"
{
	__FreeDllImports();
}



