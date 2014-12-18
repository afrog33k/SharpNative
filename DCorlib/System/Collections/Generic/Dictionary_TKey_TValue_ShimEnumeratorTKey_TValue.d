module System.Collections.Generic.Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue;


import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;

 class Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue(TKey, TValue) :  NObject ,  System.Collections.Namespace.IDictionaryEnumerator ,  System.Collections.Namespace.IEnumerator
{

  Dictionary_TKey_TValue_EnumeratorTKey_TValue host_enumerator;

public void Dispose() 
  {

    host_enumerator.IDisposable_Dispose();
  
}

public bool IEnumerator_MoveNext() 
  {

    return (host_enumerator.IEnumerator_MoveNext());
  
}
  

public    System.Collections.Namespace.DictionaryEntry  IDictionaryEnumerator_Entry() @property  {

    {

      return ((BOX!( Dictionary_TKey_TValue_EnumeratorTKey_TValue )(this.host_enumerator)).IDictionaryEnumerator_Entry);
    
}
  
}

  

public    NObject  IDictionaryEnumerator_Key() @property  {

    {

      return (host_enumerator.IEnumerator_T_Current.Key);
    
}
  
}

  

public    NObject  IDictionaryEnumerator_Value() @property  {

    {

      return (host_enumerator.IEnumerator_T_Current.Value);
    
}
  
}

  

public    NObject  IEnumerator_Current() @property  {

    {

      return (IDictionaryEnumerator_Entry);
    
}
  
}


public void IEnumerator_Reset() 
  {

    host_enumerator.Reset();
  
}


public this(Dictionary_TKey_TValue!(TKey, TValue) host)
  {

    this.host_enumerator=host.GetEnumerator();
  
}

};