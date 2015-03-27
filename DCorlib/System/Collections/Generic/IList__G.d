module System.Collections.Generic.IList__G;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IList__G(T) : ICollection__G!(T)
{


public int IndexOf(T item,IList__G!(T) j=null) ;

public void Insert(int index, T item,IList__G!(T) j=null);

public void RemoveAt(int index,IList__G!(T) j=null);
  
public abstract T opIndexAssign( T  value, int index, IList__G!(T) j=null);

};