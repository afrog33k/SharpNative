module System.Collections.IDictionary;


import System.Namespace;
import System.Collections.Namespace;

 interface IDictionary :  ICollection
{
  

public      bool  IsReadOnly(IDictionary j =null) @property;
  

public      bool  IsFixedSize(IDictionary j =null) @property;
  

public      ICollection  Keys(IDictionary j =null) @property;
  

public      ICollection  Values(IDictionary j =null) @property;
  

public    NObject  opIndexAssign( NObject  value, NObject key,IDictionary j =null );

public void Add(NObject key, NObject value,IDictionary j =null);

public void Clear(IDictionary j =null);

public bool Contains(NObject key,IDictionary j =null);

public void Remove(NObject key,IDictionary j =null);

};