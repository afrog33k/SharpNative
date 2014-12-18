module System.Collections.Generic.Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

 class Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue(TKey,TValue) :  IEnumerator_T!(TValue) ,  IDisposable ,  System.Collections.Namespace.IEnumerator
{

  Dictionary_TKey_TValue_EnumeratorTKey_TValue host_enumerator;

public void IDisposable_Dispose() 
  {

    host_enumerator.IDisposable_Dispose();
  
}

public bool IEnumerator_MoveNext() 
  {

    return (host_enumerator.IEnumerator_MoveNext());
  
}
  

public    TValue  IEnumerator_T_Current() @property  {

    {

      return (host_enumerator.current.Value);
    
}
  
}

  

   NObject  IEnumerator_Current() @property  {

    {

      return (host_enumerator.CurrentValue);
    
}
  
}


void IEnumerator_Reset() 
  {

    host_enumerator.Reset();
  
}


public this(Dictionary_TKey_TValue!(TKey, TValue) host)
  {

    this.host_enumerator=host.GetEnumerator();
  
}

};