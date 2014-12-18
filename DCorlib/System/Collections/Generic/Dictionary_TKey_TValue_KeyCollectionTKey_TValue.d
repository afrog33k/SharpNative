module System.Collections.Generic.Dictionary_TKey_TValue_KeyCollectionTKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

 class Dictionary_TKey_TValue_KeyCollectionTKey_TValue(TKey,TValue) :  NObject ,  ICollection_T!(TKey) ,  IEnumerable_T!(TKey) ,  System.Collections.Namespace.ICollection ,  System.Collections.Namespace.IEnumerable
{

  Dictionary_TKey_TValue!(TKey, TValue) dictionary = cast(Dictionary_TKey_TValue!(TKey, TValue)) null;

public void ICollection_T_CopyTo(Array_T!(TKey) array, int index) 
  {

    dictionary.CopyToCheck(array, index);
    dictionary.CopyKeys(array, index);
  
}

public Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue GetEnumerator() 
  {

    return (Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue(dictionary));
  
}

void ICollection_T_Add(TKey item) 
  {

    throw  new NotSupportedException( (new String ("this is a read-only collection")));
  
}

void ICollection_T_Clear() 
  {

    throw  new NotSupportedException( (new String ("this is a read-only collection")));
  
}

bool ICollection_T_Contains(TKey item) 
  {

    return (dictionary.IDictionary_TKey_TValue_ContainsKey(item));
  
}

bool ICollection_T_Remove(TKey item) 
  {

    throw  new NotSupportedException( (new String ("this is a read-only collection")));
  
}

IEnumerator_T!(TKey) IEnumerable_T_GetEnumerator() 
  {

    return (this.GetEnumerator());
  
}

void ICollection_CopyTo(Array_T array, int index) 
  {

     Array_T!(TKey)  target = cast( Array_T!(TKey) )(array);
    if (target !is null)
    {

      ICollection_T_CopyTo(target, index);
      return;
    
}
    dictionary.CopyToCheck(array, index);
    dictionary.Do_ICollectionCopyTo!( TKey )(array, index, new Dictionary_TKey_TValue_Transform_TRetTKey_TValue!(TKey)(__ToDelegate(&System.Collections.Generic.Namespace.Dictionary.pick_key)));
  
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

      throw  new ArgumentNullException( (new String ("dictionary")));
    
}
    this.dictionary=dictionary;
  
}

};