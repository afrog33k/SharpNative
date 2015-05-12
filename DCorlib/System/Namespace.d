module System.Namespace;
public import std.stdio;
import System.Boolean;
import System.String;
public import std.conv;
public import std.traits;
public import std.typecons;
public import std.string;


public import System.__Constraints;
public import System.__Boxing;
public import System.__NativeMethods;
public import System.__PrimitiveExtensions;
public import System.__SystemExtensions;
public import System.__PInvokeSupport;
public import System.__YieldAsyncSupport;


alias std.typecons.scoped StructAlloc;
alias System.NObject.NObject NObject;
alias System.ICloneable.ICloneable ICloneable;
alias System.Boolean.Boolean Boolean;
alias System.ArgumentException.ArgumentException ArgumentException;
alias System.Func__G.Func__G Func__G;
alias System.Comparison__G.Comparison__G Comparison__G;
alias System.Predicate__G.Predicate__G Predicate__G;
alias System.Environment.Environment Environment;

alias System.Byte.Byte Byte;
alias System.SByte.SByte SByte;
alias System.Enum.Enum Enum;

alias System.Int16.Int16 Int16;
alias System.UInt16.UInt16 UInt16;

alias System.Int32.Int32 Int32;
alias System.UInt32.UInt32 UInt32;

alias System.Int64.Int64 Int64;
alias System.UInt64.UInt64 UInt64;
alias System.EventHandler.EventHandler EventHandler;
alias System.EventHandler__G.EventHandler__G EventHandler__G;
alias System.Func__G.Func__G Func__G;
alias System.Action__G.Action__G Action__G;
alias System.Action.Action Action;



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

alias void* Handle;

size_t offsetof(alias F)() 
{
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

static void __SetupSystem() // Called before entering "main"
{

}

static void __EndSystem() // Called after ending "main"
{
	__FreeDllImports();
}


