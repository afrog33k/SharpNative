module System.Nullable__G;
import System.Namespace;


public struct Nullable__G(T) //: NObject //where T : struct
{
	private bool hasValue_ =false; 
	public T value_=T.init; 

	alias value_ this;

	public static const bool __nullable = true;


	/*static Nullable opCall(T value) {
	Optional self;
	self.value_ = value;
	self.hasValue_ = true;
	return self;
	}*/

	/*this()
	{

	}*/

	void __init(){}//default xtor
	static Nullable__G!(T) opCall(__U...)(__U args_)
	{
		Nullable__G!(T) s;
		s.__init(args_);
		return s;
	}
	
	public String ToString()
	{
		return GetType().FullName;
	}
	
	public static class __Boxed_ : Boxed!(T)
	{
		import std.traits;
		
		this()
		{
			super(Nullable__G!(T).init);
		}
		public override String ToString()
		{
			return __Value.ToString();
		}
		
		public override bool Equals(NObject other)
		{
			if (cast(Boxed!(T)) other)
			{
				auto otherValue = (cast(Boxed!(T)) other).__Value;
				return otherValue == __Value;
			}
			return false;
		}
		
		this(ref Nullable__G!(T) value)
		{
			super(value);
		}

		U opCast(U)()
			if(is(U:T))
		{
			return __Value;
		}
		
		U opCast(U)()
			if(!is(U:T))
		{
			return this;
		}
		
		auto opDispatch(string op, Args...)(Args args)
		{
			enum name = op;
			return __traits(getMember, __Value, name)(args);
		}
		
		public override Type GetType()
		{
			return __Value.GetType();
		}
	}
	
/*	public __Boxed_ __Get_Boxed()
	{
		return new __Boxed_(this);
	}*/
	//alias __Get_Boxed this;
	
	public bool opEquals(T other)
	{
		return other==value_;
	}
	
	
	public Type GetType()
	{
		return __TypeOf!(typeof(this));
	}



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
			throw new NException(new String("Nullable object must have a value."));
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
		return HasValue ? cast(int) value_.GetHashCode() : 0;
	}

	public   String ToString() 
	{
		return HasValue ? new String(value_.toString) : String.Empty;
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