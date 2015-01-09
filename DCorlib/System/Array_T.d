module System.Array_T;
import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;
import std.stdio;

class ArrayIterator(T):IEnumerator_T!(T)
	{
		int index = -1;
		Array_T!(T) _array;

		this(Array_T!(T) array)
		{
			_array = array;
			//Console.WriteLine(_S("inted with {0}"), BOX!(int)(index));
			//writeln(_array.Items[index]);
		}

		public T  Current(IEnumerator_T!(T) k=null)   @property
		{
			//writeln(_array is null);
			//Console.WriteLine(_S("returning {0}"), BOX!(int)(index));
			//writeln(_array.Items[index]);

			return _array.Items[index];
			//return cast(T)null;
		}

		void Dispose(IDisposable f=null)
		{
			_array = null;
		}


		bool MoveNext(IEnumerator j = null)
		{
			index++;
			if(index < _array.Length)
				return true;
			return false;
		}

		NObject Current(IEnumerator k=null) @property
		{
			return BOX!(T)(Current(cast(IEnumerator_T!(T))null)); // BOX should be adjusted to just pass classes as is
		}

		void Reset(IEnumerator k=null)
		{
			index = -1;
		}

	}

/*
 Array_T!(int[]) fiver = new Array_T!(int[])([ [1,2],[8],[3],[4],[6] ],2,3,4);
      
      Console.WriteLine(fiver.Ranks);
*/
public class Array_T(T=NObject) :  Array, ICollection_T!(T) 
//if(!is(T:void))
{
	private int index_;

	private T[] _items;
	private int[] _dims;

	T[] Items() @property
	{
		return _items;
	}

	void Resize(int newLength)
	{
		_items.length = newLength;
	}

	public int  IndexOf(T item)
	{
		return IndexOf(item,0, _items.length);
	}

	public int IndexOf(T item, int start, ulong count)
	{
		if(start+count > _items.length)
			throw new ArgumentOutOfRangeException();

		for(int i = start; i < (start+count); i++)
		{
			if(_items[i]==item)
				return i;
		}
		return -1;
	}

override	int Rank() @property
	{
		return cast(int)_dims.length;
	}

override	int GetLength(int dimension=0)
	{

		if(Rank==1)
		{
			if(dimension==0)
				return cast(int)_items.length;
			else
				throw new IndexOutOfRangeException();
		}
		else if(Rank > dimension)
			return _dims[dimension];
		else
			throw new IndexOutOfRangeException();

	}

override	int GetUpperBound(int dimension=0)
	{

		if(Rank==1)
		{
			if(dimension==0)
				return cast(int)_items.length-1;
			else
				throw new IndexOutOfRangeException();
		}
		else if(Rank > dimension)
			return _dims[dimension]-1;
		else
			throw new IndexOutOfRangeException();

	}


override	int GetLowerBound(int dimension=0)
	{
		//we can add support later, but its really an unoptimization
		return 0;
		//if(Rank==1)
		//{
		//	if(dimension==0)
		//		return cast(int)_items.length-1;
		//	else
		//		throw new IndexOutOfRangeException();
		//}
		//else if(Rank > dimension)
		//	return _dims[rank]-1;
		//else
		//	throw new IndexOutOfRangeException();

	}

	 void Items(T[] newItems) @property
	{
		 _items = newItems;
	}
	//params and array params are treated the same ... so int[] and 1,2,3,... are similar
	this(__IA!(T[]) ac,int[] dims...)
	{
		auto array = ac.A;
		//Console.WriteLine("initing array...");
		//Console.WriteLine(array);
		if(dims is null)
		{
			if(array !is null)
			{
				dims = [cast(int)array.length];
			}
		}
		_dims = dims;
		_items = array;
		_iter = new ArrayIterator!(T)(this);
	}



	this(int[] dims...)
	{
		_dims = dims;

		//Console.WriteLine(_dims);

		int totaldims = 1;

		for(int i=0;i<_dims.length;i++)
			totaldims*= _dims[i];
		
		//if(totaldims==1)
		//_items =  T[];
		//else
		_items = new T[totaldims];


		_iter = new ArrayIterator!(T)(this);
	}
	
override	public	int Length() @property 
	{
		return cast(int) _items.length;
	}

	public void Reverse()
	{
		_items.reverse;
	}

	
	
	public T GetValue(int index)
	{
		return _items[index];
	}

	public void SetValue(T value,int index)
	{
		 _items[index] = value;
	}

	public void CopyTo(Array_T!(T) other,int start=0,int end=-1)
	{
		if(end==-1)
			other.Items = _items[start..$].dup;
		else
			other.Items = _items[start..end+1].dup;
	}



	//Adds foreach support
//	Foreach Range Properties
//		Property	Purpose
//			.empty	returns true if no more elements
//				.front	return the leftmost element of the range
//					Foreach Range Methods
//					Method	Purpose
//					.popFront()	move the left edge of the range right by one


		final bool empty() {
			bool result = (index_ == _items.length);
			if (result)
				index_ = 0;
			return result;
		}
		
		final void popFront() {
			if (index_ < _items.length)
				index_++;
		}
		
		final T front() {
			return _items[index_];
		}

//Specialized for PInvoke and other uses
		final U opCast(U)() if(is(U:char**))// && is(T:string))
		{
			//throw new Exception("Sibitegeera");
			//exit(0);
			//copy with real addresses so the array can be modified
			char[][] charArray = new char[][Items.length];

			foreach(elem; Items)
			{
			//	Console.WriteLine(elem);
				charArray = charArray ~ cast(char[])(cast(string)elem);
			}

			return cast(U) charArray;
		}


		final U opCast(U)()
		if(is(U:T*))// && is(T:string))
		{
			//Console.WriteLine("cast... to T*");
			return cast(U) Items;
		}

//Needs fix, look at MatrixTest.cs
	final  void opIndexAssign(T value, int[] index...)
	{
		//Console.WriteLine("Assigning ...");
		int[] _indices = index; // .dup is slew

		auto finalindex = 0;
		auto len =cast(int)_indices.length;

		
		//Optimize common scenarios, slight performance boosts ... Add others
		
		 if(index.length==2) 
		{
			finalindex = _indices[0] * _dims[1]  + _indices[1];
		//Console.WriteLine("Assigning 2d...:" ~ std.conv.to!string(finalindex));


		}
		else
		 if(index.length==3) 
		{
			finalindex = _indices[0] * _dims[1] *_dims[2] + _indices[1] * _dims[2] + _indices[2];
		////Console.WriteLine("Assigning 3d...:" ~ std.conv.to!string(finalindex));

		}
		else
		{
		for(int i=len-1;i>=0;i--)
		{
			int multiplier = _indices[i];
			for(int j=i;j<len-1;j++)
			{
				multiplier*= _dims[j+1];
			}
			finalindex += multiplier;
		}
		}
	
		//Console.WriteLine("Assigning: "~ std.conv.to!string(value) ~ " to: " ~ std.conv.to!string(finalindex));


		_items[finalindex] =value;
		
	}



	final  void opIndexAssign(T value, int index)  {
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		_items[index] = value;
	}
	
	final  ref T opIndex(int index) { //TODO: ref could be a bad idea 
		//but allows alot of natural c# syntax
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		return _items[index];
	}

//Needs fix, look at MatrixTest.cs
	final  ref T opIndex(int[] index...) {
		//Console.WriteLine("Assigning ...");
		int[] _indices = index; // .dup is slew

		auto finalindex = 0;
		auto len =cast(int)_dims.length;

	//Optimize common scenarios, slight performance boosts ... Add others
		
		 if(len==2) 
		{
			finalindex = _indices[0] * _dims[1]  + _indices[1];
		//Console.WriteLine("Assigning 2d...:" ~ std.conv.to!string(finalindex));


		}
		else
		 if(len==3) 
		{
			finalindex = _indices[0] * _dims[1] *_dims[2] + _indices[1] * _dims[2] + _indices[2];
		////Console.WriteLine("Assigning 3d...:" ~ std.conv.to!string(finalindex));

		}
		else
		{
		for(int i=len-1;i>=0;i--)
		{
			int multiplier = _indices[i];
			for(int j=i;j<len-1;j++)
			{
				multiplier*= _dims[j+1];
			}
			finalindex += multiplier;
		}
		}

	

		//Console.WriteLine("Returning:" ~ std.conv.to!string(finalindex));
		//Console.WriteLine("Returning: "~ std.conv.to!string(_items[finalindex]) ~ " from: " ~ std.conv.to!string(finalindex));


		return _items[finalindex];
		
	}


	ArrayIterator!T _iter;

	//IEnumerator Methods
	IEnumerator GetEnumerator(IEnumerable j = null)
	{
		if(_iter is null)
			_iter = new ArrayIterator!(T)(this);

		_iter.Reset();

		return _iter;
		//return new ArrayIterator!(T)(this); //Highly inefficient
		
	}

	IEnumerator_T!(T) GetEnumerator(IEnumerable_T!(T) j=null)
	{
			if(_iter is null)
			_iter = new ArrayIterator!(T)(this);

		_iter.Reset();
		
		return _iter;
		//return new ArrayIterator!(T)(this); //Highly inefficient
		//throw new NotSupportedException();
	}



	//ICollection Methods
	void Add(T item, ICollection_T!(T) j=null)
	{
		throw new NotSupportedException();
	}

	public  bool  IsReadOnly(ICollection_T!(T) j=null) @property
	{
		throw new NotSupportedException();
	}


	bool Remove(T item,ICollection_T!(T) j=null)
	{
		throw new NotSupportedException();
	}

	bool Contains(T item,ICollection_T!(T) j=null)
	{
		throw new NotSupportedException();
	}

	public void CopyTo(Array_T!(T) array, int arrayIndex,ICollection_T!(T) j=null)
	{
		throw new NotSupportedException();
	}


	void Clear(ICollection_T!(T) j=null)
	{
		throw new NotSupportedException();
	}

	int Count(ICollection_T!(T) j=null) @property
	{
		return cast(int)_items.length;
	}

	int opApply(int delegate(ref T) action)
	{
		int r;
			
			for (auto i = 0; i < _items.length; i++) {
				if ((r = action(_items[i])) != 0)
					break;
			}
			
			return r;
	}

}

