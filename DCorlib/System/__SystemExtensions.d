module System.__SystemExtensions;

import System.Namespace;
import std.typetuple;
import std.traits;
//http://forum.dlang.org/thread/orcthzbphtpngsvzlblp@forum.dlang.org#post-mailman.608.1343110124.31962.digitalmars-d:40puremagic.com
//Get composition of generic type
//Philippe Sigaud
template Composition(T) //if (isComposite!T)
{
	static if (is(T comp == Name!(U), alias Name, U) || is (T 
			comp == Name!(U), alias Name, U...))
		alias TypeTuple!(Name, U) Composition;
	else static if (is(T comp == Name!(U,V), alias Name, U, V))
		alias TypeTuple!(Name, U,V) Composition;
	else static if (is(T comp == Name!(U,V,W), alias Name, U, V, 
			W))
		alias TypeTuple!(Name, U,V,W) Composition;
}

public static string __ConvertEnumToString(T)(T enumVal)
{
	auto __values = T.__values.dup;
	auto __names = T.__names.dup;
	long value = enumVal.__Value;
	bool firstTime = true;
	string retval = "";
	int index=cast(int)__values.length-1;

	 if(T.__HasFlags)
	{
		//std.stdio.writeln(value);
		for(;index>=0; index--)
	{
		if ((index == 0) && (__values[index] == 0))
			break;
		
		//std.stdio.writeln(value);

		if (value!=0 && (value & __values[index]) == __values[index])
		{
		//	std.stdio.writeln(value);
			//std.stdio.writeln(__values[index]);

			value -= __values[index];
			//std.stdio.writeln(value);

			
			if (!firstTime)
				retval =  ", " ~ retval;

			retval =  __names[index]  ~ retval;
			firstTime = false;
		}

	}
		return retval;
	}
	else
	{
	
		for(;index>=0; index--)
		{
			if (value  == __values[index])
			{
				return __names[index];
			}
		}

	}

	

	// We were unable to represent this number as a bitwise or of valid flags
	if (value != 0)
		return std.conv.to!string(BOX(enumVal.__Value).ToString().Text);
	

	// For the case when we have zero
	if (enumVal.__Value==0)
	{
		if (__values.length > 0 && __values[0] == 0)
			return __names[0]; // Zero was one of the enum values.
		else
			return "0";
	}
	else
		return retval;
	
}

public static T CompareTo(T)(T a, T b, IComparable__G!T __j=null)
if(__isScalar!(T))
{
		return a-b;
}

//Support for calling methods with const objects
__unConstType!(T) __UnConst(T)(T val) {
	return cast(__unConstType!(T)) val;
}


template __unConstType(U:const(T),T) 
{
	alias T __unConstType;
}


//TODO: Improve this to reuse strings
final static String  _S(wstring text)
{
	return new String(text);
}

final static String  _S(string text)
{
	return new String(text);
}

/*
public static String String(const(wstring) text)
{
	auto str =  String(text);
	return str;
}


public static String String(const(string) text) 
{
	auto str = String(text);
	return str;
}
public static String String(String text)
{
	return text;
}
*/

public static __IsNull(NObject value)
{
	return value is null;
}

public static __IsNull(T)(T value)
if(__isNullable!(T))
{
	return !value.HasValue;
}

public static __IsNull(T)(T value)
if(__isScalar!(T)&&!__isNullable!(T))
{
		return false;
}

public static __IsNull(T)(T value)
if(__isClass!(T)||__isInterface!(T))
{

	return value is null;
}


class __UNBOUND // object used to keep unbound generic types, for sharing and reflection
	//will switch to using simple NObject parameter and a flag to tell between bound and unbound generics
{
	NObject __Value;
	public this(NObject object)
	{
		__Value=object;
	}

	T opCast(T)() // __UNBOUND doesn't exist ;)
	{
		//return cast(T) cast(void*) __Value;
		return T.init;
	}

	//alias __Value this;

	public int GetHashCode()
	{
		return 0;
	}
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




public static double MinValue(double value)
{
	return double.min_normal;
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
	static if(__traits(compiles,T(args)))
	{
		return T(args);
	}
	return __Default!(T)();
}

T __TypeNew(T,U...)(U args)
if(!is(T==struct)&&!is(T==class)&&!is(T==void))
{
	return __Default!(T)();
}

__Void __TypeNew(T,U...)(U args)
if(is(T==void))
{
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

	this(typeof(T.funcptr) *func) // Allows direct storage of functions too
	{
		dFunc = __ToDelegate(*func);
		funcs = null;
	}

	this(T *func)
	{
		dFunc = *func;
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

	this(__Delegate!(T) *other)
	{
		dFunc = (other).dFunc;
		funcs = (other).funcs;
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

        for (int j=0;j< funcs.length; j++)
        {
			auto h = funcs[j];

            if (h.Function is func.Function)
            {
                i = j;
                break;
            }
        }

        if (i != -1)
            funcs = funcs[0..cast(uint)i] ~ funcs[cast(uint)i+1..$];

		if(dFunc !is null && dFunc==func.Function) // Function could be the primary one we added during init ;)
		{
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
    T[] handlers;
    NObject owner;
    Action__G!(T) attacher = null;
    Action__G!(T) detacher = null;

	this()
	{

	}

    this(Action__G!(T) _attacher, Action__G!(T) _detacher) {
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
    { 
		// call all handlers
        foreach (handler; handlers) //TODO fix this
        {
            if(handler)
            	handler(u);
        }

    }

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

public static bool Equals(T,U)(T a, U b)
if(__isScalar!(T) && __isScalar!(U))
{
	//std.stdio.writeln("--- Basic Equals");

	return a == b;
}

public static bool Equals(T)(T a, NObject b)
if(__isScalar!(T))
{
	//std.stdio.writeln("Basic Equals");

	return a == UNBOX!(T)(b);
}

/*public static bool Equals(T)(T object, NObject other)
{
	std.stdio.writeln("Basic Equals");
	return object == UNBOX!(T)(other);
}*/

//boxed.Value.opEquals(this.Value)
//Allows equals comparison between most basic types
/*public static bool Equals(T)(T object, T other)
{

	return object == other;
}*/


public static string toString(T)(T value) if(!is(T==struct) && !is(T==double) && !is(T==float) && !is(T==int) && !is(T==bool)) 
{
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

bool IsCast(T)(NObject obj)  
if (__isCSStruct!(T))
{
	if(is(U:T))
	{
		return true;
	}
	return (cast(Boxed!(T))(obj)) !is null;
}


bool IsCast(T, U)(U obj)  if (__isClass!(T))
{
	if(is(U:T))
	{
		return true;
	}
	return (cast(T)  obj) !is null;
}

bool IsCast(T, U)(U obj)  if (__isInterface!(T))
{
   	import std.traits;
   	if(obj !is null)
   	{
   		if(is(U:T))
   		{
   			return true;
   		}
   	}
	return (cast(T)  obj) !is null;
}

static T Cast(T)(NObject object)
{
	auto result = AsCast!(T)(object);
	static if(is(T==class) || is(T==interface))
	{
		if(result is null && object !is null)
		{
			throw new InvalidCastException(new String(("Cannot cast " ~ object.stringof ~ " to " ~ T.stringof)));
		}
	}
	return result;
}

static T Cast(T,U)(U object)
if(__isCSStruct!U && __isCSStruct!T)
{
	return cast(T)object;
}

static T Cast(T,U)(U object)
	if(__isCSStruct!U && __isClass!T)
{
	return cast(T) BOX!U(object);
}

//static T Cast(T,U)(U object)
//if(is(T==class) && is(U==struct))
//{
//	return cast(T)object;
//}

//static T Cast(T,U)(U object)
//	if((!is(U==class) &&!is(U==interface))&&(!is(T==interface) && is(U==struct)))
//{
//	return cast(T)object;
//}

static T Cast(T,U)(U object)
if(is(U==struct) && is(T==interface))
{
	return cast(T)BOX!(U)(object);
}

static T Cast(T,U)(U object)
if(!is(U:NObject) && (is(U==class) ||is(U==interface)))
{

	auto result = AsCast!(T,U)(object);
	if(result is null)
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
if(is(U==interface) && is(T==interface))
{
	return cast(T)(cast(NObject)object);
}

static T AsCast(T,U)(U object)
if(is(U==struct) && is(T==interface))
{
	return cast(T)BOX!(U)(object);
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
	return cast(T)(object);
}

import System.Reflection.Namespace;


import System.__Internal.hashmap;

static HashMap!(int,Type) CachedTypes;

public static Type __GetCachedType(TypeInfo __info)
{
	if(__info.toString.Hash() in CachedTypes)
	{
		return CachedTypes[__info.toString.Hash()];
	}
	return null;
}

public static auto __TypeOf(T)(string csName=null)
	if(is(T==struct) && __isNullable!(T))
{
	auto info = typeid(T);
	
	//Console.WriteLine(info.toString.Hash());
	int fullNameHash = fullyQualifiedName!(T).Hash();
	
	Type_T!(Boxed!(Composition!(T)[1])) type= null;
	
	bool found =false;
	for(int i =0; i<hashes.length; i++)
	{
		if(hashes[i]==fullNameHash)
		{
			//return cast(Type_T!(T)) Infos[i];
			type = cast(Type_T!(Boxed!(Composition!(T)[1])))Infos[i];
			found = true;
			break;
		}
	}
	
	if(!found)
	{
		type= new Type_T!(Boxed!(Composition!(T)[1]))(csName);
		
		hashes.length++;
		Infos.length++;
		
		hashes ~= fullNameHash;
		Infos ~= type;
		//CachedTypes[fullName.Hash()]= type;
		
		//Console.WriteLine(_S("Added " ~  type.Name.toString));
	}
	//	
	if(csName !is null)
	{
		type.FullName = new String(csName);
		if(csName.lastIndexOf(".")!=-1)
		{
			type.Name = new String(csName[csName.lastIndexOf(".")..$]);
		}
	}
	
	
	
	return cast(Type_T!(Boxed!(Composition!(T)[1])))type;
}

public static Type_T!(__BoxesTo!(T)) __TypeOf(T)(string csName=null)
if(is(T==struct) && !__traits(compiles, T.__IsEnum==true) &&!__isNullable!(T))
{
	auto info = typeid(T);
	
	//Console.WriteLine(info.toString.Hash());
	int fullNameHash = fullyQualifiedName!(T).Hash();
	
	Type_T!(__BoxesTo!(T)) type= null;
	
	bool found =false;
	for(int i =0; i<hashes.length; i++)
	{
		if(hashes[i]==fullNameHash)
		{
			//return cast(Type_T!(T)) Infos[i];
			type = cast(Type_T!(__BoxesTo!(T)))Infos[i];
			found = true;
			break;
		}
	}
	
	if(!found)
	{
		type= new Type_T!(__BoxesTo!(T))(csName);
		
		hashes.length++;
		Infos.length++;
		
		hashes ~= fullNameHash;
		Infos ~= type;
		//CachedTypes[fullName.Hash()]= type;
		
		//Console.WriteLine(_S("Added " ~  type.Name.toString));
	}
	//	
	if(csName !is null)
	{
		type.FullName = new String(csName);
		if(csName.lastIndexOf(".")!=-1)
		{
			type.Name = new String(csName[csName.lastIndexOf(".")..$]);
		}
	}
	
	
	
	return cast(Type_T!(__BoxesTo!(T)))type;
}


//Switched to this to allow gtest-53 to pass ... dont know whats happening with the dictionaries
// But this is now O(n) :(
static Type[] Infos;
static int[] hashes;

public static Type_T!(T) __TypeOf(T)(string csName=null)
	if(!is(T==struct) ||(is(T==struct) && __traits(compiles, T.__IsEnum==true)))
{
	auto info = typeid(T);

	//Console.WriteLine(info.toString.Hash());
	int fullNameHash = fullyQualifiedName!(T).Hash();

	Type_T!(T) type= null;

	bool found =false;
	for(int i =0; i<hashes.length; i++)
	{
		if(hashes[i]==fullNameHash)
		{
			//return cast(Type_T!(T)) Infos[i];
			type = cast(Type_T!(T))Infos[i];
			found = true;
			break;
		}
	}

	if(!found)
	{
		 type= new Type_T!(T)(csName);

		hashes.length++;
		Infos.length++;
		
		hashes ~= fullNameHash;
		Infos ~= type;
		//CachedTypes[fullName.Hash()]= type;
		
		//Console.WriteLine(_S("Added " ~  type.Name.toString));
	}
		//	
			if(csName !is null)
			{
				type.FullName = new String(csName);
				if(csName.lastIndexOf(".")!=-1)
				{
					type.Name = new String(csName[csName.lastIndexOf(".")..$]);
				}
			}



		return cast(Type_T!(T))type;
	}

//	if(fullName.Hash() in CachedTypes)
//	{
//		Console.WriteLine(_S(fullName));
//		Console.WriteLine(fullName.Hash());
//		auto type = CachedTypes.get(fullName.Hash(),null);
//
//		//Console.WriteLine(_S("Exists"));
//		//Console.WriteLine(type);
//		
//		//return cast(Type_T!(T)) type;
//	}
//	
////	auto type= new Type_T!(T)(csName);
////	
////	if(csName !is null)
////	{
////		type.FullName = new String(csName);
////		if(csName.lastIndexOf(".")!=-1)
////		{
////			type.Name = new String(csName[csName.lastIndexOf(".")..$]);
////		}
////	}
//	
//	CachedTypes[fullName.Hash()]= type;
//
//	//Console.WriteLine(_S("Added " ~  type.Name.toString));
//
//	return cast(Type_T!(T))type;
//	auto  info = typeid(T);
//	//std.stdio.writeln("Getting type of " ~ info.toString ~ " = " ~ csName);
//
//	Type_T!(T) type = new Type_T!(T)(csName);
//	return type;
//	/*if(csName !is null)
//	{
//		type.FullName = new String(csName);
//			if(csName.lastIndexOf(".")!=-1)
//			{
//				type.Name = new String(csName[csName.lastIndexOf(".")..$]);
//			}
//	}*/
//	//Looks like D's builtin hashmap is either crashing or has some kind of collision .. causes gtest-053 to hang at runtime
//	/*if(info.toString in CachedTypes)
//	{
//		type = cast(Type_T!(T))CachedTypes[info.toString];
//	//	std.stdio.writeln("Stage 2... in dic");
//		if(csName !is null)
//		{
//			type.FullName = new String(csName);
//			if(csName.lastIndexOf(".")!=-1)
//			{
//				type.Name = new String(csName[csName.lastIndexOf(".")..$]);
//			}
//		}
//	}
//	else
//	{
//	//std.stdio.writeln("Stage 3... not in dic");
//	//	try {
//	type= new Type_T!(T)(csName);
//			//Console.WriteLine(type);
//	//	}
//	//	catch(Exception e)
//	//	{
//	//		throw e;
//	//	}
//
//		CachedTypes[info.toString] = type;
//	}
//
//	//std.stdio.writeln("Returning type ... ");
//
//	//Console.WriteLine(type);
//
//	return type;*/
//}