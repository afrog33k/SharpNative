module System.Collections.Generic.Namespace;

alias System.Collections.Generic.IComparer__G.IComparer__G IComparer__G;

alias System.Collections.Generic.IEqualityComparer__G.IEqualityComparer__G IEqualityComparer__G;

alias System.Collections.Generic.List__G.List__G List__G;

alias System.Collections.Generic.ICollection__G.ICollection__G ICollection__G;

alias System.Collections.Generic.Dictionary_TKey_TValue.Dictionary_TKey_TValue Dictionary_TKey_TValue;

alias System.Collections.Generic.IList__G.IList__G IList__G;

alias System.Collections.Generic.Comparer_T.Comparer_T Comparer_T;

alias System.Collections.Generic.EqualityComparer__G.EqualityComparer__G EqualityComparer__G;

alias System.Collections.Generic.IEnumerator__G.IEnumerator__G IEnumerator__G;
alias System.Collections.Generic.IEnumerable__G.IEnumerable__G IEnumerable__G;


alias System.Collections.Generic.DefaultComparer_T.DefaultComparer_T DefaultComparer_T;

alias System.Collections.Generic.GenericEqualityComparer_T.GenericEqualityComparer_T GenericEqualityComparer_T;

alias System.Collections.Generic.KeyNotFoundException.KeyNotFoundException KeyNotFoundException;

alias System.Collections.Generic.List__G_EnumeratorT.List__G_EnumeratorT List__G_EnumeratorT;

alias System.Collections.Generic.KeyValuePair__G.KeyValuePair__G KeyValuePair__G;

alias System.Collections.Generic.Dictionary_TKey_TValue_EnumeratorTKey_TValue.Dictionary_TKey_TValue_EnumeratorTKey_TValue Dictionary_TKey_TValue_EnumeratorTKey_TValue;

alias System.Collections.Generic.Link.Link Link;

alias System.Collections.Generic.Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue.Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue;

alias System.Collections.Generic.Dictionary_TKey_TValue_Transform_TRetTKey_TValue.Dictionary_TKey_TValue_Transform_TRetTKey_TValue Dictionary_TKey_TValue_Transform_TRetTKey_TValue;

alias System.Collections.Generic.Dictionary_TKey_TValue_KeyCollectionTKey_TValue.Dictionary_TKey_TValue_KeyCollectionTKey_TValue Dictionary_TKey_TValue_KeyCollectionTKey_TValue;

alias System.Collections.Generic.Dictionary_TKey_TValue_ValueCollectionTKey_TValue.Dictionary_TKey_TValue_ValueCollectionTKey_TValue Dictionary_TKey_TValue_ValueCollectionTKey_TValue;

alias System.Collections.Generic.GenericComparer_T.GenericComparer_T GenericComparer_T;

alias System.Collections.Generic.Comparer_T_DefaultComparerT.Comparer_T_DefaultComparerT Comparer_T_DefaultComparerT;

alias System.Collections.Generic.EnumIntEqualityComparer_T.EnumIntEqualityComparer_T EnumIntEqualityComparer_T;

//alias System.Collections.Generic.CollectionDebuggerView_T_U.CollectionDebuggerView_T_U CollectionDebuggerView_T_U;

alias System.Collections.Generic.IntEqualityComparer.IntEqualityComparer IntEqualityComparer;

//alias System.Collections.Generic.CollectionDebuggerView_T.CollectionDebuggerView_T CollectionDebuggerView_T;

alias System.Collections.Generic.InternalStringComparer.InternalStringComparer InternalStringComparer;

alias System.Collections.Generic.Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue.Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue;

alias System.Collections.Generic.Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue.Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue;


// If we ever implement more interfaces on IReadOnlyCollection, we should also update RuntimeTypeCache.PopulateInterfaces() in rttype.cs
public interface IReadOnlyCollection__G(T) : IEnumerable__G!(T)
{
	public     abstract int  Count(IReadOnlyCollection__G!(T) j = null) @property;
}

public interface IReadOnlyList__G(T) : IReadOnlyCollection__G!(T)
{

	public abstract T opIndex(int index, IReadOnlyList__G!(T) j=null);
}

public interface IDictionary__G(TKey,TValue)  : ICollection__G!(KeyValuePair__G!(TKey, TValue))
{
	
	public void Add(TKey key, TValue value, IDictionary__G!(TKey,TValue) __j=null) ;

	public bool ContainsKey(TKey key, IDictionary__G!(TKey,TValue) __j=null) ;

	public bool Remove(TKey key, IDictionary__G!(TKey,TValue) __j=null) ;

	public bool TryGetValue(TKey key,  out TValue value, IDictionary__G!(TKey,TValue) __j=null) ;


	public     TValue opIndexAssign(TValue value, TKey key, IDictionary__G!(TKey, TValue) __j = null);

	public     TValue opIndex(TKey key, IDictionary__G!(TKey,TValue) __j=null);



	public      ICollection__G!(TKey)  Keys(IDictionary__G!(TKey,TValue) __j=null) @property;


	public      ICollection__G!(TValue)  Values(IDictionary__G!(TKey,TValue) __j=null) @property;
}

//Temporary Dictionary Implementation
public class Dictionary__G(TKey,TValue)
{
	TValue[TKey] __dict;

	public this(int capacity=0)
	{
		//__dict = new TKey[TValue](capacity); 
	}

	public void Add(TKey key, TValue value)
	{
		__dict[key]=value;
	}

	public TValue opIndex(TKey key, IDictionary__G!(TKey,TValue) j = null)
	{
		return __dict[key];
	}

	public void opIndexAssign(TKey key, TValue value, IDictionary__G!(TKey,TValue) j = null)
	{
		 __dict[key]=value;
	}
}




