module System.Namespace;
import std.stdio;
import System.Boolean;
import System.String;
import std.conv;
import std.traits;
import std.typecons;

public import System.__Constraints;
public import System.__Boxing;
public import System.__NativeMethods;
public import System.__PrimitiveExtensions;


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
alias System.IntPtr.IntPtr IntPtr;
alias System.Nullable__G.Nullable__G Nullable__G;
alias System.Delegate.Delegate Delegate;
alias System.EventArgs.EventArgs EventArgs;
alias System.NException.NException NException;
alias System.TimeSpan.TimeSpan TimeSpan;
alias System.MulticastDelegate.MulticastDelegate MulticastDelegate;

alias System.Random.Random Random;
alias System.TypeCode.TypeCode TypeCode;
alias System.ValueType.ValueType ValueType;

alias System.String.String String;
alias System.Array.Array Array;
alias System.Array_T.Array_T Array_T;
alias System.Console.Console Console;
alias System.Math.Math Math;
alias System.GC.GC GC;
alias System.__YieldIterator__G.__YieldIterator__G __YieldIterator__G;
alias System.IFormatProvider.IFormatProvider IFormatProvider;
alias System.IComparable__G.IComparable__G IComparable__G;
alias System.IComparable.IComparable IComparable;
alias System.Locale.Locale Locale;
alias System.Attribute.Attribute Attribute;
alias System.PlatformID.PlatformID PlatformID;
alias System.Version.Version Version;
alias System.IDisposable.IDisposable IDisposable;

alias System.EmptyArray_T.EmptyArray_T EmptyArray_T;

alias System.Activator.Activator Activator;
alias System.Type.Type Type;
alias System.IConvertible.IConvertible IConvertible;
alias System.IFormattable.IFormattable IFormattable;
alias System.DateTime.DateTime DateTime;
alias System.DateTimeKind.DateTimeKind DateTimeKind;
alias System.SystemException.SystemException SystemException;

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


struct __UNBOUND // object used to keep unbound generic types, for sharing and reflection
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

	alias __Value this;
}

template __Tuple(E...)
{
	alias __Tuple = E;
}

static void __dummyFunc()
{

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


class __Delegate(T): Delegate
{
	T dFunc;
    __Delegate!T[] funcs;


	this()
	{
		dFunc = null;
		funcs = null;
	}

	this(NObject object, IntPtr func)
	{
		dFunc = __ToDelegate(cast(typeof(T.funcptr)) func.m_value);
		dFunc.ptr = cast(void*) object;
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




struct __Void //Special Type for void
{

}

struct DBNull
{
	NObject obj;
	alias obj this;
}


static Type[TypeInfo] CachedTypes;// = ["":""];


import System.Reflection.Namespace;
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



public class Enum : NObject
{
	public static Array_T!(String) GetNames(Type type)
	{
		auto members = type.GetMembers();
		String[] names;

		for(int c=0;c<members.Length;c++)
		{
			names ~= members[c].Name;
		}
		return  new Array_T!(String)(__CC(names));
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


import core.thread;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
class __IteratorBlock(TSource) : Fiber,  
	IEnumerable__G!(TSource),
	IEnumerator__G!(TSource) 
{
	bool started = false;
	void delegate(__IteratorBlock!TSource) func;

	this(void function(__IteratorBlock!TSource) func)
	{
		//_threadId = Thread.getThis();
		aborted = false;
		this.func = __ToDelegate(func);
		super(&run);
	}

	this(void delegate(__IteratorBlock!TSource) func)
	{
		//_threadId = Thread.getThis();
		aborted = false;
		this.func = func;
		super(&run);
	}


	private void run()
	{
		//Console.WriteLine("starting ...");
		try {
			func(this);
		}catch(Exception ex)
		{
		}
	}
	private void ensureStarted()
	{
		if(!started)
		{

			call();
			started = true;
		}
	}
	// Member 'front' must be a function due to DMD Issue #5403
	private TSource _front;
	@property TSource front()
	{

		ensureStarted();
		return _front;
	}
	void popFront()
	{
		if(aborted)
			return;

		ensureStarted();
		if (state == Fiber.State.HOLD)
			call();
	}
	@property bool empty()
	{
		if(aborted)
			return true;

		ensureStarted();
		return state == Fiber.State.TERM;
	}


	__IteratorBlock!(TSource) clone() 
	{
		return new __IteratorBlock!(TSource)(func);
	}


	public IEnumerator__G!(TSource) GetEnumerator(IEnumerable__G!(TSource) k = null)
	{

		/*	if (Thread.getThis() == _threadId && ! _enumeratorCreated) 
		{
		_enumeratorCreated = true;
		return  cast(IEnumerator__G!(TSource))(this);
		}*/

		__IteratorBlock!(TSource) cloned = clone();
		return cloned;
		//		return this;
	}

	public IEnumerator GetEnumerator(IEnumerable j =null)
	{
		return cast(IEnumerator)GetEnumerator(cast(IEnumerable__G!(TSource))null);
	}



	public NObject  Current(IEnumerator j=null) @property
	{
		return BOX(Current(cast(IEnumerator__G!(TSource))null));
	}

	// IEnumerator
	public void Reset(IEnumerator j=null) {
		throw  new InvalidOperationException();
  	}

	public bool MoveNext(IEnumerator j=null)
	{
		//Console.WriteLine("MoveNext");
		if(!started)
		{
			ensureStarted();
			//Console.WriteLine("ensureStarted");
			static if(is(T==class) || is(T==interface))
			{
				if(front!=null)
					return true;
			}
			else
			{
				return true;
			}
		}
		else
		{
			//Console.WriteLine("aborted||state == Fiber.State.TERM");
			if(aborted||state == Fiber.State.TERM)
				return false;

			popFront();
			if(!empty)

				return true;
			else
				return false;
		}

		return false;
	}
	//	
	public TSource  Current(IEnumerator__G!(TSource) k = null)   @property
	{
		return front();
	}

	bool aborted = false;
	void yield(TSource elem)
	{
		_front = elem;
		Fiber.yield();
	}

	void yieldReturn(TSource returnValue) 
	{
		//	Console.Write("yielding ");
		//	Console.WriteLine(returnValue);
		_front = returnValue;
		try
		{
			Fiber.yield();
		}catch(Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	void yieldBreak() 
	{
		aborted = true;
		Fiber.yield();
	}

	void Dispose(IDisposable j = null)
	{

	}
}