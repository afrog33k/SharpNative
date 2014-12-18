module System.Collections.Generic.Dictionary_TKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
//import System.Runtime.Serialization.Namespace;
//import CsRoot.Namespace;
//import System.Runtime.InteropServices.Namespace;

 class Dictionary_TKey_TValue( TKey , TValue ) :  NObject ,  IDictionary_TKey_TValue!(TKey, TValue) //,  System.Collections.Namespace.IDictionary ,  System.Runtime.Serialization.Namespace.ISerializable ,  System.Runtime.Serialization.Namespace.IDeserializationCallback
{

  static const int INITIAL_SIZE = 4;
  static const float DEFAULT_LOAD_FACTOR = (90f/100);
  static const int NO_SLOT = -1;
  static const int HASH_FLAG = -2147483648;
  Array_T!(int) table = cast(Array_T!(int)) null;
  Array_T!(System.Collections.Generic.Namespace.Link) linkSlots = cast(Array_T!(System.Collections.Generic.Namespace.Link)) null;
  Array_T!(TKey) keySlots = cast(Array_T!(TKey)) null;
  Array_T!(TValue) valueSlots = cast(Array_T!(TValue)) null;
  IEqualityComparer_T!(TKey) hcp = cast(IEqualityComparer_T!(TKey)) null;
  System.Runtime.Serialization.Namespace.SerializationInfo serialization_info = cast(System.Runtime.Serialization.Namespace.SerializationInfo) null;
  int touchedSlots;
  int emptySlot;
  int count;
  int threshold;
  int generation;
  

public    int  ICollection_T_Count() @property  {

    {

      return (count);
    
}
  
}

  

public    TValue  opIndex( TKey key )   {

    {

      if (key is null)
      {

        throw  new ArgumentNullException( (new String ("key")));
      
}
      int hashCode = cast(int)(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
      int cur = this.table[(hashCode&System.Namespace.Int32.MaxValue)%table.Length]-1;
      while (cur!=NO_SLOT)
      {

        if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
        {

          return (this.valueSlots[cur]);
        
}
        cur=this.linkSlots[cur].Next;
      
}
      throw  new KeyNotFoundException();
    
}
  
}

public   void opIndexAssign( TValue  value, TKey key )   {

    {

      if (key is null)
      {

        throw  new ArgumentNullException( (new String ("key")));
      
}
      int hashCode = cast(int)(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
      int index = (hashCode&System.Namespace.Int32.MaxValue)%table.Length;
      int cur = this.table[index]-1;
      int prev = System.Collections.Generic.Namespace.Dictionary.NO_SLOT;
      if (cur!=NO_SLOT)
      {

        do
        {

          if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
          {

            break;
          
}
          prev=cur;
          cur=this.linkSlots[cur].Next;
        
}
        while (cur!=NO_SLOT);
      
}
      if (cur==NO_SLOT)
      {

        if (++this.count>this.threshold)
        {

          Resize();
          index=(hashCode&System.Namespace.Int32.MaxValue)%table.Length;
        
}
        cur=this.emptySlot;
        if (cur==NO_SLOT)
        {

          cur=this.touchedSlots++;
        
}
        else
        {

          this.emptySlot=this.linkSlots[cur].Next;
        
}
        this.linkSlots[cur].Next=this.table[index]-1;
        this.table[index]=cur+1;
        this.linkSlots[cur].HashCode=hashCode;
        this.keySlots[cur]=key;
      
}
      else
      {

        if (prev!=NO_SLOT)
        {

          this.linkSlots[prev].Next=this.linkSlots[cur].Next;
          this.linkSlots[cur].Next=this.table[index]-1;
          this.table[index]=cur+1;
        
}
      
}
      this.valueSlots[cur]=value;
      this.generation++;
    
}
  
}


void Init(int capacity, IEqualityComparer_T!(TKey) hcp) 
  {

    this.hcp=((hcp)!is null?(hcp):(EqualityComparer_T!(TKey).Default));
    capacity=Math.Max(1, (capacity/DEFAULT_LOAD_FACTOR));
    InitArrays(capacity);
  
}

void InitArrays(int size) 
  {

    this.table= new Array_T!(int )(size);
    this.linkSlots= new Array_T!(Link )(size);
    this.emptySlot=NO_SLOT;
    this.keySlots= new Array_T!(TKey )(size);
    this.valueSlots= new Array_T!(TValue )(size);
    this.touchedSlots=0;
    this.threshold=(table.Length*DEFAULT_LOAD_FACTOR);
    if (this.threshold==0&&table.Length>0)
    {

      this.threshold=1;
    
}
  
}

void CopyToCheck(Array_T array, int index) 
  {

    if (array is null)
    {

      throw  new ArgumentNullException( (new String ("array")));
    
}
    if (index<0)
    {

      throw  new ArgumentOutOfRangeException( (new String ("index")));
    
}
    if (index>array.Length)
    {

      throw  new ArgumentException( (new String ("index larger than largest valid index of array")));
    
}
    if (array.Length-index<this.ICollection_T_Count)
    {

      throw  new ArgumentException( (new String ("Destination array cannot hold the requested elements!")));
    
}
  
}

void CopyKeys(Array_T!(TKey) array, int index) 
  {

    for (int i = 0;i<this.touchedSlots;i++)
      {

                if ((this.linkSlots[i].HashCode&HASH_FLAG)!=0)
        {

          array[index++]=this.keySlots[i];
        
}
      }
    
}

void CopyValues(Array_T!(TValue) array, int index) 
    {

      for (int i = 0;i<this.touchedSlots;i++)
        {

                    if ((this.linkSlots[i].HashCode&HASH_FLAG)!=0)
          {

            array[index++]=this.valueSlots[i];
          
}
        }
      
}

static KeyValuePair_TKey_TValue!(TKey, TValue) make_pair(TKey key, TValue value) 
      {

        return (KeyValuePair_TKey_TValue!(TKey, TValue)(key, value));
      
}

static TKey pick_key(TKey key, TValue value) 
      {

        return (key);
      
}

static TValue pick_value(TKey key, TValue value) 
      {

        return (value);
      
}

void CopyTo(Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) array, int index) 
      {

        CopyToCheck(array, index);
        for (int i = 0;i<this.touchedSlots;i++)
          {

                        if ((this.linkSlots[i].HashCode&HASH_FLAG)!=0)
            {

              array[index++]=KeyValuePair_TKey_TValue!(TKey, TValue)(this.keySlots[i], this.valueSlots[i]);
            
}
          }
        
}

void Do_ICollectionCopyTo (  TRet ) (Array_T array, int index, Dictionary_TKey_TValue_Transform_TRetTKey_TValue!(TRet) transform) 
        {

           Type  src = new System.Type(classOf[TRet]);
           Type  tgt = array.GetType()._Type_GetElementType();
          try
          {

            if ((src._Type_IsPrimitive||tgt._Type_IsPrimitive)&&!tgt._Type_IsAssignableFrom(src))
            {

              throw  new NException();
            
}
             Array_T!(System.Namespace.NObject)  dest = AsCast!( Array_T!(System.Namespace.NObject) )(array);
            for (int i = 0;i<this.touchedSlots;i++)
              {

                                if ((this.linkSlots[i].HashCode&HASH_FLAG)!=0)
                {

                  dest[index++]=transform(this.keySlots[i], this.valueSlots[i]);
                
}
              }
            
}
            catch(
            NException e)            {

                throw  new ArgumentException( (new String ("Cannot copy source collection elements to destination array")),  (new String ("array")), e);
            
}
          
}

private void Resize() 
          {

            int newSize = System.Collections.Namespace.HashPrimeNumbers.ToPrime((table.Length<<1)|1);
             Array_T!(int)  newTable =  new Array_T!(int )(newSize);
             Array_T!(System.Collections.Generic.Namespace.Link)  newLinkSlots =  new Array_T!(Link )(newSize);
            for (int i = 0;i<table.Length;i++)
              {

                                int cur = this.table[i]-1;
                while (cur!=NO_SLOT)
                {

                  int hashCode = newLinkSlots[cur].HashCode=hcp.IEqualityComparer_T_GetHashCode(this.keySlots[cur])|HASH_FLAG;
                  int index = (hashCode&System.Namespace.Int32.MaxValue)%newSize;
                  newLinkSlots[cur].Next=newTable[index]-1;
                  newTable[index]=cur+1;
                  cur=this.linkSlots[cur].Next;
                
}
              }
              this.table=newTable;
              this.linkSlots=newLinkSlots;
               Array_T!(TKey)  newKeySlots =  new Array_T!(TKey )(newSize);
               Array_T!(TValue)  newValueSlots =  new Array_T!(TValue )(newSize);
              Array_T.Copy(keySlots, 0, newKeySlots, 0, touchedSlots);
              Array_T.Copy(valueSlots, 0, newValueSlots, 0, touchedSlots);
              this.keySlots=newKeySlots;
              this.valueSlots=newValueSlots;
              this.threshold=(newSize*DEFAULT_LOAD_FACTOR);
            
}

public void IDictionary_TKey_TValue_Add(TKey key, TValue value) 
            {

              if (key is null)
              {

                throw  new ArgumentNullException( (new String ("key")));
              
}
              int hashCode = cast(int)(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
              int index = (hashCode&System.Namespace.Int32.MaxValue)%table.Length;
              int cur = this.table[index]-1;
              while (cur!=NO_SLOT)
              {

                if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
                {

                  throw  new ArgumentException( (new String ("An element with the same key already exists in the dictionary.")));
                
}
                cur=this.linkSlots[cur].Next;
              
}
              if (++this.count>this.threshold)
              {

                Resize();
                index=(hashCode&System.Namespace.Int32.MaxValue)%table.Length;
              
}
              cur=this.emptySlot;
              if (cur==NO_SLOT)
              {

                cur=this.touchedSlots++;
              
}
              else
              {

                this.emptySlot=this.linkSlots[cur].Next;
              
}
              this.linkSlots[cur].HashCode=hashCode;
              this.linkSlots[cur].Next=this.table[index]-1;
              this.table[index]=cur+1;
              this.keySlots[cur]=key;
              this.valueSlots[cur]=value;
              this.generation++;
            
}
            

public              IEqualityComparer_T!(TKey)  Comparer() @property            {

              {

                return (hcp);
              
}
            
}


public void ICollection_T_Clear() 
            {

              if (this.count==0)
              {

                return;
              
}
              this.count=0;
              Array_T.Clear(table, 0, table.Length);
              Array_T.Clear(keySlots, 0, keySlots.Length);
              Array_T.Clear(valueSlots, 0, valueSlots.Length);
              Array_T.Clear(linkSlots, 0, linkSlots.Length);
              this.emptySlot=NO_SLOT;
              this.touchedSlots=0;
              this.generation++;
            
}

public bool IDictionary_TKey_TValue_ContainsKey(TKey key) 
            {

              if (key is null)
              {

                throw  new ArgumentNullException( (new String ("key")));
              
}
              int hashCode = cast(int)(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
              int cur = this.table[(hashCode&System.Namespace.Int32.MaxValue)%table.Length]-1;
              while (cur!=NO_SLOT)
              {

                if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
                {

                  return (true);
                
}
                cur=this.linkSlots[cur].Next;
              
}
              return (false);
            
}

public bool ContainsValue(TValue value) 
            {

               IEqualityComparer_T!(TValue)  cmp = EqualityComparer_T!(TValue).Default;
              for (int i = 0;i<table.Length;i++)
                {

                                    int cur = this.table[i]-1;
                  while (cur!=NO_SLOT)
                  {

                    if (cmp.IEqualityComparer_T_Equals(this.valueSlots[cur], value))
                    {

                      return (true);
                    
}
                    cur=this.linkSlots[cur].Next;
                  
}
                }
                return (false);
              
}

public void ISerializable_GetObjectData(System.Runtime.Serialization.Namespace.SerializationInfo info, System.Runtime.Serialization.Namespace.StreamingContext context) 
              {

                if (info is null)
                {

                  throw  new ArgumentNullException( (new String ("info")));
                
}
                info.AddValue( (new String ("Version")), generation);
                info.AddValue( (new String ("Comparer")), hcp);
                 Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))  data =  new Array_T!(KeyValuePair_TKey_TValue!(TKey, TValue) )(count);
                if (this.count>0)
                {

                  CopyTo(data, 0);
                
}
                info.AddValue( (new String ("HashSize")), table.Length);
                info.AddValue( (new String ("KeyValuePairs")), data);
              
}

public void IDeserializationCallback_OnDeserialization(NObject sender) 
              {

                if (this.serialization_info is null)
                {

                  return;
                
}
                int hashSize = 0;
                 Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))  data = null;
                 System.Runtime.Serialization.Namespace.SerializationInfoEnumerator  e = serialization_info.GetEnumerator();
                while (e.IEnumerator_MoveNext())
                {

                  switch( e.Name.Text )
                  {

                    case  (new String ("Version")).Text                    :
                      this.generation=UNBOX!(int)(e.Value);
break;                    case  (new String ("Comparer")).Text                    :
                      this.hcp=AsCast!( System.Collections.Generic.Namespace.IEqualityComparer_T!(TKey) )(e.Value);
break;                    case  (new String ("HashSize")).Text                    :
                      hashSize=UNBOX!(int)(e.Value);
break;                    case  (new String ("KeyValuePairs")).Text                    :
                      data=AsCast!( Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) )(e.Value);
break;                    default:
break;

                  
}
                
}
                if (this.hcp is null)
                {

                  this.hcp=EqualityComparer_T!(TKey).Default;
                
}
                if (hashSize<INITIAL_SIZE)
                {

                  hashSize=INITIAL_SIZE;
                
}
                InitArrays(hashSize);
                this.count=0;
                if (data !is null)
                {

                  for (int i = 0;i<data.Length;++i)
                    {

                                            IDictionary_TKey_TValue_Add(data[i].Key, data[i].Value);
                    }
                  
}
                  this.generation++;
                  this.serialization_info=null;
                
}

public bool IDictionary_TKey_TValue_Remove(TKey key) 
                {

                  if (key is null)
                  {

                    throw  new ArgumentNullException( (new String ("key")));
                  
}
                  int hashCode = cast( int )(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
                  int index = (hashCode&System.Namespace.Int32.MaxValue)%table.Length;
                  int cur = this.table[index]-1;
                  if (cur==NO_SLOT)
                  {

                    return (false);
                  
}
                  int prev = System.Collections.Generic.Namespace.Dictionary.NO_SLOT;
                  do
                  {

                    if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
                    {

                      break;
                    
}
                    prev=cur;
                    cur=this.linkSlots[cur].Next;
                  
}
                  while (cur!=NO_SLOT);
                  if (cur==NO_SLOT)
                  {

                    return (false);
                  
}
                  this.count--;
                  if (prev==NO_SLOT)
                  {

                    this.table[index]=this.linkSlots[cur].Next+1;
                  
}
                  else
                  {

                    this.linkSlots[prev].Next=this.linkSlots[cur].Next;
                  
}
                  this.linkSlots[cur].Next=this.emptySlot;
                  this.emptySlot=cur;
                  this.linkSlots[cur].HashCode=0;
                  this.keySlots[cur]=null;
                  this.valueSlots[cur]=null;
                  this.generation++;
                  return (true);
                
}

public bool IDictionary_TKey_TValue_TryGetValue(TKey key,  out TValue value) 
                {

                  if (key is null)
                  {

                    throw  new ArgumentNullException( (new String ("key")));
                  
}
                  int hashCode = cast(int)(hcp.IEqualityComparer_T_GetHashCode(key)|HASH_FLAG);
                  int cur = this.table[(hashCode&System.Namespace.Int32.MaxValue)%table.Length]-1;
                  while (cur!=NO_SLOT)
                  {

                    if (this.linkSlots[cur].HashCode==hashCode&&hcp.IEqualityComparer_T_Equals(this.keySlots[cur], key))
                    {

                      value=this.valueSlots[cur];
                      return (true);
                    
}
                    cur=this.linkSlots[cur].Next;
                  
}
                  value=null;
                  return (false);
                
}
                

                 ICollection_T!(TKey)  IDictionary_TKey_TValue_Keys() @property                {

                  {

                    return (Keys);
                  
}
                
}

                

                 ICollection_T!(TValue)  IDictionary_TKey_TValue_Values() @property                {

                  {

                    return (Values);
                  
}
                
}

                

public                  Dictionary_TKey_TValue_KeyCollectionTKey_TValue!(TKey)  Keys() @property                {

                  {

                    return ( new Dictionary_TKey_TValue_KeyCollectionTKey_TValue!(TKey)(this));
                  
}
                
}

                

public                  Dictionary_TKey_TValue_ValueCollectionTKey_TValue!(TValue)  Values() @property                {

                  {

                    return ( new Dictionary_TKey_TValue_ValueCollectionTKey_TValue!(TValue)(this));
                  
}
                
}

                

                 System.Collections.Namespace.ICollection  IDictionary_Keys() @property                {

                  {

                    return (Keys);
                  
}
                
}

                

                 System.Collections.Namespace.ICollection  IDictionary_Values() @property                {

                  {

                    return (Values);
                  
}
                
}

                

                 bool  IDictionary_IsFixedSize() @property                {

                  {

                    return (false);
                  
}
                
}

                

                 bool  IDictionary_IsReadOnly() @property                {

                  {

                    return (false);
                  
}
                
}


static TKey ToTKey(NObject key) 
                {

                  if (key is null)
                  {

                    throw  new ArgumentNullException( (new String ("key")));
                  
}
                  if (!((IsCast!( TKey )(key))))
                  {

                    throw  new ArgumentException( (new String ("not of type: "))+new System.Type(classOf[TKey])._Type_ToString(),  (new String ("key")));
                  
}
                  return (AsCast!( TKey )(key));
                
}

static TValue ToTValue(NObject value) 
                {

                  if (value is null&&!new System.Type(classOf[TValue])._Type_IsValueType)
                  {

                    return (null);
                  
}
                  if (!((IsCast!( TValue )(value))))
                  {

                    throw  new ArgumentException( (new String ("not of type: "))+new System.Type(classOf[TValue])._Type_ToString(),  (new String ("value")));
                  
}
                  return (AsCast!( TValue )(value));
                
}
                

                 NObject  opIndex( NObject key )                 {

                  {

                     TValue  obj = cast(TValue) null;
                    if ((IsCast!( TKey )(key))&&IDictionary_TKey_TValue_TryGetValue(AsCast!( TKey )(key), obj))
                    {

                      return (obj);
                    
}
                    return (null);
                  
}
                
}

                void opIndexAssign( NObject  value, NObject key )                 {

                  {

                    this[Dictionary_TKey_TValue!(TKey, TValue).ToTKey(key)]=Dictionary_TKey_TValue!(TKey, TValue).ToTValue(value);
                  
}
                
}


void IDictionary_Add(NObject key, NObject value) 
                {

                  this.IDictionary_TKey_TValue_Add(Dictionary_TKey_TValue!(TKey, TValue).ToTKey(key), Dictionary_TKey_TValue!(TKey, TValue).ToTValue(value));
                
}

bool IDictionary_Contains(NObject key) 
                {

                  if (key is null)
                  {

                    throw  new ArgumentNullException( (new String ("key")));
                  
}
                  if ((IsCast!( TKey )(key)))
                  {

                    return (IDictionary_TKey_TValue_ContainsKey(AsCast!( TKey )(key)));
                  
}
                  return (false);
                
}

void IDictionary_Remove(NObject key) 
                {

                  if (key is null)
                  {

                    throw  new ArgumentNullException( (new String ("key")));
                  
}
                  if ((IsCast!( TKey )(key)))
                  {

                    IDictionary_TKey_TValue_Remove(AsCast!( TKey )(key));
                  
}
                
}
                

                 bool  ICollection_IsSynchronized() @property                {

                  {

                    return (false);
                  
}
                
}

                

                 NObject  ICollection_SyncRoot() @property                {

                  {

                    return (this);
                  
}
                
}

                

                 bool  ICollection_T_IsReadOnly() @property                {

                  {

                    return (false);
                  
}
                
}


void ICollection_T_Add(KeyValuePair_TKey_TValue!(TKey, TValue) keyValuePair) 
                {

                  IDictionary_TKey_TValue_Add(keyValuePair.Key, keyValuePair.Value);
                
}

bool ICollection_T_Contains(KeyValuePair_TKey_TValue!(TKey, TValue) keyValuePair) 
                {

                  return (ContainsKeyValuePair(keyValuePair));
                
}

void ICollection_T_CopyTo(Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) array, int index) 
                {

                  this.CopyTo(array, index);
                
}

bool ICollection_T_Remove(KeyValuePair_TKey_TValue!(TKey, TValue) keyValuePair) 
                {

                  if (!ContainsKeyValuePair(keyValuePair))
                  {

                    return (false);
                  
}
                  return (IDictionary_TKey_TValue_Remove(keyValuePair.Key));
                
}

bool ContainsKeyValuePair(KeyValuePair_TKey_TValue!(TKey, TValue) pair) 
                {

                   TValue  value = cast(TValue) null;
                  if (!IDictionary_TKey_TValue_TryGetValue(pair.Key, value))
                  {

                    return (false);
                  
}
                  return (EqualityComparer_T!(TValue).Default.IEqualityComparer_T_Equals(pair.Value, value));
                
}

void ICollection_CopyTo(Array_T array, int index) 
                {

                   Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))  pairs = cast( Array_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) )(array);
                  if (pairs !is null)
                  {

                    this.CopyTo(pairs, index);
                    return;
                  
}
                  CopyToCheck(array, index);
                   Array_T!(System.Collections.Namespace.DictionaryEntry)  entries = cast( Array_T!(System.Collections.Namespace.DictionaryEntry) )(array);
                  if (entries !is null)
                  {

                    for (int i = 0;i<this.touchedSlots;i++)
                      {

                                                if ((this.linkSlots[i].HashCode&HASH_FLAG)!=0)
                        {

                          entries[index++]=System.Collections.Namespace.DictionaryEntry(this.keySlots[i], this.valueSlots[i]);
                        
}
                      }
                      return;
                    
}
                    Do_ICollectionCopyTo!( KeyValuePair_TKey_TValue!(TKey, TValue) )(array, index, new Dictionary_TKey_TValue_Transform_TRetTKey_TValue!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))(__ToDelegate(&System.Collections.Generic.Namespace.Dictionary.make_pair)));
                  
}

System.Collections.Namespace.IEnumerator IEnumerable_GetEnumerator() 
                  {

                    return (Dictionary_TKey_TValue_EnumeratorTKey_TValue(this));
                  
}

IEnumerator_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) IEnumerable_T_GetEnumerator() 
                  {

                    return (Dictionary_TKey_TValue_EnumeratorTKey_TValue(this));
                  
}

System.Collections.Namespace.IDictionaryEnumerator IDictionary_GetEnumerator() 
                  {

                    return ( new Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue(this));
                  
}

public Dictionary_TKey_TValue_EnumeratorTKey_TValue GetEnumerator() 
                  {

                    return (Dictionary_TKey_TValue_EnumeratorTKey_TValue(this));
                  
}


public this()
                  {

                    Init(System.Collections.Generic.Namespace.Dictionary.INITIAL_SIZE, null);
                  
}


public this(IEqualityComparer_T!(TKey) comparer)
                  {

                    Init(System.Collections.Generic.Namespace.Dictionary.INITIAL_SIZE, comparer);
                  
}


public this(IDictionary_TKey_TValue!(TKey, TValue) dictionary)
                  {

                    this(dictionary, null);
                  
}


public this(int capacity)
                  {

                    this(capacity, null);
                  
}


public this(IDictionary_TKey_TValue!(TKey, TValue) dictionary, IEqualityComparer_T!(TKey) comparer)
                  {

                    if (dictionary is null)
                    {

                      throw  new ArgumentNullException( (new String ("dictionary")));
                    
}
                    Init(dictionary.ICollection_T_Count, comparer);
                    foreach (entry; dictionary)
                    {

                                            this.IDictionary_TKey_TValue_Add(entry.Key, entry.Value);
                    
}
                  
}


public this(int capacity, IEqualityComparer_T!(TKey) comparer)
                  {

                    if (capacity<0)
                    {

                      throw  new ArgumentOutOfRangeException( (new String ("capacity")));
                    
}
                    Init(capacity, comparer);
                  
}


public this(System.Runtime.Serialization.Namespace.SerializationInfo info, System.Runtime.Serialization.Namespace.StreamingContext context)
                  {

                    this.serialization_info=info;
                  
}

};