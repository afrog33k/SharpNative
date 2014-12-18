module System.Collections.Generic.Dictionary_TKey_TValue_EnumeratorTKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
//import CsRoot.Namespace;

 class Dictionary_TKey_TValue_EnumeratorTKey_TValue(TKey, TValue) :  IEnumerator_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue)) ,  IDisposable ,  System.Collections.Namespace.IDictionaryEnumerator ,  System.Collections.Namespace.IEnumerator
{

  Dictionary_TKey_TValue!(TKey, TValue) dictionary = cast(Dictionary_TKey_TValue!(TKey, TValue)) null;
  int next;
  int stamp;
  public KeyValuePair_TKey_TValue!(TKey, TValue) current;

public bool IEnumerator_MoveNext() 
  {

    VerifyState();
    if (this.next<0)
    {

      return (false);
    
}
    while (this.next<dictionary.touchedSlots)
    {

      int cur = this.next++;
      if ((dictionary.linkSlots[cur].HashCode&HASH_FLAG)!=0)
      {

        this.current=KeyValuePair_TKey_TValue!(TKey, TValue)(dictionary.keySlots[cur], dictionary.valueSlots[cur]);
        return (true);
      
}
    
}
    this.next=-1;
    return (false);
  
}
  

public    KeyValuePair_TKey_TValue!(TKey, TValue)  IEnumerator_T_Current() @property  {

    {

      return (current);
    
}
  
}

  

public    TKey  CurrentKey() @property  {

    {

      VerifyCurrent();
      return (current.Key);
    
}
  
}

  

public    TValue  CurrentValue() @property  {

    {

      VerifyCurrent();
      return (current.Value);
    
}
  
}

  

   NObject  IEnumerator_Current() @property  {

    {

      VerifyCurrent();
      return (current);
    
}
  
}


void IEnumerator_Reset() 
  {

    Reset();
  
}

public void IEnumerator_Reset() 
  {

    VerifyState();
    this.next=0;
  
}
  

   System.Collections.Namespace.DictionaryEntry  IDictionaryEnumerator_Entry() @property  {

    {

      VerifyCurrent();
      return (System.Collections.Namespace.DictionaryEntry(current.Key, current.Value));
    
}
  
}

  

   NObject  IDictionaryEnumerator_Key() @property  {

    {

      return (CurrentKey);
    
}
  
}

  

   NObject  IDictionaryEnumerator_Value() @property  {

    {

      return (CurrentValue);
    
}
  
}


void VerifyState() 
  {

    if (this.dictionary is null)
    {

      throw  new ObjectDisposedException( (new String (null)));
    
}
    if (dictionary.generation!=this.stamp)
    {

      throw  new InvalidOperationException( (new String ("out of sync")));
    
}
  
}

void VerifyCurrent() 
  {

    VerifyState();
    if (this.next<=0)
    {

      throw  new InvalidOperationException( (new String ("Current is not valid")));
    
}
  
}

public void IDisposable_Dispose() 
  {

    this.dictionary=null;
  
}


public this(Dictionary_TKey_TValue!(TKey, TValue) dictionary)
  {

    this();
    this.dictionary=dictionary;
    this.stamp=dictionary.generation;
  
}

};