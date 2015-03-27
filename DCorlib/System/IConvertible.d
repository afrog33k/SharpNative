module System.IConvertible;
import System.Namespace;

static interface IConvertible
{
	// Returns the type code of this object. An implementation of this method
	// must not return TypeCode.Empty (which represents a null reference) or
	// TypeCode.Object (which represents an object that doesn't implement the
	// IConvertible interface). An implementation of this method should return
	// TypeCode.DBNull if the value of this object is a database null. For
	// example, a nullable integer type should return TypeCode.DBNull if the
	// value of the object is the database null. Otherwise, an implementation
	// of this method should return the TypeCode that best describes the
	// internal representation of the object.

	public System.Namespace.TypeCode GetTypeCode(IConvertible __j = null);
	// The ToXXX methods convert the value of the underlying object to the
	// given type. If a particular conversion is not supported, the
	// implementation must throw an InvalidCastException. If the value of the
	// underlying object is not within the range of the target type, the
	// implementation must throw an OverflowException.  The 
	// IFormatProvider will be used to get a NumberFormatInfo or similar
	// appropriate service object, and may safely be null.

	public bool ToBoolean(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public wchar ToChar(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public byte ToSByte(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public ubyte ToByte(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public short ToInt16(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public ushort ToUInt16(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public int ToInt32(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public uint ToUInt32(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public long ToInt64(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public ulong ToUInt64(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public float ToSingle(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public double ToDouble(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public System.Namespace.Decimal ToDecimal(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public System.Namespace.DateTime ToDateTime(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public String ToString(System.Namespace.IFormatProvider provider, IConvertible __j = null);

	public NObject ToType(System.Namespace.Type conversionType, System.Namespace.IFormatProvider provider, IConvertible __j = null);
}