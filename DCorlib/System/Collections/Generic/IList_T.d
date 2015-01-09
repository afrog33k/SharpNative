module System.Collections.Generic.IList_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IList_T(T) :  ICollection_T!(T)
{


public int IndexOf(T item,IList_T!(T) j=null) ;

public void Insert(int index, T item,IList_T!(T) j=null);

public void RemoveAt(int index,IList_T!(T) j=null);
  
public abstract void opIndexAssign( T  value, int index, IList_T!(T) j=null);

};