module System.Collections.IDictionary;


import System.Namespace;
import System.Collections.Namespace;

 interface IDictionary :  ICollection
{
  

public     abstract bool  IsReadOnly(IDictionary j =null) @property;
  

public     abstract bool  IsFixedSize(IDictionary j =null) @property;
  

public     abstract ICollection  Keys(IDictionary j =null) @property;
  

public     abstract ICollection  Values(IDictionary j =null) @property;
  

public    abstract void opIndexAssign( NObject  value, NObject key,IDictionary j =null );

public void Add(NObject key, NObject value,IDictionary j =null);

public void Clear(IDictionary j =null);

public bool Contains(NObject key,IDictionary j =null);

public void Remove(NObject key,IDictionary j =null);

};