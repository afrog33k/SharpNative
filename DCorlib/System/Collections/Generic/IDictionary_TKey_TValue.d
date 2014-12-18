module System.Collections.Generic.IDictionary_TKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IDictionary_TKey_TValue( TKey , TValue ) :  ICollection_T!(System.Collections.Generic.Namespace.KeyValuePair_TKey_TValue!(TKey, TValue))
{


public void IDictionary_TKey_TValue_Add(TKey key, TValue value) ;

public bool IDictionary_TKey_TValue_ContainsKey(TKey key) ;

public bool IDictionary_TKey_TValue_Remove(TKey key) ;

public bool IDictionary_TKey_TValue_TryGetValue(TKey key,  out TValue value) ;
  

public    abstract void opIndexAssign( TValue  value, TKey key )  ;
  

public     abstract ICollection_T!(TKey)  IDictionary_TKey_TValue_Keys() @property;
  

public     abstract ICollection_T!(TValue)  IDictionary_TKey_TValue_Values() @property;

};