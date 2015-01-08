module System.Collections.ICollection;


import System.Namespace;
import System.Collections.Namespace;

interface ICollection :  IEnumerable
{

public void ICollection_CopyTo(Array array, int index) ;
  

public     abstract int  ICollection_Count() @property;
  

public     abstract NObject  ICollection_SyncRoot() @property;
  

public     abstract bool  ICollection_IsSynchronized() @property;

};