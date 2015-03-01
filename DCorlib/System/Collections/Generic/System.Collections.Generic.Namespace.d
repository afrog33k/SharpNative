module System.Collections.Generic.Namespace;

alias System.Collections.Generic.IComparer_T.IComparer_T IComparer_T;

alias System.Collections.Generic.IEqualityComparer_T.IEqualityComparer_T IEqualityComparer_T;

alias System.Collections.Generic.IEnumerable_T.IEnumerable_T IEnumerable_T;

alias System.Collections.Generic.List__G.List__G List__G;

alias System.Collections.Generic.ICollection_T.ICollection_T ICollection_T;

alias System.Collections.Generic.Dictionary_TKey_TValue.Dictionary_TKey_TValue Dictionary_TKey_TValue;

alias System.Collections.Generic.IDictionary_TKey_TValue.IDictionary_TKey_TValue IDictionary_TKey_TValue;

alias System.Collections.Generic.IList__G.IList__G IList__G;

alias System.Collections.Generic.Comparer_T.Comparer_T Comparer_T;

alias System.Collections.Generic.EqualityComparer_T.EqualityComparer_T EqualityComparer_T;

alias System.Collections.Generic.IEnumerator_T.IEnumerator_T IEnumerator_T;

alias System.Collections.Generic.DefaultComparer_T.DefaultComparer_T DefaultComparer_T;

alias System.Collections.Generic.GenericEqualityComparer_T.GenericEqualityComparer_T GenericEqualityComparer_T;

alias System.Collections.Generic.KeyNotFoundException.KeyNotFoundException KeyNotFoundException;

alias System.Collections.Generic.List__G_EnumeratorT.List__G_EnumeratorT List__G_EnumeratorT;

alias System.Collections.Generic.KeyValuePair_TKey_TValue.KeyValuePair_TKey_TValue KeyValuePair_TKey_TValue;

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

// Generic version of IComparable.

public interface IComparable__G(T)
{
	// Interface does not need to be marked with the serializable attribute
	// Compares this object to another object, returning an integer that
	// indicates the relationship. An implementation of this method must return
	// a value less than zero if this is less than object, zero
	// if this is equal to object, or a value greater than zero
	// if this is greater than object.
	// 
	int CompareTo(T other, IComparable__G!(T) __j =null);

}

public interface IDictionary__G(TKey,TValue)
{
	public TValue opIndex(TKey key, IDictionary__G!(TKey,TValue) j=null);
	public void opIndexAssign(TKey key, TValue value, IDictionary__G!(TKey,TValue) j = null);
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