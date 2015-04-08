module System.Array;
import System.Namespace;

class Array : NObject
{
	public	int Length() @property
	{
		return -1;
	}

	int Rank() @property
	{
		return -1;
	}

	int GetLength(int dimension=0)
	{

		return -1;

	}

	int GetUpperBound(int dimension=0)
	{

	return -1;

	}


	int GetLowerBound(int dimension=0)
	{
	return -1;

	}

	public static void Reverse(U)(Array_T!(U) array)
	{
		array.Reverse();
	}

	public static void Reverse(U)(Array_T!(U) array, int index, int length)
	{
		array.Reverse(index, length);
	}

	public static void Resize(U)(Array_T!(U) array, int newLength)
	{
		if(array !is null)
		array.Resize(newLength);
	}

	public static void Resize(U)(Array _array, int newLength)
	{
		auto array = cast(Array_T!(U))_array;
		array.Resize(newLength);
	}


	public static int IndexOf(T)(Array_T!(T) _array, T item, int start, ulong count)
	{
		auto array = cast(Array_T!(T))_array;
		return array.IndexOf(item,start,cast(int)count);
	}

	public static int IndexOf(T)(Array_T!(T) _array, T item)
	{
		auto array = cast(Array_T!(T))_array;
		return array.IndexOf(item);
	}

	public static void Copy(U)(Array_T!(U) _array,Array_T!(U) array, ulong count)
	{
		for(int c = 0; c < count; c++)
			array[c] = _array[c];
		//_array.CopyTo(array,0,cast(int)count);
	}



	public static void Copy(U)(Array_T!(U) _array,int startIndex,Array_T!(U) array,int startIndexDest, ulong count)
	{
		//_array.CopyTo(array,0,cast(int)count);
		for(int c = 0; c < count; c++)
			array[startIndex+c] = _array[startIndexDest+c];

	}

	public static void Copy(U)(Array_T!(U) _array,int startIndex,Array array,int startIndexDest, ulong count)
	{
		auto _fqArray = cast(Array_T!(U))array;
		//_array.CopyTo(_fqArray,0,cast(int)count);

		for(int c = 0; c < count; c++)
			_fqArray[startIndex+c] = _array[startIndexDest+c];
	}


	 //Array.Copy(this.items, 0, Cast!(Array)(array), arrayIndex, this.size);
	public static void Clear(U)(Array_T!(U) _array,int start, ulong count)
	{
		//_array.CopyTo(array,0,cast(int)count);
	}

//TODO: Implement these
	import System.Collections.Generic.Namespace;
	public static int BinarySearch(T)(Array_T!(T) _items, int index, int count, T item,  IComparer__G!(T) comparer)
	{
		return -1;
	}

	public static int LastIndexOf(T)(Array_T!(T) _items, T  item, int index, int count)
	{
		return -1;
	}
	


	public static void Sort(T)(Array_T!(T) _items, int index, int count, IComparer__G!(T) comparer)
	{
		
	}


	public static  ulong partition(T)(T[] array, Comparison__G!(T) comparison) {
		
		if(array.length < 2) { return 0; }

		auto pivot = array[$-1];

		// We are going to increment i, and decrement j in each iteration of
		// the while loop before using them.  So, they must start at the
		// location before where we really want them to be.
		//
		// This seems inconvenient, but it cleans up the possible ending
		// conditions of the while loop.  The ulong cannot really hold a
		// negative number, so this -1 requires special care in the while
		// loop.
		ulong i = -1;
		ulong j = array.length-1;

		// If i == -1, it will wrap around to a really big postive number.
		// So, include a test for it here.  The only time it should be
		// tested is the first iteration, while i still is -1, and the last
		// iteration, when i >= j.
		while(i < j || i == -1) {
			// Find the next element from the start that is larger or equal to
			// the pivot element.
			//
			// The pivot at the the end of the array is a sentinel, so we
			// needn't check for overrunning this array in direction.
			i += 1;
			while(comparison(array[i], pivot) < 0) {
				i += 1;
			}

			// Find the next element from the end that is less than or equal
			// to the pivot.  We must guard against running past the i'th
			// element, since we could have an array that was all greater than
			// the pivot.  If j was 0 before this, things would be bad.  That
			// is why the function asserts that the length be more than 1.
			j -= 1;
			while(i < j && comparison(array[j], pivot)>0) {
				j -= 1;
			}

			if(i < j && j < array.length) {
				// Swap the values pointed at by i and j.  Now, array[x] for x
				// <= i is <= pivot, and array[x] for x >= j is >= pivot.
				swap(array[i], array[j]);
			}
		}


		// We know i >= j.
		// 1) If i was incremented to be equal to j, then
		//  i = j = array.length - 1, and array[i] is the only element >=
		//  pivot.
		// 2) Or, i was incremented to j, and array[j] >= pivot from the
		//    previous iteration.  In this case, array[j] >= pivot.
		// 3) Or, i was incremented so that array[i] >= pivot, then j was
		//    decremented till j == i.  So, array[i] >= pivot.
		/// So, array[i] is always greater than or equal to the pivot after
		// the while loop.  And, array[i-1] is less than or equal to pivot,
		// if i > 0.

		// Swap the pivot into the right place.  Everything to the left is
		// <= pivot.  Every thing to the right is >= pivot.
		swap(array[i], array[$-1]);
		// Return the pivot location
		return i;
	}


	public static swap(T)(ref T a, ref T b)
	{
		T temp = a;
		a=b;
		b = temp;
	}

	public static T[] quicksort(T)(T[] array, Comparison__G!(T) comparison) {
		
		if(array.length > 1) {
			// Move the middle element to the end to act as the pivot.
			swap(array[$/2], array[$-1]);
			auto p = partition!(T)(array,comparison);
			// Index p is in position, so now recursively sort the other two
			// halves of the array.
			quicksort!(T)(array[0 .. p],comparison);
			quicksort!(T)(array[p + 1 .. $],comparison);
		}
		return array;
	}

	public static void Sort(T)(Array_T!(T) _items, Comparison__G!(T) comparison)
	{
		
		_items.Items = quicksort!(T)(_items.Items,comparison);
	}

	public static void Sort(T)(Array_T!(T) _items)
	{
		auto defaultcomparer = new Comparison__G!(T)((T a, T b)
								{
									return a.CompareTo(b);
								});

		_items.Items = quicksort!(T)(_items.Items,defaultcomparer);
	}
}

