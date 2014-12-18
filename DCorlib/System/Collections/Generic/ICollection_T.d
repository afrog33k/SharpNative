module System.Collections.Generic.ICollection_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface ICollection_T( T ) :  IEnumerable_T!(T)
{

  

public     abstract int  ICollection_T_Count() @property;
  

public     abstract bool  ICollection_T_IsReadOnly() @property;

public void ICollection_T_Add(T item) ;

public void ICollection_T_Clear() ;

public bool ICollection_T_Contains(T item) ;

public void ICollection_T_CopyTo(Array_T!(T) array, int arrayIndex) ;

public bool ICollection_T_Remove(T item) ;

};