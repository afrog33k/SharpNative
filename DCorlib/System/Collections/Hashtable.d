module System.Collections.Hashtable;


import System.Namespace;
import System.Collections.Namespace;

 class Hashtable :  NObject  //,  IDictionary , ICloneable
{

	NObject[NObject] _items;

	final  NObject opIndexAssign(NObject value, NObject index, IDictionary j=null)  {
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		_items[index] = value;
		return _items[index];
	}
	
	final  ref NObject opIndex(NObject index, IDictionary j=null) { //TODO: ref could be a bad idea 
		//but allows alot of natural c# syntax
		//if (index >= _items.length)
		//	throw new ArgumentOutOfRangeException(new String("index"));
		
		return _items[index];
	}

//  Array_T!(System.Collections.Namespace.Hashtable_Entry) _buckets = cast(Array_T!(System.Collections.Namespace.Hashtable_Entry)) null;
//  int _numberOfBuckets;
//  int _count;
//  int _loadFactor;
//  int _maxLoadFactor;
//  double _growthFactor;
//  static const int _defaultCapacity = 4;
//  static const int _defaultLoadFactor = 2;

//private void InitializeHashTable(int capacity, int maxLoadFactor) 
//  {
//    this._buckets= new Array_T!(Hashtable_Entry )(capacity);
//    this._numberOfBuckets=capacity;
//    this._maxLoadFactor=maxLoadFactor;
//    this._growthFactor=2;
//  }
  

//public    int  MaxLoadFactor() @property  {
//    {
//      return (_maxLoadFactor);
//    }
//  }

//public   void MaxLoadFactor( int  value ) @property  {
//    {
//      this._maxLoadFactor=value;
//    }
//  }

  

//public    double  GrowthFactor() @property  {
//    {
//      return (_growthFactor);
//    }
//  }

//public   void GrowthFactor( double  value ) @property  {
//    {
//      this._growthFactor=value;
//    }
//  }


//private void Add( ref Array_T!(System.Collections.Namespace.Hashtable_Entry) buckets, NObject key, NObject value, bool overwrite) 
//  {
//    int whichBucket = Hash(key, _numberOfBuckets);
//     Hashtable_Entry  csmatch = EntryForKey(key, buckets[whichBucket]);
//    if (csmatch !is null&&overwrite)
//    {
//      csmatch.value=value;
//      return;
//    }
//    else if ((csmatch !is null&&!overwrite))
//    {
//      throw ( new ArgumentException( (new String ("key exists"))));
//    }
//    else
//    {
//       Hashtable_Entry  newOne = ( new Hashtable_Entry(key, value, buckets[whichBucket]));
//      buckets[whichBucket]=newOne;
//      this._count++;
//    }
//    this._loadFactor=this._count/this._numberOfBuckets;
//  }

//private int Hash(NObject key, int numOfBuckets) 
//  {
//    int hashcode = key.GetHashCode();
//    if (hashcode<0)
//    {
//      hashcode=hashcode*-1;
//    }
//    return (hashcode%numOfBuckets);
//  }

//private Hashtable_Entry EntryForKey(NObject key, Hashtable_Entry head) 
//  {
//    for (Hashtable_Entry cur = head;cur !is null;cur=cur.next)
//      {
//                if (cur.key.Equals(key))
//        {
//          return (cur);
//        }
//      }
//      return (null);
//    }

//private void Rehash(int newSize) 
//    {
//       Array_T!(System.Collections.Namespace.Hashtable_Entry)  newTable =  new Array_T!(Hashtable_Entry )(newSize);
//      this._numberOfBuckets=newSize;
//      this._count=0;
//      for (int i = 0;i<_buckets.Length;i++)
//        {
//                    if (this._buckets[i] !is null)
//          {
//            for (Hashtable_Entry cur = this._buckets[i];cur !is null;cur=cur.next)
//              {
//                                Add(newTable, cur.key, cur.value, false);
//              }
//            }
//          }
//          this._buckets=newTable;
//        }

//private void CopyToCollection(Array_T!(NObject) array, int index, Hashtable_EnumeratorType cstype) 
//        {
//          if (index<0&&index>this._numberOfBuckets)
//          {
//            throw ( new IndexOutOfRangeException( (new String ("index"))));
//          }
//          int j = 0;
//          int len = array.Length;
//          for (int i = index;i<this._numberOfBuckets;i++)
//            {
//                            for (Hashtable_Entry cur = this._buckets[i];cur !is null&&j<len;cur=cur.next)
//                {
//                                    if (cstype==Hashtable_EnumeratorType.KEY)
//                  {
//                    (AsCast!( System.Collections.Namespace.IList )(array))[j]=cur.key;
//                  }
//                  else
//                  {
//                    (AsCast!( System.Collections.Namespace.IList )(array))[j]=cur.value;
//                  }
//                  j++;
//                }
//              }
//            }

//public NObject Clone() 
//            {
//               Hashtable  ht = ( new Hashtable());
//              ht.InitializeHashTable(_numberOfBuckets, _maxLoadFactor);
//              ht._count=this._count;
//              ht._loadFactor=this._loadFactor;
//              Array_T.Copy(_buckets, ht._buckets, _numberOfBuckets);
//              return (ht);
//            }
            

//public              int  Count() @property            {
//              {
//                return (_count);
//              }
//            }

            

//public              bool  IsSynchronized() @property            {
//              {
//                return (false);
//              }
//            }

            

//public              NObject  SyncRoot() @property            {
//              {
//                return (this);
//              }
//            }


//public void CopyTo(Array_T!(NObject) array, int index) 
//            {
//              if (index<0&&index>_buckets.Length)
//              {
//                throw ( new IndexOutOfRangeException( (new String ("index"))));
//              }
//              int j = 0;
//              int len = array.Length;
//              for (int i = index;i<_buckets.Length;i++)
//                {
//                                    for (Hashtable_Entry cur = this._buckets[i];cur !is null&&j<len;cur=cur.next)
//                    {
//                                            (AsCast!( System.Collections.Namespace.IList )(array))[j]=BOX!( DictionaryEntry )((DictionaryEntry(cur.key, cur.value)));
//                      j++;
//                    }
//                  }
//                }
                

//public                  bool  IsReadOnly() @property                {
//                  {
//                    return (false);
//                  }
//                }

                

//public                  bool  IsFixedSize() @property                {
//                  {
//                    return (false);
//                  }
//                }

                

//public                  ICollection  Keys() @property                {
//                  {
//                    return (( new Hashtable_KeyCollection(this)));
//                  }
//                }

                

//public                  ICollection  Values() @property                {
//                  {
//                    return (( new Hashtable_ValueCollection(this)));
//                  }
//                }

                

//public                  NObject  opIndex( NObject key )                 {
//                  {
//                    if (key is null)
//                    {
//                      throw ( new ArgumentNullException( (new String ("key is null"))));
//                    }
//                    int whichBucket = Hash(key, _numberOfBuckets);
//                     Hashtable_Entry  csmatch = EntryForKey(key, this._buckets[whichBucket]);
//                    if (csmatch !is null)
//                    {
//                      return (csmatch.value);
//                    }
//                    return (null);
//                  }
//                }

//public                 void opIndexAssign( NObject  value, NObject key )                 {
//                  {
//                    if (key is null)
//                    {
//                      throw ( new ArgumentNullException( (new String ("key is null"))));
//                    }
//                    Add(_buckets, key, value, true);
//                    if (this._loadFactor>=this._maxLoadFactor)
//                    {
//                      Rehash((cast(int)(this._numberOfBuckets*this._growthFactor)));
//                    }
//                  }
//                }


//public void Add(NObject key, NObject value) 
//                {
//                  if (key is null)
//                  {
//                    throw ( new ArgumentNullException( (new String ("key is null"))));
//                  }
//                  Add(_buckets, key, value, false);
//                  if (this._loadFactor>=this._maxLoadFactor)
//                  {
//                    Rehash((cast(int)(this._numberOfBuckets*this._growthFactor)));
//                  }
//                }

//public void Clear() 
//                {
//                  this._buckets= new Array_T!(Hashtable_Entry )(System.Collections.Namespace.Hashtable._defaultCapacity);
//                  this._numberOfBuckets=Hashtable._defaultCapacity;
//                  this._loadFactor=0;
//                  this._count=0;
//                }

//public bool Contains(NObject key) 
//                {
//                  if (key is null)
//                  {
//                    throw ( new ArgumentNullException( (new String ("key is null"))));
//                  }
//                  int whichBucket = Hash(key, _numberOfBuckets);
//                   Hashtable_Entry  csmatch = EntryForKey(key, this._buckets[whichBucket]);
//                  if (csmatch !is null)
//                  {
//                    return (true);
//                  }
//                  return (false);
//                }

//public void Remove(NObject key) 
//                {
//                  if (key is null)
//                  {
//                    throw ( new ArgumentNullException( (new String ("key is null"))));
//                  }
//                  int whichBucket = Hash(key, _numberOfBuckets);
//                   Hashtable_Entry  csmatch = EntryForKey(key, this._buckets[whichBucket]);
//                  if (csmatch is null)
//                  {
//                    return;
//                  }
//                  if (this._buckets[whichBucket]==csmatch)
//                  {
//                    this._buckets[whichBucket]=csmatch.next;
//                    this._count--;
//                    return;
//                  }
//                  for (Hashtable_Entry cur = this._buckets[whichBucket];cur !is null;cur=cur.next)
//                    {
//                                            if (cur.next==csmatch)
//                      {
//                        cur.next=csmatch.next;
//                        this._count--;
//                        return;
//                      }
//                    }
//                  }


//public this()
//                  {
//                    InitializeHashTable(System.Collections.Namespace.Hashtable._defaultCapacity, System.Collections.Namespace.Hashtable._defaultLoadFactor);
//                  }


//public this(int capacity)
//                  {
//                    InitializeHashTable(capacity, System.Collections.Namespace.Hashtable._defaultLoadFactor);
//                  }


//public this(int capacity, int maxLoadFactor)
//                  {
//                    InitializeHashTable(capacity, maxLoadFactor);
//                  }

};