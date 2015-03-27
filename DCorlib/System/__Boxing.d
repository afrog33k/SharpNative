module System.__Boxing;

import System.Namespace;

static Boxed!T BOX(T)(T value)
if(__isStruct!(T))
{
	return new T.__Boxed_(value);
}

static BoxedEnum!T BOX(T)(T value)
if(__isEnum!(T))
{
	return new BoxedEnum!(T)(value);
}

static T BOX(T)(NObject value)
if(__isClass!(T))
{
	return cast(T)value;
}


static Boxed!(T) BOX(T)(T value)
if(!__isClass!(T) && !__isStruct!(T) &&!__isEnum!(T))
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