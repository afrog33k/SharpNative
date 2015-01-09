module System.Collections.ICollection;


import System.Namespace;
import System.Collections.Namespace;

interface ICollection :  IEnumerable
{

public void CopyTo(Array array, int index, ICollection j=null) ;
  

public     abstract int  Count(ICollection j=null) @property;
  

public     abstract NObject  SyncRoot(ICollection j=null) @property;
  

public     abstract bool  IsSynchronized(ICollection j=null) @property;

};