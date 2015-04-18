module System.__Constraints;

import System.Namespace;

template __isArray(T)
{
	enum __isArray =  std.traits.isArray!(T);
}

template __isInteger(T)
{
	enum __isInteger =  (is(T==int) || is(T==double) || is(T==float) || is(T==uint) || is(T==long) || is(T==ulong) || is(T==byte) || is(T==ubyte));
}

template __isCharacter(T)
{
	enum __isCharacter =  (is(T==char) || is(T==wchar) || is(T==dchar));
}

template __isScalar(T)
{
	enum __isScalar = __isInteger!(T) || __isCharacter!(T) || is(T==bool);
}

template __isEnum(T)
{
	enum __isEnum = is(T==struct) && __traits(compiles, T.__IsEnum==true);
}

template __isStruct(T)
{
	enum __isStruct =  is(T==struct) && !__isEnum!(T);
}


template __isCSStruct(T)
{
	enum __isCSStruct =  __isStruct!(T) || __isScalar!(T);
}

template __isClass(T)
{
	enum __isClass =  is(T==class) && is(T:NObject);
}

template __isNewwable(T)
{
	enum __isNewwable =  (__isClass!(T)  && __traits(compiles, new T())) || (__isStruct!(T) && __traits(compiles, T())) ;
}

template __isInterface(T)
{
	enum __isInterface =  is(T==interface);
}

template __isPointer(T:T*) {
	enum __isPointer = true;
}
template __isPointer(T) {
	enum __isPointer = false;
}
