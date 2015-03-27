module System.Nullable__G;

public struct Nullable__G(T) //where T : struct
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

	int opCmp(Nullable__G!(T) other) {
		if (HasValue) {
			if (other.hasValue_)
				return typeid(T).compare(&value_, &other.value_);
			return 1;
		}
		if (other.HasValue)
			return -1;
		return 0;
	}

	int opEquals(Nullable__G!(T) other) {
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