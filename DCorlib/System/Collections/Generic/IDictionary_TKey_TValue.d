module System.Collections.Generic.IDictionary__G;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IDictionary__G( TKey , TValue ) :  ICollection__G!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))
{


public void Add(TKey key, TValue value) ;

public bool ContainsKey(TKey key) ;

public bool Remove(TKey key) ;

public bool TryGetValue(TKey key,  out TValue value) ;
  

public    abstract void opIndexAssign( TValue  value, TKey key);

public    abstract TValue opIndex(TKey key);

  

public     abstract ICollection_T!(TKey)  Keys() @property;
  

public     abstract ICollection_T!(TValue)  Values() @property;

};