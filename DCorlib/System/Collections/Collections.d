module System.Collections.Namespace;
import std.c.string : memmove, memset;
import std.math;
import System.Collections.Generic.IComparer_T;


alias System.Collections.Generic.ICollection_T.ICollection_T ICollection_T;
alias System.Collections.Generic.IList_T.IList_T IList_T;
alias System.Collections.IList.IList IList;

alias System.Collections.SortedList.SortedList SortedList;
alias System.Collections.Generic.IDictionary_TKey_TValue.IDictionary_TKey_TValue IDictionary_TKey_TValue;
alias System.Collections.Generic.Dictionary_TKey_TValue.Dictionary_TKey_TValue Dictionary_TKey_TValue;
alias System.Collections.Generic.IEnumerable_T.IEnumerable_T IEnumerable_T;
alias System.Collections.Hashtable.Hashtable Hashtable;
alias System.Collections.Hashtable_HashtableEnumerator.Hashtable_HashtableEnumerator Hashtable_HashtableEnumerator;
alias System.Collections.Hashtable_ValueCollection.Hashtable_ValueCollection Hashtable_ValueCollection;
alias System.Collections.Hashtable_KeyCollection.Hashtable_KeyCollection Hashtable_KeyCollection;
alias System.Collections.Hashtable_Entry.Hashtable_Entry Hashtable_Entry;
alias System.Collections.Hashtable_EnumeratorType.Hashtable_EnumeratorType Hashtable_EnumeratorType;
alias System.Collections.IDictionary.IDictionary IDictionary;
alias System.Collections.ICollection.ICollection ICollection;
alias System.Collections.IEnumerable.IEnumerable IEnumerable;
alias System.Collections.IEnumerator.IEnumerator IEnumerator;
alias System.Collections.IEqualityComparer.IEqualityComparer IEqualityComparer;







public bool EqualityComparisonImpl(T)(T a, T b) {
	static if (is(T == class) || is(T == interface)) {
		if (a !is null) {
			if (b !is null) {
				static if (is(typeof(T.opEquals))) {
					return cast(bool)a.opEquals(b);
				}
				else {
					return cast(bool)typeid(T).equals(&a, &b);
				}
			}
			return false;
		}
		if (b !is null) {
			return false;
		}
		return true;
	}
	else static if (is(T == struct)) {
		static if (is(T.opEquals)) {
			return cast(bool)a.opEquals(b);
		}
		else {
			return cast(bool)typeid(T).equals(&a, &b);
		}
	}
	else {
		return cast(bool)typeid(T).equals(&a, &b);
	}
}

public int ComparisonImpl(T)(T a, T b) 
{
	static if (is(T : string)) {
		return typeid(T).compare(&a, &b);//Culture.current.collator.compare(a, b);
	}
	else static if (is(T == class) || is(T == interface)) {
		if (a !is b) {
			if (a !is null) {
				if (b !is null) {
					static if (is(typeof(T.opCmp))) {
						return a.opCmp(b);
					}
					else {
						return typeid(T).compare(&a, &b);
					}
				}
				return 1;
			}
			return -1;
		}
		return 0;
	}
	else static if (is(T == struct)) {
		static if (is(typeof(T.opCmp))) {
			return a.opCmp(b);
		}
		else {
			return typeid(T).compare(&a, &b);
		}
	}
	else {
		return typeid(T).compare(&a, &b);
	}
}

/*int indexOf(T)(T[] array, T item, EqualityComparison!(T) comparison = null) {
 if (comparison is null) {
 comparison = (T a, T b) {
 return equalityComparisonImpl(a, b);
 };
 }

 for (auto i = 0; i < array.length; i++) {
 if (comparison(array[i], item))
 return i;
 }

 return -1;
 }*/




/**
 * Sorts the elements int a range of element in an _array using the specified Comparison(T).
 * Params:
 *   array = The _array to _sort.
 *   index = The starting _index of the range to _sort.
 *   length = The number of elements in the range to _sort.
 *   comparison = The Comparison(T) to use when comparing element.
 */
public void Sort(T, TIndex = int, TLength = TIndex)(T[] array, TIndex index, TLength length, int delegate(T, T) comparison = null) {
	
	void quickSortImpl(int left, int right) {
		if (left >= right)
			return;
		
		int i = left, j = right;
		T pivot = array[i + ((j - i) >> 1)];
		
		do {
			while (i < right && comparison(array[i], pivot) < 0)
				i++;
			while (j > left && comparison(pivot, array[j]) < 0)
				j--;
			
			assert(i >= left && j <= right);
			
			if (i <= j) {
				T temp = array[j];
				array[j] = array[i];
				array[i] = temp;
				
				i++;
				j--;
			}
		} while (i <= j);
		
		if (left < j)
			quickSortImpl(left, j);
		if (i < right)
			quickSortImpl(i, right);
	}
	
	if (comparison is null) {
		comparison = (T a, T b) {
			return ComparisonImpl(a, b);
		};
	}
	
	quickSortImpl(index, index + length - 1);
}

/**
 */
public void Sort(T)(T[] array, int delegate(T, T) comparison = null) {
	.Sort(array, 0, array.length, comparison);
}

/**
 * Searches a range of elements in an _array for a value using the specified Comparison(T).
 * Params:
 *   array = The _array to search.
 *   index = The starting _index of the range to search.
 *   length = The number of elements in the range to search.
 *   comparison = The Comparison(T) to use when comparing elements.
 */
public int BinarySearch(T, TIndex = int, TLength = TIndex)(T[] array, TIndex index, TLength length, T value, int delegate(T, T) comparison = null) {
	if (comparison is null) {
		comparison = (T a, T b) {
			return ComparisonImpl(a, b);
		};
	}
	
	int lo = cast(int)index;
	int hi = cast(int)(index + length - 1);
	while (lo <= hi) {
		int i = lo + ((hi - lo) >> 1);
		int order = comparison(array[i], value);
		if (order == 0)
			return i;
		if (order < 0)
			lo = i + 1;
		else
			hi = i - 1;
	}
	return ~lo;
}

public void Reverse(T, TIndex = int, TLength = TIndex)(T[] array, TIndex index, TLength length) {
	auto i = index;
	auto j = index + length - 1;
	while (i < j) {
		T temp = array[i];
		array[i] = array[j];
		array[j] = temp;
		i++, j--;
	}
}

/**
 */
public void Copy(T, TIndex = int, TLength = TIndex)(T[] source, TIndex sourceIndex, T[] target, TIndex targetIndex, TLength length) {
	if (length > 0)
		memmove(target.ptr + targetIndex, source.ptr + sourceIndex, length * T.sizeof);
}

public void Clear(T, TIndex = int, TLength = IIndex)(T[] array, TIndex index, TLength length) {
	if (length > 0)
		memset(array.ptr + index, 0, length * T.sizeof);
}

public TOutput[] ConvertAll(TInput, TOutput)(TInput[] array, Converter!(TInput, TOutput) converter) {
	auto ret = new TOutput[array.length];
	for (auto i = 0; i < array.length; i++) {
		ret[i] = converter(array[i]);
	}
	return ret;
}







/**
 */
class ReadOnlyList(T) : IList!(T) {
	
	private List!(T) list_;
	
	this(List!(T) list) {
		list_ = list;
	}
	
	final int indexOf(T item) {
		return list_.indexOf(item);
	}
	
	final bool contains(T item) {
		return list_.contains(item);
	}
	
	final void clear() {
		list_.clear();
	}
	
	final int count() {
		return list_.count;
	}
	
	final T opIndex(int index) {
		return list_[index];
	}
	
	version (UseRanges) {
		final bool empty() {
			return list_.empty;
		}
		
		final void popFront() {
			list_.popFront();
		}
		
		final T front() {
			return list_.front;
		}
	}
	else {
		final int opApply(int delegate(ref T) action) {
			return list_.opApply(action);
		}
	}
	
	protected void add(T item) {
		throw new NotSupportedException;
	}
	
	protected void insert(int index, T item) {
		throw new NotSupportedException;
	}
	
	protected bool remove(T item) {
		throw new NotSupportedException;
	}
	
	protected void removeAt(int index) {
		throw new NotSupportedException;
	}
	
	protected void opIndexAssign(T item, int index) {
		throw new NotSupportedException;
	}
	
	protected final IList!(T) list() {
		return list_;
	}
	
}



public const int[] PRIMES = [ 
	3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 
	1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 
	17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 
	187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 
	1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369 ];

public int GetPrime(int min) {
	
	bool isPrime(int candidate) {
		if ((candidate & 1) == 0)
			return candidate == 2;
		
		int limit = cast(int)sqrt(cast(double)candidate);
		for (int div = 3; div <= limit; div += 2) {
			if ((candidate % div) == 0)
				return false;
		}
		
		return true;
	}
	
	foreach (p; PRIMES) {
		if (p >= min)
			return p;
	}
	
	for (int p = min | 1; p < int.max; p += 2) {
		if (isPrime(p))
			return p;
	}
	
	return min;
}

/**
 */
public class KeyNotFoundException : Exception {
	
	this(string message = "The key was not present.") {
		super(message);
	}
	
}

/**
 * Provides a base class for implementations of the IEqualityComparer(T) interface.
 */
abstract class EqualityComparer(T) : IEqualityComparer!(T) {
	
	/**
	 * $(I Property.) Returns a default equality comparer for the type specified by the template parameter.
	 */
	static EqualityComparer instance() 
	{
		static EqualityComparer instance_;
		if (instance_ is null) 
		{
			instance_ = new class EqualityComparer 
			{
				override bool  Equals(T a, T b) {
					return EqualityComparisonImpl(a, b);
				}
				override uint  GetHash(T value) {
					return cast(uint)typeid(T).getHash(&value);
				}
			};
		}
		return instance_;
	}
	
	/**
	 * Determines whether the specified objects are equal.
	 * Params:
	 *   a = The first object to compare.
	 *   b = The second object to compare.
	 * Returns: true if the specified objects are equal; otherwise, false.
	 */
	abstract bool Equals(T a, T b);
	
	/**
	 * Retrieves a hash code for the specified object.
	 * Params: value = The object for which a hash code is to be retrieved.
	 * Returns: The hash code for the specified object.
	 */
	abstract uint GetHash(T value);
	
}


/**
 * Provides a base class for implementations of the IComparer(T) interface.
 */
abstract class Comparer(T) : IComparer!(T) {
	
	/**
	 * $(I Property.) Retrieves a default comparer for the type specified by the template parameter.
	 */
	static Comparer Instance() {
		static Comparer instance_;
		if (instance_ is null) {
			instance_ = new class Comparer {
				int Compare(T a, T b) {
					return comparisonImpl(a, b);
				}
			};
		}
		return instance_;
	}
	
	/**
	 * Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
	 * Params:
	 *   a = The first object to _compare.
	 *   b = The second object to _compare.
	 * Returns: 
	 *   $(TABLE $(TR $(TH Value) $(TH Condition))
	 *   $(TR $(TD Less than zero) $(TD a is less than b.))
	 *   $(TR $(TD Zero) $(TD a equals b.))
	 *   $(TR $(TD Greater than zero) $(TD a is greater than b.)))
	 */
	abstract int Compare(T a, T b);
	
}
















