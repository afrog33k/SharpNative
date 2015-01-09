module System.Collections.Generic.IComparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 interface IComparer_T(T)
{


public int Compare(T x, T y, IComparer_T!(T) k = null);

};