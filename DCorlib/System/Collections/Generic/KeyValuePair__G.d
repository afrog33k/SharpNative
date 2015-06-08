module System.Collections.Generic.KeyValuePair__G;


import System.Namespace;
import System.Text.Namespace;
import System.Collections.Generic.Namespace;

struct KeyValuePair__G(TKey , TValue)
{
    private TKey key = TKey.init;
    private TValue value = TValue.init;

    public TKey Key() 
    {
		{
			return key;
        }

    }


    public TValue Value() 
    {
		{
			return value;
        }

    }


    public String ToString()
    {
		StringBuilder s =  new StringBuilder();
		s.Append('[');
		if(this.Key !is null)
		{
			s.Append(BOX!(TKey)(Key));
		}
		s.Append(new String(", "));
		if(this.Value !is null)
		{
			s.Append(BOX!(TValue)(Value));
		}
		s.Append(']');
		return s.ToString();
    }

	public void __init(){}

    public  void __init(TKey key, TValue value)
    {
		this.key=key;
		this.value=value;
    }
    static KeyValuePair__G opCall(U...)(U args_)
    {
		KeyValuePair__G s;
		s.__init(args_);
		return s;
    }

	public static class __Boxed_ : Boxed!(KeyValuePair__G!(TKey , TValue))
	{
		import std.traits;

		this()
		{
			super(__TypeNew!(KeyValuePair__G!(TKey , TValue))());
		}
		override String ToString()
		{
			return __Value.ToString();
		}

		this(ref KeyValuePair__G!(TKey , TValue) value)
		{
			super(value);
		}

		U opCast(U)()
			if(is(U:KeyValuePair__G!(TKey , TValue)))
			{
				return __Value;
			}

		U opCast(U)()
			if(!is(U:KeyValuePair__G!(TKey , TValue)))
			{
				return this;
			}

		auto opDispatch(string op, Args...)(Args args)
		{
			enum name = op;
			return __traits(getMember, __Value, name)(args);
		}
	}
	public __Boxed_ __Get_Boxed()
	{
		return new __Boxed_(this);
	}
	alias __Get_Boxed this;
	public Type GetType()
	{
		return __TypeOf!(__Boxed_);
	}
}
