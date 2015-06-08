module System.Collections.Generic.List__G;
import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Generic.IList__G;
import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
import System.Threading.Namespace;
//import System.Runtime.Namespace;
//import System.Runtime.Versioning.Namespace;
//import System.Diagnostics.Namespace;
//import System.Diagnostics.Contracts.Namespace;
//import System.Security.Permissions.Namespace;

class List__G(T) : NObject ,IList__G!(T) ,System.Collections.Namespace.IList //,IReadOnlyList__G!(T)
{
    private __gshared const int _defaultCapacity = cast(int )4;
    private Array_T!(T) _items = cast(Array_T!(T)) null;
    private int _size = 0;
    private int _version = 0;
    private NObject _syncRoot = cast(NObject) null;
    __gshared Array_T!(T) _emptyArray;
    // Gets and sets the capacity of this list.  The capacity is the size of
    // the internal array used to hold items.  When set, the internal 
    // array of the list is reallocated to the given capacity.
    // 

    public int Capacity() 
    {
		{
			//Contract.Ensures(//Contract.Result<int>() >= 0);
			return _items.Length;
        }

    }

    public int Capacity(int value ) 
    {
		{
			if(value<this._size)
			{
			}
			////Contract.EndContractBlock();
			if(value!=_items.Length)
			{
				if(value>0)
				{
					/*Array_T!(T) newItems = new Array_T!(T)(value);
					if(this._size>0)
					{
						Array.Copy(this._items, 0, newItems, 0, this._size);
					}
					this._items=newItems;*/
					_items.__AdjustLength(value);
				}
				else
				{
					this._items=_emptyArray;
				}
			}
        }
		return Capacity;
    }

    // Read-only property describing how many elements are in the ListT.

    public int Count() 
    {
		{
			//Contract.Ensures(//Contract.Result<int>() >= 0);
			return this._size;
        }

    }

    public int Count(ICollection__G!(T) __ig=null) 
    {
		return Count;
    }

    public int Count(System.Collections.Namespace.ICollection __ig=null) 
    {
		return Count;
    }

    public int Count(IReadOnlyCollection__G!(T) __ig=null) 
    {
		return Count;
    }


    bool IsFixedSize(System.Collections.Namespace.IList __ig=null) 
    {
		{
			return false;
        }

    }

    // Is this ListT read-only?

    bool IsReadOnly(ICollection__G!(T) __ig=null) 
    {
		{
			return false;
        }

    }


    bool IsReadOnly(System.Collections.Namespace.IList __ig=null) 
    {
		{
			return false;
        }

    }

    // Is this ListT synchronized (thread-safe)?

    bool IsSynchronized(System.Collections.Namespace.ICollection __ig=null) 
    {
		{
			return false;
        }

    }

    // Synchronization root for this object.

    NObject SyncRoot(System.Collections.Namespace.ICollection __ig=null) 
    {
		{
			if(this._syncRoot is null)
			{
				System.Threading.Namespace.Interlocked.CompareExchange!(NObject)(this._syncRoot, new NObject(), cast(NObject)null);
			}
			return this._syncRoot;
        }

    }

    // Sets or Gets the element at the given index.
    // 

    public T opIndex(int index)
    {
		{
			// Following trick can reduce the range check by one
			if(Cast!(uint)(index)>=Cast!(uint)(this._size))
			{
			}
			// //Contract.EndContractBlock();
			return this._items[index];
        }

    }

    public T opIndexAssign(T value, int index)
    {
		{
			if(Cast!(uint)(index)>=Cast!(uint)(this._size))
			{
			}
			this._items[index]=value;
			this._version++;
        }
		return value;
    }

    public T opIndex(int index, IList__G!(T) __j = null)
    {
		return opIndex(index);
    }

    public T opIndexAssign(T value, int index, IList__G!(T) __j = null)
    {
		return opIndexAssign(value,index);
    }

    public T opIndex(int index, IReadOnlyList__G!(T) __j = null)
    {
		return opIndex(index);
    }

    public T opIndexAssign(T value, int index, IReadOnlyList__G!(T) __j = null)
    {
		return opIndexAssign(value,index);
    }


    private static bool IsCompatibleObject(NObject value)
    {
		// Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
		// Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
		return (((IsCast!(T)(value)))||(__IsNull(value)&& __IsNull(__Default!(T))));
    }

    NObject opIndex(int index, System.Collections.Namespace.IList __j = null)
    {
		{
			return  BOX!(T)(this[index,cast(IList__G!(T))null]);
        }

    }

    NObject opIndexAssign(NObject value, int index, System.Collections.Namespace.IList __j = null)
    {
		{
			// //ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
			try
			{
				this[index,cast(IList__G!(T))null]=Cast!(T)(value);
			}
			catch(System.Namespace.InvalidCastException __ex)
			{
			}
        }
		return value;
    }

    // Adds the given object to the end of this list. The size of the list is
    // increased by one. If required, the capacity of the list is doubled
    // before adding the new element.
    //

    public void Add(T item, ICollection__G!(T) __j = null)
    {
		if(this._size==_items.Length)
		{
			EnsureCapacity(this._size+1);
		}
		//_items._items.length=this._size++;
		this._items[this._size++]=item;
		this._version++;
    }

    int Add(NObject item, System.Collections.Namespace.IList __j = null)
    {
		//  //ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
		try
		{
			Add(BOX!(T)(Cast!(T)(item)));
		}
		catch(System.Namespace.InvalidCastException __ex)
		{
		}
		return this.Count-1;
    }
    // Adds the elements of the given collection to the end of this list. If
    // required, the capacity of the list is increased to twice the previous
    // capacity or the new size, whichever is larger.
    //

    public void AddRange(IEnumerable__G!(T) collection)
    {
		////Contract.Ensures(Count >= //Contract.OldValue(Count));
		////Contract.Ensures(Count >= //Contract.OldValue(Count));
		InsertRange(this._size, collection);
    }

	//TODO: Fix this
	/*
    public ReadOnlyCollection__G!(T) AsReadOnly()
    {
		// //Contract.Ensures(//Contract.Result<ReadOnlyCollection<T>>() != null);
		return  new ReadOnlyCollection__G!(T)(this);
    }*/

    // Searches a section of the list for a given element using a binary search
    // algorithm. Elements of the list are compared to the search value using
    // the given IComparer interface. If comparer is null, elements of
    // the list are compared to the search value using the IComparable
    // interface, which in that case must be implemented by all elements of the
    // list and the given search value. This method assumes that the given
    // section of the list is already sorted; if this is not the case, the
    // result will be incorrect.
    //
    // The method returns the index of the given value in the list. If the
    // list does not contain the given value, the method returns a negative
    // integer. The bitwise complement operator (~) can be applied to a
    // negative result to produce the index of the first element (if any) that
    // is larger than the given search value. This is also the index at which
    // the search value should be inserted into the list in order for the list
    // to remain sorted.
    // 
    // The method uses the Array.BinarySearch method to perform the
    // search.
    // 

    public int BinarySearch(int index, int count, T item, IComparer__G!(T) comparer)
    {
		// if (index < 0)
		//     //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		//  if (count < 0)
		//      //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		//if (_size - index < count)
		//      //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		//  //Contract.Ensures(//Contract.Result<int>() <= index + count);
		//  //Contract.EndContractBlock();
		return Array.BinarySearch!(T)(this._items, index, count, BOX!(T)(item), comparer);
    }

    public int BinarySearch(T item)
    {
		//Contract.Ensures(//Contract.Result<int>() <= Count);
		return BinarySearch(0, this.Count, BOX!(T)(item), null);
    }

    public int BinarySearch(T item, IComparer__G!(T) comparer)
    {
		//Contract.Ensures(//Contract.Result<int>() <= Count);
		return BinarySearch(0, this.Count, BOX!(T)(item), comparer);
    }
    // Clears the contents of ListT.

    public void Clear()
    {
		if(this._size>0)
		{
			Array.Clear(this._items, 0, this._size);
			// Don't need to doc this but we clear the elements so that the gc can reclaim the references.
			this._size=0;
		}
		this._version++;
    }
    public void Clear(ICollection__G!(T) __j = null)
    {
		Clear();
    }
    public void Clear(System.Collections.Namespace.IList __j = null)
    {
		Clear();
    }
    // Contains returns true if the specified element is in the List.
    // It does a linear, O(n) search.  Equality is determined by calling
    // item.Equals().
    //

    public bool Contains(T item, ICollection__G!(T) __j = null)
    {
		if(Cast!(NObject)(item) is null)
		{

			for (int i = 0;i<this._size;i++)
			{
				if(Cast!(NObject)(this._items[i]) is null)
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			//TODO: this is an issue, 
			EqualityComparer__G!(T) c =  EqualityComparer__G!(T).Default;

			for (int i = 0;i<this._size;i++)
			{
				if(c.Equals(BOX!(T)(this._items[i]), BOX!(T)(item)))
				{
					return true;
				}
			}
			return false;
		}
    }

    bool Contains(NObject item, System.Collections.Namespace.IList __j = null)
    {
		if(IsCompatibleObject(item))
		{
			return Contains(BOX!(T)(Cast!(T)(item)));
		}
		return false;
    }

    public List__G!(TOutput) ConvertAll(TOutput)(Converter__G!(T, TOutput) converter)
    {
		if(converter is null)
		{
		}
		////Contract.EndContractBlock();
		List__G!(TOutput) list =  new List__G!(TOutput)(this._size);

		for (int i = 0;i<this._size;i++)
		{
			list._items[i]=converter(BOX!(T)(this._items[i]));
		}
		list._size=this._size;
		return list;
    }
    // Copies this ListT into array, which must be of a 
    // compatible array type.  
    //

    public void CopyTo(Array_T!(T) array)
    {
		CopyTo(array, 0);
    }
    // Copies this ListT into array, which must be of a 
    // compatible array type.  
    //

    void CopyTo(Array array, int arrayIndex, System.Collections.Namespace.ICollection __j = null)
    {
		if((array !is null)&&(array.Rank!=1))
		{
		}
		////Contract.EndContractBlock();
		try
		{
			// Array.Copy will check for NULL.
			// Array.Copy will check for NULL.
			Array.Copy(this._items, 0, array, arrayIndex, this._size);
		}
		catch(Exception __ex)
	//	catch(System.Namespace.ArrayTypeMismatchException __ex)
		{
		}
    }
    // Copies a section of this list to the given array at the given index.
    // 
    // The method uses the Array.Copy method to copy the elements.
    // 

    public void CopyTo(int index, Array_T!(T) array, int arrayIndex, int count)
    {
		if(this._size-index<count)
		{
		}
		// //Contract.EndContractBlock();
		// Delegate rest of error checking to Array.Copy.
		// //Contract.EndContractBlock();
		// Delegate rest of error checking to Array.Copy.
		Array.Copy(this._items, index, array, arrayIndex, count);
    }

    public void CopyTo(Array_T!(T) array, int arrayIndex, ICollection__G!(T) __j = null)
    {
		// Delegate rest of error checking to Array.Copy.
		// Delegate rest of error checking to Array.Copy.
		Array.Copy(this._items, 0, array, arrayIndex, this._size);
    }
    // Ensures that the capacity of this list is at least the given minimum
    // value. If the currect capacity of the list is less than min, the
    // capacity is increased to twice the current capacity or to min,
    // whichever is larger.

    private void EnsureCapacity(int min)
    {
		if(_items.Length<min)
		{
			int newCapacity = cast(int)((_items.Length==0) ? (_defaultCapacity) : (_items.Length*2));
			// Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
			// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
			//if ((uint)newCapacity > Array.MaxArrayLength) newCapacity = Array.MaxArrayLength;
			if(newCapacity<min)
			{
				newCapacity=min;
			}
			this.Capacity=newCapacity;
		}
    }

    public bool Exists(Predicate__G!(T) match)
    {
		return FindIndex(match)!=-1;
    }

    public T Find(Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.EndContractBlock();

		for (int i = 0;i<this._size;i++)
		{
			if(match(BOX!(T)(this._items[i])))
			{
				return this._items[i];
			}
		}
		return __Default!(T);
    }

    public List__G!(T) FindAll(Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.EndContractBlock();
		List__G!(T) list =  new List__G!(T)();

		for (int i = 0;i<this._size;i++)
		{
			if(match(BOX!(T)(this._items[i])))
			{
				list.Add(BOX!(T)(this._items[i]));
			}
		}
		return list;
    }

    public int FindIndex(Predicate__G!(T) match)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		return FindIndex(0, this._size, match);
    }

    public int FindIndex(int startIndex, Predicate__G!(T) match)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < startIndex + Count);
		return FindIndex(startIndex, this._size-startIndex, match);
    }

    public int FindIndex(int startIndex, int count, Predicate__G!(T) match)
    {
		if(Cast!(uint)(startIndex)>Cast!(uint)(this._size))
		{
		}
		if(count<0||startIndex>this._size-count)
		{
		}
		if(match is null)
		{
		}
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < startIndex + count);
		//Contract.EndContractBlock();
		int endIndex = startIndex+count;

		for (int i = startIndex;i<endIndex;i++)
		{
			if(match(BOX!(T)(this._items[i])))
			{
				return i;
			}
		}
		return -1;
    }

    public T FindLast(Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.EndContractBlock();

		for (int i = this._size-1;i>=0;i--)
		{
			if(match(BOX!(T)(this._items[i])))
			{
				return this._items[i];
			}
		}
		return __Default!(T);
    }

    public int FindLastIndex(Predicate__G!(T) match)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		return FindLastIndex(this._size-1, this._size, match);
    }

    public int FindLastIndex(int startIndex, Predicate__G!(T) match)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() <= startIndex);
		return FindLastIndex(startIndex, startIndex+1, match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() <= startIndex);
		//Contract.EndContractBlock();
		if(this._size==0)
		{
			// Special case for 0 length ListT
			if(startIndex!=-1)
			{
			}
		}
		else
		{
			// Make sure we're not out of range            
			if(Cast!(uint)(startIndex)>=Cast!(uint)(this._size))
			{
			}
		}
		// 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
		if(count<0||startIndex-count+1<0)
		{
		}
		int endIndex = startIndex-count;

		for (int i = startIndex;i>endIndex;i--)
		{
			if(match(BOX!(T)(this._items[i])))
			{
				return i;
			}
		}
		return -1;
    }

    public void ForEach(Action__G!(T) action)
    {
		if(action is null)
		{
		}
		//Contract.EndContractBlock();
		int __csversion = this._version;

		for (int i = 0;i<this._size;i++)
		{
			//  if (version != _version && BinaryCompatibility.TargetsAtLeast_Desktop_V4_5) {
			//    break;
			//}
			//  if (version != _version && BinaryCompatibility.TargetsAtLeast_Desktop_V4_5) {
			//    break;
			//}
			//  if (version != _version && BinaryCompatibility.TargetsAtLeast_Desktop_V4_5) {
			//    break;
			//}
			action(BOX!(T)(this._items[i]));
		}
    }
    // Returns an enumerator for this list with the given
    // permission for removal of elements. If modifications made to the list 
    // while an enumeration is in progress, the MoveNext and 
    // GetObject methods of the enumerator will throw an exception.
    //

    public List__G!(T).Enumerator__G GetEnumerator()
    {
		return   List__G!(T).Enumerator__G(this);
    }
    /// <internalonly/>


    IEnumerator__G!(T) GetEnumerator(IEnumerable__G!(T) __j = null)
    {
		return BOX!(List__G!(T).Enumerator__G)(  List__G!(T).Enumerator__G(this));
    }

    System.Collections.Namespace.IEnumerator GetEnumerator(System.Collections.Namespace.IEnumerable __j = null)
    {
		return BOX!(List__G!(T).Enumerator__G)(  List__G!(T).Enumerator__G(this));
    }

    public List__G!(T) GetRange(int index, int count)
    {
		if(index<0)
		{
		}
		if(count<0)
		{
		}
		if(this._size-index<count)
		{
		}
		//Contract.Ensures(//Contract.Result<ListT<T>>() != null);
		//Contract.EndContractBlock();
		List__G!(T) list =  new List__G!(T)(count);
		Array.Copy(this._items, index, list._items, 0, count);
		list._size=count;
		return list;
    }
    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards from beginning to end.
    // The elements of the list are compared to the given value using the
    // Object.Equals method.
    // 
    // This method uses the Array.IndexOf method to perform the
    // search.
    // 

    public int IndexOf(T item, IList__G!(T) __j = null)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		return Array.IndexOf!(T)(this._items, BOX!(T)(item), 0, this._size);
    }

    int IndexOf(NObject item, System.Collections.Namespace.IList __j = null)
    {
		if(IsCompatibleObject(item))
		{
			return IndexOf(BOX!(T)(Cast!(T)(item)));
		}
		return -1;
    }
    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards, starting at index
    // index and ending at count number of elements. The
    // elements of the list are compared to the given value using the
    // Object.Equals method.
    // 
    // This method uses the Array.IndexOf method to perform the
    // search.
    // 

    public int IndexOf(T item, int index)
    {
		// if (index > _size)
		//ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		//Contract.EndContractBlock();
		return Array.IndexOf!(T)(this._items, BOX!(T)(item), index, this._size-index);
    }
    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards, starting at index
    // index and upto count number of elements. The
    // elements of the list are compared to the given value using the
    // Object.Equals method.
    // 
    // This method uses the Array.IndexOf method to perform the
    // search.
    // 

    public int IndexOf(T item, int index, int count)
    {
		// if (index > _size)
		//ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
		// if (count <0 || index > _size - count) //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		//Contract.EndContractBlock();
		return Array.IndexOf!(T)(this._items, BOX!(T)(item), index, count);
    }
    // Inserts an element into this list at a given index. The size of the list
    // is increased by one. If required, the capacity of the list is doubled
    // before inserting the new element.
    // 

    public void Insert(int index, T item, IList__G!(T) __j = null)
    {
		// Note that insertions at the end are legal.
	/*	if(Cast!(uint)(index)>Cast!(uint)(this._size))
		{
		}
		//Contract.EndContractBlock();
		if(this._size==_items.Length)
		{
			EnsureCapacity(this._size+1);
		}
		if(index<this._size)
		{
			this._items.__AdjustLength(index+1);//.length = index+1;
			//Array.Copy(this._items, index, this._items, index+1, this._size-index);
		}*/
		this._items[index]=item;
		this._size++;
		this._version++;
    }

    void Insert(int index, NObject item, System.Collections.Namespace.IList __j = null)
    {
		//ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
		try
		{
			Insert(index, BOX!(T)(Cast!(T)(item)));
		}
		catch(System.Namespace.InvalidCastException __ex)
		{
		}
    }
    // Inserts the elements of the given collection at a given index. If
    // required, the capacity of the list is increased to twice the previous
    // capacity or the new size, whichever is larger.  Ranges may be added
    // to the end of the list by setting index to the ListT's size.
    //

    public void InsertRange(int index, IEnumerable__G!(T) collection)
    {
		if(collection is null)
		{
		}
		if(Cast!(uint)(index)>Cast!(uint)(this._size))
		{
		}
		//Contract.EndContractBlock();
		ICollection__G!(T) c = AsCast!(ICollection__G!(T))(collection);
		if(c !is null)
		{
			int count = c.Count;
			if(count>0)
			{
				EnsureCapacity(this._size+count);
				if(index<this._size)
				{
					Array.Copy(this._items, index, this._items, index+count, this._size-index);
				}
				// If we're inserting a ListT into itself, we want to be able to deal with that.
				if(this==c)
				{
					// Copy first part of _items to insert location
					// Copy first part of _items to insert location
					Array.Copy(this._items, 0, this._items, index, index);
					// Copy last part of _items back to inserted location
					// Copy last part of _items back to inserted location
					Array.Copy(this._items, index+count, this._items, index*2, this._size-index);
				}
				else
				{
					Array_T!(T) itemsToInsert = new Array_T!(T)(count);
					c.CopyTo(itemsToInsert, 0,cast(ICollection__G!(T)) null);
					itemsToInsert.CopyTo(this._items, index);
				}
				this._size+=count;
			}
		}
		else
		{
			//using block ... IEnumerator<T> en = collection.GetEnumerator()
			{
				IEnumerator__G!(T)  en = collection.GetEnumerator(cast(IEnumerable__G!(T)) null);
				try
				{
					while (en.MoveNext(cast(System.Collections.Namespace.IEnumerator) null))
					{
						Insert(index++, BOX!(T)(en.Current));
					}
				}
				finally
				{
					if(en !is null)
						en.Dispose(cast(IDisposable)null);
				}
			}
		}
		this._version++;
    }
    // Returns the index of the last occurrence of a given value in a range of
    // this list. The list is searched backwards, starting at the end 
    // and ending at the first element in the list. The elements of the list 
    // are compared to the given value using the Object.Equals method.
    // 
    // This method uses the Array.LastIndexOf method to perform the
    // search.
    // 

    public int LastIndexOf(T item)
    {
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(//Contract.Result<int>() < Count);
		if(this._size==0)
		{
			return -1;
		}
		else
		{
			return LastIndexOf(BOX!(T)(item), this._size-1, this._size);
		}
    }
    // Returns the index of the last occurrence of a given value in a range of
    // this list. The list is searched backwards, starting at index
    // index and ending at the first element in the list. The 
    // elements of the list are compared to the given value using the 
    // Object.Equals method.
    // 
    // This method uses the Array.LastIndexOf method to perform the
    // search.
    // 

    public int LastIndexOf(T item, int index)
    {
		//if (index >= _size)
		//ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(((Count == 0) && (//Contract.Result<int>() == -1)) || ((Count > 0) && (//Contract.Result<int>() <= index)));
		//Contract.EndContractBlock();
		return LastIndexOf(BOX!(T)(item), index, index+1);
    }
    // Returns the index of the last occurrence of a given value in a range of
    // this list. The list is searched backwards, starting at index
    // index and upto count elements. The elements of
    // the list are compared to the given value using the Object.Equals
    // method.
    // 
    // This method uses the Array.LastIndexOf method to perform the
    // search.
    // 

    public int LastIndexOf(T item, int index, int count)
    {
		if((this.Count!=0)&&(index<0))
		{
		}
		if((this.Count!=0)&&(count<0))
		{
		}
		//Contract.Ensures(//Contract.Result<int>() >= -1);
		//Contract.Ensures(((Count == 0) && (//Contract.Result<int>() == -1)) || ((Count > 0) && (//Contract.Result<int>() <= index)));
		//Contract.EndContractBlock();
		if(this._size==0)
		{
			return -1;
		}
		if(index>=this._size)
		{
		}
		if(count>index+1)
		{
		}
		return Array.LastIndexOf!(T)(this._items, BOX!(T)(item), index, count);
    }
    // Removes the element at the given index. The size of the list is
    // decreased by one.
    // 

    public bool Remove(T item, ICollection__G!(T) __j = null)
    {
		int index = IndexOf(BOX!(T)(item));
		if(index>=0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
    }

    void Remove(NObject item, System.Collections.Namespace.IList __j = null)
    {
		if(IsCompatibleObject(item))
		{
			Remove(BOX!(T)(Cast!(T)(item)));
		}
    }
    // This method removes all items which matches the predicate.
    // The complexity is O(n).   

    public int RemoveAll(Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.Ensures(//Contract.Result<int>() >= 0);
		//Contract.Ensures(//Contract.Result<int>() <= //Contract.OldValue(Count));
		//Contract.EndContractBlock();
		int freeIndex = 0;
		// the first free slot in items array
		// Find the first item which needs to be removed.
		while (freeIndex<this._size&&!match(BOX!(T)(this._items[freeIndex])))
		{
			freeIndex++;
		}
		if(freeIndex>=this._size)
		{
			return 0;
		}
		int current = freeIndex+1;
		while (current<this._size)
		{
			// Find the first item which needs to be kept.
			while (current<this._size&&match(BOX!(T)(this._items[current])))
			{
				current++;
			}
			if(current<this._size)
			{
				this._items[freeIndex++]=this._items[current++];
			}
		}
		Array.Clear(this._items, freeIndex, this._size-freeIndex);
		int result = this._size-freeIndex;
		this._size=freeIndex;
		this._version++;
		return result;
    }
    // Removes the element at the given index. The size of the list is
    // decreased by one.
    // 

    public void RemoveAt(int index)
    {
		if(Cast!(uint)(index)>=Cast!(uint)(this._size))
		{
		}
		this._size--;
		if(index<this._size)
		{
			Array.Copy(this._items, index+1, this._items, index, this._size-index);
		}
		this._items[this._size]=__Default!(T);
		this._version++;
    }
    public void RemoveAt(int index, IList__G!(T) __j = null)
    {
		RemoveAt(index);
    }
    public void RemoveAt(int index, System.Collections.Namespace.IList __j = null)
    {
		RemoveAt(index);
    }
    // Removes a range of elements from this list.
    // 

    public void RemoveRange(int index, int count)
    {
		if(index<0)
		{
		}
		if(count<0)
		{
		}
		if(this._size-index<count)
		{
			//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			//Contract.EndContractBlock();
			if(count>0)
			{
				int i = this._size;
				this._size-=count;
				if(index<this._size)
				{
					Array.Copy(this._items, index+count, this._items, index, this._size-index);
				}
				Array.Clear(this._items, this._size, count);
				this._version++;
			}
		}
    }
    // Reverses the elements in this list.

    public void Reverse()
    {
		Reverse(0, this.Count);
    }
    // Reverses the elements in a range of this list. Following a call to this
    // method, an element in the range given by index and count
    // which was previously located at index i will now be located at
    // index index + (index + count - i - 1).
    // 
    // This method uses the Array.Reverse method to reverse the
    // elements.
    // 

    public void Reverse(int index, int count)
    {
		if(index<0)
		{
		}
		if(count<0)
		{
		}
		if(this._size-index<count)
		{
			//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			//Contract.EndContractBlock();
			//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			//Contract.EndContractBlock();
			Array.Reverse(this._items, index, count);
		}
		this._version++;
    }
    // Sorts the elements in this list.  Uses the default comparer and 
    // Array.Sort.

    public void Sort()
    {
		Sort(0, this.Count, null);
    }
    // Sorts the elements in this list.  Uses Array.Sort with the
    // provided comparer.

    public void Sort(IComparer__G!(T) comparer)
    {
		Sort(0, this.Count, comparer);
    }
    // Sorts the elements in a section of this list. The sort compares the
    // elements to each other using the given IComparer interface. If
    // comparer is null, the elements are compared to each other using
    // the IComparable interface, which in that case must be implemented by all
    // elements of the list.
    // 
    // This method uses the Array.Sort method to sort the elements.
    // 

    public void Sort(int index, int count, IComparer__G!(T) comparer)
    {
		if(index<0)
		{
		}
		if(count<0)
		{
		}
		if(this._size-index<count)
		{
			//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			//Contract.EndContractBlock();
			//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			//Contract.EndContractBlock();
			Array.Sort!(T)(this._items, index, count, comparer);
		}
		this._version++;
    }

    public void Sort(Comparison__G!(T) comparison)
    {
		if(comparison is null)
		{
		}
		//Contract.EndContractBlock();
		if(this._size>0)
		{
		}
    }
    // ToArray returns a new Object array containing the contents of the ListT.
    // This requires copying the ListT, which is an O(n) operation.

    public Array_T!(T) ToArray()
    {
		//Contract.Ensures(//Contract.Result<T[]>() != null);
		//Contract.Ensures(//Contract.Result<T[]>().Length == Count);
		Array_T!(T) array = new Array_T!(T)(this._size);
		Array.Copy(this._items, 0, array, 0, this._size);
		return array;
    }
    // Sets the capacity of this list to the size of the list. This method can
    // be used to minimize a list's memory overhead once it is known that no
    // new elements will be added to the list. To completely clear a list and
    // release all memory referenced by the list, execute the following
    // statements:
    // 
    // list.Clear();
    // list.TrimExcess();
    // 

    public void TrimExcess()
    {
		int threshold = cast(int)((cast(double)_items.Length)*0.9);
		if(this._size<threshold)
		{
			this.Capacity=this._size;
		}
    }

    public bool TrueForAll(Predicate__G!(T) match)
    {
		if(match is null)
		{
		}
		//Contract.EndContractBlock();

		for (int i = 0;i<this._size;i++)
		{
			if(!match(BOX!(T)(this._items[i])))
			{
				return false;
			}
		}
		return true;
    }

    public static IList__G!(T) Synchronized(List__G!(T) list)
    {
		return  new List__G!(T).SynchronizedList__G(list);
    }

    public this()
    {
		this._items=_emptyArray;
    }

    public this(int capacity)
    {
		//            if (capacity < 0) //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		//            //Contract.EndContractBlock();
		if(capacity==0)
		{
			this._items=_emptyArray;
		}
		else
		{
			this._items=new Array_T!(T)(capacity);
		}
    }

    public this(IEnumerable__G!(T) collection)
    {
		//    if (collection==null)
		//       //ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
		//   //Contract.EndContractBlock();
		ICollection__G!(T) c = AsCast!(ICollection__G!(T))(collection);
		if(c !is null)
		{
			int count = c.Count;
			if(count==0)
			{
				this._items=_emptyArray;
			}
			else
			{
				this._items=new Array_T!(T)(count);
				c.CopyTo(this._items, 0,cast(ICollection__G!(T)) null);
				this._size=count;
			}
		}
		else
		{
			this._size=0;
			this._items=_emptyArray;
			// This enumerable could be empty.  Let Add allocate a new array, if needed.
			// Note it will also go to _defaultCapacity first, not 1, then 2, etc.
			//using block ... IEnumerator<T> en = collection.GetEnumerator()
			{
				IEnumerator__G!(T)  en = collection.GetEnumerator(cast(IEnumerable__G!(T)) null);
				try
				{
					while (en.MoveNext(cast(System.Collections.Namespace.IEnumerator) null))
					{
						Add(BOX!(T)(en.Current));
					}
				}
				finally
				{
					if(en !is null)
						en.Dispose(cast(IDisposable)null);
				}
			}
		}
    }

    static this()
    {
		_emptyArray = new Array_T!(T)(0);

    }

	import System.Namespace;
	import System.Collections.Generic.Namespace;
	import System.Collections.Namespace;


	static class SynchronizedList__G : NObject ,IList__G!(T)
	{
		private List__G!(T) _list = cast(List__G!(T)) null;
		private NObject _root = cast(NObject) null;

		public int Count(ICollection__G!(T) __ig=null) 
		{
			{
                synchronized(this._root)
                {
					return _list.Count;
                }
			}

		}


		public bool IsReadOnly(ICollection__G!(T) __ig=null) 
		{
			{
                return (Cast!(ICollection__G!(T))(this._list)).IsReadOnly;
			}

		}


		public void Add(T item, ICollection__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				_list.Add(BOX!(T)(item));
            }
		}

		public void Clear(ICollection__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				_list.Clear();
            }
		}

		public bool Contains(T item, ICollection__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				return _list.Contains(BOX!(T)(item));
            }
		}

		public void CopyTo(Array_T!(T) array, int arrayIndex, ICollection__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				_list.CopyTo(array, arrayIndex);
            }
		}

		public bool Remove(T item, ICollection__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				return _list.Remove(BOX!(T)(item));
            }
		}

		System.Collections.Namespace.IEnumerator GetEnumerator(System.Collections.Namespace.IEnumerable __j = null)
		{
            synchronized(this._root)
            {
				return BOX!(List__G!(T).Enumerator__G)(_list.GetEnumerator());
            }
		}

		IEnumerator__G!(T) GetEnumerator(IEnumerable__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				return (Cast!(IEnumerable__G!(T))(this._list)).GetEnumerator(cast(IEnumerable__G!(T)) null);
            }
		}

		public T opIndex(int index, IList__G!(T) __j = null)
		{
			{
                synchronized(this._root)
                {
					return this._list[index,cast(IList__G!(T))null];
                }
			}

		}

		public T opIndexAssign(T value, int index, IList__G!(T) __j = null)
		{
			{
                synchronized(this._root)
                {
					this._list[index,cast(IList__G!(T))null]=value;
                }
			}
			return value;
		}


		public int IndexOf(T item, IList__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				return _list.IndexOf(BOX!(T)(item));
            }
		}

		public void Insert(int index, T item, IList__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				_list.Insert(index, BOX!(T)(item));
            }
		}

		public void RemoveAt(int index, IList__G!(T) __j = null)
		{
            synchronized(this._root)
            {
				_list.RemoveAt(index);
            }
		}

		public this(List__G!(T) list)
		{
            this._list=list;
            this._root=(Cast!(System.Collections.Namespace.ICollection)(list)).SyncRoot;
		}

        public override String ToString()
        {
			return GetType().FullName;
        }

        public override Type GetType()
        {
			return __TypeOf!(typeof(this));
        }
	}

	import System.Namespace;
	import System.Collections.Generic.Namespace;
	import System.Collections.Namespace;


	static struct Enumerator__G
	{
		private List__G!(T) list = cast(List__G!(T)) null;
		private int index = 0;
		private int __csversion = 0;
		private T current = T.init;

		public void Dispose(System.Namespace.IDisposable __j = null)
		{
		}

		public bool MoveNext(System.Collections.Namespace.IEnumerator __j = null)
		{
            List__G!(T) localListT = this.list;
            if(this.__csversion==localListT._version&&(Cast!(uint)(this.index)<Cast!(uint)(localListT._size)))
            {
				this.current=localListT._items[this.index];
				this.index++;
				return true;
            }
            return MoveNextRare();
		}

		private bool MoveNextRare()
		{
            if(this.__csversion!=list._version)
            {
            }
            this.index=list._size+1;
            this.current=__Default!(T);
            return false;
		}

		public T Current(IEnumerator__G!(T) __ig=null) 
		{
			{
                return this.current;
			}

		}


		NObject Current(System.Collections.Namespace.IEnumerator __ig=null) 
		{
			{
                if(this.index==0||this.index==list._size+1)
                {
                }
                return  BOX!(T)(this.Current);
			}

		}


		void Reset(System.Collections.Namespace.IEnumerator __j = null)
		{
            if(this.__csversion!=list._version)
            {
            }
            this.index=0;
            this.current=__Default!(T);
		}

		public  void __init(List__G!(T) list)
		{
            this.list=list;
            this.index=0;
            this.__csversion=list._version;
            this.current=__Default!(T);
		}
		void __init(){}//default xtor
		static List__G!(T).Enumerator__G opCall(__U...)(__U args_)
		{
			List__G!(T).Enumerator__G s;
            s.__init(args_);
            return s;
		}

        public String ToString()
        {
			return GetType().FullName;
        }

        public static class __Boxed_ : Boxed!(List__G!(T).Enumerator__G) ,IEnumerator__G!(T) ,System.Collections.Namespace.IEnumerator
        {
			import std.traits;

			public T Current(IEnumerator__G!(T) __ig=null) { return __Value.Current;}

			public void Dispose(System.Namespace.IDisposable __j = null)
			{
				__Value.Dispose();
			}

			public bool MoveNext(System.Collections.Namespace.IEnumerator __j = null)
			{
				return __Value.MoveNext();
			}

			void Reset(System.Collections.Namespace.IEnumerator __j = null)
			{
				__Value.Reset();
			}

			NObject Current(System.Collections.Namespace.IEnumerator __ig=null) { return BOX!(T)(__Value.Current);}

			this()
			{
				super(Enumerator__G.init);
			}
			public override String ToString()
			{
				return __Value.ToString();
			}

			this(ref List__G!(T).Enumerator__G value)
			{
				super(value);
			}

			U opCast(U)()
				if(is(U:List__G!(T).Enumerator__G))
				{
					return __Value;
				}

			U opCast(U)()
				if(!is(U:List__G!(T).Enumerator__G))
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

        public __Boxed_ __Get_Boxed()
        {
			return new __Boxed_(this);
        }
        alias __Get_Boxed this;

        public Type GetType()
        {
			return __TypeOf!(typeof(this));
        }
	}

	public override String ToString()
	{
		return GetType().FullName;
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}
}


//
//
//
///**
// * Represents a list of elements that can be accessed by index.
// */
//class List__G(T) : IList__G!(T) {
//  
//  private enum DEFAULT_CAPACITY = 4;
//  
//  private T[] _items;
//  private int size_;
//  
//  private int index_;
//  
//  /**
//   * Initializes a new instance with the specified _capacity.
//   * Params: capacity = The number of elements the new list can store.
//   */
//  this(int capacity = 0) {
//    _items.length = capacity;
//  }
//  
//  /**
//   * Initializes a new instance containing elements copied from the specified _range.
//   * Params: range = The _range whose elements are copied to the new list.
//   */
//  this(T[] range) {
//    _items.length = size_ = cast(int)range.length;
//    _items = range;
//  }
//  
//  /**
//   * ditto
//   */
//  this(IEnumerable__G!(T) range) {
//    _items.length = DEFAULT_CAPACITY;
//      
//      auto __foreachIter2 = range.GetEnumerator();
//      while(__foreachIter2.MoveNext())
//      {
//        ICollection_T_Add(__foreachIter2.Current);
//       
//      }
//
//    //foreach (item; range)
//    //  ICollection_T_Add(item);
//  }
//  
//  /**
//   * Adds an element to the end of the list.
//   * Params: item = The element to be added.
//   */
//  final void ICollection_T_Add(T item) {
//    if (size_ == _items.length)
//      EnsureCapacity(size_ + 1);
//    _items[size_++] = item;
//  }
//  
//  /**
//   * Adds the elements in the specified _range to the end of the list.
//   * Params: The _range whose elements are to be added.
//   */
//  final void AddRange(T[] range) {
//    InsertRange(size_, range);
//  }
//  
//  /**
//   * ditto
//   */
//  final void AddRange(IEnumerable__G!(T) range) {
//    InsertRange(size_, range);
//  }
//  
//  /**
//   * Inserts an element into the list at the specified _index.
//   * Params:
//   *   index = The _index at which item should be inserted.
//   *   item = The element to insert.
//   */
//  final void Insert(int index, T item) {
//    if (size_ == _items.length)
//      EnsureCapacity(size_ + 1);
//
//    if (index < size_)
//      .Copy(_items, index, _items, index + 1, size_ - index);
//    
//    _items[index] = item;
//    size_++;
//  }
//  
//  /**
//   * Inserts the elements of a _range into the list at the specified _index.
//   * Params:
//   *   index = The _index at which the new elements should be inserted.
//   *   range = The _range whose elements should be inserted into the list.
//   */
//  final void InsertRange(int index, T[] range) {
//    foreach (item; range) {
//      Insert(index++, item);
//    }
//  }
//  
//  /**
//   * ditto
//   */
//  final void InsertRange(int index, IEnumerable__G!(T) range) {
//   
//      auto __foreachIter2 = range.GetEnumerator(cast(IEnumerable__G!(T)) null);
//      while(__foreachIter2.MoveNext(cast(IEnumerator__G!(T)) null))
//      {
//        Insert(index++,__foreachIter2.Current(cast(IEnumerator__G!(T)) null));
//       
//      }
//  }
//  
//  /**
//   */
//  final bool Remove(T item) {
//    int index = IndexOf(item);
//    
//    if (index < 0)
//      return false;
//
//    RemoveAt(index);
//    return true;
//  }
//
//  final void RemoveAt(int index) {
//    size_--;
//    if (index < size_)
//      .Copy(_items, index + 1, _items, index, size_ - index);
//    _items[size_] = T.init;
//  }
//
//  /**
//   */
//  final void RemoveRange(int index, int count) {
//    if (count > 0) {
//      size_ -= count;
//      if (index < size_)
//        .Copy(_items, index + count, _items, index, size_ - index);
//      .Clear(_items, size_, count);
//    }
//  }
//  
//  /**
//   */
//  final bool Contains(T item) {
//    for (auto i = 0; i < size_; i++) {
//      if (EqualityComparisonImpl(_items[i], item))
//        return true;
//    }
//    return false;
//  }
//  
//  IEnumerator GetEnumerator(IEnumerable j = null)
//{
//  return new Enumerator__G!(T)(this);
//}
//
//IEnumerator__G!(T) IEnumerable_T_GetEnumerator()
//{
//  return new Enumerator__G!(T)(this);
//}
//
//int Count(ICollection_T!(T) j = null) @property
//{
//     return cast(int)_items.length;
//}
//
//bool ICollection_T_IsReadOnly() 
//{
//  return false;
//}
//
//bool ICollection_T_Contains(T item)
//{
//  return Contains(item);;
//}
//
//void IList__G_RemoveAt(int index)
//{
//  return RemoveAt(index);
//}
//
//void IList__G_Insert(int index, T item)
//{
//  Insert(index,item);
//}
//
//int IList__G_IndexOf(T item)
//{
//  return IndexOf(item);
//}
//
//void ICollection_T_CopyTo(Array_T!T array, int arrayIndex)
//{
//  //CopyTo(array,arrayIndex);
//}
//
//
//bool ICollection_T_Remove(T item)
//{
//  return Remove(item);
//}
//
//  void ICollection_T_Clear()
//  {
//    Clear();
//  }
//  /**
//   */
//  final void Clear() {
//    if (size_ > 0) {
//      .Clear(_items, 0, size_);
//      size_ = 0;
//    }
//  }
//  
//  /**
//   */
//  final int IndexOf(T item) {
//    return IndexOf(item, null);
//  }
//  
//  /**
//   */
//  final int IndexOf(T item, EqualityComparison!(T) comparison) {
//    if (comparison is null) {
//      comparison = (T a, T b) {
//        return EqualityComparisonImpl(a, b);
//      };
//    }
//    
//    for (auto i = 0; i < size_; i++) {
//      if (comparison(_items[i], item))
//        return i;
//    }
//    
//    return -1;
//  }
//  
//  /**
//   */
//  final int LastIndexOf(T item, EqualityComparison!(T) comparison = null) {
//    if (comparison is null) {
//      comparison = (T a, T b) {
//        return EqualityComparisonImpl(a, b);
//      };
//    }
//    
//    for (auto i = size_ - 1; i >= 0; i--) {
//      if (comparison(_items[i], item))
//        return i;
//    }
//    
//    return -1;
//  }
//  
//  /**
//   */
//  final void Sort(Comparison!(T) comparison = null) {
//    .Sort(_items, 0, size_, comparison);
//  }
//  
//  /**
//   */
//  final int BinarySearch(T item, Comparison!(T) comparison = null) {
//    return .BinarySearch(_items, 0, size_, item, comparison);
//  }
//  
//  /**
//   */
//  final void CopyTo(T[] array) {
//    .Copy(_items, 0, array, 0, size_);
//  }
//  
//  /**
//   */
//  final T[] ToArray() {
//    return _items[0 .. size_].dup;
//  }
//  
//  /**
//   */
//  final T Find(Predicate!(T) match) {
//    for (auto i = 0; i < size_; i++) {
//      if (match(_items[i]))
//        return _items[i];
//    }
//    return T.init;
//  }
//  
//  /**
//   */
//  final T FindLast(Predicate!(T) match) {
//    for (auto i = size_ - 1; i >= 0; i--) {
//      if (match(_items[i]))
//        return _items[i];
//    }
//    return T.init;
//  }
//  
//  /**
//   */
//  final List__G FindAll(Predicate!(T) match) {
//    auto list = new List__G;
//    for (auto i = 0; i < size_; i++) {
//      if (match(_items[i]))
//        list.ICollection_T_Add(_items[i]);
//    }
//    return list;
//  }
//  
//  /**
//   */
//  final int FindIndex(Predicate!(T) match) {
//    for (auto i = 0; i < size_; i++) {
//      if (match(_items[i]))
//        return i;
//    }
//    return -1;
//  }
//  
//  /**
//   */
//  final int FindLastIndex(Predicate!(T) match) {
//    for (auto i = size_ - 1; i >= 0; i--) {
//      if (match(_items[i]))
//        return i;
//    }
//    return -1;
//  }
//  
//  /**
//   */
//  final bool Exists(Predicate!(T) match) {
//    return FindIndex(match) != -1;
//  }
//  
//  /**
//   */
//  final void ForEach(Action__G!(T) action) {
//    for (auto i = 0; i < size_; i++) {
//      action(_items[i]);
//    }
//  }
//  
//  /**
//   */
//  final bool TrueForAll(Predicate!(T) match) {
//    for (auto i = 0; i < size_; i++) {
//      if (!match(_items[i]))
//        return false;
//    }
//    return true;
//  }
//  
//  /**
//   */
//  final List__G!(T) GetRange(int index, int count) {
//    auto list = new List__G!(T)(count);
//    list._items[0 .. count] = _items[index .. index + count];
//    list.size_ = count;
//    return list;
//  }
//  
//  /**
//   */
//  final List!(TOutput) Convert(TOutput)(Converter!(T, TOutput) converter) {
//    auto list = new List!(TOutput)(size_);
//    for (auto i = 0; i < size_; i++) {
//      list._items[i] = converter(_items[i]);
//    }
//    list.size_ = size_;
//    return list;
//  }
//  
//  final int Count() 
//  {
//    return size_;
//  }
//  
//  final @property void Capacity(int value) 
//  {
//    _items.length = value;
//  }
//
//  final @property int Capacity() 
//  {
//    return cast(int)_items.length;
//  }
//  
//  final void opIndexAssign(T value, int index) 
//  {
//    if (index >= size_)
//      throw new ArgumentOutOfRangeException(String("index"));
//    
//    _items[index] = value;
//  }
//
//  final T opIndex(int index) 
//  {
//    if (index >= size_)
//      throw new ArgumentOutOfRangeException(String("index"));
//    
//    return _items[index];
//  }
//  
//  version (UseRanges) {
//    final bool empty() {
//      bool result = (index_ == size_);
//      if (result)
//        index_ = 0;
//      return result;
//    }
//    
//    final void popFront() {
//      if (index_ < size_)
//        index_++;
//    }
//    
//    final T front() {
//      return _items[index_];
//    }
//  }
//  else {
//    final int opApply(int delegate(ref T) action) {
//      int r;
//      
//      for (auto i = 0; i < size_; i++) {
//        if ((r = action(_items[i])) != 0)
//          break;
//      }
//      
//      return r;
//    }
//    
//    /**
//     * Ditto
//     */
//    final int opApply(int delegate(ref int, ref T) action) {
//      int r;
//      
//      for (auto i = 0; i < size_; i++) {
//        if ((r = action(i, _items[i])) != 0)
//          break;
//      }
//      
//      return r;
//    }
//  }
//  
//  final bool opIn_r(T item) {
//    return Contains(item);
//  }
//  
//  private void EnsureCapacity(int min) {
//    if (_items.length < min) {
//      int n = cast(int)((_items.length == 0) ? DEFAULT_CAPACITY : _items.length * 2);
//      if (n < min)
//        n = min;
//      this.Capacity = n;
//    }
//  }
//  
//  private class Enumerator__G(T):IEnumerator__G!(T)
//  {
//      int index = -1;
//      List__G!(T) _list;
//
//      this(List__G!(T) array)
//      {
//          _list = array;
//          //Console.WriteLine(String("inted with {0}"), BOX!(int)(index));
//          //writeln(_array.Items[index]);
//      }
//
//      public T  IEnumerator_T_Current()   @property
//      {
//
//
//          return _list[index];
//
//      }
//
//      void IDisposable_Dispose()
//      {
//          _list = null;
//      }
//
//
//      bool MoveNext(IEnumerator j=null)
//      {
//          index++;
//          if(index < _list.Count)
//              return true;
//          return false;
//      }
//
//      NObject Current(IEnumerator j=null) @property
//      {
//          return BOX!(T)(IEnumerator_T_Current); // BOX should be adjusted to just pass classes as is
//      }
//
//      void Reset(IEnumerator j=null)
//      {
//          index = -1;
//      }
//
//  }
//}
//
//
//
////module System.Collections.Generic.List__G;
//
//
////import System.Namespace;
////import System.Collections.Generic.Namespace;
////import System.Collections.Namespace;
//////import System.Collections.ObjectModel.Namespace;
//
//// class List__G( T ) :  NObject ,  IList__G!(T) ,  System.Collections.Namespace.IList
////{
//
////  Array_T!(T) _items = cast(Array_T!(T)) null;
////  int _size;
////  int _version;
////  static const int DefaultCapacity = 4;
//
////public void ICollection_T_Add(T item) 
////  {
//
////    if (this._size==_items.Length)
////    {
//
////      GrowIfNeeded(1);
//    
////}
////    this._items[this._size++]=item;
////    this._version++;
//  
////}
//
////void GrowIfNeeded(int newCount) 
////  {
//
////    int minimumSize = this._size+newCount;
////    if (minimumSize>_items.Length)
////    {
//
////      this.Capacity=Math.Max(Math.Max(this.Capacity*2, DefaultCapacity), minimumSize);
//    
////}
//  
////}
//
////void CheckRange(int idx, int count) 
////  {
//
////    if (idx<0)
////    {
//
////      throw  new ArgumentOutOfRangeException( (new String ("index")));
//    
////}
////    if (count<0)
////    {
//
////      throw  new ArgumentOutOfRangeException( (new String ("count")));
//    
////}
////    if ((cast(long)idx)+(cast(long)count)>(cast(long)this._size))
////    {
//
////      throw  new ArgumentException( (new String ("index and count exceed length of list")));
//    
////}
//  
////}
//
////void CheckRangeOutOfRange(int idx, int count) 
////  {
//
////    if (idx<0)
////    {
//
////      throw  new ArgumentOutOfRangeException( (new String ("index")));
//    
////}
////    if (count<0)
////    {
//
////      throw  new ArgumentOutOfRangeException( (new String ("count")));
//    
////}
////    if ((cast(long)idx)+(cast(long)count)>(cast(long)this._size))
////    {
//
////      throw  new ArgumentOutOfRangeException( (new String ("index and count exceed length of list")));
//    
////}
//  
////}
//
////void AddCollection(ICollection_T!(T) collection) 
////  {
//
////    int collectionCount = collection.ICollection_T_Count;
////    if (collectionCount==0)
////    {
//
////      return;
//    
////}
////    GrowIfNeeded(collectionCount);
////    collection.ICollection_T_CopyTo(_items, _size);
////    this._size+=collectionCount;
//  
////}
//
////void AddEnumerable(IEnumerable_T!(T) enumerable) 
////  {
//
////    /*foreach (t; enumerable)
////    {
//
////      ICollection_T_Add(t);
//    
////    }*/
//  
////}
//
////public void AddRange(IEnumerable_T!(T) collection) 
////  {
//
////    if (collection is null)
////    {
//
////      throw  new ArgumentNullException( (new String ("collection")));
//    
////}
////     ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
////    if (c !is null)
////    {
//
////      AddCollection(c);
//    
////}
////    else
////    {
//
////      AddEnumerable(collection);
//    
////}
////    this._version++;
//  
////}
//
//////public System.Collections.ObjectModel.Namespace.ReadOnlyCollection_T!(T) AsReadOnly() 
//////  {
//
//////    return ( new System.Collections.ObjectModel.Namespace.ReadOnlyCollection_T!(T)(this));
//  
//////}
//
////public int BinarySearch(T item) 
////  {
//
////    //return (Array.BinarySearch!( T )(_items, 0, _size, item));
////    return -1;
//  
////}
//
////public int BinarySearch(T item, IComparer_T!(T) comparer) 
////  {
//
////    //return (Array_T.BinarySearch!( T )(_items, 0, _size, item, comparer));
////    return -1;
//
//  
////}
//
////public int BinarySearch(int index, int count, T item, IComparer_T!(T) comparer) 
////  {
//
////   // CheckRange(index, count);
////    //return (Array_T.BinarySearch!( T )(_items, index, count, item, comparer));
////    return -1;
//  
////}
//
////public void ICollection_T_Clear() 
////  {
////    //_items = [];
////    //Array.Clear(_items, 0, _items.Length);
////    this._size=0;
////    this._version++;
//  
////}
//
////public bool ICollection_T_Contains(T item) 
////  {
//
////    //return (Array_T.IndexOf!( T )(_items, item, 0, _size)!=-1);
////    return false;
//  
////}
//
////public List__G!(TOutput) ConvertAll (  TOutput ) (Converter_TInput_TOutput!(T, TOutput) converter) 
////  {
//
////    if (converter is null)
////    {
//
////      throw  new ArgumentNullException( (new String ("converter")));
//    
////}
////     List__G!(TOutput)  u =  new List__G!(TOutput)(_size);
////    for (int i = 0;i<this._size;i++)
////      {
//
////                u._items[i]=converter(this._items[i]);
////      }
////      u._size=this._size;
////      return (u);
//    
////}
//
////public void CopyTo(Array_T!(T) array) 
////    {
//
////      //Array_T.Copy(_items, 0, array, 0, _size);
//    
////}
//
////public void ICollection_T_CopyTo(Array_T!(T) array, int arrayIndex) 
////    {
//
////      //Array_T.Copy(_items, 0, array, arrayIndex, _size);
//    
////}
//
////public void CopyTo(int index, Array_T!(T) array, int arrayIndex, int count) 
////    {
//
////      //CheckRange(index, count);
////      //Array_T.Copy(_items, index, array, arrayIndex, count);
//    
////}
//
////public bool Exists(Predicate!(T) csmatch) 
////    {
//
////      List__G!(T).CheckMatch(csmatch);
////      for (int i = 0;i<this._size;i++)
////        {
//
////                     T  item = this._items[i];
////          if (csmatch(item))
////          {
//
////            return (true);
//          
////}
////        }
////        return (false);
//      
////}
//
////public T Find(Predicate!(T) csmatch) 
////      {
//
////        List__G!(T).CheckMatch(csmatch);
////        for (int i = 0;i<this._size;i++)
////          {
//
////                         T  item = this._items[i];
////            if (csmatch(item))
////            {
//
////              return (item);
//            
////}
////          }
////          return cast(T)(null);
//        
////}
//
////static void CheckMatch(Predicate!(T) csmatch) 
////        {
//
////          if (csmatch is null)
////          {
//
////            throw  new ArgumentNullException( (new String ("match")));
//          
////}
//        
////}
//
////public List__G!(T) FindAll(Predicate!(T) csmatch) 
////        {
//
////          List__G!(T).CheckMatch(csmatch);
////          if (this._size<=0x10000)
////          {
//
////            return (this.FindAllStackBits(csmatch));
//          
////}
////          else
////          {
//
////            return (this.FindAllList(csmatch));
//          
////}
//        
////}
//
////private List__G!(T) FindAllStackBits(Predicate!(T) csmatch) 
////        {
/////*
////          //Unsafe
////          {
//
////            long* bits = malloc((this._size / 32) + 1);//stackalloc uint [(this._size / 32) + 1]//TODO: StackAlloc not supported yet;
////            long* ptr = bits;
////            int found = 0;
////            long bitmask = 0x80000000;
////            for (int i = 0;i<this._size;i++)
////              {
//
////                                if (csmatch(this._items[i]))
////                {
//
////                  (*ptr)=(*ptr)|bitmask;
////                  found++;
//                
////}
////                bitmask=bitmask>>1;
////                if (bitmask==0)
////                {
//
////                  ptr++;
////                  bitmask=0x80000000;
//                
////}
////              }
////               Array_T!(T)  results =  new Array_T!(T )(found);
////              bitmask=0x80000000;
////              ptr=bits;
////              int j = 0;
////              for (int i = 0;i<this._size&&j<found;i++)
////                {
//
////                                    if (((*ptr)&bitmask)==bitmask)
////                  {
//
////                    results[j++]=this._items[i];
//                  
////}
////                  bitmask=bitmask>>1;
////                  if (bitmask==0)
////                  {
//
////                    ptr++;
////                    bitmask=0x80000000;
//                  
////}
////                }
////                return ( new List__G!(T)(results, found));
//              
////}*/
////   return null;         
////}
//
////private List__G!(T) FindAllList(Predicate!(T) csmatch) 
////            {
//
////               List__G!(T)  results =  new List__G!(T)();
////              for (int i = 0;i<this._size;i++)
////                {
//
////                                    if (csmatch(this._items[i]))
////                  {
//
////                    results.ICollection_T_Add(this._items[i]);
//                  
////}
////                }
////                return (results);
//              
////}
//
////public int FindIndex(Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////               // return (Array_T.GetIndex!( T )(_items, 0, _size, csmatch));
////              return -1;
////}
//
////public int FindIndex(int startIndex, Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////                //CheckStartIndex(startIndex);
////                //return (Array_T.GetIndex!( T )(_items, startIndex, this._size-startIndex, csmatch));
////              return -1;
//              
////}
//
////public int FindIndex(int startIndex, int count, Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////                //CheckRangeOutOfRange(startIndex, count);
////                //return (Array_T.GetIndex!( T )(_items, startIndex, count, csmatch));
////              return -1;
//              
////}
//
////public T FindLast(Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////                //int i = Array_T.GetLastIndex!( T )(_items, 0, _size, csmatch);
////                //return ((i==-1) ? (null) : (this[i]));
////              return cast(T) null;
////}
//
////public int FindLastIndex(Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////                //return (Array_T.GetLastIndex!( T )(_items, 0, _size, csmatch));
////              return -1;
//              
////}
//
////public int FindLastIndex(int startIndex, Predicate!(T) csmatch) 
////              {
//
////                //List__G!(T).CheckMatch(csmatch);
////                //CheckStartIndex(startIndex);
////                //return (Array_T.GetLastIndex!( T )(_items, 0, startIndex+1, csmatch));
////              return -1;
//              
////}
//
////public int FindLastIndex(int startIndex, int count, Predicate!(T) csmatch) 
////              {
//
//////                List__G!(T).CheckMatch(csmatch);
//////                CheckStartIndex(startIndex);
//////                if (count<0)
//////                {
//
//////                  throw  new ArgumentOutOfRangeException( (new String ("count")));
//                
//////}
//////                if (startIndex-count+1<0)
//////                {
//
//////                  throw  new ArgumentOutOfRangeException( (new String ("count must refer to a location within the collection")));
//                
//////}
//////                return (Array_T.GetLastIndex!( T )(_items, startIndex-count+1, count, csmatch));
////              return -1;
//
//              
////}
//
////public void ForEach(Action__G!(T) action) 
////              {
//
////                if (action is null)
////                {
//
////                  throw  new ArgumentNullException( (new String ("action")));
//                
////}
////                for (int i = 0;i<this._size;i++)
////                  {
//
////                                        action(this._items[i]);
////                  }
//                
////}
//
////public List__G_EnumeratorT!(T) GetEnumerator() 
////{
//
////                  return new List__G_EnumeratorT!(T)(this);
//                
////}
//
////public List__G!(T) GetRange(int index, int count) 
////                {
//
////                  CheckRange(index, count);
////                   Array_T!(T)  tmpArray =  new Array_T!(T )(count);
////                  //Array_T.Copy(_items, index, tmpArray, 0, count);
////                  return ( new List__G!(T)(tmpArray, count));
//                
////}
//
////public int IList__G_IndexOf(T item) 
////                {
//
////                  //return (Array_T.IndexOf!( T )(_items, item, 0, _size));
////              return -1;
//                
////}
//
////public int IndexOf(T item, int index) 
////                {
//
////                  //CheckIndex(index);
////                  //return (Array_T.IndexOf!( T )(_items, item, index, this._size-index));
////              return -1;
//                
////}
//
////public int IndexOf(T item, int index, int count) 
////                {
//
//////                  if (index<0)
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("index")));
//                  
//////}
//////                  if (count<0)
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("count")));
//                  
//////}
//////                  if ((cast(long)index)+(cast(long)count)>(cast(long)this._size))
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("index and count exceed length of list")));
//                  
//////}
//////                  return (Array_T.IndexOf!( T )(_items, item, index, count));
//
////              return -1;
//
//                
////}
//
////void Shift(int start, int delta) 
////                {
//
////                  if (delta<0)
////                  {
//
////                    start-=delta;
//                  
////}
////                  if (start<this._size)
////                  {
//
////                    //Array_T.Copy(_items, start, _items, start+delta, this._size-start);
//                  
////}
////                  this._size+=delta;
////                  if (delta<0)
////                  {
//
////                    //Array_T.Clear(_items, _size, -delta);
//                  
////}
//                
////}
//
////void CheckIndex(int index) 
////                {
//
////                  if (index<0||(cast(long)index)>(cast(long)this._size))
////                  {
//
////                    throw  new ArgumentOutOfRangeException( (new String ("index")));
//                  
////}
//                
////}
//
////void CheckStartIndex(int index) 
////                {
//
////                  if (index<0||(cast(long)index)>(cast(long)this._size))
////                  {
//
////                    throw  new ArgumentOutOfRangeException( (new String ("startIndex")));
//                  
////}
//                
////}
//
////public void IList__G_Insert(int index, T item) 
////                {
//
////                  CheckIndex(index);
////                  if (this._size==_items.Length)
////                  {
//
////                    GrowIfNeeded(1);
//                  
////}
////                  Shift(index, 1);
////                  this._items[index]=item;
////                  this._version++;
//                
////}
//
////public void InsertRange(int index, IEnumerable_T!(T) collection) 
////                {
//
////                  if (collection is null)
////                  {
//
////                    throw  new ArgumentNullException( (new String ("collection")));
//                  
////}
////                  CheckIndex(index);
////                  if (collection==this)
////                  {
//
////                     Array_T!(T)  buffer =  new Array_T!(T )(_size);
////                    ICollection_T_CopyTo(buffer, 0);
////                    GrowIfNeeded(_size);
////                    Shift(index, buffer.Length);
////                    //Array_T.Copy(buffer, 0, _items, index, buffer.Length);
//                  
////}
////                  else
////                  {
//
////                     ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
////                    if (c !is null)
////                    {
//
////                      InsertCollection(index, c);
//                    
////}
////                    else
////                    {
//
////                      InsertEnumeration(index, collection);
//                    
////}
//                  
////}
////                  this._version++;
//                
////}
//
////void InsertCollection(int index, ICollection_T!(T) collection) 
////                {
//
////                  //int collectionCount = collection.ICollection_T_Count;
////                  //GrowIfNeeded(collectionCount);
////                  //Shift(index, collectionCount);
////                  //collection.ICollection_T_CopyTo(_items, index);
//                
////}
//
////void InsertEnumeration(int index, IEnumerable_T!(T) enumerable) 
////                {
//
//////                  foreach (t; enumerable)
//////                  {
//
//////                                        IList__G_Insert(index++, t);
//                  
//////}
//                
////}
//
////public int LastIndexOf(T item) 
////                {
//
//////                  if (this._size==0)
//////                  {
//
//////                    return (-1);
//                  
//////}
//////                  return (Array_T.LastIndexOf!( T )(_items, item, this._size-1, _size));
////              return -1;
//
//                
////}
//
////public int LastIndexOf(T item, int index) 
////                {
//
////                  //CheckIndex(index);
////                  //return (Array_T.LastIndexOf!( T )(_items, item, index, index+1));
////              return -1;
//
//                
////}
//
////public int LastIndexOf(T item, int index, int count) 
////                {
//
//////                  if (index<0)
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("index")), index,  (new String ("index is negative")));
//                  
//////}
//////                  if (count<0)
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("count")), count,  (new String ("count is negative")));
//                  
//////}
//////                  if (index-count+1<0)
//////                  {
//
//////                    throw  new ArgumentOutOfRangeException( (new String ("cound")), count,  (new String ("count is too large")));
//                  
//////}
//////                  return (Array_T.LastIndexOf!( T )(_items, item, index, count));
////              return -1;
//
//                
////}
//
////public bool ICollection_T_Remove(T item) 
////                {
//
////                  int loc = IList__G_IndexOf(item);
////                  if (loc!=-1)
////                  {
//
////                    IList__G_RemoveAt(loc);
//                  
////}
////                  return (loc!=-1);
//                
////}
//
////public int RemoveAll(Predicate!(T) csmatch) 
////                {
//
////                  List__G!(T).CheckMatch(csmatch);
////                  int i = 0;
////                  int j = 0;
////                  for (i=0;i<this._size;i++)
////                    {
//
////                                            if (csmatch(this._items[i]))
////                      {
//
////                        break;
//                      
////}
////                    }
////                    if (i==this._size)
////                    {
//
////                      return (0);
//                    
////}
////                    this._version++;
////                    for (j=i+1;j<this._size;j++)
////                      {
//
////                                                if (!csmatch(this._items[j]))
////                        {
//
////                          this._items[i++]=this._items[j];
//                        
////}
////                      }
////                      if (j-i>0)
////                      {
//
////                        //Array_T.Clear(_items, i, j-i);
//                      
////}
////                      this._size=i;
////                      return ((j-i));
//                    
////}
//
////public void IList__G_RemoveAt(int index) 
////                    {
//
////                      if (index<0||(cast(long)index)>=(cast(long)this._size))
////                      {
//
////                        throw  new ArgumentOutOfRangeException( (new String ("index")));
//                      
////}
////                      Shift(index, -1);
////                      //Array_T.Clear(_items, _size, 1);
////                      this._version++;
//                    
////}
//
////public void RemoveRange(int index, int count) 
////                    {
//
////                      CheckRange(index, count);
////                      if (count>0)
////                      {
//
////                        Shift(index, -count);
////                        //Array_T.Clear(_items, _size, count);
////                        this._version++;
//                      
////}
//                    
////}
//
////public void Reverse() 
////                    {
//
////                      //Array_T.Reverse(_items, 0, _size);
////                      this._version++;
//                    
////}
//
////public void Reverse(int index, int count) 
////                    {
//
////                      CheckRange(index, count);
////                      //Array_T.Reverse(_items, index, count);
////                      this._version++;
//                    
////}
//
////public void Sort() 
////                    {
//
////                      //Array_T.Sort!( T )(_items, 0, _size);
////                      this._version++;
//                    
////}
//
////public void Sort(IComparer_T!(T) comparer) 
////                    {
//
////                      //Array_T.Sort!( T )(_items, 0, _size, comparer);
////                      this._version++;
//                    
////}
//
////public void Sort(Comparison!(T) comparison) 
////                    {
//
////                      if (comparison is null)
////                      {
//
////                        throw  new ArgumentNullException( (new String ("comparison")));
//                      
////}
////                      //Array_T.SortImpl!( T )(_items, _size, comparison);
////                      this._version++;
//                    
////}
//
////public void Sort(int index, int count, IComparer_T!(T) comparer) 
////                    {
//
////                      CheckRange(index, count);
////                      //Array_T.Sort!( T )(_items, index, count, comparer);
////                      this._version++;
//                    
////}
//
////public Array_T!(T) ToArray() 
////                    {
//
////                       Array_T!(T)  t =  new Array_T!(T )(_size);
////                      //Array_T.Copy(_items, t, _size);
////                      return (t);
//                    
////}
//
////public void TrimExcess() 
////                    {
//
////                      this.Capacity=this._size;
//                    
////}
//
////public bool TrueForAll(Predicate!(T) csmatch) 
////                    {
//
////                      List__G!(T).CheckMatch(csmatch);
////                      for (int i = 0;i<this._size;i++)
////                        {
//
////                                                    if (!csmatch(this._items[i]))
////                          {
//
////                            return (false);
//                          
////}
////                        }
////                        return (true);
//                      
////}
//                      
//
////public                        int  Capacity() @property                      {
//
////                        {
//
////                          return (_items.Length);
//                        
////}
//                      
////}
//
////public                       void Capacity( int  value ) @property                      {
//
////                        {
//
////                          if ((cast(long)value)<(cast(long)this._size))
////                          {
//
////                            throw  new ArgumentOutOfRangeException();
//                          
////}
////                          //Array_T.Resize!( T )(_items, value);
//                        
////}
//                      
////}
//
//                      
//
////public                        int  ICollection_T_Count() @property                      {
//
////                        {
//
////                          return (_size);
//                        
////}
//                      
////}
//
//                      
//
////public                        T  opIndex( int index )                       {
//
////                        {
//
////                          if ((cast(long)index)>=(cast(long)this._size))
////                          {
//
////                            throw  new ArgumentOutOfRangeException( (new String ("index")));
//                          
////}
////                          return _items[index];
//                        
////}
//                      
////}
//
////public                       void opIndexAssign( T  value, int index )                       {
//
////                        {
//
////                          if ((cast(long)index)>=(cast(long)this._size))
////                          {
//
////                            throw  new ArgumentOutOfRangeException( (new String ("index")));
//                          
////}
////                          this._items[index]=value;
////                          this._version++;
//                        
////}
//                      
////}
//
//
////IEnumerator_T!(T) IEnumerable_T_GetEnumerator() 
////                      {
//
////                        return (GetEnumerator());
//                      
////}
//
////void ICollection_CopyTo(Array_T!(T) array, int arrayIndex) 
////                      {
//
////                       /* if (array is null)
////                        {
//
////                          throw  new ArgumentNullException( (new String ("array")));
//                        
////}
////                        if (array.Rank>1||array.GetLowerBound(0)!=0)
////                        {
//
////                          throw  new ArgumentException( (new String ("Array must be zero based and single dimentional")),  (new String ("array")));
//                        
////}
////                        Array_T.Copy(_items, 0, array, arrayIndex, _size);*/
//                      
////}
//
////System.Collections.Namespace.IEnumerator IEnumerable_GetEnumerator() 
////                      {
//
////                        return (GetEnumerator());
//                      
////}
//
////int IList_Add(NObject item) 
////                      {
//
////                        try
////                        {
//
////                          ICollection_T_Add(AsCast!( T )(item));
////                          return (this._size-1);
//                        
////}
////                        catch(
////                        NullReferenceException __ex)                        {
//
//                        
////}
////                        catch(
////                        InvalidCastException __ex)                        {
//
//                        
////}
////                        throw  new ArgumentException( (new String ("item")));
//                      
////}
//
////bool IList_Contains(NObject item) 
////                      {
//
////                        try
////                        {
//
////                          return (ICollection_T_Contains(AsCast!( T )(item)));
//                        
////}
////                        catch(
////                        NullReferenceException __ex)                        {
//
//                        
////}
////                        catch(
////                        InvalidCastException __ex)                        {
//
//                        
////}
////                        return (false);
//                      
////}
//
////int IList_IndexOf(NObject item) 
////                      {
//
////                        try
////                        {
//
////                          return (IList__G_IndexOf(AsCast!( T )(item)));
//                        
////}
////                        catch(
////                        NullReferenceException __ex)                        {
//
//                        
////}
////                        catch(
////                        InvalidCastException __ex)                        {
//
//                        
////}
////                        return (-1);
//                      
////}
//
////void IList_Insert(int index, NObject item) 
////                      {
//
////                        CheckIndex(index);
////                        try
////                        {
//
////                          IList__G_Insert(index, AsCast!( T )(item));
////                          return;
//                        
////}
////                        catch(
////                        NullReferenceException __ex)                        {
//
//                        
////}
////                        catch(
////                        InvalidCastException __ex)                        {
//
//                        
////}
////                        throw  new ArgumentException( (new String ("item")));
//                      
////}
//
////void IList_Remove(NObject item) 
////                      {
//
////                        try
////                        {
//
////                          ICollection_T_Remove(AsCast!( T )(item));
////                          return;
//                        
////}
////                        catch(
////                        NullReferenceException __ex)                        {
//
//                        
////}
////                        catch(
////                        InvalidCastException __ex)                        {
//
//                        
////}
//                      
////}
//                      
//
////                       bool  ICollection_T_IsReadOnly() @property                      {
//
////                        {
//
////                          return (false);
//                        
////}
//                      
////}
//
//                      
//
////                       bool  ICollection_IsSynchronized() @property                      {
//
////                        {
//
////                          return (false);
//                        
////}
//                      
////}
//
//                      
//
////                       NObject  ICollection_SyncRoot() @property                      {
//
////                        {
//
////                          return (this);
//                        
////}
//                      
////}
//
//                      
//
////                       bool  IList_IsFixedSize() @property                      {
//
////                        {
//
////                          return (false);
//                        
////}
//                      
////}
//
//                      
//
////                       bool  IList_IsReadOnly() @property                      {
//
////                        {
//
////                          return (false);
//                        
////}
//                      
////}
//
//                      
//
//////                       NObject  opIndex( int index )                       {
//
//////                        {
//
//////                          return (this[index]);
//                        
//////}
//                      
//////}
//
////                      void opIndexAssign( NObject  value, int index )                       {
//
////                        {
//
////                          try
////                          {
//
////                            this[index]=AsCast!( T )(value);
////                            return;
//                          
////}
////                          catch(
////                          NullReferenceException __ex)                          {
//
//                          
////}
////                          catch(
////                          InvalidCastException __ex)                          {
//
//                          
////}
////                          throw  new ArgumentException( (new String ("value")));
//                        
////}
//                      
////}
//
//
//
////public this()
////                      {
//
////                        this._items=EmptyArray_T!(T).Value;
//                      
////}
//
//
////public this(IEnumerable_T!(T) collection)
////                      {
//
////                        if (collection is null)
////                        {
//
////                          throw  new ArgumentNullException( (new String ("collection")));
//                        
////}
////                         ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
////                        if (c is null)
////                        {
//
////                          this._items=EmptyArray_T!(T).Value;
////                          AddEnumerable(collection);
//                        
////}
////                        else
////                        {
//
////                          this._size=c.ICollection_T_Count;
////                          this._items= new Array_T!(T )(_size);
////                          c.ICollection_T_CopyTo(_items, 0);
//                        
////}
//                      
////}
//
//
////public this(int capacity)
////                      {
//
////                        if (capacity<0)
////                        {
//
////                          throw  new ArgumentOutOfRangeException( (new String ("capacity")));
//                        
////}
////                        this._items= new Array_T!(T )(capacity);
//                      
////}
//
//
////public this(Array_T!(T) data, int size)
////                      {
//
////                        this._items=data;
////                        this._size=size;
//                      
////}
//
////};