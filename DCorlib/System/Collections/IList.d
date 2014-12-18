module System.Collections.IList;


import System.Namespace;
import System.Collections.Namespace;

 interface IList :  ICollection ,  IEnumerable
{

  

public     abstract bool  IList_IsFixedSize() @property;
  

public     abstract bool  IList_IsReadOnly() @property;
  

public    abstract void opIndexAssign( NObject  value, int index )  ;

public int IList_Add(NObject value) ;

public void IList_Clear() ;

public bool IList_Contains(NObject value) ;

public int IList_IndexOf(NObject value) ;

public void IList_Insert(int index, NObject value) ;

public void IList_Remove(NObject value) ;

public void IList_RemoveAt(int index) ;

};