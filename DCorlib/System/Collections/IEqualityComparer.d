module System.Collections.IEqualityComparer;


import System.Namespace;
import System.Collections.Namespace;

 interface IEqualityComparer
{


public bool Equals(NObject x, NObject y, IEqualityComparer k =null) ;

public int GetHashCode(NObject obj, IEqualityComparer k =null) ;

};