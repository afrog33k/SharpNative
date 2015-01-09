module System.Collections.Generic.IEqualityComparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IEqualityComparer_T(T)
{


public bool Equals(T x, T y, IEqualityComparer_T!(T) j =null);

public int GetHashCode(T obj, IEqualityComparer_T!(T) j =null);

};