module System.Collections.IDictionary;


import System.Namespace;
import System.Collections.Namespace;

 interface IDictionary :  ICollection
{
  

public     abstract bool  IsReadOnly() @property;
  

public     abstract bool  IsFixedSize() @property;
  

public     abstract ICollection  Keys() @property;
  

public     abstract ICollection  Values() @property;
  

public    abstract void opIndexAssign( NObject  value, NObject key )  ;

public void Add(NObject key, NObject value) ;

public void Clear() ;

public bool Contains(NObject key) ;

public void Remove(NObject key) ;

};