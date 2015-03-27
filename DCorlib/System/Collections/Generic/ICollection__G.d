module System.Collections.Generic.ICollection__G;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface ICollection__G(T) :  IEnumerable__G!(T)
{

  

public     abstract int  Count(ICollection__G!(T) j = null) @property;
  

public     abstract bool  IsReadOnly(ICollection__G!(T) j = null) @property;

public void Add(T item,ICollection__G!(T) j = null);

public void Clear(ICollection__G!(T) j = null);

public bool Contains(T item,ICollection__G!(T) j = null);

public void CopyTo(Array_T!(T) array, int arrayIndex,ICollection__G!(T) j = null);

public bool Remove(T item,ICollection__G!(T) j = null);

};