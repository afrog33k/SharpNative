module System.__Boxing;

import System.Namespace;
import std.conv;

template __BoxesTo(T)
if(__isPointer!(T) && is(T==int*))
{
	alias __BoxesTo = IntPtr;
}

template __BoxesTo(T)
if(__isInterface!(T))
{
	alias __BoxesTo = T;
}

template __BoxesTo(T)
if(__isClass!(T))
{
	alias __BoxesTo = T;
}

template __BoxesTo(T)
if(__isStruct!(T))
{
	alias __BoxesTo = T.__Boxed_;
}

template __BoxesTo(T)
if(__isScalar!(T))
{
	//alias __BoxesTo = Boxed!T;
	static if(is(T==float))
	{
		alias __BoxesTo = System.Single.Single;
	}

	static if(is(T==wchar))
	{
		alias __BoxesTo = System.Char.Char;
	}

	static if(is(T==bool))
	{
		alias __BoxesTo = System.Boolean.Boolean;
	}

	static if(is(T==double))
	{
		alias __BoxesTo = System.Double.Double;
	}
//ubyte, ushort, uint, ulong, wchar

	static if(is(T==short))
	{
		alias __BoxesTo = System.Int16.Int16;
	}

	static if(is(T==ushort))
	{
		alias __BoxesTo = System.UInt16.UInt16;
	}

	static if(is(T==byte))
	{
		alias __BoxesTo = System.SByte.SByte;
	}

	static if(is(T==ubyte))
	{
		alias __BoxesTo = System.Byte.Byte;
	}

	static if(is(T==int))
	{
		alias __BoxesTo = System.Int32.Int32;
	}

	static if(is(T==uint))
	{
		alias __BoxesTo = System.UInt32.UInt32;
	}

	static if(is(T==long))
	{
		alias __BoxesTo = System.Int64.Int64;
	}

	static if(is(T==ulong))
	{
		alias __BoxesTo = System.UInt64.UInt64;
	}

	static if(__isEnum!(T))
	{
		alias __BoxesTo = BoxedEnum!(T);
	}
}

template __BoxesTo(T)
if(__isEnum!(T))
{
	alias __BoxesTo = BoxedEnum!T;
}

static Boxed!T BOX(T)(T value)
if(__isStruct!(T) && !__isNullable!(T))
{
	return new T.__Boxed_(value);
}

static Boxed!(Composition!(T)[1]) BOX(T)(T value)
if(__isStruct!(T) && __isNullable!(T))
{
	if(value.HasValue)
		return BOX(value.value_);//new T.__Boxed_(value);
	return null;
}

static BoxedEnum!T BOX(T)(T value)
if(__isEnum!(T))
{
	return new BoxedEnum!(T)(value);
}

static  T BOX(T)( NObject value)
if(__isClass!(T))
{
	return cast(T)value;
}

static  T BOX(T)( T value)
if(__isClass!(T) && !is(T==NObject))
{
	return value;
}

static  NObject BOX(T)( T value)
if(__isInterface!(T))
{
	return cast(NObject)value;
}


static Boxed!(T) BOX(T)(T value)
if(__isScalar!(T)) //&& !__isStruct!(T) &&!__isEnum!(T))
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


static T UNBOX(T,U)(U nobject) 
if(__isClass!(T)) // This should never happen how did you box a class and why ?
{
		return  cast(T) nobject;
}

static T UNBOX(T,U)(U object)
if(__isScalar!(T) && is(U==T))
{
	return cast(T)object;
}
static T UNBOX(T,U)(U object)
if(__isScalar!(T) && is(U==const(T)))
{
	return cast(T)object;
}

static T UNBOX(T,U)(U nobject) 
if(!__isClass!(T) && __isClass!(U))
{
	return (Cast!(Boxed!(T))(nobject)).__Value;
}

static T UNBOX(T,U)(U nobject) 
if(__isStruct!(T) && __isInterface!(U))
{
	return cast(T)cast(Boxed!T)nobject;
}

//Reflection Support for fields and properties, fixes up structs 
static T* UNBOX_R(T,U)(ref U nobject) 
if(is(T==class)) // This should never happen how did you box a class and why ?
{
		return  cast(T*)&(nobject);
}
	

static T* UNBOX_R(T,U)(U nobject) 
if(!is(T==class))
{
	/*static if(is(T==class)) // This should never happen /// how did you box a class and why ?
	{
		return  &(cast(T) nobject);
	}*/
	return &((Cast!(Boxed!(T))(nobject)).__Value);
}


public class BoxedEnum(T): Enum
{
	T __Value;
	this(T value)
	{
		__Value = value;
	}

	override string toString()
	{
		return __Value.toString;
	}

	override String ToString()
	{
		return new String(__Value.toString);
	}

	override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

	T opCast()
	{
		return __Value;
	}
}

class Boxed (T) : NObject
{


    this(T value = T.init)
    {
        this.__Value = value;
    }

	alias __Value this;

public:
    T __Value;

    public override string toString()
	{
		static if(is(T==bool))
		{
			return __Value?"True":"False";
		}

		return to!string(__Value);
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
