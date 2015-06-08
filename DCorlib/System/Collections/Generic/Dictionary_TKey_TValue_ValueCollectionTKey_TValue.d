module System.Collections.Generic.Dictionary_TKey_TValue_ValueCollectionTKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

 class Dictionary_TKey_TValue_ValueCollectionTKey_TValue(TKey,TValue) :  NObject ,  ICollection_T!(TValue) ,  IEnumerable_T!(TValue) ,  System.Collections.Namespace.ICollection ,  System.Collections.Namespace.IEnumerable
{

  Dictionary_TKey_TValue!(TKey, TValue) dictionary = cast(Dictionary_TKey_TValue!(TKey, TValue)) null;

public void ICollection_T_CopyTo(Array_T!(TValue) array, int index) 
  {

    dictionary.CopyToCheck(array, index);
    dictionary.CopyValues(array, index);
  
}

public Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue GetEnumerator() 
  {

    return (Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue(dictionary));
  
}

void ICollection_T_Add(TValue item) 
  {

    throw  new NotSupportedException( ( String ("this is a read-only collection")));
  
}

void ICollection_T_Clear() 
  {

    throw  new NotSupportedException( ( String ("this is a read-only collection")));
  
}

bool ICollection_T_Contains(TValue item) 
  {

    return (dictionary.ContainsValue(item));
  
}

bool ICollection_T_Remove(TValue item) 
  {

    throw  new NotSupportedException( ( String ("this is a read-only collection")));
  
}

IEnumerator_T!(TValue) IEnumerable_T_GetEnumerator() 
  {

    return (this.GetEnumerator());
  
}

void ICollection_CopyTo(Array_T array, int index) 
  {

     Array_T!(TValue)  target = cast( Array_T!(TValue) )(array);
    if (target !is null)
    {

      ICollection_T_CopyTo(target, index);
      return;
    
}
    dictionary.CopyToCheck(array, index);
    dictionary.Do_ICollectionCopyTo!( TValue )(array, index, new Dictionary_TKey_TValue_Transform_TRetTKey_TValue!(TValue)(__ToDelegate(&System.Collections.Generic.Namespace.Dictionary.pick_value)));
  
}

System.Collections.Namespace.IEnumerator IEnumerable_GetEnumerator() 
  {

    return (this.GetEnumerator());
  
}
  

public    int  ICollection_T_Count() @property  {

    {

      return (dictionary.ICollection_T_Count);
    
}
  
}

  

   bool  ICollection_T_IsReadOnly() @property  {

    {

      return (true);
    
}
  
}

  

   bool  ICollection_IsSynchronized() @property  {

    {

      return (false);
    
}
  
}

  

   NObject  ICollection_SyncRoot() @property  {

    {

      return ((AsCast!( System.Collections.Namespace.ICollection )(this.dictionary)).ICollection_SyncRoot);
    
}
  
}



public this(Dictionary_TKey_TValue!(TKey, TValue) dictionary)
  {

    if (dictionary is null)
    {

      throw  new ArgumentNullException( ( String ("dictionary")));
    
}
    this.dictionary=dictionary;
  
}

};