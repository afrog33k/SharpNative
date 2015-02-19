module System.Collections.Generic.KeyValuePair_TKey_TValue;


import System.Namespace;
import System.Text.Namespace;
import System.Collections.Generic.Namespace;

struct KeyValuePair_TKey_TValue(TKey , TValue)
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
		s.Append(_S(", "));
		if(this.Value !is null)
		{
			s.Append(BOX!(TValue)(Value));
		}
		s.Append(']');
		return s.ToString();
    }

    public  void __init(TKey key, TValue value)
    {
		this.key=key;
		this.value=value;
    }
    static KeyValuePair_TKey_TValue opCall(U...)(U args_)
    {
		KeyValuePair_TKey_TValue s;
		s.__init(args_);
		return s;
    }

	public static class __Boxed_ : Boxed!(KeyValuePair_TKey_TValue!(TKey , TValue))
	{
		import std.traits;

		this()
		{
			super(__TypeNew!(KeyValuePair_TKey_TValue!(TKey , TValue))());
		}
		override String ToString()
		{
			return __Value.ToString();
		}

		this(ref KeyValuePair_TKey_TValue!(TKey , TValue) value)
		{
			super(value);
		}

		U opCast(U)()
			if(is(U:KeyValuePair_TKey_TValue!(TKey , TValue)))
			{
				return __Value;
			}

		U opCast(U)()
			if(!is(U:KeyValuePair_TKey_TValue!(TKey , TValue)))
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
