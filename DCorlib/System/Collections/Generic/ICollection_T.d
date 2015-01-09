module System.Collections.Generic.ICollection_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface ICollection_T(T) :  IEnumerable_T!(T)
{

  

public     abstract int  Count(ICollection_T!(T) j = null) @property;
  

public     abstract bool  IsReadOnly(ICollection_T!(T) j = null) @property;

public void Add(T item,ICollection_T!(T) j = null);

public void Clear(ICollection_T!(T) j = null);

public bool Contains(T item,ICollection_T!(T) j = null);

public void CopyTo(Array_T!(T) array, int arrayIndex,ICollection_T!(T) j = null);

public bool Remove(T item,ICollection_T!(T) j = null);

};