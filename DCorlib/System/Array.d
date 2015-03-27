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
}

