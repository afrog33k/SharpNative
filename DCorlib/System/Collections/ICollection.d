module System.Collections.ICollection;


import System.Namespace;
import System.Collections.Namespace;

interface ICollection :  IEnumerable
{

public void CopyTo(Array_T!(NObject[]) array, int index) ;
  

public     abstract int  Count() @property;
  

public     abstract NObject  SyncRoot() @property;
  

public     abstract bool  IsSynchronized() @property;

};