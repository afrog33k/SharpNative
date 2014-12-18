module System.Collections.Generic.List_T;
import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Generic.IList_T;


private class List_T_Enumerator_T(T):IEnumerator_T!(T)
  {
    int index = -1;
    List_T!(T) _list;

    this(List_T!(T) array)
    {
      _list = array;
      //Console.WriteLine(_S("inted with {0}"), BOX!(int)(index));
      //writeln(_array.Items[index]);
    }

    public T  IEnumerator_T_Current()   @property
    {
    

      return _list[index];
     
    }

    void IDisposable_Dispose()
    {
      _list = null;
    }


    bool IEnumerator_MoveNext()
    {
      index++;
      if(index < _list.Count)
        return true;
      return false;
    }

    NObject IEnumerator_Current() @property
    {
      return BOX!(T)(IEnumerator_T_Current); // BOX should be adjusted to just pass classes as is
    }

    void IEnumerator_Reset()
    {
      index = -1;
    }

  }

/**
 * Represents a list of elements that can be accessed by index.
 */
class List_T(T) : IList_T!(T) {
  
  private enum DEFAULT_CAPACITY = 4;
  
  private T[] _items;
  private int size_;
  
  private int index_;
  
  /**
   * Initializes a new instance with the specified _capacity.
   * Params: capacity = The number of elements the new list can store.
   */
  this(int capacity = 0) {
    _items.length = capacity;
  }
  
  /**
   * Initializes a new instance containing elements copied from the specified _range.
   * Params: range = The _range whose elements are copied to the new list.
   */
  this(T[] range) {
    _items.length = size_ = cast(int)range.length;
    _items = range;
  }
  
  /**
   * ditto
   */
  this(IEnumerable_T!(T) range) {
    _items.length = DEFAULT_CAPACITY;
      
      auto __foreachIter2 = range.IEnumerable_T_GetEnumerator();
      while(__foreachIter2.IEnumerator_MoveNext())
      {
        ICollection_T_Add(__foreachIter2.IEnumerator_T_Current);
       
      }

    //foreach (item; range)
    //  ICollection_T_Add(item);
  }
  
  /**
   * Adds an element to the end of the list.
   * Params: item = The element to be added.
   */
  final void ICollection_T_Add(T item) {
    if (size_ == _items.length)
      EnsureCapacity(size_ + 1);
    _items[size_++] = item;
  }
  
  /**
   * Adds the elements in the specified _range to the end of the list.
   * Params: The _range whose elements are to be added.
   */
  final void AddRange(T[] range) {
    InsertRange(size_, range);
  }
  
  /**
   * ditto
   */
  final void AddRange(IEnumerable_T!(T) range) {
    InsertRange(size_, range);
  }
  
  /**
   * Inserts an element into the list at the specified _index.
   * Params:
   *   index = The _index at which item should be inserted.
   *   item = The element to insert.
   */
  final void Insert(int index, T item) {
    if (size_ == _items.length)
      EnsureCapacity(size_ + 1);

    if (index < size_)
      .Copy(_items, index, _items, index + 1, size_ - index);
    
    _items[index] = item;
    size_++;
  }
  
  /**
   * Inserts the elements of a _range into the list at the specified _index.
   * Params:
   *   index = The _index at which the new elements should be inserted.
   *   range = The _range whose elements should be inserted into the list.
   */
  final void InsertRange(int index, T[] range) {
    foreach (item; range) {
      Insert(index++, item);
    }
  }
  
  /**
   * ditto
   */
  final void InsertRange(int index, IEnumerable_T!(T) range) {
    //foreach (item; range) {
    //  Insert(index++, item);
    //}
      auto __foreachIter2 = range.IEnumerable_T_GetEnumerator();
      while(__foreachIter2.IEnumerator_MoveNext())
      {
        Insert(index++,__foreachIter2.IEnumerator_T_Current);
       
      }
  }
  
  /**
   */
  final bool Remove(T item) {
    int index = IndexOf(item);
    
    if (index < 0)
      return false;

    RemoveAt(index);
    return true;
  }

  final void RemoveAt(int index) {
    size_--;
    if (index < size_)
      .Copy(_items, index + 1, _items, index, size_ - index);
    _items[size_] = T.init;
  }

  /**
   */
  final void RemoveRange(int index, int count) {
    if (count > 0) {
      size_ -= count;
      if (index < size_)
        .Copy(_items, index + count, _items, index, size_ - index);
      .Clear(_items, size_, count);
    }
  }
  
  /**
   */
  final bool Contains(T item) {
    for (auto i = 0; i < size_; i++) {
      if (EqualityComparisonImpl(_items[i], item))
        return true;
    }
    return false;
  }
  
IEnumerator IEnumerable_GetEnumerator()
{
  return new List_T_Enumerator_T!(T)(this);
}

IEnumerator_T!(T) IEnumerable_T_GetEnumerator()
{
  return new List_T_Enumerator_T!(T)(this);
}

 int ICollection_T_Count()
  {
    return cast(int)_items.length;
  }

bool ICollection_T_IsReadOnly() 
{
  return false;
}

bool ICollection_T_Contains(T item)
{
  return Contains(item);;
}

void IList_T_RemoveAt(int index)
{
  return RemoveAt(index);
}

void IList_T_Insert(int index, T item)
{
  Insert(index,item);
}

int IList_T_IndexOf(T item)
{
  return IndexOf(item);
}

void ICollection_T_CopyTo(Array_T!T array, int arrayIndex)
{
  //CopyTo(array,arrayIndex);
}


bool ICollection_T_Remove(T item)
{
  return Remove(item);
}

  void ICollection_T_Clear()
  {
    Clear();
  }
  /**
   */
  final void Clear() {
    if (size_ > 0) {
      .Clear(_items, 0, size_);
      size_ = 0;
    }
  }
  
  /**
   */
  final int IndexOf(T item) {
    return IndexOf(item, null);
  }
  
  /**
   */
  final int IndexOf(T item, EqualityComparison!(T) comparison) {
    if (comparison is null) {
      comparison = (T a, T b) {
        return EqualityComparisonImpl(a, b);
      };
    }
    
    for (auto i = 0; i < size_; i++) {
      if (comparison(_items[i], item))
        return i;
    }
    
    return -1;
  }
  
  /**
   */
  final int LastIndexOf(T item, EqualityComparison!(T) comparison = null) {
    if (comparison is null) {
      comparison = (T a, T b) {
        return EqualityComparisonImpl(a, b);
      };
    }
    
    for (auto i = size_ - 1; i >= 0; i--) {
      if (comparison(_items[i], item))
        return i;
    }
    
    return -1;
  }
  
  /**
   */
  final void Sort(Comparison!(T) comparison = null) {
    .Sort(_items, 0, size_, comparison);
  }
  
  /**
   */
  final int BinarySearch(T item, Comparison!(T) comparison = null) {
    return .BinarySearch(_items, 0, size_, item, comparison);
  }
  
  /**
   */
  final void CopyTo(T[] array) {
    .Copy(_items, 0, array, 0, size_);
  }
  
  /**
   */
  final T[] ToArray() {
    return _items[0 .. size_].dup;
  }
  
  /**
   */
  final T Find(Predicate!(T) match) {
    for (auto i = 0; i < size_; i++) {
      if (match(_items[i]))
        return _items[i];
    }
    return T.init;
  }
  
  /**
   */
  final T FindLast(Predicate!(T) match) {
    for (auto i = size_ - 1; i >= 0; i--) {
      if (match(_items[i]))
        return _items[i];
    }
    return T.init;
  }
  
  /**
   */
  final List_T FindAll(Predicate!(T) match) {
    auto list = new List_T;
    for (auto i = 0; i < size_; i++) {
      if (match(_items[i]))
        list.ICollection_T_Add(_items[i]);
    }
    return list;
  }
  
  /**
   */
  final int FindIndex(Predicate!(T) match) {
    for (auto i = 0; i < size_; i++) {
      if (match(_items[i]))
        return i;
    }
    return -1;
  }
  
  /**
   */
  final int FindLastIndex(Predicate!(T) match) {
    for (auto i = size_ - 1; i >= 0; i--) {
      if (match(_items[i]))
        return i;
    }
    return -1;
  }
  
  /**
   */
  final bool Exists(Predicate!(T) match) {
    return FindIndex(match) != -1;
  }
  
  /**
   */
  final void ForEach(Action_T!(T) action) {
    for (auto i = 0; i < size_; i++) {
      action(_items[i]);
    }
  }
  
  /**
   */
  final bool TrueForAll(Predicate!(T) match) {
    for (auto i = 0; i < size_; i++) {
      if (!match(_items[i]))
        return false;
    }
    return true;
  }
  
  /**
   */
  final List_T!(T) GetRange(int index, int count) {
    auto list = new List_T!(T)(count);
    list._items[0 .. count] = _items[index .. index + count];
    list.size_ = count;
    return list;
  }
  
  /**
   */
  final List!(TOutput) Convert(TOutput)(Converter!(T, TOutput) converter) {
    auto list = new List!(TOutput)(size_);
    for (auto i = 0; i < size_; i++) {
      list._items[i] = converter(_items[i]);
    }
    list.size_ = size_;
    return list;
  }
  
  final int Count() {
    return size_;
  }
  
  final @property void Capacity(int value) {
    _items.length = value;
  }
  final @property int Capacity() {
    return cast(int)_items.length;
  }
  
  final void opIndexAssign(T value, int index) {
    if (index >= size_)
      throw new ArgumentOutOfRangeException(new String("index"));
    
    _items[index] = value;
  }
  final T opIndex(int index) {
    if (index >= size_)
      throw new ArgumentOutOfRangeException(new String("index"));
    
    return _items[index];
  }
  
  version (UseRanges) {
    final bool empty() {
      bool result = (index_ == size_);
      if (result)
        index_ = 0;
      return result;
    }
    
    final void popFront() {
      if (index_ < size_)
        index_++;
    }
    
    final T front() {
      return _items[index_];
    }
  }
  else {
    final int opApply(int delegate(ref T) action) {
      int r;
      
      for (auto i = 0; i < size_; i++) {
        if ((r = action(_items[i])) != 0)
          break;
      }
      
      return r;
    }
    
    /**
     * Ditto
     */
    final int opApply(int delegate(ref int, ref T) action) {
      int r;
      
      for (auto i = 0; i < size_; i++) {
        if ((r = action(i, _items[i])) != 0)
          break;
      }
      
      return r;
    }
  }
  
  final bool opIn_r(T item) {
    return Contains(item);
  }
  
  private void EnsureCapacity(int min) {
    if (_items.length < min) {
      int n = cast(int)((_items.length == 0) ? DEFAULT_CAPACITY : _items.length * 2);
      if (n < min)
        n = min;
      this.Capacity = n;
    }
  }
  
}



//module System.Collections.Generic.List_T;


//import System.Namespace;
//import System.Collections.Generic.Namespace;
//import System.Collections.Namespace;
////import System.Collections.ObjectModel.Namespace;

// class List_T( T ) :  NObject ,  IList_T!(T) ,  System.Collections.Namespace.IList
//{

//  Array_T!(T) _items = cast(Array_T!(T)) null;
//  int _size;
//  int _version;
//  static const int DefaultCapacity = 4;

//public void ICollection_T_Add(T item) 
//  {

//    if (this._size==_items.Length)
//    {

//      GrowIfNeeded(1);
    
//}
//    this._items[this._size++]=item;
//    this._version++;
  
//}

//void GrowIfNeeded(int newCount) 
//  {

//    int minimumSize = this._size+newCount;
//    if (minimumSize>_items.Length)
//    {

//      this.Capacity=Math.Max(Math.Max(this.Capacity*2, DefaultCapacity), minimumSize);
    
//}
  
//}

//void CheckRange(int idx, int count) 
//  {

//    if (idx<0)
//    {

//      throw  new ArgumentOutOfRangeException( (new String ("index")));
    
//}
//    if (count<0)
//    {

//      throw  new ArgumentOutOfRangeException( (new String ("count")));
    
//}
//    if ((cast(long)idx)+(cast(long)count)>(cast(long)this._size))
//    {

//      throw  new ArgumentException( (new String ("index and count exceed length of list")));
    
//}
  
//}

//void CheckRangeOutOfRange(int idx, int count) 
//  {

//    if (idx<0)
//    {

//      throw  new ArgumentOutOfRangeException( (new String ("index")));
    
//}
//    if (count<0)
//    {

//      throw  new ArgumentOutOfRangeException( (new String ("count")));
    
//}
//    if ((cast(long)idx)+(cast(long)count)>(cast(long)this._size))
//    {

//      throw  new ArgumentOutOfRangeException( (new String ("index and count exceed length of list")));
    
//}
  
//}

//void AddCollection(ICollection_T!(T) collection) 
//  {

//    int collectionCount = collection.ICollection_T_Count;
//    if (collectionCount==0)
//    {

//      return;
    
//}
//    GrowIfNeeded(collectionCount);
//    collection.ICollection_T_CopyTo(_items, _size);
//    this._size+=collectionCount;
  
//}

//void AddEnumerable(IEnumerable_T!(T) enumerable) 
//  {

//    /*foreach (t; enumerable)
//    {

//      ICollection_T_Add(t);
    
//    }*/
  
//}

//public void AddRange(IEnumerable_T!(T) collection) 
//  {

//    if (collection is null)
//    {

//      throw  new ArgumentNullException( (new String ("collection")));
    
//}
//     ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
//    if (c !is null)
//    {

//      AddCollection(c);
    
//}
//    else
//    {

//      AddEnumerable(collection);
    
//}
//    this._version++;
  
//}

////public System.Collections.ObjectModel.Namespace.ReadOnlyCollection_T!(T) AsReadOnly() 
////  {

////    return ( new System.Collections.ObjectModel.Namespace.ReadOnlyCollection_T!(T)(this));
  
////}

//public int BinarySearch(T item) 
//  {

//    //return (Array.BinarySearch!( T )(_items, 0, _size, item));
//    return -1;
  
//}

//public int BinarySearch(T item, IComparer_T!(T) comparer) 
//  {

//    //return (Array_T.BinarySearch!( T )(_items, 0, _size, item, comparer));
//    return -1;

  
//}

//public int BinarySearch(int index, int count, T item, IComparer_T!(T) comparer) 
//  {

//   // CheckRange(index, count);
//    //return (Array_T.BinarySearch!( T )(_items, index, count, item, comparer));
//    return -1;
  
//}

//public void ICollection_T_Clear() 
//  {
//    //_items = [];
//    //Array.Clear(_items, 0, _items.Length);
//    this._size=0;
//    this._version++;
  
//}

//public bool ICollection_T_Contains(T item) 
//  {

//    //return (Array_T.IndexOf!( T )(_items, item, 0, _size)!=-1);
//    return false;
  
//}

//public List_T!(TOutput) ConvertAll (  TOutput ) (Converter_TInput_TOutput!(T, TOutput) converter) 
//  {

//    if (converter is null)
//    {

//      throw  new ArgumentNullException( (new String ("converter")));
    
//}
//     List_T!(TOutput)  u =  new List_T!(TOutput)(_size);
//    for (int i = 0;i<this._size;i++)
//      {

//                u._items[i]=converter(this._items[i]);
//      }
//      u._size=this._size;
//      return (u);
    
//}

//public void CopyTo(Array_T!(T) array) 
//    {

//      //Array_T.Copy(_items, 0, array, 0, _size);
    
//}

//public void ICollection_T_CopyTo(Array_T!(T) array, int arrayIndex) 
//    {

//      //Array_T.Copy(_items, 0, array, arrayIndex, _size);
    
//}

//public void CopyTo(int index, Array_T!(T) array, int arrayIndex, int count) 
//    {

//      //CheckRange(index, count);
//      //Array_T.Copy(_items, index, array, arrayIndex, count);
    
//}

//public bool Exists(Predicate!(T) csmatch) 
//    {

//      List_T!(T).CheckMatch(csmatch);
//      for (int i = 0;i<this._size;i++)
//        {

//                     T  item = this._items[i];
//          if (csmatch(item))
//          {

//            return (true);
          
//}
//        }
//        return (false);
      
//}

//public T Find(Predicate!(T) csmatch) 
//      {

//        List_T!(T).CheckMatch(csmatch);
//        for (int i = 0;i<this._size;i++)
//          {

//                         T  item = this._items[i];
//            if (csmatch(item))
//            {

//              return (item);
            
//}
//          }
//          return cast(T)(null);
        
//}

//static void CheckMatch(Predicate!(T) csmatch) 
//        {

//          if (csmatch is null)
//          {

//            throw  new ArgumentNullException( (new String ("match")));
          
//}
        
//}

//public List_T!(T) FindAll(Predicate!(T) csmatch) 
//        {

//          List_T!(T).CheckMatch(csmatch);
//          if (this._size<=0x10000)
//          {

//            return (this.FindAllStackBits(csmatch));
          
//}
//          else
//          {

//            return (this.FindAllList(csmatch));
          
//}
        
//}

//private List_T!(T) FindAllStackBits(Predicate!(T) csmatch) 
//        {
///*
//          //Unsafe
//          {

//            long* bits = malloc((this._size / 32) + 1);//stackalloc uint [(this._size / 32) + 1]//TODO: StackAlloc not supported yet;
//            long* ptr = bits;
//            int found = 0;
//            long bitmask = 0x80000000;
//            for (int i = 0;i<this._size;i++)
//              {

//                                if (csmatch(this._items[i]))
//                {

//                  (*ptr)=(*ptr)|bitmask;
//                  found++;
                
//}
//                bitmask=bitmask>>1;
//                if (bitmask==0)
//                {

//                  ptr++;
//                  bitmask=0x80000000;
                
//}
//              }
//               Array_T!(T)  results =  new Array_T!(T )(found);
//              bitmask=0x80000000;
//              ptr=bits;
//              int j = 0;
//              for (int i = 0;i<this._size&&j<found;i++)
//                {

//                                    if (((*ptr)&bitmask)==bitmask)
//                  {

//                    results[j++]=this._items[i];
                  
//}
//                  bitmask=bitmask>>1;
//                  if (bitmask==0)
//                  {

//                    ptr++;
//                    bitmask=0x80000000;
                  
//}
//                }
//                return ( new List_T!(T)(results, found));
              
//}*/
//   return null;         
//}

//private List_T!(T) FindAllList(Predicate!(T) csmatch) 
//            {

//               List_T!(T)  results =  new List_T!(T)();
//              for (int i = 0;i<this._size;i++)
//                {

//                                    if (csmatch(this._items[i]))
//                  {

//                    results.ICollection_T_Add(this._items[i]);
                  
//}
//                }
//                return (results);
              
//}

//public int FindIndex(Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//               // return (Array_T.GetIndex!( T )(_items, 0, _size, csmatch));
//              return -1;
//}

//public int FindIndex(int startIndex, Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//                //CheckStartIndex(startIndex);
//                //return (Array_T.GetIndex!( T )(_items, startIndex, this._size-startIndex, csmatch));
//              return -1;
              
//}

//public int FindIndex(int startIndex, int count, Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//                //CheckRangeOutOfRange(startIndex, count);
//                //return (Array_T.GetIndex!( T )(_items, startIndex, count, csmatch));
//              return -1;
              
//}

//public T FindLast(Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//                //int i = Array_T.GetLastIndex!( T )(_items, 0, _size, csmatch);
//                //return ((i==-1) ? (null) : (this[i]));
//              return cast(T) null;
//}

//public int FindLastIndex(Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//                //return (Array_T.GetLastIndex!( T )(_items, 0, _size, csmatch));
//              return -1;
              
//}

//public int FindLastIndex(int startIndex, Predicate!(T) csmatch) 
//              {

//                //List_T!(T).CheckMatch(csmatch);
//                //CheckStartIndex(startIndex);
//                //return (Array_T.GetLastIndex!( T )(_items, 0, startIndex+1, csmatch));
//              return -1;
              
//}

//public int FindLastIndex(int startIndex, int count, Predicate!(T) csmatch) 
//              {

////                List_T!(T).CheckMatch(csmatch);
////                CheckStartIndex(startIndex);
////                if (count<0)
////                {

////                  throw  new ArgumentOutOfRangeException( (new String ("count")));
                
////}
////                if (startIndex-count+1<0)
////                {

////                  throw  new ArgumentOutOfRangeException( (new String ("count must refer to a location within the collection")));
                
////}
////                return (Array_T.GetLastIndex!( T )(_items, startIndex-count+1, count, csmatch));
//              return -1;

              
//}

//public void ForEach(Action_T!(T) action) 
//              {

//                if (action is null)
//                {

//                  throw  new ArgumentNullException( (new String ("action")));
                
//}
//                for (int i = 0;i<this._size;i++)
//                  {

//                                        action(this._items[i]);
//                  }
                
//}

//public List_T_EnumeratorT!(T) GetEnumerator() 
//{

//                  return new List_T_EnumeratorT!(T)(this);
                
//}

//public List_T!(T) GetRange(int index, int count) 
//                {

//                  CheckRange(index, count);
//                   Array_T!(T)  tmpArray =  new Array_T!(T )(count);
//                  //Array_T.Copy(_items, index, tmpArray, 0, count);
//                  return ( new List_T!(T)(tmpArray, count));
                
//}

//public int IList_T_IndexOf(T item) 
//                {

//                  //return (Array_T.IndexOf!( T )(_items, item, 0, _size));
//              return -1;
                
//}

//public int IndexOf(T item, int index) 
//                {

//                  //CheckIndex(index);
//                  //return (Array_T.IndexOf!( T )(_items, item, index, this._size-index));
//              return -1;
                
//}

//public int IndexOf(T item, int index, int count) 
//                {

////                  if (index<0)
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("index")));
                  
////}
////                  if (count<0)
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("count")));
                  
////}
////                  if ((cast(long)index)+(cast(long)count)>(cast(long)this._size))
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("index and count exceed length of list")));
                  
////}
////                  return (Array_T.IndexOf!( T )(_items, item, index, count));

//              return -1;

                
//}

//void Shift(int start, int delta) 
//                {

//                  if (delta<0)
//                  {

//                    start-=delta;
                  
//}
//                  if (start<this._size)
//                  {

//                    //Array_T.Copy(_items, start, _items, start+delta, this._size-start);
                  
//}
//                  this._size+=delta;
//                  if (delta<0)
//                  {

//                    //Array_T.Clear(_items, _size, -delta);
                  
//}
                
//}

//void CheckIndex(int index) 
//                {

//                  if (index<0||(cast(long)index)>(cast(long)this._size))
//                  {

//                    throw  new ArgumentOutOfRangeException( (new String ("index")));
                  
//}
                
//}

//void CheckStartIndex(int index) 
//                {

//                  if (index<0||(cast(long)index)>(cast(long)this._size))
//                  {

//                    throw  new ArgumentOutOfRangeException( (new String ("startIndex")));
                  
//}
                
//}

//public void IList_T_Insert(int index, T item) 
//                {

//                  CheckIndex(index);
//                  if (this._size==_items.Length)
//                  {

//                    GrowIfNeeded(1);
                  
//}
//                  Shift(index, 1);
//                  this._items[index]=item;
//                  this._version++;
                
//}

//public void InsertRange(int index, IEnumerable_T!(T) collection) 
//                {

//                  if (collection is null)
//                  {

//                    throw  new ArgumentNullException( (new String ("collection")));
                  
//}
//                  CheckIndex(index);
//                  if (collection==this)
//                  {

//                     Array_T!(T)  buffer =  new Array_T!(T )(_size);
//                    ICollection_T_CopyTo(buffer, 0);
//                    GrowIfNeeded(_size);
//                    Shift(index, buffer.Length);
//                    //Array_T.Copy(buffer, 0, _items, index, buffer.Length);
                  
//}
//                  else
//                  {

//                     ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
//                    if (c !is null)
//                    {

//                      InsertCollection(index, c);
                    
//}
//                    else
//                    {

//                      InsertEnumeration(index, collection);
                    
//}
                  
//}
//                  this._version++;
                
//}

//void InsertCollection(int index, ICollection_T!(T) collection) 
//                {

//                  //int collectionCount = collection.ICollection_T_Count;
//                  //GrowIfNeeded(collectionCount);
//                  //Shift(index, collectionCount);
//                  //collection.ICollection_T_CopyTo(_items, index);
                
//}

//void InsertEnumeration(int index, IEnumerable_T!(T) enumerable) 
//                {

////                  foreach (t; enumerable)
////                  {

////                                        IList_T_Insert(index++, t);
                  
////}
                
//}

//public int LastIndexOf(T item) 
//                {

////                  if (this._size==0)
////                  {

////                    return (-1);
                  
////}
////                  return (Array_T.LastIndexOf!( T )(_items, item, this._size-1, _size));
//              return -1;

                
//}

//public int LastIndexOf(T item, int index) 
//                {

//                  //CheckIndex(index);
//                  //return (Array_T.LastIndexOf!( T )(_items, item, index, index+1));
//              return -1;

                
//}

//public int LastIndexOf(T item, int index, int count) 
//                {

////                  if (index<0)
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("index")), index,  (new String ("index is negative")));
                  
////}
////                  if (count<0)
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("count")), count,  (new String ("count is negative")));
                  
////}
////                  if (index-count+1<0)
////                  {

////                    throw  new ArgumentOutOfRangeException( (new String ("cound")), count,  (new String ("count is too large")));
                  
////}
////                  return (Array_T.LastIndexOf!( T )(_items, item, index, count));
//              return -1;

                
//}

//public bool ICollection_T_Remove(T item) 
//                {

//                  int loc = IList_T_IndexOf(item);
//                  if (loc!=-1)
//                  {

//                    IList_T_RemoveAt(loc);
                  
//}
//                  return (loc!=-1);
                
//}

//public int RemoveAll(Predicate!(T) csmatch) 
//                {

//                  List_T!(T).CheckMatch(csmatch);
//                  int i = 0;
//                  int j = 0;
//                  for (i=0;i<this._size;i++)
//                    {

//                                            if (csmatch(this._items[i]))
//                      {

//                        break;
                      
//}
//                    }
//                    if (i==this._size)
//                    {

//                      return (0);
                    
//}
//                    this._version++;
//                    for (j=i+1;j<this._size;j++)
//                      {

//                                                if (!csmatch(this._items[j]))
//                        {

//                          this._items[i++]=this._items[j];
                        
//}
//                      }
//                      if (j-i>0)
//                      {

//                        //Array_T.Clear(_items, i, j-i);
                      
//}
//                      this._size=i;
//                      return ((j-i));
                    
//}

//public void IList_T_RemoveAt(int index) 
//                    {

//                      if (index<0||(cast(long)index)>=(cast(long)this._size))
//                      {

//                        throw  new ArgumentOutOfRangeException( (new String ("index")));
                      
//}
//                      Shift(index, -1);
//                      //Array_T.Clear(_items, _size, 1);
//                      this._version++;
                    
//}

//public void RemoveRange(int index, int count) 
//                    {

//                      CheckRange(index, count);
//                      if (count>0)
//                      {

//                        Shift(index, -count);
//                        //Array_T.Clear(_items, _size, count);
//                        this._version++;
                      
//}
                    
//}

//public void Reverse() 
//                    {

//                      //Array_T.Reverse(_items, 0, _size);
//                      this._version++;
                    
//}

//public void Reverse(int index, int count) 
//                    {

//                      CheckRange(index, count);
//                      //Array_T.Reverse(_items, index, count);
//                      this._version++;
                    
//}

//public void Sort() 
//                    {

//                      //Array_T.Sort!( T )(_items, 0, _size);
//                      this._version++;
                    
//}

//public void Sort(IComparer_T!(T) comparer) 
//                    {

//                      //Array_T.Sort!( T )(_items, 0, _size, comparer);
//                      this._version++;
                    
//}

//public void Sort(Comparison!(T) comparison) 
//                    {

//                      if (comparison is null)
//                      {

//                        throw  new ArgumentNullException( (new String ("comparison")));
                      
//}
//                      //Array_T.SortImpl!( T )(_items, _size, comparison);
//                      this._version++;
                    
//}

//public void Sort(int index, int count, IComparer_T!(T) comparer) 
//                    {

//                      CheckRange(index, count);
//                      //Array_T.Sort!( T )(_items, index, count, comparer);
//                      this._version++;
                    
//}

//public Array_T!(T) ToArray() 
//                    {

//                       Array_T!(T)  t =  new Array_T!(T )(_size);
//                      //Array_T.Copy(_items, t, _size);
//                      return (t);
                    
//}

//public void TrimExcess() 
//                    {

//                      this.Capacity=this._size;
                    
//}

//public bool TrueForAll(Predicate!(T) csmatch) 
//                    {

//                      List_T!(T).CheckMatch(csmatch);
//                      for (int i = 0;i<this._size;i++)
//                        {

//                                                    if (!csmatch(this._items[i]))
//                          {

//                            return (false);
                          
//}
//                        }
//                        return (true);
                      
//}
                      

//public                        int  Capacity() @property                      {

//                        {

//                          return (_items.Length);
                        
//}
                      
//}

//public                       void Capacity( int  value ) @property                      {

//                        {

//                          if ((cast(long)value)<(cast(long)this._size))
//                          {

//                            throw  new ArgumentOutOfRangeException();
                          
//}
//                          //Array_T.Resize!( T )(_items, value);
                        
//}
                      
//}

                      

//public                        int  ICollection_T_Count() @property                      {

//                        {

//                          return (_size);
                        
//}
                      
//}

                      

//public                        T  opIndex( int index )                       {

//                        {

//                          if ((cast(long)index)>=(cast(long)this._size))
//                          {

//                            throw  new ArgumentOutOfRangeException( (new String ("index")));
                          
//}
//                          return _items[index];
                        
//}
                      
//}

//public                       void opIndexAssign( T  value, int index )                       {

//                        {

//                          if ((cast(long)index)>=(cast(long)this._size))
//                          {

//                            throw  new ArgumentOutOfRangeException( (new String ("index")));
                          
//}
//                          this._items[index]=value;
//                          this._version++;
                        
//}
                      
//}


//IEnumerator_T!(T) IEnumerable_T_GetEnumerator() 
//                      {

//                        return (GetEnumerator());
                      
//}

//void ICollection_CopyTo(Array_T!(T) array, int arrayIndex) 
//                      {

//                       /* if (array is null)
//                        {

//                          throw  new ArgumentNullException( (new String ("array")));
                        
//}
//                        if (array.Rank>1||array.GetLowerBound(0)!=0)
//                        {

//                          throw  new ArgumentException( (new String ("Array must be zero based and single dimentional")),  (new String ("array")));
                        
//}
//                        Array_T.Copy(_items, 0, array, arrayIndex, _size);*/
                      
//}

//System.Collections.Namespace.IEnumerator IEnumerable_GetEnumerator() 
//                      {

//                        return (GetEnumerator());
                      
//}

//int IList_Add(NObject item) 
//                      {

//                        try
//                        {

//                          ICollection_T_Add(AsCast!( T )(item));
//                          return (this._size-1);
                        
//}
//                        catch(
//                        NullReferenceException __ex)                        {

                        
//}
//                        catch(
//                        InvalidCastException __ex)                        {

                        
//}
//                        throw  new ArgumentException( (new String ("item")));
                      
//}

//bool IList_Contains(NObject item) 
//                      {

//                        try
//                        {

//                          return (ICollection_T_Contains(AsCast!( T )(item)));
                        
//}
//                        catch(
//                        NullReferenceException __ex)                        {

                        
//}
//                        catch(
//                        InvalidCastException __ex)                        {

                        
//}
//                        return (false);
                      
//}

//int IList_IndexOf(NObject item) 
//                      {

//                        try
//                        {

//                          return (IList_T_IndexOf(AsCast!( T )(item)));
                        
//}
//                        catch(
//                        NullReferenceException __ex)                        {

                        
//}
//                        catch(
//                        InvalidCastException __ex)                        {

                        
//}
//                        return (-1);
                      
//}

//void IList_Insert(int index, NObject item) 
//                      {

//                        CheckIndex(index);
//                        try
//                        {

//                          IList_T_Insert(index, AsCast!( T )(item));
//                          return;
                        
//}
//                        catch(
//                        NullReferenceException __ex)                        {

                        
//}
//                        catch(
//                        InvalidCastException __ex)                        {

                        
//}
//                        throw  new ArgumentException( (new String ("item")));
                      
//}

//void IList_Remove(NObject item) 
//                      {

//                        try
//                        {

//                          ICollection_T_Remove(AsCast!( T )(item));
//                          return;
                        
//}
//                        catch(
//                        NullReferenceException __ex)                        {

                        
//}
//                        catch(
//                        InvalidCastException __ex)                        {

                        
//}
                      
//}
                      

//                       bool  ICollection_T_IsReadOnly() @property                      {

//                        {

//                          return (false);
                        
//}
                      
//}

                      

//                       bool  ICollection_IsSynchronized() @property                      {

//                        {

//                          return (false);
                        
//}
                      
//}

                      

//                       NObject  ICollection_SyncRoot() @property                      {

//                        {

//                          return (this);
                        
//}
                      
//}

                      

//                       bool  IList_IsFixedSize() @property                      {

//                        {

//                          return (false);
                        
//}
                      
//}

                      

//                       bool  IList_IsReadOnly() @property                      {

//                        {

//                          return (false);
                        
//}
                      
//}

                      

////                       NObject  opIndex( int index )                       {

////                        {

////                          return (this[index]);
                        
////}
                      
////}

//                      void opIndexAssign( NObject  value, int index )                       {

//                        {

//                          try
//                          {

//                            this[index]=AsCast!( T )(value);
//                            return;
                          
//}
//                          catch(
//                          NullReferenceException __ex)                          {

                          
//}
//                          catch(
//                          InvalidCastException __ex)                          {

                          
//}
//                          throw  new ArgumentException( (new String ("value")));
                        
//}
                      
//}



//public this()
//                      {

//                        this._items=EmptyArray_T!(T).Value;
                      
//}


//public this(IEnumerable_T!(T) collection)
//                      {

//                        if (collection is null)
//                        {

//                          throw  new ArgumentNullException( (new String ("collection")));
                        
//}
//                         ICollection_T!(T)  c = cast( ICollection_T!(T) )(collection);
//                        if (c is null)
//                        {

//                          this._items=EmptyArray_T!(T).Value;
//                          AddEnumerable(collection);
                        
//}
//                        else
//                        {

//                          this._size=c.ICollection_T_Count;
//                          this._items= new Array_T!(T )(_size);
//                          c.ICollection_T_CopyTo(_items, 0);
                        
//}
                      
//}


//public this(int capacity)
//                      {

//                        if (capacity<0)
//                        {

//                          throw  new ArgumentOutOfRangeException( (new String ("capacity")));
                        
//}
//                        this._items= new Array_T!(T )(capacity);
                      
//}


//public this(Array_T!(T) data, int size)
//                      {

//                        this._items=data;
//                        this._size=size;
                      
//}

//};