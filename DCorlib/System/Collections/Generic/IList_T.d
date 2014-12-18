module System.Collections.Generic.IList_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IList_T( T ) :  ICollection_T!(T)
{


public int IList_T_IndexOf(T item) ;

public void IList_T_Insert(int index, T item) ;

public void IList_T_RemoveAt(int index) ;
  

public    abstract void opIndexAssign( T  value, int index )  ;

};