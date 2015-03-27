module System.Collections.Generic.IEqualityComparer__G;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IEqualityComparer__G(T)
{


public bool Equals(T x, T y, IEqualityComparer__G!(T) j =null);

public int GetHashCode(T obj, IEqualityComparer__G!(T) j =null);

};